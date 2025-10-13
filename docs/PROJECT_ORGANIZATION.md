# Project Organization Plan - Fadebook

## Current Issues Identified

### Backend (API)
1. DTO folder should be renamed to DTOs (plural, consistent with convention)
2. Converters folder contains only one file - should be part of a Common or Utilities folder
3. Missing separate folder for Configuration classes
4. Middleware folder is good but could use organization
5. Missing Constants folder for magic strings/numbers
6. Missing Validators folder for validation logic
7. Test files (test-results.txt, errors.md) should not be in source folder

### Frontend
1. Good structure overall but could benefit from:
   - Feature-based organization for larger components
   - Separate constants/config folder
   - Utils folder for helper functions
   - Types could be more organized by feature

### General
1. Missing root-level documentation
2. No ARCHITECTURE.md file
3. No CONTRIBUTING.md file
4. Could benefit from better separation of concerns

---

## Recommended Backend Structure

```
api/
├── Controllers/                    # API Controllers
│   ├── AppointmentController.cs
│   ├── AuthController.cs
│   ├── BarberController.cs
│   └── ...
│
├── Core/                          # Core business logic
│   ├── Models/                    # Domain models
│   │   ├── AppointmentModel.cs
│   │   ├── BarberModel.cs
│   │   └── ...
│   │
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Appointments/
│   │   │   ├── AppointmentDto.cs
│   │   │   └── AppointmentRequestDto.cs
│   │   ├── Auth/
│   │   │   ├── LoginDto.cs
│   │   │   ├── RegisterDto.cs
│   │   │   └── AuthDtos.cs
│   │   ├── Barbers/
│   │   │   ├── BarberDto.cs
│   │   │   └── CreateBarberDto.cs
│   │   └── Common/
│   │       └── MiscDTOs.cs
│   │
│   ├── Exceptions/                # Custom exceptions
│   │   ├── NotFoundException.cs
│   │   ├── BadRequestException.cs
│   │   └── UnauthorizedException.cs
│   │
│   └── Interfaces/                # Core interfaces
│       ├── IEntity.cs
│       └── IAuditable.cs
│
├── Infrastructure/                # Infrastructure concerns
│   ├── Data/                      # Database context
│   │   ├── FadebookDbContext.cs
│   │   └── Migrations/
│   │       └── ...
│   │
│   ├── Repositories/              # Data access layer
│   │   ├── Interfaces/
│   │   │   ├── IAppointmentRepository.cs
│   │   │   ├── IBarberRepository.cs
│   │   │   └── ...
│   │   └── Implementations/
│   │       ├── AppointmentRepository.cs
│   │       ├── BarberRepository.cs
│   │       └── ...
│   │
│   └── Configuration/             # Infrastructure config
│       ├── DatabaseConfiguration.cs
│       └── CacheConfiguration.cs
│
├── Application/                   # Application services
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IAppointmentManagementService.cs
│   │   │   ├── IAuthService.cs
│   │   │   └── ...
│   │   └── Implementations/
│   │       ├── AppointmentManagementService.cs
│   │       ├── AuthService.cs
│   │       └── ...
│   │
│   ├── Mapping/                   # AutoMapper profiles
│   │   └── ApiMappingProfile.cs
│   │
│   └── Validators/                # FluentValidation validators
│       ├── AppointmentValidator.cs
│       └── BarberValidator.cs
│
├── Common/                        # Shared utilities
│   ├── Constants/
│   │   ├── AppointmentStatus.cs
│   │   ├── UserRoles.cs
│   │   └── ErrorMessages.cs
│   │
│   ├── Helpers/
│   │   ├── DateTimeHelper.cs
│   │   └── StringHelper.cs
│   │
│   ├── Converters/
│   │   └── UtcDateTimeConverter.cs
│   │
│   └── Extensions/
│       ├── ServiceCollectionExtensions.cs
│       └── DateTimeExtensions.cs
│
├── Middleware/                    # ASP.NET Core middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
│
├── Configuration/                 # App configuration
│   ├── JwtConfiguration.cs
│   ├── DatabaseSettings.cs
│   └── ApiSettings.cs
│
├── Properties/                    # Project properties
│   └── launchSettings.json
│
├── Logs/                         # Log files (gitignored)
├── Program.cs                    # Application entry point
├── appsettings.json             # App settings
├── appsettings.Development.json # Dev settings
├── .env                         # Environment variables (gitignored)
├── .env.example                 # Example env file
├── api.csproj                   # Project file
└── README.md                    # API documentation
```

