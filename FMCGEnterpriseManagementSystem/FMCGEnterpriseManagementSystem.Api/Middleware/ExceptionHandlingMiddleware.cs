using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

/*****************************
*    Title: Handle errors in ASP.NET Core APIs
*    Author: Microsoft
*    Date: 2024
*    Code version: ASP.NET Core 10
*    Availability: https://learn.microsoft.com/aspnet/core/web-api/handle-errors
******************************/


namespace FMCGEnterpriseManagementSystem.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


            /*****************************
*    Title: Handle errors in ASP.NET Core APIs
*    Author: Microsoft
*    Date: 2024
*    Code version: ASP.NET Core 10
*    Availability: https://learn.microsoft.com/aspnet/core/web-api/handle-errors
******************************/

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problem);
            return context.Response.WriteAsync(json);
        }
    }
}