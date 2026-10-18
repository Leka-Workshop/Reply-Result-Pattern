namespace ExceptionHandling.Error
{
 internal sealed record ErrorCore : IError
 {
  public string Title { get; }
  public string Message { get; }
  public int StatusCode { get; }
  public object? Payload { get; }

  private ErrorCore(string title, string message, int statusCode, object? payload)
  {
   Title = title;
   Message = message;
   StatusCode = statusCode;
   Payload = payload;
  }

  // Factory methods: the only way to create an error.
  internal static IError ValidationError(string message, object? payload = null) =>
   new ErrorCore("ValidationError", message, 400, payload);

  internal static IError ForbiddenError(string message, object? payload = null) =>
   new ErrorCore("ForbiddenError", message, 403, payload);

  internal static IError ResourceNotFoundError(string message, object? payload = null) =>
   new ErrorCore("ResourceNotFoundError", message, 404, payload);

  internal static IError ConflictError(string message, object? payload = null) =>
   new ErrorCore("ConflictError", message, 409, payload);

  internal static IError FatalError(string message, object? payload = null) =>
   new ErrorCore("FatalError", message, 500, payload);

  // The Exception overload may expose internal details so use it with caution.
  internal static IError FatalError(Exception ex, object? payload = null) =>
   new ErrorCore("FatalError", ex.Message, 500, payload);
 }
}
