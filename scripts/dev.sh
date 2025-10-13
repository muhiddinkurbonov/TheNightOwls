#!/bin/bash
#
# Fadebook - Complete Development Environment Setup & Run Script
# Usage: ./scripts/dev.sh [command]
#
# This script orchestrates the entire development environment including
# database, backend API, and frontend application.
#

set -e  # Exit on error

# --- Colors for Output ---
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m'

# --- Configuration ---
# Automatically detect project root (parent of scripts directory)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
API_DIR="${PROJECT_ROOT}/Fadebook.Api"
FRONTEND_DIR="${PROJECT_ROOT}/Fadebook.Frontend"
TEST_DIR="${PROJECT_ROOT}/Fadebook.Api.Tests"
SLN_FILE="${PROJECT_ROOT}/Fadebook.sln"
COMPOSE_FILE="${PROJECT_ROOT}/docker-compose.yml"

# Test reporting configuration
REPORT_BASE_DIR="${PROJECT_ROOT}/TestResults"
TRX_REPORT_FILE="test_results.trx"
COVERAGE_REPORT_FILE="coverage.cobertura.xml"
HTML_REPORT_SUBDIR="html"
HTML_REPORT_FILE="index.html"

# --- Helper Functions ---

log() {
    echo -e "${GREEN}[$(date +'%H:%M:%S')]${NC} $1"
}

info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
    exit 1
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

section() {
    echo ""
    echo -e "${CYAN}============================================${NC}"
    echo -e "${CYAN}  $1${NC}"
    echo -e "${CYAN}============================================${NC}"
}

# --- Environment Detection ---

detect_os() {
    case "$(uname -s)" in
        Linux*)     echo "linux";;
        Darwin*)    echo "macos";;
        CYGWIN*|MINGW*|MSYS*) echo "windows";;
        *)          echo "unknown";;
    esac
}

OS_TYPE=$(detect_os)

# Check if a port is in use
is_port_in_use() {
    local port=$1
    if [[ "$OS_TYPE" == "windows" ]]; then
        netstat -an | grep ":${port}" | grep -q "LISTENING"
    else
        lsof -ti:${port} >/dev/null 2>&1
    fi
}

# Kill process using a port
kill_port() {
    local port=$1
    if is_port_in_use $port; then
        warning "Port ${port} is in use, attempting to free it..."
        if [[ "$OS_TYPE" == "windows" ]]; then
            local pid=$(netstat -ano | grep ":${port}" | grep "LISTENING" | awk '{print $5}' | head -1)
            [ -n "$pid" ] && taskkill //F //PID "$pid" 2>/dev/null || true
        else
            lsof -ti:${port} | xargs kill -9 2>/dev/null || true
        fi
        sleep 1
        success "Port ${port} freed"
    fi
}

# --- Prerequisites Check ---

check_prerequisites() {
    section "Checking Prerequisites"

    local missing=0

    if command -v dotnet >/dev/null 2>&1; then
        local dotnet_version=$(dotnet --version)
        success ".NET SDK installed: ${dotnet_version}"
    else
        error ".NET SDK not found. Install from: https://dotnet.microsoft.com/download"
        missing=1
    fi

    if command -v node >/dev/null 2>&1; then
        local node_version=$(node --version)
        success "Node.js installed: ${node_version}"
    else
        error "Node.js not found. Install from: https://nodejs.org"
        missing=1
    fi

    if command -v docker >/dev/null 2>&1; then
        success "Docker installed"
        if ! docker ps >/dev/null 2>&1; then
            error "Docker is installed but not running. Please start Docker Desktop"
            missing=1
        fi
    else
        error "Docker not found. Install from: https://www.docker.com/products/docker-desktop"
        missing=1
    fi

    if [ $missing -eq 1 ]; then
        error "Missing required prerequisites. Please install them and try again."
    fi

    success "All prerequisites installed and running"
}

# --- Database Functions ---

wait_for_sql_server() {
    log "Waiting for SQL Server to be ready..."
    local MAX_RETRIES=30
    local RETRY_COUNT=0

    until docker logs mssql-express 2>&1 | grep -q "Recovery is complete"; do
        RETRY_COUNT=$((RETRY_COUNT + 1))
        if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
            error "SQL Server failed to start after $MAX_RETRIES attempts"
        fi
        echo "  Waiting for SQL Server... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
        sleep 2
    done

    success "SQL Server is ready!"
}

