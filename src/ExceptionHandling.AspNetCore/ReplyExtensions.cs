using ExceptionHandling.Error;
using ExceptionHandling.Reply;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandling.AspNetCore
{
 public static class ReplyExtensions
 {
  public static IActionResult ToIActionResult<T>(this Reply<T> reply, ControllerBase controller)
  {
   if (reply.IsSuccess || reply.Error is null)
   {
    return controller.Ok(reply.Value);
   }

   return ToProblem(reply.Error, controller);
  }

  public static ActionResult<T> ToActionResult<T>(this Reply<T> reply, ControllerBase controller)
  {
   if (reply.IsSuccess || reply.Error is null)
   {
    return controller.Ok(reply.Value);
   }

   return ToProblem(reply.Error, controller);
  }

  private static ObjectResult ToProblem(IError error, ControllerBase controller)
  {
   var problem = controller.ProblemDetailsFactory.CreateProblemDetails(
    controller.HttpContext,
    statusCode: error.StatusCode,
    title: error.Title,
    detail: error.Message,
    instance: controller.Request.Path);

   problem.Extensions["timestamp"] = DateTime.UtcNow;

   return new ObjectResult(problem) { StatusCode = problem.Status };
  }
 }
}
