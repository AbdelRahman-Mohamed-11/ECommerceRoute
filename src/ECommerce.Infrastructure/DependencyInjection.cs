using System.Text;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Interceptors;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable("__ApplicationMigrationsHistory", "app"))
                .EnableSensitiveDataLogging();
        });

        services.AddDbContext<AppIdentityDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable("__IdentityMigrationsHistory", "identity"))
                .EnableSensitiveDataLogging();
        });

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
        var jwtSettings = config.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException($"Configuration section '{JwtSettings.SectionName}' is missing.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 characters.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailVerificationCodeStore, HybridEmailVerificationCodeStore>();
        services.AddScoped<IEmailSender, FluentEmailSender>();

        services.Configure<EmailVerificationSettings>(
            config.GetSection(EmailVerificationSettings.SectionName));

        AddFluentEmail(services, config);

        services.AddScoped<IAuditInterceptor, AuditInterceptor>();
        services.AddScoped<ISoftDeleteInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IDataSeeder, IdentitySeeder>();
        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();

        services.AddScoped<DatabaseSeeder>();

        services.Configure<CloudinarySettings>(config.GetSection(nameof(CloudinarySettings)));
        services.AddScoped<IAttachmentService, AttachmentService>();

        AddBasketCaching(services, config);

        return services;
    }

    private static void AddFluentEmail(IServiceCollection services, IConfiguration config)
    {
        services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));

        var emailSettings = config.GetSection(EmailSettings.SectionName).Get<EmailSettings>()
            ?? throw new InvalidOperationException($"Configuration section '{EmailSettings.SectionName}' is missing.");

        if (string.IsNullOrWhiteSpace(emailSettings.FromEmail))
            throw new InvalidOperationException("Email:FromEmail is required.");

        var builder = services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName);

        if (!string.IsNullOrWhiteSpace(emailSettings.Username))
        {
            builder.AddSmtpSender(
                emailSettings.Host,
                emailSettings.Port,
                emailSettings.Username,
                emailSettings.Password);
        }
        else
        {
            builder.AddSmtpSender(emailSettings.Host, emailSettings.Port);
        }
    }

    private static void AddBasketCaching(IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<CacheEntryPolicy>("Basket")
            .Bind(config.GetSection("CachedAggregates:Basket"))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<CacheEntryPolicy>, CacheEntryPolicyValidator>();

        var redisConnection = config.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnection);
        }

        services.AddHybridCache();

        services.AddScoped(typeof(ICachedAggregateStore<>), typeof(HybridCacheAggregateStore<>));
        services.AddScoped<IBasketStore, HybridBasketStore>();
    }
}
