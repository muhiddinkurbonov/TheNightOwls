# Contributing to Fadebook

Thank you for your interest in contributing to Fadebook! This document provides guidelines and instructions for contributing to the project.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Testing Requirements](#testing-requirements)
- [Documentation](#documentation)
- [Questions and Support](#questions-and-support)

---

## Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inclusive environment for all contributors. We expect everyone to:

- Be respectful and considerate
- Accept constructive criticism gracefully
- Focus on what is best for the project and community
- Show empathy towards other community members

### Unacceptable Behavior

- Harassment, trolling, or discriminatory comments
- Personal attacks or inflammatory language
- Publishing others' private information without permission
- Any conduct that could be considered inappropriate in a professional setting

---

## Getting Started

### 1. Fork the Repository

```bash
# Fork the repository on GitHub, then clone your fork
git clone https://github.com/muhiddinkurbonov/TheNightOwls.git
cd TheNightOwls
```

### 2. Set Up Your Development Environment

**Quick Setup (Recommended):**

```bash
# Use the automated setup script
./scripts/dev.sh setup

# This will:
# - Check prerequisites
# - Setup database
# - Install all dependencies
# - Apply migrations
# - Build projects
```

**Manual Setup:**

If you prefer manual setup, follow the instructions in [`docs/guides/SETUP_AND_USAGE.md`](docs/guides/SETUP_AND_USAGE.md):

```bash
# Install dependencies
dotnet restore Fadebook.sln
cd Fadebook.Frontend && npm install && cd ..

# Set up environment variables
cp Fadebook.Api/.env.example Fadebook.Api/.env
# Edit Fadebook.Api/.env with your configuration

# Start the database with fresh setup
./scripts/dev.sh setup-db-fresh
```

### 3. Create a Branch

```bash
# Create a new branch for your work
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b fix/bug-description
```

### Branch Naming Conventions

- `feature/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `docs/` - Documentation updates
- `test/` - Test additions or updates
- `chore/` - Maintenance tasks

---

## Development Workflow

### 1. Make Your Changes

- Write clean, well-documented code
- Follow our [Coding Standards](docs/standards/CODING_STANDARDS.md)
- Keep commits focused and atomic
- Test your changes thoroughly

### 2. Test Your Changes

**Using dev.sh (Recommended):**
```bash
# Run all tests
./scripts/dev.sh test
```

**Manual Testing:**

**Backend:**
```bash
# Run all tests with coverage
./scripts/cicd.sh test

# Or manually
cd Fadebook.Api.Tests
dotnet test
```

**Frontend:**
```bash
cd Fadebook.Frontend
npm test
npm run lint
```

### 3. Update Documentation

If your changes affect:
- **API endpoints** - Update Swagger documentation comments
- **Setup process** - Update `docs/guides/SETUP_AND_USAGE.md`
- **Architecture** - Update `docs/ARCHITECTURE.md`
- **User features** - Update README.md

### 4. Commit Your Changes

Follow our [commit guidelines](#commit-guidelines):

```bash
git add .
git commit -m "feat(appointments): add cancellation feature"
```

### 5. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub.

---

## Coding Standards

We follow strict coding standards to maintain code quality and consistency. Please read our comprehensive guide:

**[📖 Full Coding Standards Documentation](docs/standards/CODING_STANDARDS.md)**

### Quick Reference

#### C# / .NET

```csharp
// Classes, Interfaces - PascalCase
public class AppointmentController : ControllerBase { }
public interface IAuthService { }

// Private fields - _camelCase
private readonly IAuthService _authService;

// Methods, Properties - PascalCase
public async Task<User> GetUserAsync(int id) { }
public string UserName { get; set; }

// Local variables - camelCase
var userId = 1;
string userName = "john";
```

#### TypeScript / React

```typescript
// Components - PascalCase
export function Navigation() { }

// Functions, variables - camelCase
const fetchAppointments = async () => { };
const isLoading = false;

// Hooks - "use" prefix, camelCase
export function useAppointments() { }

// Types, Interfaces - PascalCase
interface User {
  id: number;
  name: string;
}
```

#### File Naming

- **C# files**: `PascalCase.cs` (AppointmentController.cs)
- **React components**: `PascalCase.tsx` (Navigation.tsx)
- **Utilities/Hooks**: `camelCase.ts` (useAuth.ts)
- **Scripts**: `kebab-case.sh` (dev.sh, cicd.sh)

---

## Commit Guidelines

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation changes
- `style` - Code style changes (formatting, no logic change)
- `refactor` - Code refactoring
- `test` - Adding or updating tests
- `chore` - Maintenance tasks (dependencies, config)
- `perf` - Performance improvements

### Examples

```bash
# Feature
feat(appointments): add appointment cancellation

# Bug fix
fix(auth): resolve JWT token expiration issue

# Documentation
docs(readme): update installation instructions

# Refactoring
refactor(services): reorganize DTOs by feature

# Tests
test(controllers): add comprehensive controller tests

# Chore
chore(deps): update Next.js to version 15.5
```

### Scope

The scope should specify the area of change:
- `appointments` - Appointment-related changes
- `auth` - Authentication/authorization
- `barbers` - Barber management
- `services` - Service management
- `frontend` - Frontend changes
- `backend` - Backend changes
- `db` - Database changes
- `docs` - Documentation

---

## Pull Request Process

### Before Submitting

- [ ] Code follows our [Coding Standards](docs/standards/CODING_STANDARDS.md)
- [ ] All tests pass (`./scripts/cicd.sh test`)
- [ ] New features have tests
- [ ] Documentation is updated
- [ ] Commit messages follow conventions
- [ ] No merge conflicts with main branch

### PR Description Template

```markdown
## Description
Brief description of the changes

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

## Testing
Describe the tests you ran to verify your changes:
- [ ] Unit tests
- [ ] Integration tests
- [ ] Manual testing

## Screenshots (if applicable)
Add screenshots for UI changes

## Checklist
- [ ] My code follows the project's coding standards
- [ ] I have performed a self-review of my code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have made corresponding changes to the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix is effective or that my feature works
- [ ] New and existing unit tests pass locally
```

### Review Process

1. **Automated Checks** - CI/CD pipeline must pass
2. **Code Review** - At least one team member must review
3. **Testing** - Reviewer verifies functionality
4. **Approval** - PR approved by reviewer
5. **Merge** - Merged to main branch

### Merge Strategy

- **Squash and Merge** - For feature branches
- **Rebase and Merge** - For small, atomic commits
- **No Fast-Forward Merge** - To preserve history

---

## Testing Requirements

### Backend Tests

All new backend code must have corresponding tests:

- **Controllers** - Test all endpoints (happy path and error cases)
- **Services** - Test business logic with mocked dependencies
- **Repositories** - Test data access with in-memory database

**Minimum Coverage**: 50% overall, 80% for new code

```bash
# Run tests with coverage
./scripts/cicd.sh test
```

### Frontend Tests

All new frontend components should have tests:

- **Components** - Test rendering and user interactions
- **Hooks** - Test custom hook behavior
- **API calls** - Test with mocked responses

```bash
cd Fadebook.Frontend
npm run test:coverage
```

### Writing Good Tests

```csharp
// Good test structure (Arrange-Act-Assert)
[Fact]
public async Task CreateAppointment_ValidData_ReturnsCreated()
{
    // Arrange
    var appointmentDto = new AppointmentDto { /* ... */ };
    _mockService.Setup(s => s.AddAppointmentAsync(It.IsAny<AppointmentModel>()))
        .ReturnsAsync(appointmentModel);

    // Act
    var result = await _controller.Create(appointmentDto);

    // Assert
    result.Should().BeOfType<CreatedResult>();
}
```

---

## Documentation

### Code Documentation

**C# XML Comments:**
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

**TypeScript JSDoc:**
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

### Documentation Updates

When to update documentation:
- **New features** - Update README.md and relevant guides
- **API changes** - Update Swagger comments
- **Architecture changes** - Update `docs/ARCHITECTURE.md`
- **Setup changes** - Update `docs/guides/SETUP_AND_USAGE.md`

---

## Questions and Support

### Where to Get Help

1. **Documentation** - Check [`docs/`](docs/) folder
2. **Troubleshooting** - See [Setup Guide](docs/guides/SETUP_AND_USAGE.md#troubleshooting)
3. **Issues** - Search existing GitHub issues
4. **Team** - Contact a team member

### Asking Good Questions

When asking for help, include:
- What you're trying to accomplish
- What you've already tried
- Error messages (full stack trace)
- Environment details (OS, .NET version, Node version)
- Steps to reproduce the issue

---

## Recognition

We appreciate all contributions! Contributors will be:
- Acknowledged in commit history
- Added to the team contributors list (if significant contributions)
- Recognized in release notes

---

## Additional Resources

- [Project Architecture](docs/ARCHITECTURE.md)
- [Coding Standards](docs/standards/CODING_STANDARDS.md)
- [Setup Guide](docs/guides/SETUP_AND_USAGE.md)
- [Project Organization](docs/PROJECT_ORGANIZATION.md)

---

## License

By contributing to Fadebook, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Fadebook! 🎉

**Questions?** Open an issue or contact a team member.
