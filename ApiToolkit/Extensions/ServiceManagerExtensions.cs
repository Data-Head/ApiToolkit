using System.Text;
using ApiToolkit.Authorization.Handlers;
using ApiToolkit.Authorization.Requirements;
using ApiToolkit.Models;
using ApiToolkit.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ApiToolkit.Extensions;

public static class ServiceManagerExtensions
{
    public static IServiceCollection AddAuthorizationWithPerms<T>(this IServiceCollection services)
        where T : Enum
    {
        services.AddAuthorization((options =>
        {
            var enumValues = Enum.GetValues(typeof(T));

            foreach (var value in (T[]) enumValues)
            {
                options.AddPolicy(
                    Enum.GetName(typeof(T), value.GetHashCode()) ?? throw new InvalidOperationException(),
                    bd => bd.Requirements.Add(new PermissionRequirement<T>(value)));
            }
        }));

        return services;
    }

    public static IServiceCollection AddAuthorizationWithPerms<T>(this IServiceCollection services,
        Action<AuthorizationOptions> configure) where T : Enum
    {
        services.AddAuthorization(configure + (options =>
        {
            var enumValues = Enum.GetValues(typeof(T));

            foreach (var value in (T[]) enumValues)
            {
                options.AddPolicy(
                    Enum.GetName(typeof(T), value.GetHashCode()) ?? throw new InvalidOperationException(),
                    bd => bd.Requirements.Add(new PermissionRequirement<T>(value)));
            }
        }));

        return services;
    }

    public static IServiceCollection AddPermissionHandler<T, TU, TR>(this IServiceCollection services)
        where T : Enum
        where TU : IdentityUser
        where TR : RoleWithPermissions<T>
    {
        services.AddScoped<IAuthorizationHandler, PermissionHandler<T, TU, TR>>();
        return services;
    }

    public static IServiceCollection AddIdentityRepositories<T, TU, TR, TJ>(this IServiceCollection services)
        where T : Enum
        where TU : IdentityUser
        where TR : RoleWithPermissions<T>
        where TJ : DbContext
    {
        services.AddScoped<IGenericRepository<TR>, GenericRepository<TR, TJ>>();
        services.AddScoped<IGenericRepository<TU>, GenericRepository<TU, TJ>>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        string secret,
        string audience,
        string issuer
    )
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidAudience = audience,
                ValidIssuer = issuer
            };
        });

        return services;
    }
}