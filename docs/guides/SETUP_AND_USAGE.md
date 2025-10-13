# Fadebook - Setup and Usage Guide

## Table of Contents
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Initial Setup](#initial-setup)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Development Scripts](#development-scripts)
- [Testing](#testing)
- [CI/CD Automation](#cicd-automation)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before you start, ensure you have the following installed:

### Required
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js** (v18 or higher) - [Download](https://nodejs.org/)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop/)
- **Git** - [Download](https://git-scm.com/downloads)

### Optional (for enhanced features)
- **ReportGenerator** (for HTML test coverage reports) - Auto-installed via scripts
- **GitHub CLI** (for PR management) - [Download](https://cli.github.com/)

---

## Project Structure

```
TheNightOwls/
├── api/                      # .NET Web API backend
├── Api.Tests/                # Backend unit tests
├── fadebook-frontend/        # Next.js frontend
├── docs/                     # Documentation
│   ├── diagrams/             # Project diagrams (ERD, wireframes)
│   ├── guides/               # Setup and usage guides
│   └── standards/            # Coding standards
├── scripts/                  # Build and automation scripts
│   ├── cicd.sh              # CI/CD automation script
│   └── start-db.sh          # Database startup script
├── docker-compose.yml        # SQL Server container configuration
├── .env.example             # Environment variables template
└── api.sln                  # .NET solution file
```

---

## Initial Setup

### 1. Clone the Repository

```bash
git clone git@github.com:250908-NET/TheNightOwls.git
cd TheNightOwls
```

### 2. Set Up Environment Variables

Create a `.env` file in the **api/ directory**:

```bash
cp .env.example .env
```

The `.env` file should contain:
```env
CONNECTION_STRING="Server=127.0.0.1,1433;Database=<DB_NAME>;User Id=sa;Password=<PASSWORD>;TrustServerCertificate=True"
MSSQL_SA_PASSWORD="PASSWORD"
Google__ClientSecret=SECRET
Google__ClientId=CLIENT_ID
```

### 3. Install Frontend Dependencies

```bash
cd fadebook-frontend
npm install
cd ..
```

### 4. Restore Backend Dependencies

```bash
dotnet restore api.sln
```

---

## Database Setup

### Start the Database

The project uses SQL Server running in a Docker container.

```bash
./scripts/start-db.sh
```

This script will:
1. Check for required environment variables
2. Start SQL Server in Docker
3. Wait for the database to be ready
4. Drop and recreate the database (if exists)
5. Apply Entity Framework migrations

**Note:** On Windows, run this in Git Bash or WSL.

### Manual Database Management

If you need to manage the database manually:

```bash
# Start the container
docker-compose up -d

# Stop the container
docker-compose down

# View container logs
docker logs mssql-express

# Apply migrations manually
cd api
dotnet ef database update
```

### Create New Migrations

When you modify your data models:

```bash
cd api
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## Running the Application

### Option 1: Run Both Backend and Frontend

#### Terminal 1 - Backend API
```bash
cd api
dotnet run
```
The API will be available at `http://localhost:5288`

#### Terminal 2 - Frontend
```bash
cd fadebook-frontend
npm run dev
```
The frontend will be available at `http://localhost:3000`

### Option 2: Using CI/CD Script

```bash
./scripts/cicd.sh run
```

This builds and runs the API in one command.

---

## Development Scripts

### Backend Scripts (via cicd.sh)

```bash
# Full CI pipeline (clean, restore, build, test)
./scripts/cicd.sh all

# Build the solution
./scripts/cicd.sh build

# Run tests with coverage
./scripts/cicd.sh test

# Run tests (alternative method)
./scripts/cicd.sh test2

# Clean build artifacts
./scripts/cicd.sh clean

# Setup solution (first time)
./scripts/cicd.sh setup

# Generate HTML coverage report
./scripts/cicd.sh report

# Show help
./scripts/cicd.sh help
```

### Frontend Scripts

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

# Run tests
npm test

# Run tests with UI
npm run test:ui

# Generate test coverage
npm run test:coverage
```

---

## Testing

### Backend Tests

```bash
# Using CI/CD script (recommended)
./scripts/cicd.sh test

# Manual testing
cd Api.Tests
dotnet test --configuration Release
```

Test results will be saved in:
- `TestResults/test_results.trx` - Test execution results
- `TestResults/coverage.cobertura.xml` - Coverage data
- `TestResults/html/index.html` - HTML coverage report (auto-opens)

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

## CI/CD Automation

The `cicd.sh` script automates the entire build/test/deploy cycle.

### Common Workflows

#### Development Cycle
```bash
# Make code changes, then run:
./scripts/cicd.sh all
```

#### Quick Test
```bash
./scripts/cicd.sh test
```

#### Clean Build
```bash
./scripts/cicd.sh clean
./scripts/cicd.sh build
```

### Script Configuration

The script can be customized by editing these variables at the top of `scripts/cicd.sh`:

```bash
API_PROJECT_DIR="./api"
TEST_PROJECT_DIR="./Api.Tests"
SLN_FILE="./api.sln"
REPORT_BASE_DIR="./TestResults"
```

---

## Troubleshooting

### Database Connection Issues

**Problem:** Can't connect to SQL Server

**Solutions:**
1. Ensure Docker is running: `docker ps`
2. Check container status: `docker logs mssql-express`
3. Verify port 1433 is not in use: `netstat -an | grep 1433`
4. Restart the container: `docker-compose restart`

### Migration Errors

**Problem:** Entity Framework migrations fail

**Solutions:**
1. Ensure database is running: `docker ps`
2. Check connection string in `.env`
3. Delete migrations and recreate:
   ```bash
   cd api
   rm -rf Migrations/
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Frontend Build Errors

**Problem:** npm install fails or modules missing

**Solutions:**
1. Clear node_modules and reinstall:
   ```bash
   cd fadebook-frontend
   rm -rf node_modules package-lock.json
   npm install
   ```
2. Clear Next.js cache:
   ```bash
   rm -rf .next
   npm run dev
   ```

### Backend Build Errors

**Problem:** .NET build fails

**Solutions:**
1. Clean and restore:
   ```bash
   ./scripts/cicd.sh clean
   dotnet restore api.sln
   dotnet build api.sln
   ```
2. Check .NET version: `dotnet --version` (should be 9.0+)

### Port Already in Use

**Problem:** Port 5000 or 3000 already in use

**Solutions:**
1. Find and kill the process:
   ```bash
   # On Linux/Mac
   lsof -ti:5000 | xargs kill -9
   lsof -ti:3000 | xargs kill -9

   # On Windows (PowerShell)
   Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
   ```

### Docker Issues

**Problem:** Docker container won't start

**Solutions:**
1. Restart Docker Desktop
2. Remove and recreate container:
   ```bash
   docker-compose down -v
   docker-compose up -d
   ```
3. Check Docker logs: `docker logs mssql-express`

---

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Next.js Documentation](https://nextjs.org/docs)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Docker Documentation](https://docs.docker.com/)

---

## Team Members

- [Christian Brewer](https://github.com/BAXENdev)
- [Victor Torres](https://github.com/vitugo23)
- [Charles Trangay](https://github.com/DevPiece)
- [Dean Gelbaum](https://github.com/deanscottg)
- [Jeremiah Ogembo](https://github.com/jomlabs)
- [Muhiddin Kurbonov](https://github.com/muhiddinkurbonov)

---

## Quick Start Checklist

- [ ] Install all prerequisites
- [ ] Clone repository
- [ ] Create `.env` files (root and api/)
- [ ] Install frontend dependencies: `cd fadebook-frontend && npm install`
- [ ] Start database: `./scripts/start-db.sh`
- [ ] Run backend: `cd api && dotnet run`
- [ ] Run frontend: `cd fadebook-frontend && npm run dev`
- [ ] Access application at `http://localhost:3000`

---

**Need help?** Check the [Troubleshooting](#troubleshooting) section or contact a team member.
