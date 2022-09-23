using System.Security.Claims;
using ApiToolkit.Authorization.Requirements;
using ApiToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiToolkit.Authorization.Handlers;

public class PermissionHandler<T, TU, TR> : AuthorizationHandler<PermissionRequirement<T>>
    where T : Enum
    where TU : IdentityUser
    where TR : RoleWithPermissions<T>
{
    private readonly UserManager<TU> _userManager;
    private readonly RoleManager<TR> _roleManager;

    public PermissionHandler(UserManager<TU> userManager, RoleManager<TR> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement<T> requirement)
    {
        var id = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
        if (id == null)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.FindByIdAsync(id.Value);
        if (user == null)
        {
            context.Fail();
            return;
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = await _roleManager.Roles.Where(role => roleNames.Contains(role.Name)).ToListAsync();

        if (roles.Any(role => role.Permissions.Contains(requirement.Permission)))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
