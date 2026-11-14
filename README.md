# Running AvistaRD Project

1. **Install prerequisites:** .NET SDK, Node.js (LTS), Angular CLI (`npm install -g @angular/cli`) and Git.

2. **Clone the repository:**
   ```bash
   git clone <repo-url>
   cd <repo-folder>
   ```

3. **Create the `.env` file** from the template:
   ```bash
   cp .env.example .env
   ```

4. **Fill in `.env`** with your real values (database connection, API URL, keys). Never commit `.env`; only `.env.example` goes to Git.

5. **Restore backend dependencies:**
   ```bash
   cd backend
   dotnet restore
   ```

6. **Set up the database**:
   ```bash
   dotnet ef database update
   ```

7. **Run the backend:**
   ```bash
   dotnet run
   ```
   Check that the API responds, e.g. at `https://localhost:<port>/swagger`.

8. **Install frontend dependencies** in a new terminal:
   ```bash
   cd frontend
   npm install
   ```

9. **Run the frontend:**
   ```bash
   ng serve
   ```

10. **Open the app** at `http://localhost:4200`. If something fails, compare `.env` with `.env.example` and make sure the backend is running.