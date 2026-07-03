using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.API.OpenApi;

public sealed class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters is null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.Name == parameter.Name);

            if (description is null)
                continue;

            parameter.Description ??= description.ModelMetadata?.Description;
        }
    }
}
