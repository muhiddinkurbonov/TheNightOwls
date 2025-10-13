# Coding Standards and Naming Conventions - Fadebook

## Overview

This document defines the coding standards and naming conventions for the Fadebook project to ensure consistency across the codebase.

---

## 1. General Principles

- **Consistency**: Follow the conventions established in each language/framework
- **Clarity**: Names should be descriptive and self-documenting
- **Maintainability**: Code should be easy to read and understand
- **Convention over Configuration**: Follow established conventions rather than creating new ones

---

## 2. Backend (C# / .NET)

### File Naming

- **Classes/Interfaces**: `PascalCase.cs`
  - ✅ `AppointmentController.cs`
  - ✅ `IAuthService.cs`
  - ✅ `BarberManagementService.cs`

- **Test Files**: `{ClassName}Tests.cs`
  - ✅ `AppointmentControllerTests.cs`
  - ✅ `AuthServiceTests.cs`

### Namespace Conventions

```csharp
// Feature-based organization
namespace Fadebook.Controllers;
namespace Fadebook.Services;
namespace Fadebook.DTOs.Appointments;
namespace Fadebook.Common.Constants;
```

### Class and Member Naming

```csharp
// Classes, Interfaces, Enums - PascalCase
public class AppointmentController { }
public interface IAuthService { }
public enum UserRole { }

// Public Properties, Methods - PascalCase
public string UserName { get; set; }
public async Task<User> GetUserAsync(int id) { }

// Private Fields - _camelCase with underscore prefix
private readonly IAuthService _authService;
private readonly IMapper _mapper;

// Local Variables, Parameters - camelCase
int userId = 1;
string userName = "john";
public void ProcessUser(string userName, int userId) { }

// Constants - PascalCase
public const string Pending = "Pending";
public const int MaxRetries = 3;

// Static readonly - PascalCase
public static readonly string DefaultRole = "Customer";
```

### DTO Naming

```csharp
// Request DTOs
public class CreateBarberDto { }
public class UpdateAppointmentDto { }
public class RegisterDto { }

// Response DTOs
public class AppointmentDto { }
public class UserDto { }
public class LoginResponseDto { }
```

### Database Entity Naming

```csharp
// Model suffix for entities
public class AppointmentModel { }
public class BarberModel { }

// Table names in DbContext - camelCase with "Table" suffix
public DbSet<AppointmentModel> appointmentTable { get; set; }
public DbSet<BarberModel> barberTable { get; set; }
```

### Controller Naming

```csharp
// Controller suffix, inherit from ControllerBase
[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase { }
```

### Service and Repository Naming

```csharp
// Interface with "I" prefix
public interface IAuthService { }
public interface IBarberRepository { }

// Implementation matches interface name without "I"
public class AuthService : IAuthService { }
public class BarberRepository : IBarberRepository { }
```

---

## 3. Frontend (TypeScript / React / Next.js)

### File Naming

- **Components**: `PascalCase.tsx`
  - ✅ `Navigation.tsx`
  - ✅ `AppointmentsTab.tsx`
  - ✅ `ProtectedRoute.tsx`

- **Pages (Next.js App Router)**: `page.tsx`, `layout.tsx`, `loading.tsx`, `error.tsx`
  - ✅ `app/admin/page.tsx`
  - ✅ `app/layout.tsx`

- **Utilities, Hooks, API**: `camelCase.ts`
  - ✅ `lib/api/appointments.ts`
  - ✅ `hooks/useAppointments.ts`
  - ✅ `lib/utils.ts`

- **Types**: `camelCase.ts` or `types.ts`
  - ✅ `types/api.ts`
  - ✅ `types/appointment.ts`

- **Test Files**: `{filename}.test.ts` or `{ComponentName}.test.tsx`
  - ✅ `appointments.test.ts`
  - ✅ `Navigation.test.tsx`

### Component Naming

```typescript
// Function components - PascalCase
export function Navigation() { }
export function AppointmentCard() { }

// Component files match component name
// Navigation.tsx exports Navigation
// AppointmentCard.tsx exports AppointmentCard
```

### Variable and Function Naming

```typescript
// Variables - camelCase
const userName = "john";
let appointmentCount = 0;

// Functions - camelCase
function fetchAppointments() { }
const handleSubmit = () => { };

// Boolean variables - use is/has/should prefix
const isLoading = false;
const hasError = true;
const shouldRedirect = false;

// Constants - UPPER_SNAKE_CASE or PascalCase for objects
const MAX_RETRIES = 3;
const API_BASE_URL = "http://localhost:5288";

const ApiRoutes = {
  Appointments: "/api/appointment",
  Auth: "/api/auth"
};
```

