# Fadebook Architecture

## Overview

Fadebook is a barbershop appointment management system built with a .NET backend API and a Next.js frontend. The system enables customers to book appointments, barbers to manage their schedules, and administrators to oversee the entire operation.

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0 (Web API)
- **Language**: C# with nullable reference types enabled
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.Net
- **Logging**: Serilog (Console and File sinks)
- **Mapping**: AutoMapper
- **Testing**: xUnit, Moq, FluentAssertions

### Frontend
- **Framework**: Next.js (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: shadcn/ui
- **State Management**: React Hooks, Context API
- **HTTP Client**: Axios

## System Architecture

### High-Level Architecture

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│                 │         │                 │         │                 │
│  Next.js        │  HTTP   │  ASP.NET Core   │   EF    │  SQL Server     │
│  Frontend       │ ◄─────► │  Web API        │ ◄─────► │  Database       │
│                 │  REST   │                 │  Core   │                 │
└─────────────────┘         └─────────────────┘         └─────────────────┘
```

### Backend Architecture

The backend follows a **layered architecture** pattern with clear separation of concerns:

```
Controllers (API Layer)
    ↓
Services (Business Logic Layer)
    ↓
Repositories (Data Access Layer)
    ↓
Database Context (EF Core)
    ↓
SQL Server Database
```

#### Layer Responsibilities

1. **Controllers** (`/Controllers`)
   - Handle HTTP requests and responses
   - Input validation and model binding
   - Route definition
   - JWT authorization checks
   - Delegate business logic to services

2. **Services** (`/Services`)
   - Business logic implementation
   - Transaction management
   - Data validation and transformation
   - Coordinate multiple repositories
   - Map between models and DTOs

3. **Repositories** (`/Repositories`)
   - Data access abstraction
   - CRUD operations
   - Query composition
   - Database-specific logic
   - No business logic

4. **Models** (`/Models`)
   - Entity Framework Core entities
   - Database schema representation
   - Relationships and constraints

5. **DTOs** (`/DTOs`)
   - Data transfer objects for API communication
   - Organized by feature (Appointments, Auth, Barbers, etc.)
   - Separate from domain models

6. **Common** (`/Common`)
   - Constants (Status, Roles, Error Messages)
   - Converters (UTC DateTime)
   - Shared utilities

7. **Middleware** (`/Middleware`)
   - Exception handling
   - Request logging
   - Security headers

## Database Schema

### Core Tables

#### Users (`userTable`)
- Handles authentication and authorization
- Stores user credentials, roles, and profile information
- Roles: Customer, Barber, Admin

#### Barbers (`barberTable`)
- Barber-specific information
- Specialty and contact details
- Linked to Users table via Username

#### Customers (`customerTable`)
- Customer-specific information
- Contact details
- Linked to Users table via Username

#### Services (`serviceTable`)
- Available barbershop services
- Service names and prices

#### Appointments (`appointmentTable`)
- Scheduled appointments
- Links customers, barbers, and services
- Tracks appointment status (Pending, Completed, Cancelled, Expired)

#### BarberServices (`barberServiceTable`)
- Many-to-many relationship between barbers and services
- Defines which services each barber offers

#### BarberWorkHours (`barberWorkHoursTable`)
- Defines barber availability
- Day of week and time ranges
- Used for appointment scheduling validation

## Key Features

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (Customer, Barber, Admin)
- Secure password hashing with BCrypt
- Token expiration and refresh

### Appointment Management
- Book appointments with specific barbers and services
- View upcoming and past appointments
- Cancel appointments
- Conflict detection and validation

### Barber Management
- Define work hours and availability
- Manage offered services
- View scheduled appointments

### Admin Features
- Manage barbers, services, and appointments
- View system-wide statistics
- User management

## Security Features

### Backend Security
- JWT token validation on protected endpoints
- Role-based authorization policies
- Security headers (X-Frame-Options, CSP, HSTS, etc.)
- CORS configuration for frontend
- Input validation and sanitization
- Centralized exception handling

### Frontend Security
- JWT token storage and management
- Protected routes with authentication checks
- Role-based UI rendering
- HTTPS enforcement

## API Design

### RESTful Endpoints

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/appointment` - List appointments
- `POST /api/appointment` - Create appointment
- `PUT /api/appointment/{id}` - Update appointment
- `DELETE /api/appointment/{id}` - Cancel appointment
- `GET /api/barber` - List barbers
- `GET /api/service` - List services

### Response Format

**Success Response:**
```json
{
  "appointmentId": 1,
  "appointmentDate": "2025-10-15T14:00:00Z",
  "status": "Pending",
  "barberId": 1,
  "serviceId": 1,
  "customerId": 1
}
```

**Error Response:**
```json
{
  "message": "Appointment not found",
  "statusCode": 404
}
```

## Data Flow

### Example: Creating an Appointment

1. **Frontend**: User fills out appointment form
2. **Frontend**: Sends POST request to `/api/appointment` with JWT token
3. **API**: JwtBearerMiddleware validates token
4. **API**: AppointmentController receives request
5. **API**: Controller validates input and maps DTO to Model
6. **Service**: AppointmentManagementService validates business rules
7. **Service**: Checks barber availability via BarberWorkHoursService
8. **Service**: Checks for scheduling conflicts
9. **Repository**: AppointmentRepository saves to database
10. **Service**: Maps result to DTO
11. **Controller**: Returns 201 Created with appointment details
12. **Frontend**: Displays success message and appointment details

## Error Handling

### Backend
- **ExceptionHandlingMiddleware**: Catches all unhandled exceptions
- Environment-aware error details (detailed in dev, generic in production)
- Consistent error response format
- Logging with Serilog

### Frontend
- Try-catch blocks around API calls
- User-friendly error messages
- Toast notifications for errors
- Loading states and error boundaries

## Testing Strategy

### Backend Testing
- **Unit Tests**: Services and repositories with mocked dependencies
- **Integration Tests**: Controllers with in-memory database
- **Test Coverage**: 174 tests covering critical functionality
- **Tools**: xUnit, Moq, FluentAssertions, SQLite in-memory

### Test Organization
```
Api.Tests/
├── Controllers/          # Controller tests
├── Services/            # Service tests
├── Repositories/        # Repository tests
└── TestHelpers/         # Shared test utilities
```

## Configuration Management

### Environment Variables (.env)
- `CONNECTION_STRING` - Database connection
- `JWT_SECRET_KEY` - JWT signing key
- `JWT_ISSUER` - Token issuer
- `JWT_AUDIENCE` - Token audience
- `JWT_EXPIRATION_MINUTES` - Token lifetime

### Configuration Files
- `appsettings.json` - General app settings
- `appsettings.Development.json` - Development overrides
- `.env` - Sensitive environment variables (gitignored)
- `.env.example` - Template for environment variables

## Deployment

### CI/CD
- Script: `scripts/cicd.sh`
- Builds both frontend and backend
- Runs tests
- Validates code quality

### Production Considerations
- Environment-specific configuration
- Database migrations
- HTTPS enforcement
- CORS configuration
- Logging and monitoring
- Error tracking

## Future Enhancements

### Planned Features
1. Email notifications for appointments
2. SMS reminders
3. Google Calendar integration
4. Payment processing
5. Customer reviews and ratings
6. Barber performance analytics
7. Appointment history and statistics

### Architectural Improvements
1. Move to Clean Architecture with separate projects
2. Implement CQRS pattern for complex operations
3. Add API versioning (v1, v2)
4. Implement caching layer (Redis)
5. Add health check endpoints
6. Implement rate limiting
7. Add API documentation with Swagger annotations

## Development Guidelines

### Code Style
- Follow .NET naming conventions
- Use async/await for I/O operations
- Enable nullable reference types
- Organize code by feature

### Git Workflow
- Feature branches for new work
- Pull requests for code review
- Descriptive commit messages
- CI/CD validation before merge

### Best Practices
- Keep controllers thin, services fat
- Single responsibility principle
- Dependency injection for testability
- Use DTOs for API boundaries
- Validate input at multiple layers
- Log important events and errors

## Troubleshooting

### Common Issues

**Database Connection Failed**
- Verify CONNECTION_STRING in .env
- Ensure SQL Server is running
- Check firewall rules

**JWT Token Invalid**
- Verify JWT_SECRET_KEY matches between services
- Check token expiration
- Validate token format

**CORS Errors**
- Verify frontend URL in CORS policy
- Check credentials configuration
- Validate HTTP methods allowed

## Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Next.js Documentation](https://nextjs.org/docs)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)

---

**Last Updated**: October 12, 2025
**Version**: 1.0
**Maintained By**: The Night Owls Development Team
