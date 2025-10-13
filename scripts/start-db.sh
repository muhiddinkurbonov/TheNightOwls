#!/bin/bash
#
# start-db.sh - Database Initialization Script
#
# Description:
#   Starts MSSQL Server Express in Docker and runs Entity Framework Core migrations.
#   Handles database creation, migration, and ensures SQL Server is ready before proceeding.
#
# Usage:
#   ./scripts/start-db.sh
#
# Note: Run this script from the project root directory
#
# Prerequisites:
#   - Docker and Docker Compose installed
#   - .env file configured in api/ directory (use .env.example as template)
#   - .NET SDK installed for EF Core migrations
#
# Environment Variables Required (from api/.env):
#   - MSSQL_SA_PASSWORD: SQL Server SA password
#   - CONNECTION_STRING: Database connection string
#
# Author: The Night Owls Development Team
# Last Updated: October 12, 2025
#

set -e  # Exit on error
set -u  # Exit on undefined variable

echo "=========================================="
echo "Starting MSSQL Server Express in Docker"
echo "=========================================="

# Check if .env file exists
if [ ! -f api/.env ]; then
    echo "ERROR: .env file not found!"
    echo "Please create a .env file based on .env.example"
    echo "Example: cp .env.example .env"
    exit 1
fi

# Load environment variables from .env
set -a  # automatically export all variables
source api/.env
set +a  # stop automatically exporting

# Check if MSSQL_SA_PASSWORD is set
if [ -z "$MSSQL_SA_PASSWORD" ]; then
    echo "ERROR: MSSQL_SA_PASSWORD not set in .env file!"
    exit 1
fi

# Check if CONNECTION_STRING is set
if [ -z "$CONNECTION_STRING" ]; then
    echo "ERROR: CONNECTION_STRING not set in .env file!"
    exit 1
fi

# Start Docker containers
echo ""
echo "Starting Docker containers..."
docker-compose up -d

# Wait for SQL Server to be ready
echo ""
echo "Waiting for SQL Server to be ready..."
sleep 10

# Check if SQL Server is ready by checking the logs
MAX_RETRIES=30
RETRY_COUNT=0
until docker logs mssql-express 2>&1 | grep -q "Recovery is complete"; do
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
        echo "ERROR: SQL Server failed to start after $MAX_RETRIES attempts"
        docker logs mssql-express --tail 20
        exit 1
    fi
    echo "Waiting for SQL Server to be ready... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
    sleep 2
done

echo ""
echo "SQL Server is ready!"

# Drop and recreate the database
echo ""
echo "=========================================="
echo "Dropping and Recreating Database"
echo "=========================================="

cd api

echo "Dropping existing database if it exists..."
dotnet ef database drop --force

# Run EF Core migrations
echo ""
echo "=========================================="
echo "Running EF Core Migrations"
echo "=========================================="

# Check if migrations directory exists
if [ ! -d "Migrations" ]; then
    echo "No migrations found. Creating initial migration..."
    dotnet ef migrations add InitialCreate
fi

# Check for pending model changes and create migration if needed
echo ""
echo "Checking for pending model changes..."

# Try to apply migrations first
set +e
UPDATE_OUTPUT=$(dotnet ef database update 2>&1)
UPDATE_EXIT_CODE=$?
set -e

# Check if the update failed due to pending model changes
if [ $UPDATE_EXIT_CODE -ne 0 ] && echo "$UPDATE_OUTPUT" | grep -q "PendingModelChangesWarning"; then
    echo ""
    echo "⚠️  Pending model changes detected!"
    echo "Creating new migration to capture changes..."

    # Generate a migration name with timestamp
    MIGRATION_NAME="ModelChanges_$(date +%Y%m%d_%H%M%S)"

    echo "Creating migration: $MIGRATION_NAME"
    dotnet ef migrations add "$MIGRATION_NAME"

    if [ $? -ne 0 ]; then
        echo "❌ ERROR: Failed to create migration"
        exit 1
    fi

    echo "✅ Migration created successfully!"

    # Now apply migrations again
    echo ""
    echo "Applying migrations to database..."
    dotnet ef database update

    if [ $? -ne 0 ]; then
        echo ""
        echo "❌ ERROR: Failed to apply migrations"
        exit 1
    fi
elif [ $UPDATE_EXIT_CODE -ne 0 ]; then
    # Some other error occurred
    echo ""
    echo "❌ ERROR: Failed to apply migrations"
    echo "$UPDATE_OUTPUT"
    exit 1
else
    # Success on first try
    echo "✅ Migrations applied successfully (no pending changes)"
fi

echo ""
echo "=========================================="
echo "Database setup complete!"
echo "=========================================="
echo ""
echo "MSSQL Server is running on localhost:1433"
echo "Connection string available in .env file"
echo ""
echo "To stop the database, run:"
echo "  docker-compose down"
echo ""
