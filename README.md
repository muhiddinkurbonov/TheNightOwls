# Fadebook - Modern Barbershop Management System

<div align="center">

![Fadebook Logo](./fadebook-frontend/public/FadeBook%20Logo%20with%20Razor%20Icon.svg)

**A full-stack web application for barbershop appointment management**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15.5-000000?logo=next.js)](https://nextjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

[Features](#features) • [Quick Start](#quick-start) • [Documentation](#documentation) • [Team](#team)

</div>

---

## 📋 Table of Contents

- [About](#about)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Development](#development)
- [Testing](#testing)
- [Deployment](#deployment)
- [Team](#team)
- [License](#license)

---

## 🎯 About

**Fadebook** is a modern, full-stack web application designed to streamline barbershop operations. It enables customers to book appointments online, barbers to manage their schedules, and administrators to oversee the entire operation—all through an intuitive, responsive interface.

### Key Highlights

- 🗓️ **Real-time Appointment Management** - Book, view, and manage appointments with ease
- 👤 **Role-based Access Control** - Separate interfaces for customers, barbers, and administrators
- 🔐 **Secure Authentication** - JWT-based authentication with BCrypt password hashing
- 📱 **Responsive Design** - Seamless experience across desktop, tablet, and mobile devices
- 🎨 **Modern UI/UX** - Built with Next.js, Tailwind CSS, and shadcn/ui components
- ⚡ **High Performance** - Optimized with Turbopack and server-side rendering
- 🧪 **Well-Tested** - Comprehensive unit and integration tests (174+ backend tests)

---

## ✨ Features

### For Customers
- Browse available barbers and services
- Book appointments with preferred barbers
- View appointment history
- Manage profile and preferences
- Real-time availability checking

### For Barbers
- Manage work hours and availability
- View scheduled appointments
- Update appointment status
- Track service history
- Profile management

### For Administrators
- Manage barbers, services, and appointments
- View system-wide statistics and reports
- User management and role assignment
- Complete system oversight

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0 Web API
- **Language**: C# with nullable reference types
- **Database**: SQL Server Express (Docker)
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt.Net
- **Logging**: Serilog (Console + File)
- **Testing**: xUnit, Moq, FluentAssertions
- **Mapping**: AutoMapper

### Frontend
- **Framework**: Next.js 15.5 (App Router)
- **Language**: TypeScript 5.0
- **Styling**: Tailwind CSS
- **UI Components**: shadcn/ui
- **State Management**: React Hooks, Context API
- **HTTP Client**: Axios
- **Testing**: Vitest, React Testing Library

### Infrastructure
- **Containerization**: Docker, Docker Compose
- **Database**: SQL Server Express 2022
- **Build Tool**: .NET CLI, npm/pnpm
- **CI/CD**: Custom automation scripts

---

## 🚀 Quick Start

### Prerequisites

Ensure you have the following installed:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/250908-NET/TheNightOwls.git
   cd TheNightOwls
   ```

2. **Set up environment variables**
   ```bash
   # Root .env for Docker Compose
   cp .env.example .env

   # API .env for application configuration
   cp api/.env.example api/.env

   # Edit both files with your configuration
   ```

3. **Install dependencies**
   ```bash
   # Backend dependencies
   dotnet restore api.sln

   # Frontend dependencies
   cd fadebook-frontend
   npm install
   cd ..
   ```

4. **Start the database**
   ```bash
   ./scripts/start-db.sh
   ```

5. **Run the application**

   **Terminal 1 - Backend:**
   ```bash
   cd api
   dotnet run
   ```

   **Terminal 2 - Frontend:**
   ```bash
   cd fadebook-frontend
   npm run dev
   ```

6. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5288
   - API Documentation: http://localhost:5288/swagger

### Default Accounts

| Role     | Username          | Password    |
|----------|-------------------|-------------|
| Admin    | admin             | admin123    |
| Barber   | dean.barber       | barber123   |
| Customer | john.customer     | customer123 |

---

## 📁 Project Structure

```
TheNightOwls/
├── api/                        # .NET Web API backend
│   ├── Common/                 # Shared utilities, constants, converters
│   ├── Controllers/            # API controllers
│   ├── DTOs/                   # Data Transfer Objects (organized by feature)
│   ├── DB/                     # Database context
│   ├── Middleware/             # Custom middleware
│   ├── Migrations/             # EF Core migrations
│   ├── Models/                 # Entity models
│   ├── Repositories/           # Data access layer
│   └── Services/               # Business logic layer
│
├── Api.Tests/                  # Backend unit tests
│   ├── Controllers/            # Controller tests
│   ├── Services/               # Service tests
│   └── Repositories/           # Repository tests
│
├── fadebook-frontend/          # Next.js frontend
│   ├── src/
│   │   ├── app/                # Next.js App Router pages
│   │   ├── components/         # React components
│   │   ├── hooks/              # Custom React hooks
│   │   ├── lib/                # Utilities and API client
│   │   ├── providers/          # Context providers
│   │   └── types/              # TypeScript type definitions
│   └── tests/                  # Frontend tests
│
├── docs/                       # 📚 Documentation
│   ├── diagrams/               # ERD, wireframes, flow diagrams
│   ├── guides/                 # Setup and usage guides
│   ├── standards/              # Coding standards and conventions
│   ├── ARCHITECTURE.md         # System architecture documentation
│   ├── PROJECT_ORGANIZATION.md # Project organization plan
│   └── PROJECT_REQUIREMENTS.md # Original project requirements
│
├── scripts/                    # 🔧 Build and automation scripts
│   ├── cicd.sh                 # CI/CD automation
│   └── start-db.sh             # Database initialization
│
├── TestResults/                # Test results and coverage reports
├── .dockerignore               # Docker build context exclusions
├── .editorconfig               # Editor configuration
├── .env.example                # Environment variables template for Docker
├── .gitignore                  # Git ignore rules
├── docker-compose.yml          # Docker services configuration
├── api.sln                     # .NET solution file
├── LICENSE                     # MIT License
├── CONTRIBUTING.md             # Contribution guidelines
└── README.md                   # This file

```

---

## 📚 Documentation

### For Developers

- **[Setup and Usage Guide](docs/guides/SETUP_AND_USAGE.md)** - Detailed setup instructions, development workflows
- **[Architecture Documentation](docs/ARCHITECTURE.md)** - System design, patterns, and technical decisions
- **[Coding Standards](docs/standards/CODING_STANDARDS.md)** - Naming conventions, best practices
- **[Project Organization](docs/PROJECT_ORGANIZATION.md)** - Folder structure and organization guidelines

### Diagrams

- **[Entity Relationship Diagram](docs/diagrams/ERD-and-USER-STORIES.svg)** - Database schema and relationships
- **[Webpage Flow Diagram](docs/diagrams/Webpage%20Flow%20Diagram.excalidraw)** - User flow and navigation
- **[Page Wire Diagram](docs/diagrams/Page%20Wire%20Diagram.svg)** - UI wireframes

### API Documentation

- **Swagger UI**: Available at `http://localhost:5288/swagger` when running the API
- **Endpoints**: RESTful API with comprehensive CRUD operations

---

## 💻 Development

### Backend Development

```bash
# Restore dependencies
dotnet restore api.sln

# Build the solution
dotnet build api.sln

# Run the API
cd api
dotnet run

# Run with watch mode (auto-reload)
dotnet watch run

# Create new migration
cd api
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Frontend Development

```bash
cd fadebook-frontend

# Start development server with Turbopack
npm run dev

# Build for production
npm run build

# Start production server
npm start

# Run linter
npm run lint
```

### Using CI/CD Scripts

```bash
# Full CI pipeline (clean, restore, build, test)
./scripts/cicd.sh all

# Build only
./scripts/cicd.sh build

# Run tests with coverage
./scripts/cicd.sh test

# Generate HTML coverage report
./scripts/cicd.sh report

# Show help
./scripts/cicd.sh help
```

---

## 🧪 Testing

### Backend Tests

```bash
# Using CI/CD script (recommended)
./scripts/cicd.sh test

# Manual testing
cd Api.Tests
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

**Test Coverage**: 174+ tests with comprehensive coverage

Test results available in `TestResults/`:
- `test_results.trx` - Test execution results
- `coverage.cobertura.xml` - Coverage data
- `html/index.html` - HTML coverage report

### Frontend Tests

```bash
cd fadebook-frontend

# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run with coverage
npm run test:coverage

# Run with UI
npm run test:ui
```

---

## 🚢 Deployment

### Database

The application uses SQL Server running in Docker:

```bash
# Start the database
./scripts/start-db.sh

# Stop the database
docker-compose down

# View database logs
docker logs mssql-express
```

### Production Build

```bash
# Backend
cd api
dotnet publish -c Release -o ./publish

# Frontend
cd fadebook-frontend
npm run build
npm start
```

---

## 👥 Team

**The Night Owls Development Team**

- [Christian Brewer](https://github.com/BAXENdev) - Team Lead
- [Victor Torres](https://github.com/vitugo23) - Full-Stack Developer
- [Charles Trangay](https://github.com/DevPiece) - Backend Developer
- [Dean Gelbaum](https://github.com/deanscottg) - Full-Stack Developer
- [Jeremiah Ogembo](https://github.com/jomlabs) - Frontend Developer
- [Muhiddin Kurbonov](https://github.com/muhiddinkurbonov) - Full-Stack Developer

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Revature** - For providing the project requirements and training
- **Cohort 250908-NET** - For collaboration and support
- **Open Source Community** - For the amazing tools and libraries

---

## 📞 Support

For questions, issues, or contributions, please:

1. Check the [Documentation](docs/)
2. Review [Troubleshooting Guide](docs/guides/SETUP_AND_USAGE.md#troubleshooting)
3. Open an issue on GitHub
4. Contact a team member

---

<div align="center">

Made with ❤️ by The Night Owls

**[⬆ Back to Top](#fadebook---modern-barbershop-management-system)**

</div>
