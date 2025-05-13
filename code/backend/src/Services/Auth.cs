using System.Collections.Immutable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Extensions;

namespace backend.src.Services;

public enum Role
{
    Voter,
    BoardPresident,
    Admin,
    Police,
}

public class SafeAuthorizeAttribute : AuthorizeAttribute
{
    public SafeAuthorizeAttribute(Role[] roles)
        : base()
    {
        Roles = RolesToString(roles);
    }

    public static string RolesToString(IEnumerable<Role> roles) =>
        string.Join(
            ",",
            roles.Select(r => r.GetDisplayName()).ToImmutableSortedSet()
        );
}