---

## Recommended Frontend Structure

```
fadebook-frontend/
├── public/                       # Static assets
│   ├── FadeBook Logo.svg
│   └── favicon.ico
│
├── src/
│   ├── app/                      # Next.js App Router
│   │   ├── (auth)/              # Auth route group
│   │   │   ├── signin/
│   │   │   └── signup/
│   │   │
│   │   ├── (customer)/          # Customer route group
│   │   │   ├── page.tsx         # Home page
│   │   │   ├── barbers/
│   │   │   ├── book/
│   │   │   └── my-appointments/
│   │   │
│   │   ├── (barber)/            # Barber route group
│   │   │   └── barber-dashboard/
│   │   │
│   │   ├── (admin)/             # Admin route group
│   │   │   └── admin/
│   │   │
│   │   ├── layout.tsx           # Root layout
│   │   ├── globals.css          # Global styles
│   │   └── error.tsx            # Error boundary
│   │
│   ├── components/              # React components
│   │   ├── common/              # Shared components
│   │   │   ├── Navigation.tsx
│   │   │   ├── Footer.tsx
│   │   │   └── Loading.tsx
│   │   │
│   │   ├── features/            # Feature-specific components
│   │   │   ├── appointments/
│   │   │   │   ├── AppointmentCard.tsx
│   │   │   │   └── AppointmentList.tsx
│   │   │   ├── barbers/
│   │   │   │   ├── BarberCard.tsx
│   │   │   │   └── BarberList.tsx
│   │   │   └── admin/
│   │   │       ├── AppointmentsTab.tsx
│   │   │       ├── BarbersTab.tsx
│   │   │       └── ServicesTab.tsx
│   │   │
│   │   └── ui/                  # UI primitives (shadcn)
│   │       ├── button.tsx
│   │       ├── card.tsx
│   │       └── ...
│   │
│   ├── hooks/                   # Custom React hooks
│   │   ├── useAppointments.ts
│   │   ├── useAuth.ts
│   │   ├── useBarbers.ts
│   │   └── useServices.ts
│   │
│   ├── lib/                     # Libraries and utilities
│   │   ├── api/                 # API client functions
│   │   │   ├── client.ts        # Axios instance
│   │   │   ├── appointments.ts
│   │   │   ├── auth.ts
│   │   │   ├── barbers.ts
│   │   │   └── services.ts
│   │   │
│   │   ├── utils/               # Utility functions
│   │   │   ├── date.ts
│   │   │   ├── format.ts
│   │   │   └── validation.ts
│   │   │
│   │   └── utils.ts             # General utilities
│   │
│   ├── types/                   # TypeScript type definitions
│   │   ├── api/                 # API response types
│   │   │   ├── appointment.ts
│   │   │   ├── barber.ts
│   │   │   └── user.ts
│   │   │
│   │   └── index.ts             # Export all types
│   │
│   ├── config/                  # Configuration
│   │   ├── constants.ts         # App constants
│   │   ├── routes.ts            # Route definitions
│   │   └── api-config.ts        # API configuration
│   │
│   ├── providers/               # React context providers
│   │   ├── AuthProvider.tsx
│   │   └── ThemeProvider.tsx
│   │
│   └── test/                    # Test utilities
│       └── setup.ts             # Test configuration
│
├── tests/                       # Test files (mirroring src structure)
│   ├── components/
│   ├── hooks/
│   ├── lib/
│   └── providers/
│
├── .gitignore
├── components.json              # shadcn config
├── eslint.config.mjs           # ESLint config
├── next.config.ts              # Next.js config
├── package.json
├── postcss.config.mjs          # PostCSS config
├── README.md                   # Frontend docs
├── tsconfig.json               # TypeScript config
└── vitest.config.ts            # Vitest config
```