setup_database() {
    section "Setting up Database"

    # Check for .env in API directory
    if [ ! -f "${API_DIR}/.env" ]; then
        if [ -f "${API_DIR}/.env.example" ]; then
            warning "Creating .env from .env.example..."
            cp "${API_DIR}/.env.example" "${API_DIR}/.env"
            warning "Please update ${API_DIR}/.env with your configuration"
        fi
    fi

    # Load environment variables to check them
    if [ -f "${API_DIR}/.env" ]; then
        set +e  # Don't exit on error temporarily
        source "${API_DIR}/.env" 2>/dev/null
        set -e

        if [ -z "$MSSQL_SA_PASSWORD" ]; then
            warning "MSSQL_SA_PASSWORD not set in ${API_DIR}/.env"
        fi
        if [ -z "$CONNECTION_STRING" ]; then
            warning "CONNECTION_STRING not set in ${API_DIR}/.env"
        fi
    fi

    if docker ps | grep -q mssql-express; then
        info "Database container already running"
    else
        log "Starting SQL Server container..."
        docker-compose -f "${COMPOSE_FILE}" up -d

        wait_for_sql_server
    fi
}

setup_database_fresh() {
    section "Setting up Database (Fresh Install)"

    # Check environment
    if [ ! -f "${API_DIR}/.env" ]; then
        error "No .env file found in ${API_DIR}. Please create one from .env.example"
    fi

    # Load environment variables
    set +e
    source "${API_DIR}/.env"
    set -e

    if [ -z "$MSSQL_SA_PASSWORD" ] || [ -z "$CONNECTION_STRING" ]; then
        error "MSSQL_SA_PASSWORD and CONNECTION_STRING must be set in ${API_DIR}/.env"
    fi

    # Start container
    log "Starting SQL Server container..."
    docker-compose -f "${COMPOSE_FILE}" up -d

    wait_for_sql_server

    # Drop and recreate database
    log "Dropping and recreating database..."
    cd "${API_DIR}"
    dotnet ef database drop --force 2>/dev/null || info "No existing database to drop"

    # Check for migrations
    if [ ! -d "Migrations" ]; then
        log "No migrations found. Creating initial migration..."
        dotnet ef migrations add InitialCreate
    fi

    # Apply migrations with pending model change detection
    log "Applying migrations..."
    set +e
    UPDATE_OUTPUT=$(dotnet ef database update 2>&1)
    UPDATE_EXIT_CODE=$?
    set -e

    if [ $UPDATE_EXIT_CODE -ne 0 ] && echo "$UPDATE_OUTPUT" | grep -q "PendingModelChangesWarning"; then
        warning "Pending model changes detected!"
        MIGRATION_NAME="ModelChanges_$(date +%Y%m%d_%H%M%S)"
        log "Creating migration: $MIGRATION_NAME"
        dotnet ef migrations add "$MIGRATION_NAME"

        log "Applying new migration..."
        dotnet ef database update
    elif [ $UPDATE_EXIT_CODE -ne 0 ]; then
        error "Failed to apply migrations:\n$UPDATE_OUTPUT"
    fi

    cd "${PROJECT_ROOT}"
    success "Database setup complete!"
}

stop_database() {
    section "Stopping Database"
    docker-compose -f "${COMPOSE_FILE}" down
    success "Database stopped"
}

# --- Backend Functions ---

setup_backend() {
    section "Setting up Backend"

    # Check for .env file
    if [ ! -f "${API_DIR}/.env" ]; then
        if [ -f "${API_DIR}/.env.example" ]; then
            log "Creating .env file from template..."
            cp "${API_DIR}/.env.example" "${API_DIR}/.env"
            warning "Please update ${API_DIR}/.env with your configuration"
        else
            warning "No .env.example found in ${API_DIR}"
        fi
    else
        info ".env file exists"
    fi

    log "Restoring NuGet packages..."
    dotnet restore "${SLN_FILE}"

    log "Building solution..."
    dotnet build "${SLN_FILE}" --no-restore

    log "Applying database migrations..."
    cd "${API_DIR}"
    if dotnet ef database update; then
        success "Migrations applied successfully"
    else
        warning "Failed to apply migrations. Database may need to be running."
    fi
    cd "${PROJECT_ROOT}"

    success "Backend setup complete"
}

