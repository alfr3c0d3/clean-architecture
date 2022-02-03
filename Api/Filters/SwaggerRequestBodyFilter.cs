using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArchitecture.Api.Filters
{
    /// <summary>
    /// This Filter is for showing the Properties' Names instead of "body" in Swagger UI.
    /// </summary>
    public class SwaggerRequestBodyFilter : IRequestBodyFilter
    {
        public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
        {
            var parameterInfo = context.BodyParameterDescription?.ParameterInfo();

            if (parameterInfo != null)
            {
                requestBody.Extensions.Add("x-bodyName", new OpenApiString(parameterInfo.Name));
            }
        }
    }
}