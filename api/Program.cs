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

Env.Load();
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(); // let's add the controller classes as well...

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
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

// service classes for DI
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<ICustomerAppointmentService, CustomerAppointmentService>();
builder.Services.AddScoped<IAppointmentManagementService, AppointmentManagementService>();
builder.Services.AddScoped<IBarberManagementService, BarberManagementService>();
builder.Services.AddScoped<IServiceManagementService, ServiceManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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

// Exception handling middleware should be first in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

// Apply CORS before mapping controllers
app.UseCors("Frontend");
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
                    Username = "dean-the-machine",
                    Email = "dean@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Name = "Dean",
                    PhoneNumber = "123",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userVictor = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "v-for-victor",
                    Email = "victor@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Name = "Victor",
                    PhoneNumber = "456",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userCharles = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "charles-xavier",
                    Email = "charles@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Name = "Charles",
                    PhoneNumber = "789",
                    Role = "Barber",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                var userMuhid = await dbContext.userTable.AddAsync(new UserModel
                {
                    Username = "m-kurbonov",
                    Email = "muhiddin@fadebook.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Name = "Muhiddin",
                    PhoneNumber = "1456",
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
                    Name = "Administrator",
                    PhoneNumber = "000",
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
                    Username = "dean-the-machine",
                    Name = "Dean",
                    Specialty = "Beards",
                    ContactInfo = "123"
                });
                var barberVictor = await dbContext.barberTable.AddAsync(new BarberModel
                {
                    Username = "v-for-victor",
                    Name = "Victor",
                    Specialty = "Styled Hair Cuts",
                    ContactInfo = "456"
                });
                var barberCharles = await dbContext.barberTable.AddAsync(new BarberModel
                {
                    Username = "charles-xavier",
                    Name = "Charles",
                    Specialty = "The Works",
                    ContactInfo = "789"
                });

                // Seed Customer (legacy table)
                var customerMuhid = await dbContext.customerTable.AddAsync(new CustomerModel
                {
                    Username = "m-kurbonov",
                    Name = "Muhiddin",
                    ContactInfo = "1456"
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

                var muhidAppointmentWithDean = await dbContext.appointmentTable.AddAsync(new AppointmentModel
                {
                    AppointmentDate = DateTime.UtcNow.AddYears(1),
                    Status = "Pending", // Pending, Completed, Cancelled, Expired
                    BarberId = barberDean.Entity.BarberId,
                    ServiceId = serviceBeard.Entity.ServiceId,
                    CustomerId = customerMuhid.Entity.CustomerId
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