build_backend() {
    section "Building Backend"
    log "Building solution in Release mode..."
    dotnet build "${SLN_FILE}" --configuration Release
    success "Backend build complete"
}

start_backend() {
    section "Starting Backend API"

    kill_port 5288

    cd "${API_DIR}"
    log "Starting .NET API on http://localhost:5288"
    log "Swagger documentation: http://localhost:5288/swagger"
    dotnet run --launch-profile "http"
}

# --- Frontend Functions ---

setup_frontend() {
    section "Setting up Frontend"

    cd "${FRONTEND_DIR}"

    if [ ! -d "node_modules" ]; then
        log "Installing npm dependencies..."
        npm install
        success "Dependencies installed"
    else
        info "Dependencies already installed"
    fi

    cd "${PROJECT_ROOT}"
    success "Frontend setup complete"
}

build_frontend() {
    section "Building Frontend"
    cd "${FRONTEND_DIR}"
    log "Building Next.js application..."
    npm run build
    cd "${PROJECT_ROOT}"
    success "Frontend build complete"
}

start_frontend() {
    section "Starting Frontend"

    kill_port 3000

    cd "${FRONTEND_DIR}"
    log "Starting Next.js on http://localhost:3000"
    npm run dev
}

# --- Test & Coverage Functions ---

convert_to_win_path() {
    local shell_path="$1"
    local converted_path="${shell_path}"

    if command -v cygpath >/dev/null 2>&1; then
        converted_path=$(cygpath -w "${shell_path}")
    elif command -v wslpath >/dev/null 2>&1; then
        converted_path=$(wslpath -w "${shell_path}")
    fi
    echo "${converted_path}"
}

install_report_generator() {
    if ! command -v reportgenerator &> /dev/null; then
        warning "ReportGenerator not found. Installing dotnet global tool..."
        dotnet tool install -g ReportGenerator
        if [ $? -ne 0 ]; then
            error "Failed to install ReportGenerator. HTML report generation will fail."
            return 1
        fi
        success "ReportGenerator installed successfully."
    else
        info "ReportGenerator is already installed."
    fi
    return 0
}

clean_test_results() {
    log "Cleaning up test results directory..."
    if [ -d "${REPORT_BASE_DIR}" ]; then
        rm -rf "${REPORT_BASE_DIR}"
        info "Test results directory removed"
    fi
    mkdir -p "${REPORT_BASE_DIR}"
}

find_and_consolidate_coverage() {
    local target_file="$1"
    local search_dir="$2"

    log "Searching for coverage files..."

    # Find all cobertura.xml files
    local coverage_files=$(find "${search_dir}" -name "*.cobertura.xml" -type f ! -path "*/TestReport/*" | head -5)

    if [ -z "${coverage_files}" ]; then
        warning "No coverage files found in ${search_dir}"
        return 1
    fi

    # Use the first found coverage file
    local first_coverage=$(echo "${coverage_files}" | head -1)

    if [ -n "${first_coverage}" ] && [ -f "${first_coverage}" ]; then
        info "Using coverage file: ${first_coverage}"

        # Copy to target location
        mkdir -p "$(dirname "${target_file}")"
        cp "${first_coverage}" "${target_file}"
        success "Coverage file consolidated to: ${target_file}"
        return 0
    else
        warning "Could not find a valid coverage file"
        return 1
    fi
}

generate_html_report() {
    local coverage_file="$1"
    local output_dir="$2"

    log "Generating HTML coverage report..."

    if [ ! -f "${coverage_file}" ]; then
        warning "Coverage file not found: ${coverage_file}"
        return 1
    fi

    local coverage_win_path=$(convert_to_win_path "${coverage_file}")
    local output_win_path=$(convert_to_win_path "${output_dir}")

    reportgenerator \
        -reports:"${coverage_win_path}" \
        -targetdir:"${output_win_path}" \
        -reporttypes:Html

    if [ $? -eq 0 ]; then
        success "HTML coverage report generated successfully."
        return 0
    else
        warning "Failed to generate HTML coverage report."
        return 1
    fi
}

