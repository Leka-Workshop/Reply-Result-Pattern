using Microsoft.Extensions.DependencyInjection;

namespace ExceptionHandling.AspNetCore
{
 public static class ServiceCollectionExtensions
 {
  /// <summary>
  /// Registers the global exception handler and Problem Details.
  /// Don't forget app.UseExceptionHandler() in the request pipeline.
  /// </summary>
  public static IServiceCollection AddReplyExceptionHandling(this IServiceCollection services)
  {
   services.AddExceptionHandler<GlobalExceptionsHandler>();
   services.AddProblemDetails();

   return services;
  }
 }
}
