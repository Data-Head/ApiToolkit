# API Toolkit

This package contains a set of various tools that can be used during the development of a Web API.

## Identity and Authentication

This package contains a permissions system build on top of .NET Identity. All that needs to be supplied is an Enum
containing application permissions. An example enum is listed here:

```csharp
public enum Permission
{
    CreateUsers = 101,
    ReadUsers = 102,
    UpdateUsers = 103,
    DeleteUsers = 104,
    CreateRoles = 201,
    ReadRoles = 202,
    UpdateRoles = 203,
    DeleteRoles = 204,
}
```

To set this up, make sure your database context extends ``IdentityDbContext`` and specifies the following types:

```csharp
public class ApplicationContext : IdentityDbContext<IdentityUser, RoleWithPermissions<Permission>, string> {

}
```

The ``RoleWithPermissions`` model required a Permissions enum to be specified. You can use the default ``IdentityUser``
class or provide one of your own.

After this, you can call ``AddIdentity`` on the ``IServiceCollection`` using the ``RoleWithPermissions`` role class.

A helper function is included to configure authentication. You can call ``AddJwtAuthentication`` on
the ``IServiceCollection`` and provide a secret, audience, and issuer in order to configure JWT authentication.

In order for the permissions policies to function, you must call two more functions on the service collection.

```csharp
builder.Services.AddAuthorizationWithPerms<Permission>();
builder.Services.AddPermissionHandler<Permission, IdentityUser, RoleWithPermissions<Permission>>();
```

If you want to ensure a SuperAdmin role is always available, you can call the following function before ``app.Run()`` is
called.

```csharp
WebApplicationHelpers.GenerateDefaultRoles<Permission, RoleWithPermissions<Permission>>(builder.Services);
```

Make sure that authentication and authorization is set before the run function is called.

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## Models, View Models and the Generic Repository

A ``BaseModel`` class is provided for database models that provides  ``CreatedAt`` and ``UpdatedAt`` date fields. If a
model extends from this class, then these values are updated if saved through the ``GenericRepository`` class. The
Generic Repository provides a repository class for creating and modifying database models. It also exposes the
underlying query system if it needs to be used. You can add a repository to the IOC container using the code below:

```csharp
builder.Services.AddScoped<IGenericRepository<MyModel>, GenericRepository<MyModel>>();
```

## API Response and Pagination

An ``ApiResponse`` class is provided. This standardizes the format of an API response from the server.

A ``PaginatedData`` class also exists that formats data along with pagination information.
