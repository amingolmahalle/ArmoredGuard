using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Helpers;
using Common.Settings;
using Data;
using Data.Contracts;
using Data.Repositories;
using Entities.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using Services.Services;
using Services.Services.Redis;

namespace Web.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                SecurityKey secretKey = SecurityHelper.CreateSecurityKey(jwtSettings.SecretKey);
                SecurityKey encryptionKey = SecurityHelper.CreateEncryptionKey(jwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secretKey,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = encryptionKey
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        ILogger logger = context.HttpContext
                            .RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger(nameof(JwtBearerEvents));

                        logger.LogError("Authentication failed.", context.Exception);
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 401;

                        return context.Response.WriteAsync("Unauthorized");
                    },
                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                        if (context.Principal != null)
                        {
                            ClaimsIdentity claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                            if (claimsIdentity?.Claims.Any() != true)
                                context.Fail("This token has no claims.");

                            string securityStamp =
                                claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);

                            if (!securityStamp.HasValue())
                                context.Fail("This token has no security stamp");

                            //Find user and token from database and perform your custom validation
                            int userId = claimsIdentity.GetUserId<int>();
                            User user = await userService.GetByIdAsync(userId, context.HttpContext.RequestAborted);

                            SignInManager<User> signInManager = context.HttpContext.RequestServices
                                .GetRequiredService<SignInManager<User>>();

                            User validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);

                            if (validatedUser == null)
                            {
                                //UnAuthorized
                                context.Fail("Token security stamp is not valid.");
                            }

                            if (!user.IsActive)
                                context.Fail("User is not active");

                            await userService.UpdateLastSeenDateAsync(user, context.HttpContext.RequestAborted);
                        }
                    },
                    OnChallenge = context =>
                    {
                        ILogger logger = context.HttpContext
                            .RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger(nameof(JwtBearerEvents));

                        logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                        if (!context.Response.HasStarted)
                        {
                            context.Response.ContentType = "application/json";

                            context.Response.StatusCode = 401;

                            return context.Response.WriteAsync("Unauthorized");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            services.AddIdentity<User, Role>(identityOptions =>
                {
                    //Password Settings
                    identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
                    identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
                    identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric; //#@!
                    identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
                    identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

                    //UserName Settings
                    identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }


        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
        }

        public static void AddInjectionServices(this IServiceCollection services)
        {
            //Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOAuthService, OAuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRedisService, RedisService>();

            //Repositories
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOAuthClientRepository, OAuthClientRepository>();
            services.AddScoped<IOAuthRefreshTokenRepository, OAuthRefreshTokenRepository>();
        }
    }
}