open_report_in_browser() {
    local report_path="$1"
    log "Opening HTML test report..."

    if [ ! -f "${report_path}" ]; then
        warning "HTML test report not found at ${report_path}"
        return 1
    fi

    if command -v open >/dev/null 2>&1; then
        open "${report_path}"
    elif command -v xdg-open >/dev/null 2>&1; then
        xdg-open "${report_path}"
    elif command -v start >/dev/null 2>&1; then
        start "$(convert_to_win_path "${report_path}")"
    else
        info "Please view the report manually at: ${report_path}"
        return 0
    fi
    success "HTML Report opened in the default browser."
    return 0
}

clean_extra_directories() {
    # Remove all directories except our report subdirectory
    find "${REPORT_BASE_DIR}" -mindepth 1 -maxdepth 1 -type d ! -name "${HTML_REPORT_SUBDIR}" -exec rm -rf {} + 2>/dev/null || true
}

show_test_results_summary() {
    echo ""
    success "Test Results Summary:"
    echo "Location: ${REPORT_BASE_DIR}"
    echo ""

    if [ -f "${REPORT_BASE_DIR}/${TRX_REPORT_FILE}" ]; then
        echo "  ✅ ${TRX_REPORT_FILE} - Test results"
    fi
    if [ -f "${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}" ]; then
        echo "  ✅ ${COVERAGE_REPORT_FILE} - Coverage data"
    fi
    if [ -f "${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}/${HTML_REPORT_FILE}" ]; then
        echo "  ✅ HTML Report - ${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}/${HTML_REPORT_FILE}"
    fi
    echo ""
}

run_tests() {
    section "Running Tests with Coverage"

    # Clean up completely
    clean_test_results

    # Define direct paths
    local trx_path="${REPORT_BASE_DIR}/${TRX_REPORT_FILE}"
    local coverage_path="${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}"
    local html_report_dir="${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}"

    log "Test Results: ${trx_path}"
    log "Coverage Report: ${coverage_path}"
    log "HTML Report: ${html_report_dir}/${HTML_REPORT_FILE}"

    # Run backend tests with coverage
    log "Running backend tests with coverage..."
    dotnet test ${TEST_DIR} \
        --configuration Release \
        --verbosity normal \
        --logger "trx;LogFileName=${trx_path}" \
        --collect:"XPlat Code Coverage" \
        --results-directory "${REPORT_BASE_DIR}" \
        --settings ${TEST_DIR}/.runsettings

    local test_exit_code=$?

    if [ ${test_exit_code} -eq 0 ]; then
        success "Backend tests passed!"

        # Consolidate coverage file
        if find_and_consolidate_coverage "${coverage_path}" "${REPORT_BASE_DIR}"; then
            # Generate HTML report
            if install_report_generator && generate_html_report "${coverage_path}" "${html_report_dir}"; then
                # Try to open the report
                local html_report_path="${html_report_dir}/${HTML_REPORT_FILE}"
                if [ -f "${html_report_path}" ]; then
                    open_report_in_browser "${html_report_path}"
                fi
            fi
        fi

        # Clean up extra directories
        clean_extra_directories

        # Show final summary
        show_test_results_summary

    else
        error "Backend tests failed. Review the results."
    fi

    # Run frontend tests
    log "Running frontend tests..."
    cd "${FRONTEND_DIR}"
    npm test || warning "Frontend tests failed or not configured"
    cd "${PROJECT_ROOT}"

    return ${test_exit_code}
}

regenerate_coverage_report() {
    section "Regenerating HTML Coverage Report"

    local coverage_file="${REPORT_BASE_DIR}/${COVERAGE_REPORT_FILE}"
    local html_report_dir="${REPORT_BASE_DIR}/${HTML_REPORT_SUBDIR}"

    # First try the expected location, then search for any coverage file
    if [ ! -f "${coverage_file}" ]; then
        warning "Coverage file not found at expected location, searching..."
        if ! find_and_consolidate_coverage "${coverage_file}" "${REPORT_BASE_DIR}"; then
            error "No coverage file found. Run tests first: $0 test"
        fi
    fi

    if install_report_generator && generate_html_report "${coverage_file}" "${html_report_dir}"; then
        local html_report_path="${html_report_dir}/${HTML_REPORT_FILE}"
        if [ -f "${html_report_path}" ]; then
            open_report_in_browser "${html_report_path}"
        else
            error "HTML report generation failed - main file not found."
        fi
    fi

    success "Coverage report regenerated"
}

