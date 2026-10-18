using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExceptionHandling.AspNetCore
{
 public sealed class GlobalExceptionsHandler(ILogger<GlobalExceptionsHandler> logger) : IExceptionHandler
 {
  public async ValueTask<bool> TryHandleAsync(
   HttpContext httpContext,
   Exception exception,
   CancellationToken cancellationToken)
  {
   var request = httpContext.Request;
   logger.LogError(exception, "Unhandled exception: {Method} {Path}", request.Method, request.Path);

   var problem = new ProblemDetails
   {
    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
    Title = "An unexpected error occurred",
    Status = StatusCodes.Status500InternalServerError,
    Detail = "Something went wrong on our side. Please try again later.",
    Instance = request.Path
   };

   problem.Extensions["traceId"] = httpContext.TraceIdentifier;
   httpContext.Response.StatusCode = problem.Status.Value;

   await httpContext.Response.WriteAsJsonAsync(
    problem,
    options: null,
    contentType: "application/problem+json",
    cancellationToken);

   return true;
  }
 }
}
