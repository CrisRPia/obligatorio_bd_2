using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace backend.src.Services;

// Somewhat ai generated.
public class AppendAuthorizeToSummaryOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var anonymousAttributes = endpointMetadata.OfType<AllowAnonymousAttribute>().ToList();

        var authorizeAttributes = endpointMetadata.OfType<AuthorizeAttribute>().ToList();

        if (!anonymousAttributes.Any() && !authorizeAttributes.Any())
            return;

        // Endpoint requires authentication/authorization
        operation.Summary += " (Secured";

        var roles = authorizeAttributes
            .Where(a => !string.IsNullOrEmpty(a.Roles))
            .Select(a => a.Roles)
            .Distinct();

        if (roles.Any())
        {
            // Split roles string by comma and trim whitespace
            var roleList = roles
                .SelectMany(r => r?.Split(',').Select(role => role.Trim()) ?? [])
                .Distinct();
            operation.Summary += " - Roles: " + string.Join(", ", roleList);
        }

        // Get policies from all Authorize attributes
        var policies = authorizeAttributes
            .Where(a => !string.IsNullOrEmpty(a.Policy))
            .Select(a => a.Policy)
            .Distinct();

        if (policies.Any())
            operation.Summary += " - Policies: " + string.Join(", ", policies);

        operation.Summary += ")";
    }
}