# --- Combined Operations ---

full_setup() {
    section "Complete First-Time Setup"

    check_prerequisites
    setup_database
    setup_backend
    setup_frontend

    echo ""
    success "✓✓✓ SETUP COMPLETE ✓✓✓"
    echo ""
    info "Next steps:"
    echo "  1. Verify ${API_DIR}/.env has correct database settings"
    echo "  2. Run: ./scripts/dev.sh start"
    echo ""
}

start_all() {
    section "Starting All Services"

    check_prerequisites

    # Start database
    setup_database

    # Start backend in background
    log "Starting backend API..."
    kill_port 5288
    cd "${API_DIR}"
    dotnet run --launch-profile "http" > "${PROJECT_ROOT}/logs/backend.log" 2>&1 &
    BACKEND_PID=$!
    cd "${PROJECT_ROOT}"

    # Wait for backend to start
    log "Waiting for backend to start..."
    sleep 5

    # Start frontend in background
    log "Starting frontend..."
    kill_port 3000
    cd "${FRONTEND_DIR}"
    npm run dev > "${PROJECT_ROOT}/logs/frontend.log" 2>&1 &
    FRONTEND_PID=$!
    cd "${PROJECT_ROOT}"

    # Wait for frontend to start
    sleep 3

    echo ""
    success "✓✓✓ ALL SERVICES STARTED ✓✓✓"
    echo ""
    echo -e "${CYAN}Service URLs:${NC}"
    echo "  Backend API:  http://localhost:5288"
    echo "  Swagger Docs: http://localhost:5288/swagger"
    echo "  Frontend:     http://localhost:3000"
    echo ""
    echo -e "${CYAN}Logs:${NC}"
    echo "  Backend:  ${PROJECT_ROOT}/logs/backend.log"
    echo "  Frontend: ${PROJECT_ROOT}/logs/frontend.log"
    echo ""
    info "Press Ctrl+C to stop all services"

    # Setup trap to cleanup on Ctrl+C
    trap "
        echo ''
        log 'Stopping all services...'
        kill $BACKEND_PID $FRONTEND_PID 2>/dev/null || true
        docker-compose -f '${COMPOSE_FILE}' down
        success 'All services stopped'
        exit 0
    " INT TERM

    # Keep script running
    wait
}

stop_all() {
    section "Stopping All Services"

    log "Stopping backend processes..."
    pkill -f "dotnet run" 2>/dev/null || true
    pkill -f "dotnet.*Fadebook.Api" 2>/dev/null || true

    log "Stopping frontend processes..."
    pkill -f "npm run dev" 2>/dev/null || true
    pkill -f "next dev" 2>/dev/null || true

    log "Stopping database..."
    docker-compose -f "${COMPOSE_FILE}" down

    sleep 2
    success "All services stopped"
}

restart_all() {
    stop_all
    sleep 2
    start_all
}

# --- Clean Operations ---

clean_all() {
    section "Cleaning All Build Artifacts"

    log "Cleaning backend..."
    dotnet clean "${SLN_FILE}"

    log "Cleaning frontend..."
    cd "${FRONTEND_DIR}"
    rm -rf .next node_modules
    cd "${PROJECT_ROOT}"

    log "Cleaning database..."
    docker-compose -f "${COMPOSE_FILE}" down -v

    log "Cleaning test results..."
    rm -rf "${PROJECT_ROOT}/TestResults"

    log "Cleaning logs..."
    rm -rf "${PROJECT_ROOT}/logs"

    success "Clean complete"
}

# --- Status Functions ---

