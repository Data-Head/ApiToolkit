using ApiToolkit.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ApiToolkit;

public class WebApplicationHelpers
{
    /**
     * Generates a SuperAdmin role if it does not exist
     */
    public static void GenerateDefaultRoles<T, TR>(IServiceProvider services)
        where T : Enum where TR : RoleWithPermissions<T>, new()
    {
        var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TR>>();

        var hasSuperAdminRole = roleManager.RoleExistsAsync("SuperAdmin");
        hasSuperAdminRole.Wait();

        if (hasSuperAdminRole.Result) return;

        var roleResult = roleManager.CreateAsync(new TR()
            {Name = "SuperAdmin", Permissions = Enum.GetValues(typeof(T)).Cast<T>().ToList()});

        roleResult.Wait();
    }
}