using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace EExamSystem.Api.Configuration;

public class SecurityRequirementsTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var authAttributes = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (!authAttributes.Any()) return Task.CompletedTask;

        var roles = string.Join(", ", authAttributes
            .Select(a => a.Roles)
            .Where(r => !string.IsNullOrWhiteSpace(r)));

        operation.Description += "\n\n<br/>**SECURITY REQUIREMENTS:**<br/>";
        operation.Description += string.IsNullOrEmpty(roles) 
            ? "Requires a valid login token." 
            : $"Requires one of the following roles: **{roles.Replace(",", ", ")}**";

        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized (Missing or invalid token)" });
        }

        if (!string.IsNullOrEmpty(roles) && !operation.Responses.ContainsKey("403"))
        {
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden (Missing required role)" });
        }

        return Task.CompletedTask;
    }
}