# Fadebook API

A comprehensive barbershop management API built with ASP.NET Core 9.0, providing appointment scheduling, customer management, and barber service coordination.

## 🚀 Features

- **Appointment Management** - Create, update, and track barbershop appointments
- **Customer Management** - Handle customer accounts and profiles
- **Barber Services** - Manage barbers, their specialties, and available services
- **Service Catalog** - Maintain a catalog of services with pricing
- **RESTful API** - Clean, intuitive endpoints following REST principles
- **Swagger Documentation** - Interactive API documentation and testing interface
- **Structured Logging** - Comprehensive logging with Serilog
- **CORS Support** - Configured for frontend integration

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- A code editor (Visual Studio, VS Code, or Rider)

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: JWT Bearer (configured, currently disabled)
- **Mapping**: AutoMapper 12.0
- **Logging**: Serilog 9.0
- **API Documentation**: Swagger/OpenAPI
- **Configuration**: DotNetEnv for environment variables

## 📦 Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TheNightOwls/api
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure environment variables**
   
   Create a `.env` file in the `api` directory:
   ```env
   CONNECTION_STRING=Server=localhost;Database=FadebookDb;Trusted_Connection=True;TrustServerCertificate=True;
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## 🗂️ Project Structure

```
api/
├── Controllers/          # API endpoint controllers
│   ├── AppointmentController.cs
│   ├── BarberController.cs
│   ├── CustomerAccountController.cs
│   ├── CustomerController.cs
│   └── ServiceController.cs
├── DB/                   # Database context
├── DTO/                  # Data Transfer Objects
├── Exceptions/           # Custom exception classes
├── Mapping/              # AutoMapper profiles
├── Middleware/           # Custom middleware
├── Migrations/           # EF Core migrations
├── Models/               # Entity models
├── Repositories/         # Data access layer
│   └── implementations/
├── Services/             # Business logic layer
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration settings
```

## 🔌 API Endpoints

### Appointments
- `POST /api/appointment` - Create a new appointment
- `GET /api/appointment/{id}` - Get appointment by ID
- `PUT /api/appointment/{id}` - Update an appointment
- `DELETE /api/appointment/{id}` - Delete an appointment
- `GET /api/appointment/by-date?date={date}` - Get appointments by date
- `GET /api/appointment/by-username/{username}` - Get appointments by customer username

### Customers
- `POST /api/customer` - Create a new customer
- `GET /api/customer/{id}` - Get customer by ID
- `PUT /api/customer/{id}` - Update customer information
- `DELETE /api/customer/{id}` - Delete a customer

### Barbers
- `POST /api/barber` - Create a new barber
- `GET /api/barber/{id}` - Get barber by ID
- `PUT /api/barber/{id}` - Update barber information
- `DELETE /api/barber/{id}` - Delete a barber
- `GET /api/barber/{id}/services` - Get services offered by a barber

### Services
- `POST /api/service` - Create a new service
- `GET /api/service/{id}` - Get service by ID
- `PUT /api/service/{id}` - Update service information
- `DELETE /api/service/{id}` - Delete a service
- `GET /api/service` - Get all services

For detailed API documentation, visit the Swagger UI at `/swagger` when running the application.

## 🗃️ Database Schema

The application uses the following main entities:

- **Customer** - Customer information and contact details
- **Barber** - Barber profiles with specialties
- **Service** - Available services with pricing
- **BarberService** - Many-to-many relationship between barbers and services
- **Appointment** - Scheduled appointments linking customers, barbers, and services

## 🔧 Configuration

### CORS
The API is configured to accept requests from `http://localhost:3000` by default. Update the CORS policy in `Program.cs` to add additional origins:

```csharp
policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
```

### Logging
Logging is configured via `appsettings.json`. Logs are written to:
- Console output
- File system (in the `Logs/` directory)

### Database Connection
The connection string is loaded from the `CONNECTION_STRING` environment variable. Ensure this is set in your `.env` file or system environment variables.

## 🧪 Development

### Running Migrations
Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Apply migrations:
```bash
dotnet ef database update
```

### Seed Data
The application automatically seeds initial data on first run, including:
- Sample services (Haircut, Beard, Shampoo, The Works)
- Sample barbers (Dean, Victor, Charles)
- Sample customer (Muhiddin)
- Sample appointment

## 🔐 Authentication (Configured, Currently Disabled)

The API includes JWT authentication infrastructure that is currently commented out. To enable:

1. Uncomment the authentication/authorization sections in `Program.cs`
2. Configure JWT settings in `appsettings.json`
3. Implement user registration and login endpoints

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## 👥 Team

The Night Owls - Revature .NET Training
