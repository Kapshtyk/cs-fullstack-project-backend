using Ecommerce.Controllers;
using Ecommerce.Controllers.CustomMiddleware;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Database;
using Ecommerce.Infrastructure.Repository;
using Ecommerce.Infrastructure.Services;
using Ecommerce.Services.AuthService;
using Ecommerce.Services.AuthService.Intefaces;
using Ecommerce.Services.CategoryService;
using Ecommerce.Services.CategoryService.Interfaces;
using Ecommerce.Services.ProductService;
using Ecommerce.Services.ProductService.Interfaces;
using Ecommerce.Services.ReviewService;
using Ecommerce.Services.ReviewService.Interfaces;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.CartItemService;
using Ecommerce.Services.CartItemService.Interfaces;
using Ecommerce.Services.OrderService;
using Ecommerce.Services.OrderService.Interfaces;
using Ecommerce.Services.UserService;
using Ecommerce.Services.UserService.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Controllers.CustomAuthorization;
using static Ecommerce.Services.CategoryService.DTO.CreateCategoryDto;
using Microsoft.Extensions.Caching.Distributed;
using Ecommerce.Services.FrontPageService.Interfaces;
using Ecommerce.Services.FrontPageService;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure Services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure Middleware and Endpoints
Configure(app, builder.Environment);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<EcommerceContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

await app.RunAsync();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Controllers and Routing
    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    // Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Crazy Shop API",
            Version = "v1"
        });
        c.EnableAnnotations();
        c.SchemaFilter<EnumSchemaFilter>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Database
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));
    var dataSource = dataSourceBuilder.Build();
    builder.Services.AddDbContext<EcommerceContext>(options =>
        options.UseNpgsql(dataSource).UseSnakeCaseNamingConvention());

    // Cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = configuration["Redis:ConnectionString"];
        options.InstanceName = "local";
    });

    // File Service
    services.AddScoped<IFileService, FileService>(provider =>
    {
        var environment = provider.GetRequiredService<IWebHostEnvironment>();
        return new FileService(environment);
    });

    // FrontPage Services
    services.AddScoped<IFrontPageService, FrontPageService>();
    services.AddScoped<IFrontPageRepo, FrontPageRepository>();
    services.AddScoped(provider =>
    {
        var frontPageService = provider.GetRequiredService<IFrontPageService>();
        return new FrontPageController(frontPageService);
    });

    // Category Services
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<ICategoryRepo, CategoryRepository>();
    services.AddScoped(provider =>
    {
        var categoryService = provider.GetRequiredService<ICategoryService>();
        return new CategoryController(categoryService);
    });

    // CartItem Services
    services.AddScoped<ICartItemService, CartItemService>();
    services.AddScoped<ICartItemRepo, CartItemRepository>();
    services.AddScoped(provider =>
    {
        return new CartItemController(
            provider.GetRequiredService<ICartItemService>(),
            provider.GetRequiredService<IAuthorizationService>(),
            provider.GetRequiredService<ICartItemRepo>()
        );
    });

    // Order Services
    services.AddScoped<IOrderService, OrderService>();
    services.AddScoped<IOrderRepo, OrderRepository>();
    services.AddScoped(provider =>
    {
        return new OrderController(
            provider.GetRequiredService<IOrderService>(),
            provider.GetRequiredService<IOrderRepo>(),
            provider.GetRequiredService<IAuthorizationService>()
        );
    });

    // Product Services
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<IProductRepo, ProductRepository>();
    services.AddScoped(provider =>
    {
        var productService = provider.GetRequiredService<IProductService>();
        return new ProductController(productService);
    });

    // Review Services
    services.AddScoped<IReviewService, ReviewService>();
    services.AddScoped<IReviewRepo, ReviewRepository>();
    services.AddScoped(provider =>
    {
        var reviewService = provider.GetRequiredService<IReviewService>();
        var fileService = provider.GetRequiredService<IFileService>();
        return new ReviewController(reviewService);
    });

    // FluentValidation
    services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();
    services.AddFluentValidationAutoValidation(options =>
    {
        options.DisableBuiltInModelValidation = true;
    });

    // Custom Middleware
    services.AddScoped<ExceptionHandlerMiddleware>();

    // Salt
    services.AddScoped<ISaltRepo, SaltRepository>();

    // Token
    services.AddScoped<ITokenService>(provider =>
    {
        var privateKey = configuration["AuthSettings:PrivateKey"];
        if (string.IsNullOrEmpty(privateKey))
        {
            throw new Exception("Private key is missing");
        }
        return new TokenService(privateKey);
    });

    // Hasher
    services.AddScoped<IHashingService, HashingService>();

    // Auth  
    services.AddScoped(provider => new AuthController(
        provider.GetRequiredService<IAuthService>()
    ));
    services.AddScoped<IAuthService, AuthService>(provider =>
    {
        return new AuthService(
            provider.GetRequiredService<IUserRepo>(),
            provider.GetRequiredService<IHashingService>(),
            provider.GetRequiredService<ITokenService>(),
            provider.GetRequiredService<ISaltRepo>(),
            provider.GetRequiredService<IDistributedCache>()
        );
    });

    // JWT Authentication
    services.AddScoped(provider =>
    {
        return new RevokedTokenValidator(
            provider.GetRequiredService<IDistributedCache>(),
            provider.GetRequiredService<ITokenService>()
        );
    });
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var privateKey = configuration["AuthSettings:PrivateKey"];
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new Exception("Private key is missing");
            }
            var key = Encoding.ASCII.GetBytes(privateKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var customValidator = context.HttpContext.RequestServices.GetRequiredService<RevokedTokenValidator>();
                    if (!await customValidator.ValidateAsync(context))
                    {
                        context.Fail("Token is revoked");
                        return;
                    }
                }
            };
        });

    // User Services
    services.AddScoped<IUserRepo, UserRepository>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped(provider =>
    {
        return new UserController(
            provider.GetRequiredService<IUserService>(),
            provider.GetRequiredService<IAuthorizationService>(),
            provider.GetRequiredService<IUserRepo>()
        );
    });

    // Authorization
    services.AddSingleton<IAuthorizationHandler, CartItemAuthorizationHandler>();
    services.AddSingleton<IAuthorizationHandler, OrderAuthorizationHandler>();
    services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();

    services.AddAuthorizationBuilder()
        .AddPolicy("Ownership", policy =>
            policy.Requirements.Add(new OwnershipAuthorizationRequirement()));

}

void Configure(WebApplication app, IHostEnvironment env)
{
    var allowedOrigin = app.Configuration["AllowedOrigin"];

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", $"{allowedOrigin}");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 204;
            return;
        }

        await next();
    });
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

public partial class Program { }