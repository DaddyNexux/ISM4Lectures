
using FuckingLectures.ActionFilter;
using FuckingLectures.ActionFilters;
using FuckingLectures.Data;
using FuckingLectures.Helpers;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Models.Entities;
using FuckingLectures.Services;
using FuckingLectures.Services.Attachment;
using FuckingLectures.Services.AuthServicesFolder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;

namespace FuckingLectures.Extentions;

public static class AppServicesExtensions
{
    // ----------------------------
    // IDENTITY CONFIG
    // ----------------------------
    public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentity<User, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = false;

            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppData>()
        .AddDefaultTokenProviders();

        services.AddMemoryCache();
        services.AddSession();

        return services;
    }

    // ----------------------------
    // DATABASE CONFIG
    // ----------------------------
    public static IServiceCollection AddDbConnection(this IServiceCollection services)
    {
        var connectionString = ConfigProvider.config.GetConnectionString("C_str");

        services.AddDbContext<AppData>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    // ----------------------------
    // CORS CONFIG
    // ----------------------------
    public static IServiceCollection AddCorss(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    // ----------------------------
    // AUTHENTICATION / JWT CONFIG
    // ----------------------------
    public static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
        string jwtSignInKey = ConfigProvider.config.GetSection("Jwt:SecretKey").Get<string>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "supernova-iq.com",
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignInKey))
            };
        });

        services.AddAuthorization();

        return services;
    }

    // ----------------------------
    // SWAGGER CONFIG (.NET 10 / SWASHBUCKLE 7)
    // ----------------------------
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            // Define two Swagger documents
            opt.SwaggerDoc("app", new OpenApiInfo
            {
                Title = "Lecure App API",
                Version = "v1",
                Description = "Endpoints for the Lecure mobile app."
            });
            opt.SwaggerDoc("dashboard", new OpenApiInfo
            {
                Title = "Lecure Dashboard API",
                Version = "v1",
                Description = "Endpoints used by the admin dashboard."
            });

            // JWT Authorization
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header, \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            // Include endpoints in proper document
            opt.DocInclusionPredicate((docName, apiDesc) =>
            {
                var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                if (controllerActionDescriptor == null) return false;

                var groupAttr = controllerActionDescriptor.MethodInfo
                    .GetCustomAttributes(typeof(SwaggerGroupAttribute), false)
                    .Cast<SwaggerGroupAttribute>()
                    .FirstOrDefault();

                if (groupAttr == null)
                {
                    // Try class-level attribute
                    groupAttr = controllerActionDescriptor.ControllerTypeInfo
                        .GetCustomAttributes(typeof(SwaggerGroupAttribute), false)
                        .Cast<SwaggerGroupAttribute>()
                        .FirstOrDefault();
                }

                if (groupAttr == null)
                {
                    // Include in all documents by default
                    return true;
                }

                return groupAttr.GroupNames.Contains(docName.ToLower());
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[0]
            }
        });
        });

        return services;
    }


    public static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddScoped<IAuthServices, AuthService>();
        services.AddScoped<IAttachmentsService, AttachmentsService>();
        services.AddScoped<DynamicAuthActionFilter>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<ICourseServices, CourseServices>();
        services.AddScoped<ILectureServices, LectureServices>();
        return services;
    }

    // ----------------------------
    // CENTRALIZED API RESPONSE
    // ----------------------------
    public static IServiceCollection CentralizeAPiRespose(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response = ApiResponse<object>.Fail(
                    string.Join("; ", errors),
                    statusCode: StatusCodes.Status400BadRequest
                );

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }

    // ----------------------------
    // REQUEST SIZE LIMIT
    // ----------------------------
    public static IServiceCollection AddSizeLimit(this IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 10 * 1024 * 30 * 100;
        });

        return services;
    }
}
