# Da Horta API

Da Horta is a mobile-first hyperlocal marketplace for Portugal. Local producers publish surplus fresh produce, and nearby consumers can discover and reserve available stock for pickup directly at the farm.

The backend is built with ASP.NET Core 10 and PostgreSQL/PostGIS, with a modular-monolith structure focused on a clean domain-oriented design.

## Product vision

- Target market: Portugal
- Tone and copy: European Portuguese (PT-PT), not PT-BR
- Primary communication: WhatsApp deep links
- Payment model for MVP: cash on pickup (Pagamento no local)
- Geographic requirement: farms must provide precise GPS coordinates and a short landmark note (`AddressHint`)
- Listing lifecycle: simple, fast farm listing creation with short-lived product availability

## Current MVP scope

This stage focuses on the foundational backend experience:

- farm creation and retrieval
- listing creation and discovery
- basic product reservation flow
- geographic filtering with PostGIS
- OpenAPI and Scalar docs for API exploration

The following are intentionally not part of the current phase:

- automatic listing expiry jobs
- reservation lifecycle automation
- advanced payment flows
- full user authentication and authorization
- production-scale operational tooling

## Tech stack

- .NET 10 Web API
- ASP.NET Core MVC / API controllers
- Entity Framework Core 10
- PostgreSQL 16 + PostGIS
- Npgsql + NetTopologySuite
- Scalar.AspNetCore for API docs
- Docker Compose for local database setup

## Architecture

The project is organized around a layered backend structure:

- `src/DaHorta.Api/Domain/` — domain entities and enums
- `src/DaHorta.Api/Application/` — contracts, DTOs, abstractions, and service orchestration
- `src/DaHorta.Api/Infrastructure/` — persistence, repositories, middleware, and infrastructure concerns
- `src/DaHorta.Api/Presentation/` — controllers and HTTP surface
- `src/DaHorta.Api/Program.cs` — application startup and dependency wiring

This is intentionally a clean layered design that moves toward DDD ideas without over-engineering the MVP.

## Repository layout

```text
.
├── README.md
├── DaHorta.sln
├── compose.yaml
├── docs/
│   └── PROJECT.md (legacy pointer, kept for reference)
├── src/
│   └── DaHorta.Api/
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       ├── Presentation/
│       ├── Program.cs
│       └── DaHorta.Api.csproj
└── docker/
    └── postgres/
```

## Local setup

### Prerequisites

- .NET 10 SDK
- Docker Desktop / Docker Engine with Compose
- WSL or a Linux/macOS shell environment

### Start PostgreSQL + PostGIS

```bash
docker compose up -d
```

Check the service state:

```bash
docker compose ps
```

### Run the API

From the repo root:

```bash
dotnet restore
dotnet build
dotnet run --project src/DaHorta.Api
```

## Configuration

The application reads the database connection string from `appsettings.json` or environment variables. The default local development setup assumes a local PostgreSQL instance using the project defaults.

Example configuration style:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=dahorta;Username=dahorta;Password=dahorta_local"
  }
}
```

## API documentation

The API exposes OpenAPI metadata and a Scalar UI for local testing.

- OpenAPI JSON: `/openapi/v1.json`
- Interactive docs: `/scalar`
- Health check: `/health`

These are enabled in development / non-production environments.

## Core API endpoints

### Farms

- `GET /api/farms/{id}` — get farm by id
- `POST /api/farms` — create farm

### Listings

- `GET /api/listings` — list listings, optionally filtered by latitude, longitude, and radius
- `GET /api/listings/{id}` — get listing detail
- `POST /api/listings` — create listing
- `POST /api/listings/{id}/reservations` — reserve quantity

## Domain model

The core MVP domain objects are:

- `Farm`
- `Listing`
- `Reservation`
- related enums such as listing and reservation status

The data model uses `Point` with SRID `4326`, mapped to PostgreSQL `geography(point, 4326)` for accurate meter-based distance calculations.

## Development conventions

- Use European Portuguese text in all user-facing output.
- Keep controllers thin and delegate to the application layer.
- Use DTOs instead of exposing EF entities directly.
- Separate infrastructure, application, and domain concerns.
- Favor simple, testable service orchestration over unnecessary abstraction.

## Working checklist

### Completed

- [x] Initialize .NET solution and API project
- [x] Add PostgreSQL 16 + PostGIS compose config
- [x] Add health checks and startup configuration
- [x] Add EF Core + PostGIS + NetTopologySuite setup
- [x] Add domain entities and enums
- [x] Add repository and service abstractions
- [x] Add controllers for farms and listings
- [x] Add exception handling middleware
- [x] Add OpenAPI + Scalar docs
- [x] Add project documentation

### Planned for next implementation phase

- [ ] Add authentication and producer identity
- [ ] Add validation and edge-case handling for farms and listings
- [ ] Add automated tests for service and controller behavior
- [ ] Add dev seed data for local testing
- [ ] Add readme usage examples and sample payloads
- [ ] Add API versioning strategy if needed
- [ ] Add deployment configuration and environment profiles
- [ ] Add operational monitoring and structured logging
- [ ] Add background jobs for listing cleanup when the project needs them
- [ ] Add reservation lifecycle management when the next phase begins

### Explicitly deferred for this stage

- [ ] Auto-expiring listings
- [ ] Reservation lifecycle automation
- [ ] Payment flows
- [ ] Full marketplace commerce features

## Notes

This repository is intentionally focused on the backend MVP. The frontend is planned as a separate Angular PWA project and is not part of this codebase.

## Legacy note

The older project document was folded into this README. See [docs/PROJECT.md](docs/PROJECT.md) only as a historical pointer if needed.
