# SISMOGRAPHS

**Seismic Inspection Management Platform**

.NET 9 · React 19 · SQL Server · Clean Architecture · Observer Pattern

[![CI](https://github.com/baltasargaitan/sismographs/actions/workflows/ci.yml/badge.svg)](https://github.com/baltasargaitan/sismographs/actions)

---

## Overview

**Sismographs** is a full-stack platform for managing seismic station inspection orders and real-time event monitoring. It demonstrates professional software engineering practices through Clean Architecture principles, event-driven design using the Observer pattern, and secure configuration management.

The system enables operators to close inspection orders and automatically triggers multi-channel notifications (email, console, web dashboard) via a decoupled Observer implementation, showcasing the practical application of design patterns in production systems.

### Key Features

- **Order Lifecycle Management** — Create, schedule, and close inspection orders
- **Event-Driven Notifications** — Observer pattern enables extensible notification channels
- **Real-Time Monitoring** — Live dashboard polling for order events  
- **Secure Configuration** — Environment variables for secrets; no hardcoded credentials
- **Clean Architecture** — Strict separation of concerns across five layers
- **Modern Tech Stack** — Latest .NET and React with industry-standard tooling
- **Automated Testing** — xUnit test projects for domain and application logic
- **CI/CD Pipeline** — GitHub Actions validates every commit

---

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     React Frontend (Vite)                    │
│                                                              │
│  - Pages: Order Management, Inspection Close, Monitoring    │
│  - Components: Forms, Tables, Notifications, Real-time UI   │
│  - Styling: Tailwind CSS, Framer Motion animations          │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP/JSON
                           ↓
┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Core 9 REST API (Api Layer)             │
│                                                              │
│  - Controllers: REST endpoints for orders, closures         │
│  - CORS: Environment-based configuration                    │
│  - Swagger: API documentation                               │
│  - DI: Orchestrates all layers                              │
└──────────────────────────┬──────────────────────────────────┘
                           │
        ┌──────────────────┴──────────────────┐
        │                                     │
        ↓                                     ↓
┌───────────────────┐          ┌──────────────────────────┐
│  Application      │          │  Observer Notifications  │
│  (Aplicacion)     │          │                          │
│                   │          │ ├─ Email (SMTP/Mailjet)  │
│ - UseCases        │          │ ├─ Console (Logging)     │
│ - DTOs            │          │ └─ Web (In-memory Queue) │
│ - Interfaces      │          └──────────────────────────┘
│ - Services        │
└──────────────────┬┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ↓                     ↓
┌──────────────────┐  ┌──────────────────┐
│ Domain           │  │ Infrastructure   │
│ (Dominio)        │  │ (Infraestructura)│
│                  │  │                  │
│ - Entities       │  │ - DbContext      │
│ - Interfaces     │  │ - Repositories   │
│ - Repository     │  │ - Migrations     │
│   Contracts      │  │ - Config         │
└──────────────────┘  └────────┬─────────┘
                               │
                               ↓
                       ┌──────────────────┐
                       │  SQL Server      │
                       │                  │
                       │ SistemaSismografos│
                       │ Database         │
                       └──────────────────┘
```

### Layer Responsibilities

#### **Domain (Dominio) Layer**
Core business logic and data contracts. Zero external dependencies.

- **Entities**: `OrdenDeInspeccion`, `Empleado`, `EstacionSismologica`, `Sismografo`, etc.
- **Repository Interfaces**: Define data access contracts
- **Business Rules**: State transitions, validations (defined in entities)
- **Design Principle**: Entities are POCOs; no framework dependencies

#### **Application (Aplicacion) Layer**
Orchestration of business logic and notification workflows.

- **Use Cases**: `CerrarOrdenUseCase` orchestrates order closure
- **DTOs**: Request/response models for API contracts
- **Observers**: Concrete implementations of notification patterns
  - `ObservadorEmailSMTP` — Sends email via Mailjet SMTP
  - `ObservadorConsola` — Logs events to console
  - `ObservadorWebMonitor` — Queues events for dashboard polling
- **Interfaces**: Service contracts for seeding, sessions, notifications
- **Dependency Injection**: Wired in `Program.cs`, accessible to API

#### **Infrastructure (Infraestructura) Layer**
Data persistence and external service implementations.

- **DbContext**: Entity Framework Core with SQL Server provider
- **Repositories**: Concrete implementations of domain repository interfaces
- **Migrations**: EF Core-managed schema versioning (13 migrations)
- **Seeding**: Initial data population
- **Configuration**: Connection string override via environment variables

#### **API (Api) Layer**
HTTP interface and dependency orchestration.

- **Controllers**: REST endpoints; delegate to use cases
- **Program.cs**: DI container, middleware pipeline, observer subscription
- **Configuration**: appsettings.json with environment variable override
- **CORS**: Restricted to configured frontend origin (environment variable)
- **Swagger**: Auto-generated API documentation

#### **Frontend (frontend-react/)**
React Single Page Application with real-time capabilities.

- **Pages**: Home, Order Closure Form, Monitoring Dashboard
- **Components**: Reusable UI elements (forms, tables, notifications)
- **API Client**: Axios-based service for backend communication
- **State Management**: React hooks and component state
- **Styling**: Tailwind CSS with custom animations

---

## Observer Pattern Implementation

The system uses Observer to decouple order closure logic from notification delivery, enabling extensibility without modifying core business logic.

### Class Structure

```
IObservadorCierreOrden (interface)
  ├─ ObservadorEmailSMTP
  │   └─ Actualizar() → sends SMTP email
  │
  ├─ ObservadorConsola
  │   └─ Actualizar() → logs to console
  │
  └─ ObservadorWebMonitor
      └─ Actualizar() → queues event in ConcurrentQueue

ISujetoCierreOrden (subject)
  └─ SujetoCierreOrden
      ├─ Suscribir(observer)
      ├─ Desuscribir(observer)
      └─ Notificar(mensaje, destinatario) → calls all observers
```

### Event Flow

```
1. Frontend: User fills order closure form and submits
                            ↓
2. API: POST /api/cierreorden receives request
                            ↓
3. Application: CerrarOrdenUseCase.Execute(dto)
   - Validates order state
   - Updates domain entity
   - Calls repository to persist
                            ↓
4. Subject: _sujeto.Notificar(mensaje, email)
                            ↓
5. Observers (parallel execution):
   ├─ ObservadorEmailSMTP
   │  └─ Connects to Mailjet SMTP
   │  └─ Sends email notification
   │  └─ Logs success/failure (retries 3x)
   │
   ├─ ObservadorConsola
   │  └─ Prints event to console
   │  └─ Useful for local development/debugging
   │
   └─ ObservadorWebMonitor
      └─ Stores event in in-memory queue
      └─ Frontend polls /api/events for real-time updates
      └─ Limited to 100 most recent events
                            ↓
6. Response: API returns success to frontend
```

### Why Observer?

1. **Decoupling**: Order closure logic (`CerrarOrdenUseCase`) is completely independent of how notifications are delivered
2. **Extensibility**: Add new observers (SMS, Slack, Teams, push notifications) without touching `CerrarOrdenUseCase`
3. **Reliability**: If one observer fails (e.g., SMTP timeout), others continue executing
4. **Single Responsibility**: Each observer owns its specific notification channel
5. **Testability**: Observers can be mocked/replaced for testing; logic can be verified independently

---

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Runtime** | .NET | 9.0 |
| **Web Framework** | ASP.NET Core | 9.0.5 |
| **ORM** | Entity Framework Core | 9.0.10 |
| **Database** | SQL Server | Express or higher |
| **API Documentation** | Swagger/OpenAPI | 6.6.2 |
| **Email** | MailKit | 4.14.1 |
| **Frontend Framework** | React | 19.1.1 |
| **Build Tool** | Vite | 7.1.7 |
| **Routing** | React Router | 7.9.4 |
| **HTTP Client** | Axios | 1.12.2 |
| **Styling** | Tailwind CSS | 4.1.16 |
| **Animations** | Framer Motion | 12.23.24 |
| **Icons** | Lucide React | 0.552.0 |
| **Testing** | xUnit | 2.6+ |
| **Configuration** | DotNetEnv | 3.1.1 |

---

## Repository Structure

```
sismographs/
│
├── Dominio/                            # Domain layer
│   ├── Entidades/                      # Core business entities
│   └── Repositorios/                   # Repository interfaces
│
├── Aplicacion/                         # Application layer
│   ├── UseCases/
│   │   └── CerrarOrdenUseCase.cs
│   ├── Servicios/Notificaciones/
│   │   ├── SujetoCierreOrden.cs
│   │   ├── ObservadorEmailSMTP.cs
│   │   ├── ObservadorConsola.cs
│   │   └── ObservadorWebMonitor.cs
│   ├── DTOs/
│   ├── Interfaces/
│   └── Mocks/
│
├── Infraestructura/                    # Infrastructure layer
│   ├── Persistencia/
│   │   ├── AppDbContext.cs
│   │   ├── AppDbContextFactory.cs
│   │   ├── AppDbContextSeed.cs
│   │   └── Migrations/
│   └── Repositorios/
│
├── Api/                                # API layer
│   ├── Controllers/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── .env.example
│   └── Api.csproj
│
├── frontend-react/                     # React SPA
│   ├── src/
│   │   ├── pages/
│   │   ├── components/
│   │   └── api/
│   ├── vite.config.js
│   ├── tailwind.config.js
│   └── package.json
│
├── Tests/                              # Test projects
│   ├── Dominio.Tests/
│   ├── Aplicacion.Tests/
│   └── Api.Tests/
│
├── .github/workflows/
│   └── ci.yml
│
├── .gitignore
├── .env.example
└── SistemaSismografos.sln
```

---

## Local Development

### Prerequisites

- **.NET 9 SDK** — [Download](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- **SQL Server** — Express (free) or higher  
- **Node.js 18+** — [Download](https://nodejs.org/)
- **Git** — [Download](https://git-scm.com/)
- **IDE** — Visual Studio, VS Code, or JetBrains Rider

### Backend Setup

#### 1. Clone Repository

```bash
git clone https://github.com/baltasargaitan/sismographs.git
cd sismographs
```

#### 2. Configure Environment Variables

```bash
cp .env.example .env
```

Edit `.env` with your local settings:

```env
# Database (use your SQL Server instance)
ConnectionStrings__DefaultConnection=Server=localhost\SQLEXPRESS;Database=SistemaSismografosDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;

# SMTP (for email notifications - optional for local dev)
MAIL_USER=your-mailjet-api-key
MAIL_KEY=your-mailjet-secret-key
SMTP_FROM=noreply@yourdomain.com
SMTP_NAME=Sistema Sismografos

# Frontend origin (dev server URL)
FRONTEND_ORIGIN=http://localhost:5173
```

#### 3. Restore and Build

```bash
dotnet restore
dotnet build
```

#### 4. Create Database

```bash
cd Infraestructura
dotnet ef database update --context AppDbContext --project . --startup-project ..\Api
cd ..
```

#### 5. Run Backend

```bash
cd Api
dotnet run
```

**Backend available at:** `https://localhost:5001`  
**Swagger UI (API docs):** `https://localhost:5001`

### Frontend Setup

#### 1. Install Dependencies

```bash
cd frontend-react
npm ci
```

#### 2. Start Development Server

```bash
npm run dev
```

**Frontend available at:** `http://localhost:5173`

#### 3. Build for Production

```bash
npm run build
```

---

## Environment Configuration

### Required Environment Variables

| Variable | Purpose | Example |
|----------|---------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection | `Server=localhost\SQLEXPRESS;Database=SistemaSismografosDB;...` |
| `MAIL_USER` | Mailjet API key for SMTP | `your-mailjet-api-key` |
| `MAIL_KEY` | Mailjet secret key | `your-mailjet-secret-key` |
| `SMTP_FROM` | Email sender address | `noreply@yourdomain.com` |
| `SMTP_NAME` | Email sender display name | `Sistema Sismografos` |
| `FRONTEND_ORIGIN` | React dev server URL (CORS) | `http://localhost:5173` |

### Security Best Practices

- `.env` file contains secrets and is **NOT committed** to Git
- `.env.example` is committed with **placeholder values only**
- Never print secret values to console
- Environment variables override configuration files
- Production deployment uses secure secret management

---

## Testing

### Run Tests

```bash
dotnet test
```

### Test Projects

- **Dominio.Tests** — Domain entity and business rule tests
- **Aplicacion.Tests** — Application logic and use case tests
- **Api.Tests** — API endpoint and contract tests

### Test Candidates

1. Order state transitions and closure logic
2. Observer notification delivery to all subscribers
3. Multiple observers receiving same event
4. Observer error resilience (one failure doesn't block others)
5. API response structure and validation

---

## Continuous Integration

Every push and pull request triggers:

```bash
# Backend
dotnet restore
dotnet build --configuration Release
dotnet test

# Frontend
cd frontend-react
npm ci
npm run lint
npm run build
```

See [`.github/workflows/ci.yml`](.github/workflows/ci.yml) for full configuration.

---

## Architecture & Design Decisions

### 1. Clean Architecture
Five-layer separation enables testability, maintainability, and framework independence.

### 2. Observer Pattern
Decouple order closure logic from notification delivery; enables extensibility without modifying use cases.

### 3. Repository Pattern
Abstract data access via interfaces; enables swapping data stores without changing business logic.

### 4. Dependency Injection
ASP.NET Core built-in DI enables loose coupling and seamless testing with mocks.

### 5. Entity Framework Core
Type-safe ORM with LINQ queries; database agnostic schema versioning via migrations.

### 6. Environment-Based Configuration
Secure secrets handling: environment variables override configuration files; no hardcoded credentials.

### 7. React with Vite
Fast development server, modern tooling, instant hot module reloading for responsive UX development.

---

## Known Limitations & Future Work

1. **Testing Coverage** — Automated test coverage for core domain and application logic
2. **Containerization** — Docker support (Compose for local dev, Kubernetes for production)
3. **Monitoring** — Application Insights or similar telemetry for production observability
4. **Scalability** — Message queue for distributed notifications (RabbitMQ, Azure Service Bus)
5. **Mobile** — React Native or PWA for mobile inspection workflows
6. **Advanced Notifications** — SMS, Slack, Teams, push notifications as additional observers

---



## Author

**Baltasar Gaitan Acevedo** —  Software Engineer · Backend · Data · Integrations 

---

**Last Updated**: September 2026
