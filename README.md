# Reply
A small Result pattern for .NET: `Reply` / `Reply<T>` put success and failure into the method signature, with consistent errors and an optional ASP.NET Core integration. MIT-licensed, free to copy. See [LICENSE.md](LICENSE.md).

## Repository layout

```
Reply/
├── Directory.Build.props              # shared settings (net8.0, nullable, version)
├── Reply.sln
└── src/
    ├── ExceptionHandling/             # core library, zero dependencies
    │   ├── Error/IError.cs
    │   ├── Error/ErrorCore.cs         # internal, created only via factory methods
    │   └── Reply/Reply.cs             # Reply and Reply<T>
    └── ExceptionHandling.AspNetCore/  # references the core library
        ├── ReplyExtensions.cs         # ToActionResult / ToIActionResult -> Problem Details
        ├── GlobalExceptionsHandler.cs # IExceptionHandler for unhandled exceptions
        └── ServiceCollectionExtensions.cs
```

## Get started

**1. Reference the projects** (copy the folders, or add them to your solution):

```bash
dotnet add <YourApi> reference src/ExceptionHandling/ExceptionHandling.csproj
dotnet add <YourApi> reference src/ExceptionHandling.AspNetCore/ExceptionHandling.AspNetCore.csproj
```

Non-web projects only need the core `ExceptionHandling` library.

**2. Return a `Reply` from your services**

```csharp
using ExceptionHandling.Reply;

public async Task<Reply<Blog>> GetBlogByID(int id)
{
    var blog = await _dbContext.Blog.FirstOrDefaultAsync(x => x.Id == id);

    if (blog is null)
        return Reply<Blog>.NotFoundError($"Blog with id '{id}' was not found.");

    return blog; // implicit Reply<Blog>.Success(blog)
}
```

Available helpers:
* `ValidationError` (400),
* `ForbiddenError` (403),
* `NotFoundError` (404),
* `ConflictError` (409),
* `FatalError` (500)

Each takes an optional `payload`. Use `Failure(reply)` to pass an error from one reply to another, even across different `Reply<T>` types.

**3. Map it to an HTTP response**

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Blog>> GetBlog(int id)
{
    var reply = await _blogService.GetBlogByID(id);

    if (reply.IsSuccess)
    {
        return reply.Value;
    }

    return StatusCode(reply.Error.StatusCode, reply.Error);
}
```
Success returns `200 OK` with the value.
Failure returns the shape of the error you've defined (`IError`).

*Problem Details*

```csharp
using ExceptionHandling.AspNetCore;

[HttpGet("{id}")]
public async Task<ActionResult<Blog>> GetBlog(int id)
{
    var reply = await _blogService.GetBlogByID(id);
    return reply.ToActionResult(this);
}
```

Success returns `200 OK` with the value.
Failure returns `application/problem+json` (RFC 9457) with the error's status code and `timestamp` extension.

**4. Catch unhandled exceptions (optional)**

```csharp
// Program.cs
builder.Services.AddReplyExceptionHandling();
// ...
app.UseExceptionHandler();
```

Anything that escapes the pattern (database outage, null reference) is logged and returned as a generic 500 Problem Details response.

## Build

```bash
dotnet build Reply.sln
```

## Docs
The full walkthrough, with the complete list of examples, is in the blog post: [Result Pattern in ASP.NET Core: Build Your Own API Error Handling](https://medium.com/@mirzaleka/result-pattern-in-asp-net-core-build-your-own-api-error-handling-1a1e307bb813).

It covers:
- `Reply` and `Reply<T>`, and the error factory helpers
- Passing errors between replies with `Failure()`
- Mapping replies to HTTP responses and Problem Details
- Handling unexpected exceptions with a global handler

To add the library to an existing solution, see [Manual-Installation.md](Manual-Installation.md).


## Notes

- `FatalError(Exception)` puts `ex.Message` into the response. Avoid it where messages could leak internals.
- `ErrorCore` is `internal`: callers can only create errors through the factory helpers.