show_status() {
    section "System Status"
    echo ""

    # Database
    if docker ps | grep -q mssql-express; then
        echo -e "  Database:  ${GREEN}● Running${NC}"
    else
        echo -e "  Database:  ${RED}○ Stopped${NC}"
    fi

    # Backend
    if is_port_in_use 5288; then
        echo -e "  Backend:   ${GREEN}● Running${NC} (http://localhost:5288)"
    else
        echo -e "  Backend:   ${RED}○ Stopped${NC}"
    fi

    # Frontend
    if is_port_in_use 3000; then
        echo -e "  Frontend:  ${GREEN}● Running${NC} (http://localhost:3000)"
    else
        echo -e "  Frontend:  ${RED}○ Stopped${NC}"
    fi

    echo ""
}

# --- Help ---

show_help() {
    cat << EOF

${CYAN}Fadebook Development Script${NC}
Complete orchestration of development environment

${YELLOW}USAGE:${NC}
  ./scripts/dev.sh [command]

${YELLOW}COMMANDS:${NC}

  ${GREEN}Main Commands:${NC}
    setup         Complete first-time setup (db, backend, frontend)
    start         Start all services (database, backend, frontend)
    stop          Stop all running services
    restart       Restart all services
    status        Show status of all services

  ${GREEN}Build & Test:${NC}
    build         Build all projects (backend + frontend)
    test          Run all tests with coverage reports
    coverage      Regenerate HTML coverage report from existing data
    clean         Clean all build artifacts and containers

  ${GREEN}Individual Services:${NC}
    db            Start only database
    backend       Start only backend API
    frontend      Start only frontend

  ${GREEN}Setup Steps:${NC}
    check         Check prerequisites only
    setup-db      Setup database only (quick start)
    setup-db-fresh Setup database with drop/recreate and migrations
    setup-api     Setup backend only
    setup-fe      Setup frontend only

  ${GREEN}Other:${NC}
    help          Show this help message

${YELLOW}EXAMPLES:${NC}
  ${CYAN}First time setup:${NC}
    ./scripts/dev.sh setup

  ${CYAN}Start everything:${NC}
    ./scripts/dev.sh start

  ${CYAN}Check what's running:${NC}
    ./scripts/dev.sh status

  ${CYAN}Run tests with coverage:${NC}
    ./scripts/dev.sh test

  ${CYAN}View coverage report (after running tests):${NC}
    ./scripts/dev.sh coverage

  ${CYAN}Start only backend for API development:${NC}
    ./scripts/dev.sh db          # Start database first
    ./scripts/dev.sh backend     # Then start backend

  ${CYAN}Fresh database setup (drop and recreate):${NC}
    ./scripts/dev.sh setup-db-fresh

  ${CYAN}Reset everything:${NC}
    ./scripts/dev.sh clean       # Remove all artifacts
    ./scripts/dev.sh setup       # Fresh setup

${YELLOW}LOGS:${NC}
  When running with 'start', logs are saved to:
    - logs/backend.log
    - logs/frontend.log

${YELLOW}TIPS:${NC}
  • Run 'setup' once before first use
  • Use 'status' to check what's running
  • Use 'stop' before running 'clean'
  • Backend must be built before running tests

EOF
}

# --- Main Command Handler ---

main() {
    # Ensure we're in project root
    cd "${PROJECT_ROOT}"

    # Create logs directory if it doesn't exist
    mkdir -p "${PROJECT_ROOT}/logs"

    # Handle commands
    case "${1:-help}" in
        # Main commands
        setup)
            full_setup
            ;;
        start)
            start_all
            ;;
        stop)
            stop_all
            ;;
        restart)
            restart_all
            ;;
        status)
            show_status
            ;;

        # Build & Test
        build)
            build_backend
            build_frontend
            ;;
        test)
            run_tests
            ;;
        coverage|report)
            regenerate_coverage_report
            ;;
        clean)
            clean_all
            ;;

        # Individual services
        db)
            setup_database
            show_status
            ;;
        backend)
            check_prerequisites
            setup_database
            start_backend
            ;;
        frontend)
            start_frontend
            ;;

        # Setup steps
        check)
            check_prerequisites
            ;;
        setup-db)
            setup_database
            ;;
        setup-db-fresh)
            setup_database_fresh
            ;;
        setup-api)
            setup_backend
            ;;
        setup-fe)
            setup_frontend
            ;;

        # Help
        help|--help|-h)
            show_help
            ;;

        *)
            error "Unknown command: '$1'\nRun './scripts/dev.sh help' for usage"
            ;;
    esac
}

# Run main with all arguments
main "$@"