### Hook Naming

```typescript
// Custom hooks - "use" prefix, camelCase
export function useAppointments() { }
export function useAuth() { }
export function useLocalStorage(key: string) { }
```

### Type and Interface Naming

```typescript
// Interfaces and Types - PascalCase
interface User {
  id: number;
  name: string;
}

type AppointmentStatus = "Pending" | "Completed" | "Cancelled";

// Props interfaces - {ComponentName}Props
interface NavigationProps {
  isOpen: boolean;
}

// API response types - descriptive names
interface ApiResponse<T> {
  data: T;
  error?: string;
}
```

### API Function Naming

```typescript
// API functions in lib/api/ - verb + noun, camelCase
export async function getAppointments() { }
export async function createAppointment(data: AppointmentDto) { }
export async function updateAppointment(id: number, data: AppointmentDto) { }
export async function deleteAppointment(id: number) { }
```

---

## 4. Scripts (Shell/Bash)

### File Naming

- **Shell scripts**: `kebab-case.sh`
  - ✅ `dev.sh`
  - ✅ `cicd.sh`

### Script Best Practices

```bash
#!/bin/bash

# Use strict error handling
set -e  # Exit on error
set -u  # Exit on undefined variable
set -o pipefail  # Exit on pipe failure

# Define constants at the top - UPPER_SNAKE_CASE
readonly API_PROJECT_DIR="./api"
readonly TEST_PROJECT_DIR="./Api.Tests"
readonly MAX_RETRIES=30

# Functions - snake_case
check_dependencies() {
    # Function implementation
}

validate_configuration() {
    # Function implementation
}

# Main script execution
main() {
    check_dependencies
    validate_configuration
}

main "$@"
```

### Script Header Template

```bash
#!/bin/bash
#
# Script Name and Purpose
#
# Usage: ./script-name.sh [options]
# Options:
#   -h, --help    Show help message
#   -v, --verbose Enable verbose output
#
# Author: Development Team
# Date: YYYY-MM-DD
```

---

## 5. Database Naming Conventions

### Table Names

