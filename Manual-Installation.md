# How to include the exception handler in your current project
The libraries target .NET 8 and can be referenced from .NET 8 and newer projects.

**1. Clone the repository**

```bash
git clone <RepositoryURL>
```

**2. Move the required files into your project**
Copy the directories `src/ExceptionHandling` and `src/ExceptionHandling.AspNetCore`, plus the file `Directory.Build.props`, into your project folder, at the same level as your solution file.

> **Already have a `Directory.Build.props`?** Don't overwrite it. Merge the properties from this one into yours instead, and make sure `TargetFramework` resolves to `net8.0` (or newer) for the two libraries. Without a target framework, the libraries won't build.

Your folder should look like this:

```
ProjectDirectory/
├── Directory.Build.props
├── YourSolution.sln
├── ExceptionHandling/
│   ├── Error/IError.cs
│   ├── Error/ErrorCore.cs
│   └── Reply/Reply.cs
├── ExceptionHandling.AspNetCore/
│   ├── ReplyExtensions.cs
│   ├── GlobalExceptionsHandler.cs
│   └── ServiceCollectionExtensions.cs
└── YourApi/
```

**3. Add both projects to the solution**
Open a terminal (PowerShell or similar) in the solution folder and run:

```bash
dotnet sln <YourSolution>.sln add ExceptionHandling/ExceptionHandling.csproj ExceptionHandling.AspNetCore/ExceptionHandling.AspNetCore.csproj
```

**4. Reference them from your API project**

```bash
dotnet add <YourApi>/<YourApi>.csproj reference ExceptionHandling/ExceptionHandling.csproj ExceptionHandling.AspNetCore/ExceptionHandling.AspNetCore.csproj
```

**5. Build**

```bash
dotnet build <YourSolution>.sln
```

**6. Wire it up**
In `Program.cs`, register the global exception handler and add it to the pipeline:

```csharp
using ExceptionHandling.AspNetCore;

builder.Services.AddReplyExceptionHandling();
// ...
app.UseExceptionHandler();
```

Then see the **Get started** section in the [README](README.md) for returning a `Reply` from your services and mapping it to HTTP responses.

You're done.
