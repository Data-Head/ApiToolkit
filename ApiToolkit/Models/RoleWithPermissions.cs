using Microsoft.AspNetCore.Identity;

namespace ApiToolkit.Models;

public class RoleWithPermissions<T> : IdentityRole where T : Enum
{
    public List<T> Permissions { get; set; } = null!;
}