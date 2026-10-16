# RaceDay API — Part 2

ASP.NET Core Web API for managing race events, categories, enrolments and results. The API uses SQL Server, Entity Framework Core, JWT bearer authentication and role-based authorisation.

## Project contents

- `RaceDayAPI/` — C# API source, models, controllers, EF Core context and Swagger configuration.
- `PROG SQL QUERY 1.sql` — provided database schema and sample data.
- `PROG SQL QUERY 2 TESTING.sql` — SQL verification queries.
- `RaceDay Organiser Test Setup.sql` — optional local-only helper to promote a registered test account to Organiser.
- `API_Endpoint_Plan.pdf` — planned endpoints.
- `PROG ERD.drawio.png` — database ERD.
- `.github/workflows/ci.yml` — GitHub Actions build and repository checks.

## Requirements

- .NET 8 SDK
- SQL Server and SQL Server Management Studio (SSMS)
- Visual Studio 2022 with the ASP.NET and web development workload, or VS Code with C# tooling

## 1. Database setup

Use the existing `RaceDayDB` database if you have already created it. Otherwise, create a database named `RaceDayDB` in SSMS, select it, then execute `PROG SQL QUERY 1.sql`. Execute `PROG SQL QUERY 2 TESTING.sql` afterwards to review the sample data and relationships.

**Important:** the SQL seed data contains placeholder strings such as `HASH_Thabo123` rather than ASP.NET Identity password hashes. Those seeded users cannot log in through this API. Register a fresh test account using Swagger first.

## 2. Configure the connection string and JWT secret

Open `RaceDayAPI/appsettings.json` and replace `YOUR_SQL_SERVER` in `ConnectionStrings:RaceDayConnection` with your SQL Server instance name. Examples may be `localhost`, `localhost\\SQLEXPRESS`, or `MACHINE_NAME\\SQLEXPRESS`; use the instance that works in your SSMS connection. The example uses Windows integrated authentication.

Change `Jwt:Key` to a private random secret at least 32 bytes long. Do not use a real production secret in a public repository. If you use SQL authentication instead of Windows authentication, update the connection string locally and do not commit credentials.

## 3. Run the API

From a terminal in the repository root:

```bash
dotnet restore
dotnet build ./RaceDayAPI/RaceDayAPI.csproj
dotnet run --project ./RaceDayAPI/RaceDayAPI.csproj
```

Alternatively, open `RaceDayAPI/RaceDayAPI.csproj` in Visual Studio and press Run. Swagger is configured at the launch URL. If the configured port is already in use, change `RaceDayAPI/Properties/launchSettings.json`.

The `/api/health` endpoint should return `{ "status": "ok", "service": "RaceDay API" }` when the application starts. This checks the web application only; database endpoints also require a working SQL Server connection.

## 4. Authentication and role testing

1. In Swagger, call `POST /api/Auth/register` with `firstName`, `lastName`, `email`, `password` (at least 8 characters), and optional `phone`. Registration assigns the `Participant` role by design.
2. Call `POST /api/Auth/login` using the new account's email and password. Copy the `token` value from the response.
3. Click **Authorize** in Swagger and paste the token only (do not type `Bearer `; Swagger adds it).
4. Participant access: profile endpoints and creating/viewing their own enrolments.
5. Organiser access: event and category creation/update/deletion, linking event categories, and managing results.
6. To test Organiser-only endpoints in a local development database, register a test account and run `RaceDay Organiser Test Setup.sql` after replacing its email placeholder. Then log in again to obtain a token reflecting the updated role. Never expose a production database or elevate a real user's role just to bypass access control.

## Endpoint coverage

- Authentication: `POST /api/Auth/register`, `POST /api/Auth/login`
- Profile: `GET /api/profile`, `PUT /api/profile`
- Events: `GET /api/events`, `GET /api/events/{id}`, `POST /api/events`, `PUT /api/events/{id}`, `DELETE /api/events/{id}`
- Categories: `GET /api/categories`, `GET /api/categories/{id}`, `POST /api/categories`, `PUT /api/categories/{id}`, `DELETE /api/categories/{id}`
- Event categories: `GET /api/events/{eventId}/categories`, `POST /api/events/{eventId}/categories`
- Enrolments: `POST /api/enrolments`, `GET /api/enrolments`, `GET /api/events/{eventId}/enrolments`, `GET /api/enrolments/{id}`, `DELETE /api/enrolments/{id}`
- Results: `GET /api/results`, `GET /api/results/{id}`, `GET /api/events/{eventId}/results`, `POST /api/results`, `PUT /api/results/{id}`, `DELETE /api/results/{id}`
- Additional reference data: `GET /api/routes`, `GET /api/routes/{id}`, `GET /api/weather`, `GET /api/weather/{id}`

## Schema alignment notes

The API models intentionally match the supplied SQL schema. `Events` has `EventName`, `OrganizerID`, `EventDate`, `Location`, and `Status`; it has no `Description` column. `Categories` has `CategoryName`, `DistanceKM`, and `ActivityType`; it has no `Description` column. `EventCategory` uses the composite primary key `(EventID, CategoryID)` and does not contain an `EventCategoryID` column. `Routes` uses `RouteName`, and `Weather` uses `Temp`. These alignments avoid querying columns that do not exist in the provided SQL tables.

## Verification status and submission checklist

This package was generated from the SQL and documentation included in the submitted ZIP. The current environment did not have the .NET SDK or a SQL Server instance available, so a local compile and live database/API test could not be performed here. Before submission, open the solution/project on your computer, set the correct SQL Server instance and JWT secret, run `dotnet restore` and `dotnet build`, start the API, and exercise the endpoints in Swagger. Fix any environment-specific errors before describing the build as successful.

- [ ] Set the SQL Server connection string and private JWT key locally.
- [ ] Run `dotnet restore` and `dotnet build` successfully.
- [ ] Confirm `/api/health` responds and database endpoints return data.
- [ ] Test registration/login, role restrictions, and required CRUD endpoints.
- [ ] Confirm the GitHub Actions workflow passes after pushing.
- [ ] Add the required demonstration video link and genuine screenshots/evidence.
- [ ] Commit and push all required files to the correct GitHub repository.
