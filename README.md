# Request Logging Middleware

A lightweight ASP.NET Core middleware to log incoming HTTP request data (including JSON body). Easily integrable into your application and distributable via NuGet.

## 🚀 Features

-   Logs request method, path, headers, and body
-   Store data content in MongoDb instance Or in other storage (use: IRequestStorage)  
-   Non-blocking (async) logging
-   Filtering on the Endpoint level
-   Modifying the request object data

## 📦 Installation

Install the package via NuGet:

```bash
dotnet add package R.Tools.Requests
```
Or using the NuGet Package Manager:
```bash
Install-Package R.Tools.Requests
```

## 🛠️ Configuration

If you want to use mongodb storage, add the following to your `appsettings.json`:

```json
{
  "StorageOptions": {
    "ConnectionString": "mongodb://localhost:27017/mydatabase",
  }
}

```

In your `Startup.cs` or `Program.cs`, register the middleware:

```csharp
using R.Tools.Requests;
...
builder.RegisterRequestsCollector(o => {
    o.DefineUserId = c => c.FindFirst("Id")?.Value;
    o.ShouldCollect = p => !p.StartsWithSegments("/api/ping");
});
...
var app = builder.Build();
app.CollectRequests();
...
app.Run();

```


## 🧩 Features


### ✂️ Body Content Editing

You can customize how request bodies are logged by omitting, transforming, or masking parts of the data — useful for removing sensitive information or reducing log noise.

To achieve this, implement a generic abstract class `ContentFormatter<T>` for the body type you want to format. Then, apply your custom formatter as a method attribute on controller actions.

### 🚫 Request Filtering

Not all requests need to be logged — static files, health checks, or specific endpoints can be excluded to keep your logs clean and relevant.

You can filter out unneeded requests in **two ways**:

----------

#### 1. Global Filtering via Configuration

Use the `ShouldCollect` predicate in your configuration to globally exclude requests based on their path or other logic. This is a lambda function that takes a `PathString` and returns a `bool`.

##### Example
```csharp
services.Configure<RequestLoggingOptions>(options =>
{
    options.ShouldCollect = path =>
        !path.StartsWithSegments("/health") &&
        !path.StartsWithSegments("/static");
});
```
#### 2. Per-Action Filtering with Attribute

Use the `[SkipRequestSaving]` attribute to skip logging on specific controller actions or endpoints.

##### Example
```csharp
[HttpPost]
[SkipRequestSaving]
public IActionResult Ping([FromBody] PingRequest request)
{
    return Ok("Ignored from logging");
}

```

This gives you fine-grained control to exclude endpoints such as:
-   Health checks
-   Performance probes
-   Internal background jobs

## 📌 Notes

-   Body parsing only works for buffered JSON body types
-   Ensure middleware is added **before** other middleware that may consume the request body.
    

## 📃 License

MIT License

## 🔗 Links

-   [NuGet Package](https://www.nuget.org/packages/R.Tools.Requests)
    
-   [GitHub Repository](https://github.com/aspirinv/R.Tools.Requests)
    
