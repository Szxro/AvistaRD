# CI/CD with Azure Pipelines

The pipeline lives in [`azure-pipelines.yml`](../azure-pipelines.yml) and has two stages.

| Stage | Runs on | What it does |
|---|---|---|
| **Build** | every pull request to `master` and every push to `master` | **API:** restore, build, xUnit tests, check that the EF model has no changes without a migration, build the migrations bundle. **Frontend:** `npm ci`, lint, Vitest, production build. |
| **Deploy** | pushes to `master` only (after Build succeeds) | Apply migrations to Azure SQL (`efbundle`), deploy the API to App Service, smoke-test `GET /health`, deploy the frontend to Static Web Apps. |

If any step fails, nothing after it runs: a failed migration stops the deploy before the API or the
frontend change.

## Secrets

No secret is stored in the repository or in pipeline variables. All of them live in Azure Key Vault:

| Key Vault secret | Read by | Used for |
|---|---|---|
| `DATABASE-CONNECTION` | App Service (Key Vault reference) | The API's connection string (least-privilege SQL user). |
| `DATABASE-MIGRATIONS-CONNECTION` | Pipeline (`AzureKeyVault@2`) | Applying migrations (SQL user allowed to change the schema). |
| `MEDIATR-LICENSE-KEY` | App Service (Key Vault reference) | MediatR license. |
| `SWA-DEPLOYMENT-TOKEN` | Pipeline (`AzureKeyVault@2`) | Deploying to Static Web Apps. |
| `JWT-SIGNING-KEY` | App Service (Key Vault reference) | *Add it when JWT authentication is configured* (not yet in the code). |

Key Vault secret names can't contain `_`, so they use `-`. The API keeps reading the environment
variables it already uses (`DATABASE_CONNECTION`, `MEDIATR_LICENSE_KEY`): App Service app settings are
exposed as environment variables, and each one points to its secret.

## One-time setup

Names below are examples; whatever you choose, update the `variables` block in `azure-pipelines.yml`
(`azureServiceConnection`, `keyVaultName`, `apiAppName`).

```bash
RG=rg-avistard
LOCATION=eastus2
SQL_SERVER=sql-avistard
SQL_DB=sqldb-avistard
KV=kv-avistard
API_APP=app-avistard-api
SWA=swa-avistard-web
```

### 1. Azure SQL Database

1. Create the server and the database (the Basic or serverless tier is enough).
2. Allow Azure services through the firewall. This lets both App Service and the Microsoft-hosted
   pipeline agents connect:
   ```bash
   az sql server firewall-rule create -g $RG -s $SQL_SERVER -n AllowAzureServices \
     --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
   ```
3. Create two contained users in the database (run in the Azure portal's query editor, connected to
   `$SQL_DB`):
   ```sql
   -- Used only by the pipeline to apply migrations
   CREATE USER avistard_migrator WITH PASSWORD = '<strong password>';
   ALTER ROLE db_owner ADD MEMBER avistard_migrator;

   -- Used by the API: data only, no schema changes
   CREATE USER avistard_app WITH PASSWORD = '<another strong password>';
   ALTER ROLE db_datareader ADD MEMBER avistard_app;
   ALTER ROLE db_datawriter ADD MEMBER avistard_app;
   ```
   Connection string format:
   `Server=tcp:<server>.database.windows.net,1433;Database=<db>;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;`

### 2. Key Vault

1. Create the vault with the **Azure RBAC** permission model.
2. Give yourself the *Key Vault Secrets Officer* role on it so you can add secrets.
3. Add `DATABASE-CONNECTION` (with `avistard_app`), `DATABASE-MIGRATIONS-CONNECTION` (with
   `avistard_migrator`) and `MEDIATR-LICENSE-KEY`. `SWA-DEPLOYMENT-TOKEN` is added in step 4.

### 3. App Service (API)

