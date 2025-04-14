using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TSU360.Configurations;
using TSU360.Database;
using TSU360.Models.Entities;
using TSU360.Services.Implementations;
using TSU360.Services.Interfaces;
using FluentValidation;
using TSU360.Models.Enums;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Your frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // If using cookies/auth
        });
});
// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TSU360 API", Version = "v1" });

    c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
    {
        Description = "Raw JWT Token (without 'Bearer' prefix)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", 
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>() 
.AddEntityFrameworkStores<ApplicationDbContext>() 
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<User>>() 
.AddUserManager<UserManager<User>>();

// JWT Configuration
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
if (jwtConfig == null)
{
    throw new InvalidOperationException("JwtConfig is not configured properly in appsettings.json");
}
builder.Services.AddSingleton(jwtConfig);

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        ClockSkew = TimeSpan.Zero
    };
});

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IShopService, ShopService>();


builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed Database
await SeedDatabase();

app.Run();

async Task SeedDatabase()
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Seed roles
        string[] roleNames = { "Admin", "Curator", "Volunteer", "Attendee" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed admin user
        var adminEmail = "admin@tsu360.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                UserRole = UserRole.Admin, // Explicitly set
                Faculty = Faculty.Other,
                Degree = Degree.Bachelor
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed a Curator user (example)
        var curatorEmail = "curator@tsu360.com";
        var curatorUser = await userManager.FindByEmailAsync(curatorEmail);
        if (curatorUser == null)
        {
            curatorUser = new User
            {
                UserName = curatorEmail,
                Email = curatorEmail,
                FirstName = "Curator",
                LastName = "User",
                EmailConfirmed = true,
                UserRole = UserRole.Curator, // Explicitly set
                Faculty = Faculty.Other,
                Degree = Degree.Bachelor
            };

            var result = await userManager.CreateAsync(curatorUser, "Curator@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(curatorUser, "Curator");
            }
        }

        // Seed a Volunteer user (example)
        var volunteerEmail = "volunteer@tsu360.com";
        var volunteerUser = await userManager.FindByEmailAsync(volunteerEmail);
        if (volunteerUser == null)
        {
            volunteerUser = new User
            {
                UserName = volunteerEmail,
                Email = volunteerEmail,
                FirstName = "Volunteer",
                LastName = "User",
                EmailConfirmed = true,
                UserRole = UserRole.Volunteer, // Explicitly set
                Faculty = Faculty.Other,
                Degree = Degree.Bachelor
            };

            var result = await userManager.CreateAsync(volunteerUser, "Volunteer@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(volunteerUser, "Volunteer");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}