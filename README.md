# PrimeChip

PrimeChip is a single-project ASP.NET Core MVC inventory application. It manages products, stock-in and stock-out records, low-stock alerts, vendors, inventory reports, and a user profile image. Its dashboard combines database-backed inventory totals with some placeholder UI content.

> [!IMPORTANT]
> The database schema can be recreated completely from the Entity Framework Core migrations in this repository. No database backup is required. However, the migrations do **not** create a login account, and the repository does not contain a safe, working first-user provisioning process. Read [Account and seed-data status](#account-and-seed-data-status) before trying to sign in.

## Technologies Used

- ASP.NET Core MVC on .NET 10 (`net10.0`)
- C# with nullable reference types and implicit global usings enabled
- Entity Framework Core 10.0.7
- Microsoft SQL Server through `Microsoft.EntityFrameworkCore.SqlServer` 10.0.7
- SQL Server Express LocalDB for the checked-in development connection
- Session state for the application's custom login gate
- BCrypt.Net-Next 4.1.0
- Razor views, Bootstrap, jQuery, JavaScript, and Chart.js

### NuGet packages

The following direct package references are defined in `PrimeChip/PrimeChip.csproj`:

| Package | Version | Purpose in this project |
| --- | ---: | --- |
| `BCrypt.Net-Next` | 4.1.0 | BCrypt support referenced by the login controller |
| `Microsoft.EntityFrameworkCore` | 10.0.7 | EF Core runtime |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.7 | SQL Server EF Core provider |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.7 | Visual Studio Package Manager Console EF commands |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 10.0.2 | ASP.NET scaffolding/design-time support |
| `NuGet.Packaging` | 7.3.1 | NuGet packaging APIs |
| `NuGet.Protocol` | 7.3.1 | NuGet protocol APIs |

## Requirements

Install these on the new Windows PC:

1. **Git for Windows**.
2. **Visual Studio 2026 18.0 or newer**. The project targets .NET 10, for which Microsoft lists Visual Studio 2026 18.0 as the minimum supported Visual Studio version. In Visual Studio Installer, select:
   - **ASP.NET and web development** workload.
   - **SQL Server Express LocalDB** as an individual component if it was not installed by the workload.
3. **.NET 10 SDK**. The repository does not contain a `global.json`, so it does not pin a particular .NET 10 patch. The project was verified with SDK 10.0.400.
4. **SQL Server Express LocalDB**. The default connection uses the standard `MSSQLLocalDB` instance and Windows Authentication.
5. Optional: **SQL Server Management Studio (SSMS)** if you want a graphical way to inspect the database. Visual Studio's SQL Server Object Explorer is sufficient for basic verification.

Useful checks in PowerShell:

```powershell
dotnet --version
dotnet --list-sdks
sqllocaldb info
sqllocaldb info MSSQLLocalDB
```

If `sqllocaldb` is not recognized, add SQL Server Express LocalDB through Visual Studio Installer. If LocalDB is installed but the standard instance does not exist, create and start it:

```powershell
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

Microsoft installation references:

- [.NET on Windows and Visual Studio version support](https://learn.microsoft.com/dotnet/core/install/windows)
- [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)

## Getting Started

### 1. Clone the repository

```powershell
git clone https://github.com/jphabel/PrimeChip.git
cd PrimeChip
```

All remaining PowerShell commands in this README assume that the current directory is the repository root—the directory containing `PrimeChip.slnx`.

### 2. Open the solution

Open `PrimeChip.slnx` in Visual Studio. It contains one web project:

```text
PrimeChip/PrimeChip.csproj
```

The repository uses the modern XML `.slnx` solution format. Do not open or extract the tracked `PrimeChip.zip`; it is a stale archive containing old build artifacts and is not part of the setup process.

### 3. Restore dependencies

Visual Studio normally restores NuGet packages when the solution opens. To restore explicitly from PowerShell, run:

```powershell
dotnet restore .\PrimeChip.slnx
```

This command was verified against the current solution.

## Database Setup

### Database facts

| Setting | Repository value |
| --- | --- |
| Database engine | Microsoft SQL Server |
| Development edition/instance | SQL Server Express LocalDB, `MSSQLLocalDB` |
| EF Core provider | `Microsoft.EntityFrameworkCore.SqlServer` 10.0.7 |
| Database name | `PrimeChipDb` |
| DbContext | `PrimeChip.Data.AppDbContext` |
| DbContext file | `PrimeChip/data/AppDbContext.cs` |
| Connection-string name | `DefaultConnection` |
| Connection-string file | `PrimeChip/appsettings.json` |
| Automatic creation at startup | No—neither `Database.Migrate()` nor `EnsureCreated()` is called |
| EF migrations | Yes, five migrations in `PrimeChip/Migrations/` |
| EF/model seed data | None |
| ASP.NET Identity tables | None; the project uses its own `Users` entity/table |
| Required SQL schema script | None |
| Required `.bak`/`.bacpac` file | None exists and none is required |

`AppDbContext` exposes these tables:

- `Users`
- `Inventories`
- `Vendors`
- `Sales`

EF Core also creates `__EFMigrationsHistory` to track applied migrations.

### 1. Configure the connection string

The checked-in development configuration already targets the standard LocalDB instance with Windows Authentication. A safe equivalent example is:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PrimeChipDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Meaning of each part:

- `Server=(localdb)\MSSQLLocalDB` selects the standard per-user LocalDB instance. It contains no PC name and normally needs no change on another Windows PC.
- `Database=PrimeChipDb` is the database EF Core will create or update.
- `Trusted_Connection=True` uses the current Windows account. No database username or password belongs in this connection string.
- `TrustServerCertificate=True` accepts the development SQL Server certificate.

If you intentionally use a full SQL Server or SQL Server Express named instance instead of LocalDB, keep Windows Authentication and replace only the server value with your own instance:

```json
"DefaultConnection": "Server=<SERVER_OR_MACHINE>\\<INSTANCE>;Database=PrimeChipDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Do not commit a real machine name or credentials. For a temporary per-shell override, ASP.NET Core's default configuration can read the nested setting from an environment variable:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=<SERVER_OR_MACHINE>\<INSTANCE>;Database=PrimeChipDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

`PrimeChip/appsettings.Development.json` currently changes logging only. The project has no `UserSecretsId` and no repository-specific environment-variable setup.

### 2. Create the database with EF Core migrations

Choose **one** of the following methods. Do not run both unless you are only confirming that the database is already up to date.

#### Option A: Visual Studio Package Manager Console

This is the simplest method when following the Visual Studio setup:

1. In Solution Explorer, right-click the `PrimeChip` project and choose **Set as Startup Project**.
2. Open **Tools > NuGet Package Manager > Package Manager Console**.
3. Set **Default project** to `PrimeChip`.
4. Run:

```powershell
Update-Database
```

`Microsoft.EntityFrameworkCore.Tools` 10.0.7 is referenced by the project, so `Update-Database` applies to this solution.

#### Option B: .NET CLI

The `dotnet ef` command is a separate tool; the NuGet package in the project does not install the global CLI command. Install a matching EF 10 tool version:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
dotnet ef --version
```

If `dotnet-ef` is already installed globally, align it with the project's EF version:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.7
dotnet ef --version
```

From the repository root, create/update `PrimeChipDb` with the exact project paths:

```powershell
dotnet ef database update --project .\PrimeChip\PrimeChip.csproj --startup-project .\PrimeChip\PrimeChip.csproj
```

The expected migration order is:

1. `20260513061233_InitialCreate`
2. `20260513140849_AddProfileImage`
3. `20260513225251_InitialStock`
4. `20260513225629_Sales` (an intentionally empty migration in the repository)
5. `20260513235705_FixDecimalPrecision`

The migration chain was checked by listing the migrations, generating an idempotent SQL migration script, and comparing the current model with the snapshot. EF reported no pending model changes.

### 3. Verify the database

In Visual Studio, open **View > SQL Server Object Explorer**, connect to:

```text
(localdb)\MSSQLLocalDB
```

Use Windows Authentication. Expand **Databases > PrimeChipDb > Tables** and confirm these tables exist:

```text
dbo.__EFMigrationsHistory
dbo.Inventories
dbo.Sales
dbo.Users
dbo.Vendors
```

The history table should contain the five migrations listed above.

### Account and seed-data status

The database migrations create an **empty** database:

- There is no `HasData(...)` model seed.
- There is no startup seeder.
- There is no registration screen or first-user workflow.
- There are no ASP.NET Identity tables.
- `SQLQuery1.sql` and `SQLQueryNew.sql` are account-data scripts, not database-creation scripts.

Do **not** treat the two SQL files as the normal setup process. They contain a fixed account identity and reusable credential hashes. In addition, `SQLQuery1.sql` does not supply all non-null columns in the final `Users` schema, while `SQLQueryNew.sql` only updates an already-existing row with a particular ID. Neither script can independently provision a valid first user after a fresh migration.

Consequently, migrations are sufficient to run the application and display its login page, but the repository alone does not define a safe way to create the first login account. Obtain an approved development account/provisioning procedure from the project owner before testing authenticated screens. Do not copy account values from the committed SQL files into documentation or new environments.

## Configuration

### `PrimeChip/appsettings.json`

Contains logging, allowed hosts, and `ConnectionStrings:DefaultConnection`. The committed connection uses LocalDB and Windows Authentication; it does not contain a SQL username or password.

### `PrimeChip/appsettings.Development.json`

Contains development logging settings only. It does not override the connection string.

### `PrimeChip/Properties/launchSettings.json`

Defines two development profiles:

- `http`: `http://localhost:5285`
- `https`: `https://localhost:7183` and `http://localhost:5285`

Both set `ASPNETCORE_ENVIRONMENT=Development`. These are local development URLs, not external services. If a port is already occupied, stop the conflicting process or choose another launch URL locally; do not commit a personal port change unless the team intends to share it.

### Files and external resources

- Profile images are written under `PrimeChip/wwwroot/uploads` by combining `IWebHostEnvironment.WebRootPath` with the relative `uploads` folder. No original-developer absolute path is used, and the application creates the folder if it is absent. The running process needs write permission there.
- Bootstrap and jQuery files are included under `wwwroot/lib`.
- Bootstrap Icons and Chart.js are loaded from public CDNs in Razor views. The pages can therefore lose icons or chart functionality when the PC has no internet access or those CDNs are blocked.
- No API keys, access tokens, client secrets, custom certificates, or required external-service credentials were found in the live project configuration.

## Build and Run

### Visual Studio

1. Ensure `PrimeChip` is the startup project.
2. Restore packages and apply migrations as described above.
3. Select the `https` launch profile.
4. Choose **Build > Build Solution**.
5. Press **Ctrl+F5** to run without the debugger or **F5** to debug.
6. Accept/trust the ASP.NET Core development certificate prompt if Visual Studio displays one.

### .NET CLI

From the repository root:

```powershell
dotnet build .\PrimeChip.slnx
dotnet run --project .\PrimeChip\PrimeChip.csproj --launch-profile https
```

`dotnet run` is supported. It does **not** apply migrations automatically, so run the database-update step first.

## First Run

After a successful setup:

1. The browser opens the default `Login/Index` route.
2. `PrimeChipDb` exists in `(localdb)\MSSQLLocalDB` with the tables listed above.
3. The database is empty unless account or application data was provisioned separately.
4. Authenticated pages are session-gated and redirect to the login page when the session key is missing.

Do not test a random or nonexistent email on the current login form: the current controller can throw a null-reference exception when no matching user exists. See the security notes below.

## Authentication and Authorization

This project does **not** use ASP.NET Core Identity, cookie authentication, claims, roles, or `[Authorize]` attributes.

The current flow is:

1. `LoginController` queries the custom `Users` table by email.
2. A string is stored in session under the `user` key.
3. Most application controllers inherit from `CheckController`, which redirects to `Login/Index` when that session value is absent.
4. Logout clears the session.

> [!WARNING]
> The current login action does not safely implement authentication: it does not verify the submitted password with BCrypt and it dereferences a missing user. A diagnostic endpoint also contains a hard-coded test password. Treat this as a known application security defect. It must be corrected before deployment or use with real accounts, but no application source was changed as part of this README work.

## Application Startup Flow

`PrimeChip/Program.cs` performs these steps:

1. Creates the web application builder and loads normal ASP.NET Core configuration sources.
2. Registers session services.
3. Registers `AppDbContext` with `UseSqlServer(...)` and `DefaultConnection`.
4. Registers MVC controllers/views and `IHttpContextAccessor`.
5. In non-development environments, enables the exception handler and HSTS.
6. Adds response headers that disable browser caching.
7. Enables HTTPS redirection, routing, session, authorization middleware, and static assets.
8. Maps the conventional route `{controller=Login}/{action=Index}/{id?}`.
9. Runs the web host.

There is no hosted background service and no separate `Services/` folder. Controllers use `AppDbContext` directly.

## Project Structure

```text
PrimeChip.slnx                  Single-project solution
PrimeChip/
  PrimeChip.csproj             Target framework and NuGet references
  Program.cs                   Dependency registration and request pipeline
  appsettings*.json            Logging and database configuration
  Controllers/                 Login, dashboard, inventory, alerts, sales,
                               vendors, reports, and profile actions
  Models/                      EF entities and view models
  data/AppDbContext.cs         EF Core DbContext (namespace PrimeChip.Data)
  Migrations/                  Five SQL Server EF Core migrations and snapshot
  Views/                       Razor MVC views and shared layout/partials
  Properties/launchSettings.json
                               Local HTTP/HTTPS profiles
  wwwroot/                     CSS, JavaScript, images, libraries, and uploads
SQLQuery1.sql                  Legacy account insert; not schema setup
SQLQueryNew.sql                Legacy account update; not schema setup
PrimeChip.zip                  Stale archive; not used by the solution
```

Main functional areas:

- **Dashboard**: inventory totals and charts; some alert/order/top-item markup is placeholder content.
- **Inventory**: list, create, and delete products; records initial stock.
- **Alerts/Vendors**: lists low-stock products, records stock-in movements, and manages basic vendor records.
- **Sales**: records stock-out movements and reduces inventory.
- **Reports**: inventory summary, stock movement report, and inventory value grouped by category.
- **Profile**: updates the current custom user and stores an uploaded profile image under `wwwroot/uploads`.

## Troubleshooting

### `dotnet` cannot target .NET 10 / `NETSDK1045`

Check installed SDKs:

```powershell
dotnet --list-sdks
```

Install the .NET 10 SDK and use Visual Studio 2026 18.0 or newer. Restart Visual Studio and the terminal after installation.

### Visual Studio cannot open `PrimeChip.slnx`

Update Visual Studio. The project targets .NET 10 and requires a modern Visual Studio/MSBuild that supports the `.slnx` format. As a fallback for inspection, open `PrimeChip/PrimeChip.csproj`, but the intended entry point is `PrimeChip.slnx`.

### NuGet restore fails

Confirm internet access and configured NuGet sources, then retry:

```powershell
dotnet nuget list source
dotnet restore .\PrimeChip.slnx
```

Do not build the contents of `PrimeChip.zip`; restore the live `PrimeChip.slnx` instead.

### `dotnet ef` is not recognized

Install the matching EF CLI tool and open a new terminal if necessary:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
dotnet ef --version
```

If another version is installed, use the `dotnet tool update` command shown in the database section.

### EF cannot find the project or `AppDbContext`

Run the command from the repository root and include both exact paths:

```powershell
dotnet ef database update --project .\PrimeChip\PrimeChip.csproj --startup-project .\PrimeChip\PrimeChip.csproj
```

The context is `PrimeChip.Data.AppDbContext`; there is no separate migrations project.

### Database connection fails or LocalDB is unavailable

Check the instance:

```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

Then confirm that `DefaultConnection` points to `(localdb)\MSSQLLocalDB`, not another developer's server. If `sqllocaldb` is missing, install SQL Server Express LocalDB through Visual Studio Installer.

### `PrimeChipDb` or its tables do not exist

Startup does not create the database. Run `Update-Database` in Package Manager Console or the exact `dotnet ef database update` command above. Do not use the account SQL files to create the schema.

### Database authentication or permission errors

The LocalDB connection uses Windows Authentication. Run Visual Studio, EF commands, and the app as the same Windows user who owns the LocalDB instance. Do not add SQL credentials to the checked-in file. If a custom full SQL Server instance is used, that Windows account must have permission to create/update `PrimeChipDb`.

### Migration errors

- Confirm `dotnet ef --version` reports 10.0.7.
- Restore and build before retrying.
- Ensure the command targets the live `PrimeChip/PrimeChip.csproj`, not the archived copy.
- Inspect `dbo.__EFMigrationsHistory` before adding or removing migrations.
- Do not use `EnsureCreated()` with this migration-based database.

### HTTPS development certificate error

Run this once in a normal user PowerShell window and accept the Windows prompt:

```powershell
dotnet dev-certs https --trust
```

### Login fails on a fresh database

This is expected because migrations do not seed a user. Do not run the committed SQL files as a general fix. Ask the project owner for an approved first-user provisioning process. A nonexistent email can currently cause a null-reference error because of the known login-controller defect.

### Icons or charts are missing

Bootstrap Icons and Chart.js are loaded from CDNs. Confirm that the PC can reach the CDN URLs and that a firewall, proxy, or content blocker is not preventing those requests.

## Development Notes

- Keep EF Core runtime, SQL Server provider, tools, and `dotnet-ef` on compatible 10.0 versions.
- Add a new EF migration whenever an entity/schema change is made; do not edit an already-applied migration casually.
- Do not add `EnsureCreated()` to bypass migrations. The repository is already migration-based.
- `AddSession()` appears twice in `Program.cs`; this is redundant but was left unchanged.
- Several model nullability warnings and login-controller warnings are present. The solution currently builds despite them.
- Some views reference first-party scripts that are absent (`wwwroot/js/reports.js` and `wwwroot/js/Vendors.js`), which can produce browser 404s but does not prevent the server from building.
- Uploaded files are stored directly under the web root. Validate file types, size limits, and access policy before production use.
- Do not commit `.vs/`, `*.csproj.user`, build output, database files, or personal configuration. The repository's `.gitignore` already excludes common Visual Studio files and SQL data files.
- The tracked `PrimeChip.zip` contains stale `bin/obj` output, an older migration history, and a generated machine-specific path. It is not a dependency and should not be used as the source of truth.

## Security and Repository Hygiene Findings

- The live `appsettings.json` uses a generic LocalDB connection with Windows Authentication and contains no database password.
- No API keys, bearer tokens, client secrets, private certificates, or custom secret environment variables were found in the live project files.
- The two tracked SQL files contain a fixed account email and BCrypt credential hashes. Even hashes are sensitive credential material and should not be reused or documented. Remove the scripts from Git/history if they are not intentionally retained, and rotate any account whose credentials they represent.
- The login implementation currently bypasses password verification and is unsafe for production.
- The diagnostic password-hash endpoint contains a hard-coded test password and should not be exposed in a deployed application.
- The tracked ZIP duplicates old source/build output and contains generated machine-specific metadata; remove it from version control if it is not intentionally distributed.

## Verification Performed

The repository was reviewed without changing application source code. The following checks succeeded on Windows with .NET SDK 10.0.400:

```powershell
dotnet restore .\PrimeChip.slnx
dotnet build .\PrimeChip.slnx --no-restore
dotnet ef migrations list --project .\PrimeChip\PrimeChip.csproj --startup-project .\PrimeChip\PrimeChip.csproj --no-connect
dotnet ef migrations script --idempotent --project .\PrimeChip\PrimeChip.csproj --startup-project .\PrimeChip\PrimeChip.csproj
dotnet ef migrations has-pending-model-changes --project .\PrimeChip\PrimeChip.csproj --startup-project .\PrimeChip\PrimeChip.csproj
```

The build completed with 17 compiler/Razor warnings and no errors. EF listed all five migrations, generated the migration SQL successfully, and reported no model changes since the last migration. A live `database update` was intentionally not run during documentation verification to avoid modifying an existing developer database; the migration chain was validated through EF's design-time commands instead.
