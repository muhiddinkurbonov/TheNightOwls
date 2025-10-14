using AutoMapper;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Fadebook.DB;
using Fadebook.Services;
using Fadebook.Repositories;
using Fadebook.Models;
using Fadebook.Middleware;
using Fadebook.Common.Converters;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to handle dates as UTC
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ensure DateTime values are serialized as UTC in ISO 8601 format
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// CORS: allow Next.js dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: **Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAutoMapper(typeof(Program));

// Register HttpClientFactory
builder.Services.AddHttpClient();

// Make Configuration explicitly available for DI
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Register HttpClient factory and a named client for Google APIs
// builder.Services.AddHttpClient("google", client =>
// {
//     client.BaseAddress = new Uri("https://www.googleapis.com/");
//     client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
// });

builder.Services.AddDbContext<FadebookDbContext>((options) =>
{
    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    options.UseSqlServer(connectionString);
});

// JWT Authentication
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT_SECRET_KEY is missing in .env file!");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? throw new InvalidOperationException("JWT_ISSUER is missing in .env file!");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? throw new InvalidOperationException("JWT_AUDIENCE is missing in .env file!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes of clock skew
    };

    // Add event handlers for better security
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
    options.AddPolicy("Barber", policy => policy.RequireRole("Barber"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// repository classes for DI
builder.Services.AddScoped<IDbTransactionContext, DbTransactionContext>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IBarberRepository, BarberRepository>();
builder.Services.AddScoped<IBarberServiceRepository, BarberServiceRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBarberWorkHoursRepository, BarberWorkHoursRepository>();

// service classes for DI
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<ICustomerAppointmentService, CustomerAppointmentService>();
builder.Services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
builder.Services.AddScoped<IBarberManagementService, BarberManagementService>();
builder.Services.AddScoped<IServiceManagementService, ServiceManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBarberWorkHoursService, BarberWorkHoursService>();
// builder.Services.AddScoped<IInstructorService, InstructorService>();
// builder.Services.AddScoped<ICourseService, CourseService>();

// configure logger
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger(); // read from appsettings.json
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add security headers
app.Use(async (context, next) =>
{
    // Prevent clickjacking attacks
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Prevent MIME type sniffing
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Enable XSS protection (for older browsers)
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Enforce HTTPS
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    // Content Security Policy
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' http://localhost:3000");

    // Referrer Policy
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Permissions Policy (formerly Feature Policy)
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    await next();
});

// Apply CORS early in the pipeline, before authentication
app.UseCors("Frontend");

// Exception handling middleware should be first in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers(); 

await SeedApp(app);

app.Run();

static async Task SeedApp(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FadebookDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var customerList = await dbContext.customerTable.ToListAsync();
            if (customerList.Count() == 0)
            {
                // Seed Users (for authentication)
                var userDean = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "dean.barber",
                    Email = "dean.barber@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("barber123"),
                    Name = "Dean Barber",
                    PhoneNumber = "555-0101",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userVictor = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "victor.barber",
                    Email = "victor.barber@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("barber123"),
                    Name = "Victor Barber",
                    PhoneNumber = "555-0102",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userCharles = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "charles.barber",
                    Email = "charles.barber@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("barber123"),
                    Name = "Charles Barber",
                    PhoneNumber = "555-0103",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userCustomer = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "john.customer",
                    Email = "john.customer@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"),
                    Name = "John Customer",
                    PhoneNumber = "555-0201",
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                // Seed an Admin user
                var userAdmin = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "admin",
                    Email = "admin@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Name = "Admin User",
                    PhoneNumber = "555-0001",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded UserModel records");

                // Seed Services
                var serviceHaircut = await dbContext.serviceTable.AddAsync(new ServiceModel
                {
                    ServiceName = "Haircut",
                    ServicePrice = 20.00
                });
                var serviceBeard = await dbContext.serviceTable.AddAsync(new ServiceModel
                {
                    ServiceName = "Haircut and Beard",
                    ServicePrice = 15.00
                });
                var serviceShampoo = await dbContext.serviceTable.AddAsync(new ServiceModel
                {
                    ServiceName = "Shampoo",
                    ServicePrice = 18.00
                });
                var serviceTheWorks = await dbContext.serviceTable.AddAsync(new ServiceModel
                {
                    ServiceName = "TheWorks",
                    ServicePrice = 25.00
                });

                // Seed Barbers (legacy table)
                var barberDean = await dbContext.barberTable.AddAsync(new BarberModel
                {
                    Username = "dean.barber",
                    Name = "Dean Barber",
                    Specialty = "Beard Specialist",
                    ContactInfo = "555-0101"
                });
                var barberVictor = await dbContext.barberTable.AddAsync(new BarberModel
                {
                    Username = "victor.barber",
                    Name = "Victor Barber",
                    Specialty = "Modern Styles",
                    ContactInfo = "555-0102"
                });
                var barberCharles = await dbContext.barberTable.AddAsync(new BarberModel
                {
                    Username = "charles.barber",
                    Name = "Charles Barber",
                    Specialty = "Full Service",
                    ContactInfo = "555-0103"
                });

                // Seed Customer (legacy table)
                var customerJohn = await dbContext.customerTable.AddAsync(new CustomerModel
                {
                    Username = "john.customer",
                    Name = "John Customer",
                    ContactInfo = "555-0201"
                });

                await dbContext.SaveChangesAsync();
                logger.LogInformation("Seeded Services, Barbers, and Customers");

                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberDean.Entity.BarberId,
                    ServiceId = serviceHaircut.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberDean.Entity.BarberId,
                    ServiceId = serviceBeard.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberVictor.Entity.BarberId,
                    ServiceId = serviceBeard.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberCharles.Entity.BarberId,
                    ServiceId = serviceHaircut.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberCharles.Entity.BarberId,
                    ServiceId = serviceBeard.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberCharles.Entity.BarberId,
                    ServiceId = serviceShampoo.Entity.ServiceId
                });
                await dbContext.barberServiceTable.AddAsync(new BarberServiceModel
                {
                    BarberId = barberCharles.Entity.BarberId,
                    ServiceId = serviceTheWorks.Entity.ServiceId
                });

                // Seed work hours for Dean (Monday-Friday, 9 AM - 5 PM)
                for (int day = 1; day <= 5; day++) // Monday to Friday
                {
                    await dbContext.barberWorkHoursTable.AddAsync(new BarberWorkHoursModel
                    {
                        BarberId = barberDean.Entity.BarberId,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsActive = true
                    });
                }

                // Seed work hours for Victor (Tuesday-Saturday, 10 AM - 6 PM)
                for (int day = 2; day <= 6; day++) // Tuesday to Saturday
                {
                    await dbContext.barberWorkHoursTable.AddAsync(new BarberWorkHoursModel
                    {
                        BarberId = barberVictor.Entity.BarberId,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(10, 0),
                        EndTime = new TimeOnly(18, 0),
                        IsActive = true
                    });
                }

                // Seed work hours for Charles (Monday-Saturday, 8 AM - 4 PM)
                for (int day = 1; day <= 6; day++) // Monday to Saturday
                {
                    await dbContext.barberWorkHoursTable.AddAsync(new BarberWorkHoursModel
                    {
                        BarberId = barberCharles.Entity.BarberId,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(8, 0),
                        EndTime = new TimeOnly(16, 0),
                        IsActive = true
                    });
                }

                var johnAppointmentWithDean = await dbContext.appointmentTable.AddAsync(new AppointmentModel
                {
                    AppointmentDate = DateTime.UtcNow.AddYears(1),
                    Status = "Pending", // Pending, Completed, Cancelled, Expired
                    BarberId = barberDean.Entity.BarberId,
                    ServiceId = serviceBeard.Entity.ServiceId,
                    CustomerId = customerJohn.Entity.CustomerId
                });

                await dbContext.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }
}