---

## Root Level Organization

```
TheNightOwls/
├── api/                         # Backend API
├── Api.Tests/                   # Backend tests
├── fadebook-frontend/           # Frontend app
├── docs/                        # Documentation
│   ├── diagrams/                # ERD, wireframes, flow diagrams
│   ├── guides/                  # Setup and usage guides
│   │   └── SETUP_AND_USAGE.md
│   ├── standards/               # Coding standards
│   │   └── CODING_STANDARDS.md
│   ├── ARCHITECTURE.md
│   ├── PROJECT_ORGANIZATION.md
│   └── PROJECT_REQUIREMENTS.md
│
├── scripts/                     # Build and automation scripts
│   ├── cicd.sh                  # CI/CD automation script
│   └── start-db.sh              # Database initialization script
│
├── .editorconfig                # Editor configuration
├── .gitignore                   # Root gitignore
├── docker-compose.yml           # Docker configuration
├── api.sln                      # .NET solution
├── README.md                    # Project overview
├── CONTRIBUTING.md              # Contribution guidelines
└── LICENSE                      # MIT License
```

---

## Benefits of This Organization

### Backend Benefits
1. **Clear Separation of Concerns**: Core, Infrastructure, and Application layers are distinct
2. **Easier Testing**: Dependencies are clear and mockable
3. **Scalability**: Easy to add new features without cluttering existing folders
4. **Maintenance**: Related files are grouped together
5. **Onboarding**: New developers can understand structure quickly

### Frontend Benefits
1. **Route Groups**: Logical separation by user role
2. **Feature-Based Components**: Easy to find and maintain related components
3. **Centralized Configuration**: All configs in one place
4. **Type Safety**: Well-organized TypeScript types
5. **Test Organization**: Tests mirror source structure

### Overall Benefits
1. **Documentation**: Centralized docs folder
2. **Consistency**: Follows .NET and Next.js best practices
3. **Maintainability**: Clear structure reduces cognitive load
4. **Collaboration**: Easy for team members to work on different features
5. **Professional**: Industry-standard organization

---

## Migration Steps (Recommended Order)

### Phase 1: Backend Reorganization
1. Create new folder structure
2. Move DTOs to organized subfolders
3. Reorganize Repositories and Services
4. Create Constants and Helpers folders
5. Update namespaces and imports
6. Run tests to verify nothing broke

### Phase 2: Frontend Reorganization
1. Create route groups for auth, customer, barber, admin
2. Reorganize components into common/features/ui
3. Create config folder with constants
4. Organize types by feature
5. Update imports
6. Run tests and build to verify

### Phase 3: Documentation
1. Create docs folder
2. Write ARCHITECTURE.md
3. Write CONTRIBUTING.md
4. Update README files
5. Create diagrams
6. Add changelog

### Phase 4: Cleanup
1. Remove temporary files (test-results.txt, errors.md)
2. Update .gitignore files
3. Remove unused code
4. Run final tests
5. Create pull request with changes

---

## Quick Wins (Do These First)

1. **Rename DTO to DTOs** - Simple rename, follows convention
2. **Move Converters into Common folder** - Only one file, easy move
3. **Create Constants.cs** - Extract magic strings from code
4. **Remove test-results.txt and errors.md** - Should not be in source
5. **Create root-level ARCHITECTURE.md** - Document current system
6. **Organize DTOs by feature** - Group related DTOs together

---

## Notes

- This is a **recommended** structure, not mandatory
- Implement changes incrementally to avoid breaking changes
- Run tests after each major reorganization
- Update documentation as you go
- Get team buy-in before major refactoring
- Consider creating a separate branch for reorganization

---

## Questions to Consider

1. Do you want to implement Clean Architecture more strictly?
2. Should we use CQRS pattern for complex operations?
3. Do you want separate projects for Core, Infrastructure, Application?
4. Should we add API versioning (v1, v2 folders)?
5. Do you need a shared library project for common code?

---

**Status**: Draft - Review and adjust based on team preferences
**Last Updated**: October 12, 2025
**Author**: Development Team
