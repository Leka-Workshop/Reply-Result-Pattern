using ExceptionHandling.Error;

namespace ExceptionHandling.Reply
{
 public class Reply
 {
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public IError? Error { get; }

  protected Reply(bool isSuccess, IError? error)
  {
   IsSuccess = isSuccess;
   Error = error;
  }

  public static Reply Success() => new(true, null);

  public static Reply Failure(IError error)
  {
   ArgumentNullException.ThrowIfNull(error);

   return new(false, error);
  }

  /// <summary>
  /// Pass the Error from any Reply (generic or not) to a plain Reply
  /// </summary>
  public static Reply Failure(Reply reply)
  {
   ArgumentNullException.ThrowIfNull(reply);

   if (reply.Error is null)
   {
    throw new InvalidOperationException("Cannot create a failure from a successful reply!");
   }

   return new(false, reply.Error);
  }

  #region Error Helpers

  public static Reply ValidationError(string message, object? payload = null)
  {
   var error = ErrorCore.ValidationError(message, payload);
   return new(false, error);
  }

  public static Reply ForbiddenError(string message, object? payload = null)
  {
   var error = ErrorCore.ForbiddenError(message, payload);
   return new(false, error);
  }

  public static Reply NotFoundError(string message, object? payload = null)
  {
   var error = ErrorCore.ResourceNotFoundError(message, payload);
   return new(false, error);
  }

  public static Reply ConflictError(string message, object? payload = null)
  {
   var error = ErrorCore.ConflictError(message, payload);
   return new(false, error);
  }

  public static Reply FatalError(string message, object? payload = null)
  {
   var error = ErrorCore.FatalError(message, payload);
   return new(false, error);
  }

  public static Reply FatalError(Exception ex, object? payload = null)
  {
   var error = ErrorCore.FatalError(ex, payload);
   return new(false, error);
  }

  #endregion
 }

 public sealed class Reply<T> : Reply
 {
  public T? Value { get; }

  private Reply(T value) : base(true, null)
  {
   Value = value;
  }

  private Reply(IError error) : base(false, error)
  {
   Value = default;
  }

  public static Reply<T> Success(T value) => new(value);

  public static new Reply<T> Failure(IError error)
  {
   ArgumentNullException.ThrowIfNull(error);

   return new(error);
  }

  /// <summary>
  /// Transform the Error of any Reply (generic or not) into a new Reply&lt;T&gt;
  /// </summary>
  public static new Reply<T> Failure(Reply reply)
  {
   ArgumentNullException.ThrowIfNull(reply);

   if (reply.Error is null)
   {
    throw new InvalidOperationException("Cannot create a failure from a successful reply!");
   }

   return new(reply.Error);
  }

  #region Error Helpers

  public static new Reply<T> ValidationError(string message, object? payload = null)
  {
   var error = ErrorCore.ValidationError(message, payload);
   return new(error);
  }

  public static new Reply<T> ForbiddenError(string message, object? payload = null)
  {
   var error = ErrorCore.ForbiddenError(message, payload);
   return new(error);
  }

  public static new Reply<T> NotFoundError(string message, object? payload = null)
  {
   var error = ErrorCore.ResourceNotFoundError(message, payload);
   return new(error);
  }

  public static new Reply<T> ConflictError(string message, object? payload = null)
  {
   var error = ErrorCore.ConflictError(message, payload);
   return new(error);
  }

  public static new Reply<T> FatalError(string message, object? payload = null)
  {
   var error = ErrorCore.FatalError(message, payload);
   return new(error);
  }

  public static new Reply<T> FatalError(Exception ex, object? payload = null)
  {
   var error = ErrorCore.FatalError(ex, payload);
   return new(error);
  }

  #endregion

  #region Implicit Operators

  /// <summary>
  /// Return a value of type T as a Reply&lt;T&gt; without calling Reply&lt;T&gt;.Success(value)
  /// </summary>
  public static implicit operator Reply<T>(T value) => new(value);

  #endregion
 }
}
