using Microsoft.AspNetCore.Authorization;

namespace ApiToolkit.Authorization.Requirements;

public class PermissionRequirement<T> : IAuthorizationRequirement where T : Enum
{
    public T Permission { get; set; }

    public PermissionRequirement(T permission)
    {
        Permission = permission;
    }
}
