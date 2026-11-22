# Running AvistaRD Project

1. **Install prerequisites:** Git, the .NET 10 SDK (the exact version is pinned in `global.json`) and Node.js (the exact version is pinned in `src/AvistaRD.Web/.nvmrc`; with nvm run `nvm use` inside that folder). The Angular CLI and `dotnet-ef` are project-local, so no global install is needed.

2. **Clone the repository:**
   ```bash
   git clone <repo-url>
   cd <repo-folder>
   ```

3. **Create the `.env` file** for the API from the template:
   ```bash
   cp src/AvistaRD.Api/.env.example src/AvistaRD.Api/.env
   ```

4. **Fill in `.env`** with your real values (database connection, keys). Never commit `.env`; only `.env.example` goes to Git.

5. **Restore backend dependencies and tools:**
   ```bash
   dotnet restore
   dotnet tool restore
   ```

6. **Run the backend:**
   ```bash
   cd src/AvistaRD.Api
   dotnet run
   ```
   In Development the API applies pending migrations to the database at startup. Check that it responds, e.g. at `https://localhost:<port>/swagger` or `https://localhost:<port>/health`.

7. **Install frontend dependencies** in a new terminal:
   ```bash
   cd src/AvistaRD.Web
   npm ci
   ```

8. **Run the frontend:**
   ```bash
   npm start
   ```

9. **Open the app** at `http://localhost:4200`. If something fails, compare `.env` with `.env.example` and make sure the backend is running.

## Tests and lint

```bash
dotnet test                 # API (xUnit), from the repository root
cd src/AvistaRD.Web
npm run lint                # ESLint
npm test                    # Vitest in watch mode (npm run test:ci for a single run)
```

## Database migrations

The EF Core tools use the design-time factory in `AvistaRD.Infrastructure`, so pass that project as both `-p` and `-s` (they don't start the API and don't need `.env`):

```bash
dotnet ef migrations add <Name> -p src/AvistaRD.Infrastructure -s src/AvistaRD.Infrastructure
dotnet ef migrations has-pending-model-changes -p src/AvistaRD.Infrastructure -s src/AvistaRD.Infrastructure
```

To apply migrations from the CLI, pass the connection string explicitly (the tools don't read `.env`):

```bash
dotnet ef database update -p src/AvistaRD.Infrastructure -s src/AvistaRD.Infrastructure --connection "<connection string>"
```

## CI/CD

Pull requests and merges to `master` run the Azure Pipelines pipeline in `azure-pipelines.yml`. See [docs/ci-cd.md](docs/ci-cd.md) for what it does and for the one-time Azure setup.
