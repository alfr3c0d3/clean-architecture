using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace CleanArchitecture.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            object errors = null;

            switch (ex)
            {
                case RestException re:
                    Log.Error(ex, "REST ERROR");
                    errors = re.Errors;
                    context.Response.StatusCode = (int)re.Code;
                    break;
                case { } e:
                    Log.Error(ex, "SERVER ERROR");
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            if (errors != null)
            {
                var result = JsonSerializer.Serialize(new
                {
                    errors
                });

                await context.Response.WriteAsync(result);
            }

        }
    }
}