- Use `camelCase` with "Table" suffix (matching C# DbSet convention)
  - ✅ `appointmentTable`
  - ✅ `barberTable`
  - ✅ `userTable`

### Column Names

- Use `PascalCase` (matching C# property names)
  - ✅ `AppointmentId`
  - ✅ `AppointmentDate`
  - ✅ `CustomerId`

### Foreign Keys

- Use `{RelatedEntity}Id` format
  - ✅ `CustomerId`
  - ✅ `BarberId`
  - ✅ `ServiceId`

---

## 6. API Route Naming

### REST API Routes

```
# Collection routes - plural nouns
GET    /api/appointments
POST   /api/appointments
GET    /api/barbers
POST   /api/services

# Resource routes - singular resource
GET    /api/appointment/{id}
PUT    /api/appointment/{id}
DELETE /api/appointment/{id}

# Nested resources
GET    /api/barber/{barberId}/appointments
GET    /api/appointment/by-username/{username}

# Actions (when not CRUD)
POST   /api/auth/login
POST   /api/auth/register
POST   /api/auth/change-password
```

---

## 7. Git Commit Message Convention

### Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

### Examples

```
feat(appointments): add appointment cancellation feature

fix(auth): resolve JWT token expiration issue

docs(readme): update installation instructions

refactor(services): reorganize DTOs by feature

test(controllers): add comprehensive controller tests
```

---

## 8. Folder Structure Conventions

### Backend Structure

```
api/
├── Common/                 # Shared utilities
│   ├── Constants/          # PascalCase files
│   ├── Converters/
│   ├── Helpers/
│   └── Extensions/
├── Controllers/            # PascalCase files
├── Core/
│   ├── DTOs/              # Feature-based subfolders
│   ├── Models/
│   └── Interfaces/
├── Services/
│   ├── interfaces/        # lowercase folder
│   └── implementations/   # lowercase folder
└── Repositories/
    ├── interfaces/
    └── implementations/
```

### Frontend Structure

```
src/
├── app/                    # Next.js App Router (lowercase folders)
│   ├── (auth)/
│   ├── (customer)/
│   ├── admin/
│   └── page.tsx
├── components/             # PascalCase .tsx files
│   ├── admin/
│   ├── common/
│   └── ui/
├── hooks/                  # camelCase .ts files
├── lib/                    # camelCase .ts files
│   ├── api/
│   └── utils/
├── types/                  # camelCase .ts files
└── providers/              # PascalCase .tsx files
```

---

## 9. Environment Variables

### Naming Convention

- Use `UPPER_SNAKE_CASE`
  - ✅ `CONNECTION_STRING`
  - ✅ `JWT_SECRET_KEY`
  - ✅ `MSSQL_SA_PASSWORD`
  - ✅ `NEXT_PUBLIC_API_URL`

### .env File Structure

```bash
# Database Configuration
CONNECTION_STRING="Server=localhost,1433;Database=Fadebook;..."
MSSQL_SA_PASSWORD="YourStrong!Password"

# JWT Configuration
JWT_SECRET_KEY="your-secret-key-here"
JWT_ISSUER="Fadebook"
JWT_AUDIENCE="FadebookApp"
JWT_EXPIRATION_MINUTES="60"

# Frontend (Next.js) - must start with NEXT_PUBLIC_
NEXT_PUBLIC_API_URL="http://localhost:5288"
```

---

## 10. Comments and Documentation

### C# XML Documentation

```csharp
/// <summary>
/// Gets all appointments for a specific customer
/// </summary>
/// <param name="customerId">The customer ID</param>
/// <returns>A collection of appointments</returns>
public async Task<IEnumerable<AppointmentModel>> GetAppointmentsByCustomerIdAsync(int customerId)
{
    // Implementation
}
```

### TypeScript JSDoc

```typescript
/**
 * Fetches all appointments from the API
 * @returns Promise containing array of appointments
 * @throws Error if the API request fails
 */
export async function getAppointments(): Promise<Appointment[]> {
  // Implementation
}
```

### Shell Script Comments

```bash
# Section headers - descriptive comment
# --- Configuration Variables ---

# Inline comments - explain why, not what
# Wait for SQL Server to fully initialize before proceeding
sleep 10
```

---

## 11. Naming Anti-Patterns (Avoid)

### ❌ Don't Do This

```csharp
// Hungarian notation
string strUserName;
int intCount;

// Abbreviations (unless widely known)
var appt = new AppointmentModel(); // Use 'appointment'
var cust = GetCustomer();          // Use 'customer'

// Single letter variables (except in loops)
var u = await GetUser();           // Use 'user'

// Unclear names
var data = GetData();              // What data?
var temp = ProcessTemp();          // Temporary what?

// Inconsistent naming
getUserName()    // camelCase
GetUserId()      // PascalCase  - Pick one style per context

// Prefixes indicating type in modern code
public class AppointmentClass { }  // Just 'Appointment'
public interface InterfaceAuth { } // 'IAuth' for interface
```

### ✅ Do This Instead

```csharp
// Clear, descriptive names
string userName;
int appointmentCount;

// Full words
var appointment = new AppointmentModel();
var customer = GetCustomer();

// Descriptive variables
var user = await GetUser();

// Clear purpose
var appointments = GetAppointments();
var validatedRequest = ProcessRequest();

// Consistent casing
public string GetUserName() { }
public int GetUserId() { }
```

---

## 12. Checklist for New Code

### Before Committing

- [ ] File names follow convention for the technology
- [ ] Class/component names match file names
- [ ] Variables use appropriate casing (camelCase, PascalCase, etc.)
- [ ] No abbreviations or unclear names
- [ ] Comments explain "why" not "what"
- [ ] Consistent naming within the file/module
- [ ] No magic strings or numbers (use constants)
- [ ] Follow established patterns in the codebase

---

## 13. Tools and Enforcement

### C# (.NET)

- **StyleCop**: Enforce C# coding conventions
- **EditorConfig**: Consistent formatting across editors
- **SonarLint**: Code quality and naming analysis

### TypeScript/JavaScript

- **ESLint**: Enforce JavaScript/TypeScript conventions
- **Prettier**: Consistent code formatting
- **TypeScript strict mode**: Type safety

### Git Hooks

- **Pre-commit**: Run linters before committing
- **Commit-msg**: Validate commit message format

---

## Resources

- [C# Naming Guidelines (Microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
- [TypeScript Style Guide](https://google.github.io/styleguide/tsguide.html)
- [React TypeScript Cheatsheet](https://react-typescript-cheatsheet.netlify.app/)
- [Bash Style Guide](https://google.github.io/styleguide/shellguide.html)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Last Updated**: October 12, 2025
**Version**: 1.0
**Maintained By**: The Night Owls Development Team