1. Create a **Linux** web app with the **.NET 8** runtime stack.
2. Enable its system-assigned managed identity and let it read secrets:
   ```bash
   PRINCIPAL_ID=$(az webapp identity assign -g $RG -n $API_APP --query principalId -o tsv)
   az role assignment create --assignee-object-id $PRINCIPAL_ID --assignee-principal-type ServicePrincipal \
     --role "Key Vault Secrets User" --scope $(az keyvault show -n $KV --query id -o tsv)
   ```
3. Add the app settings as Key Vault references:
   ```bash
   az webapp config appsettings set -g $RG -n $API_APP --settings \
     ASPNETCORE_ENVIRONMENT=Production \
     DATABASE_CONNECTION="@Microsoft.KeyVault(VaultName=$KV;SecretName=DATABASE-CONNECTION)" \
     MEDIATR_LICENSE_KEY="@Microsoft.KeyVault(VaultName=$KV;SecretName=MEDIATR-LICENSE-KEY)"
   ```
   In the portal (*Settings → Environment variables*), each reference must show a green
   *Key vault Reference* status. If it doesn't, the identity or the role assignment is wrong.
4. Set the health check to the liveness endpoint, which doesn't depend on the database:
   ```bash
   az webapp config set -g $RG -n $API_APP --generic-configurations '{"healthCheckPath": "/health/live"}'
   ```

### 4. Static Web Apps (frontend)

1. Create a Static Web App on the **Free** plan with deployment source **Other** (the pipeline deploys it).
2. Store its deployment token in Key Vault:
   ```bash
   az keyvault secret set --vault-name $KV -n SWA-DEPLOYMENT-TOKEN \
     --value "$(az staticwebapp secrets list -n $SWA --query properties.apiKey -o tsv)"
   ```

### 5. Azure DevOps

1. **Service connection:** *Project settings → Service connections → New → Azure Resource Manager →
   Workload identity federation (automatic)*, scoped to the resource group `$RG`. Name it as
   `azureServiceConnection` in the YAML.
2. Let the service connection read the pipeline's secrets: give its identity the
   *Key Vault Secrets User* role on the vault. Find its object ID under the service connection's
   *Manage service connection roles* / *Manage App registration* link.
3. **Pipeline:** *Pipelines → New pipeline →* pick the repository *→ Existing Azure Pipelines YAML file →*
   `/azure-pipelines.yml`.
4. **Environment:** the first deploy creates the `avistard-production` environment. Under
   *Pipelines → Environments → avistard-production → Approvals and checks* add an *Exclusive lock* so two
   merges don't deploy at the same time. Don't add a manual approval: JAR-40 asks for `master` to deploy
   with no manual steps.

### 6. Block merges when the build fails

How you do this depends on where the repository is hosted:

- **GitHub (current `origin`):** *Settings → Branches → Add branch protection rule* for `master`:
  require a pull request before merging and require status checks to pass, selecting the Azure
  Pipelines check. The `pr:` section of the YAML is what triggers the build on pull requests.
- **Azure Repos:** the `pr:` section is ignored there. Use *Project settings → Repositories →
  Policies → Branch policies → master → Build validation*, pick this pipeline and set it to *Required*.

## Running the same checks locally

```bash
# API
dotnet tool restore
dotnet build -c Release
dotnet test -c Release --no-build
dotnet ef migrations has-pending-model-changes -p src/AvistaRD.Infrastructure -s src/AvistaRD.Infrastructure

# Frontend (from src/AvistaRD.Web)
npm ci
npm run lint
npm run test:ci
npm run build
```

## Known gaps (outside JAR-40)

- **CORS:** `AddPresentationLayer` defines a `"default"` policy that allows any origin, but
  `app.UseCors()` is never called. The frontend on Static Web Apps (another origin) can't call the API
  until CORS is enabled, ideally restricted to the Static Web Apps URL.
- **JWT:** `AddJwtBearer()` has no configuration and the auth endpoints come from `MapIdentityApi`
  (Identity bearer tokens, not JWT). When JWT is configured, its signing key goes to Key Vault as
  `JWT-SIGNING-KEY`.
- **Frontend tests use Vitest** (Angular's default runner), not Jasmine/Jest.
