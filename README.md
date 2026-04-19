# Vertical Slice Architecture Template

A modern .NET 10 REST API template implementing **Vertical Slice Architecture** with **CQRS pattern**, minimal APIs, FluentValidation, and Entity Framework Core with PostgreSQL.

## Tech Stack

- **.NET 10** - Latest .NET runtime
- **ASP.NET Core Minimal APIs** - Lightweight HTTP endpoints
- **Entity Framework Core 10.0.3** - ORM for database access
- **PostgreSQL** - Database provider (Npgsql)
- **FluentValidation 12.1.1** - Request validation
- **CQRS Pattern** - Command Query Responsibility Segregation
- **Vertical Slice Architecture** - Feature-driven organization
- **Pipeline Decorators** - Logging and validation decorators
- **Scalar UI** - OpenAPI documentation viewer

## Project Structure

```
VerticalSliceArchitectureTemplate/
├── Abstractions/                      # Core interfaces and abstractions
│   ├── IApiEndpoint.cs               # Endpoint contract
│   ├── IDomainEvent.cs               # Marker interface for domain events
│   ├── IEventDispatcher.cs           # Dispatcher contract for domain events
│   ├── IEventHandler.cs              # Typed domain event handler contract
│   ├── IHandler.cs                   # Request handler contract
│   ├── Result.cs                     # Success/Failure result wrapper
│   └── Errors/
│       ├── Error.cs                  # Error type definition
│       └── ValidationError.cs        # Validation error aggregate
│
├── Constants/                        # Shared constants
│   └── ApiTags.cs                   # OpenAPI tags
│
├── Database/
│   └── ApplicationDbContext.cs       # EF Core DbContext
│
├── Exceptions/
│   └── CustomExceptionHandler.cs     # Global exception handler
│
├── Extensions/                       # DI & configuration extensions
│   ├── DatabaseExtensions.cs
│   ├── EventDispatcher.cs
│   ├── HandlerRegistrationExtensions.cs
│   ├── HealthChecksExtensions.cs
│   ├── MapEndpointExtensions.cs
│   └── ResultExtensions.cs
│
├── Features/                         # Vertical slices (features)
│   └── Book/
│       ├── Book.cs                  # Domain entity co-located with feature
│       ├── BookErrors.cs            # Feature-specific errors
│       ├── CreateBook/
│       │   ├── BookCreatedEvent.cs   # Domain event raised after a book is created
│       │   ├── BookCreatedEventHandler.cs # Handles post-create behavior
│       │   ├── CreateBookHandler.cs
│       │   ├── CreateBookValidator.cs
│       │   └── CreateBookEndpoint.cs
│       ├── GetAllBooks/
│       │   ├── GetAllBooksHandler.cs
│       │   └── GetAllBooksEndpoint.cs
│       ├── GetBookById/
│       │   ├── GetBookByIdHandler.cs
│       │   ├── GetBookByIdValidator.cs
│       │   └── GetBookByIdEndpoint.cs
│       ├── UpdateBook/
│       │   ├── UpdateBookHandler.cs
│       │   ├── UpdateBookValidator.cs
│       │   └── UpdateBookEndpoint.cs
│       └── DeleteBook/
│           ├── DeleteBookHandler.cs
│           ├── DeleteBookValidator.cs
│           └── DeleteBookEndpoint.cs
│
├── Pipelines/                        # Request processing decorators
│   ├── ValidationDecorator.cs       # FluentValidation pipeline
│   └── LoggingDecorator.cs          # Logging pipeline
│
├── appsettings.json                 # Configuration
├── appsettings.Development.json
├── Program.cs                        # App startup & DI setup
└── VerticalSliceArchitectureTemplate.csproj
```

## Key Concepts

### 1. Vertical Slice Architecture
Each feature is self-contained with its own request/response DTOs, handler, validator, and endpoint:
- **Independent** - Changes to one feature don't affect others
- **Scalable** - Easy to add new features
- **Testable** - Each slice can be tested in isolation

### 2. CQRS Pattern
- **Commands** - Operations that modify state (Create, Update, Delete)
- **Queries** - Operations that read data (GetAll, GetById)
- **Handlers** - `IHandler<TRequest, TResponse>` processes each request

### 3. Domain Events
Domain events let a feature publish a fact that happened inside the domain without coupling the command handler to the follow-up work.
- **`IDomainEvent`** marks an event type as a domain event.
- **`IEventHandler<TEvent>`** handles one specific event type.
- **`IEventDispatcher`** resolves all handlers for an event and invokes them.
- **`DispatchAsync`** is the entry point used by handlers to publish events after the main write completes.

In the Book feature, `CreateBookHandler` saves the book, then dispatches `BookCreatedEvent`. `BookCreatedEventHandler` handles that event and logs the creation.

### 4. Request/Response as Records
All DTOs use C# records for immutability and cleaner syntax:
```csharp
public sealed record CreateBookRequest(string Title, string Author, string ISBN, decimal Price, int PublishedYear);
public sealed record CreateBookResponse(Guid Id, string Title, string Author, string ISBN, decimal Price, int PublishedYear);
```

### 5. Validation Pipeline
FluentValidation decorators automatically validate requests before handlers:
```csharp
RuleFor(c => c.Title)
    .NotEmpty().WithMessage("Title is required")
    .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
```

### 6. Pipeline Logging
Decorators log request/response and execution time:
```csharp
LoggingDecorator     → Logs request, response, duration
ValidationDecorator  → Validates request
Handler              → Processes actual business logic
```

### 7. Error Handling
Centralized error handling with `Result<T>` wrapper:
```csharp
// Success
return Result.Success(new CreateBookResponse(...));

// Failure
return Result.Failure<CreateBookResponse>(BookErrors.NotFound(id));
```

## NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 10.0.3 | ORM & data access |
| Microsoft.EntityFrameworkCore.Design | 10.0.3 | EF Core tools support |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.3 | SQLite provider |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.3 | SQL Server provider |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0 | PostgreSQL provider |
| Microsoft.EntityFrameworkCore.Tools | 10.0.3 | Migrations & scaffolding |
| FluentValidation | 12.1.1 | Request validation |
| FluentValidation.DependencyInjectionExtensions | 12.1.1 | DI integration |
| Scalar.AspNetCore | 2.12.40 | OpenAPI UI |
| Scrutor | 7.0.0 | Assembly scanning for DI |
| AspNetCore.HealthChecks.UI.Client | 9.0.0 | Health checks |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | 10.0.3 | DB health checks |

## Database Setup

### Using PostgreSQL (Current)

**Connection String** (appsettings.json):
```json
{
  "ConnectionStrings": {
    "connection": "Host=localhost;Port=5432;Database=VerticalSlice;Username=postgres;Password=your_password"
  }
}
```

### Entity Framework migrations

**Package Manager Console (Visual Studio):**
```powershell
Add-Migration InitialCreate -Project VerticalSliceArchitectureTemplate
Update-Database -Project VerticalSliceArchitectureTemplate
```

**1. Create Initial Migration:**
```bash
dotnet ef migrations add InitialCreate -p VerticalSliceArchitectureTemplate
```

**2. Apply Migration to Database:**
```bash
dotnet ef database update -p VerticalSliceArchitectureTemplate
```

**3. Add New Migration (after model changes):**
```bash
dotnet ef migrations add MigrationName -p VerticalSliceArchitectureTemplate
dotnet ef database update -p VerticalSliceArchitectureTemplate
```

**4. Revert Last Migration:**
```bash
dotnet ef database update LastGoodMigration -p VerticalSliceArchitectureTemplate
dotnet ef migrations remove -p VerticalSliceArchitectureTemplate
```

### Switching Database Providers

**For SQL Server:**
```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"));
});
```

**For SQLite (Development):**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("connection"));
});
```

## Running the Application

**1. Install Dependencies:**
```bash
dotnet restore
```

**2. Build Project:**
```bash
dotnet build
```

**3. Run Application:**
```bash
dotnet run
```

## Scalar API Testing UI

Scalar is a modern, beautiful alternative to Swagger UI for testing and exploring APIs.

### Accessing Scalar

- **URL**: `https://localhost:5001/scalar/v1`
- **OpenAPI Spec**: `https://localhost:5001/openapi/v1.json`

### Features

✨ **Scalar provides:**
- 🚀 Beautiful, modern UI for API documentation
- 📝 Interactive request/response testing
- 🔄 Request history
- 📦 Request/response examples with syntax highlighting
- 🏷️ Endpoint organization by tags (our `ApiTags.Books`)
- 📱 Mobile-responsive design
- 🔐 Support for authentication headers
- 💾 Request templates and presets

### Using Scalar to Test Book API

1. **Open Scalar**: Navigate to `https://localhost:5001/scalar/v1`
2. **Select Endpoint**: Click any book endpoint under the "books" tag
3. **Enter Request Data**: Fill in parameters and request body
4. **Execute Request**: Click "Send" button
5. **View Response**: See status code, headers, and response body

### Example: Testing Create Book Endpoint

**In Scalar UI:**
```
Endpoint: POST /books
Request Body:
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "9780132350884",
  "price": 39.99,
  "publishedYear": 2008
}

Response 200:
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "9780132350884",
  "price": 39.99,
  "publishedYear": 2008
}
```

### Book Create Flow

The create-book slice publishes a domain event after persistence:

1. `CreateBookHandler` creates the `Book` entity and saves it with EF Core.
2. The handler calls `IEventDispatcher.DispatchAsync(new BookCreatedEvent(...))`.
3. `EventDispatcher` resolves every `IEventHandler<BookCreatedEvent>` from DI.
4. `BookCreatedEventHandler` runs and logs the new book.

This keeps the command focused on the write model while still allowing feature-specific follow-up work.

---

## Health Checks

Health checks monitor the application and dependent services (database, external APIs, etc.).

### Built-in Health Checks

```csharp
// Program.cs
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();
```

### Health Check Endpoint

**URL**: `https://localhost:5001/health`

**Response (Healthy):**
```http
GET /health

HTTP/1.1 200 OK
Content-Type: application/json

{
  "status": "Healthy",
  "timestamp": "2026-02-23T10:30:00Z",
  "checks": {
    "ApplicationDbContext": {
      "status": "Healthy",
      "description": "Database connection successful"
    }
  }
}
```

**Response (Unhealthy):**
```http
HTTP/1.1 503 Service Unavailable

{
  "status": "Unhealthy",
  "timestamp": "2026-02-23T10:31:00Z",
  "checks": {
    "ApplicationDbContext": {
      "status": "Unhealthy",
      "description": "Database connection failed"
    }
  }
}
```

### Health Check Configuration

**Map Health Check Endpoint:**

```csharp
// In Program.cs after building app
var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse
});

// Custom response formatter
static async Task WriteHealthCheckResponse(
    HttpContext httpContext,
    HealthReport report)
{
    httpContext.Response.ContentType = "application/json";
    
    var response = new
    {
        status = report.Status.ToString(),
        timestamp = DateTime.UtcNow,
        checks = report.Entries.ToDictionary(
            e => e.Key,
            e => new
            {
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
    };

    await httpContext.Response.WriteAsJsonAsync(response);
}
```

### Health Check Status Codes

| Status | HTTP Code | Meaning |
|--------|-----------|---------|
| `Healthy` | 200 | All checks passed |
| `Degraded` | 200 | Some checks passed, others degraded |
| `Unhealthy` | 503 | One or more checks failed |

### Best Practices for Health Checks

✅ **Do:**
- Check critical dependencies (database, cache, external APIs)
- Set reasonable timeouts for checks
- Include descriptive status messages
- Log health check failures
- Use health checks in load balancers
- Implement liveness & readiness probes

❌ **Don't:**
- Make health checks too complex
- Hit external APIs on every health check
- Override built-in status codes
- Ignore degraded status
- Leave health checks unconfigured

---



### Endpoints

All book endpoints are tagged with `"books"` in OpenAPI documentation.

#### 1. Create Book
```http
POST /books
Content-Type: application/json

{
  "title": "The Art of Programming",
  "author": "John Doe",
  "isbn": "9781234567890",
  "price": 49.99,
  "publishedYear": 2023
}

Response 200:
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "The Art of Programming",
  "author": "John Doe",
  "isbn": "9781234567890",
  "price": 49.99,
  "publishedYear": 2023
}
```

#### 2. Get All Books
```http
GET /books

Response 200:
{
  "books": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "The Art of Programming",
      "author": "John Doe",
      "isbn": "9781234567890",
      "price": 49.99,
      "publishedYear": 2023
    }
  ]
}
```

#### 3. Get Book by ID
```http
GET /books/550e8400-e29b-41d4-a716-446655440000

Response 200:
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "The Art of Programming",
  "author": "John Doe",
  "isbn": "9781234567890",
  "price": 49.99,
  "publishedYear": 2023
}

Response 404:
{
  "code": "Books.NotFound",
  "description": "The Book with Id '550e8400-e29b-41d4-a716-446655440000' was not found"
}
```

#### 4. Update Book (Partial Updates Supported)
```http
PUT /books/550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "title": "Advanced Programming",
  "author": null,
  "isbn": null,
  "price": 59.99,
  "publishedYear": null
}

Response 200:
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Advanced Programming",
  "author": "John Doe",
  "isbn": "9781234567890",
  "price": 59.99,
  "publishedYear": 2023
}
```

#### 5. Delete Book
```http
DELETE /books/550e8400-e29b-41d4-a716-446655440000

Response 200:
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}

Response 404:
{
  "code": "Books.NotFound",
  "description": "The Book with Id '550e8400-e29b-41d4-a716-446655440000' was not found"
}
```

### Validation Example

**Invalid Create Request:**
```http
POST /books
{
  "title": "",
  "author": "John Doe",
  "isbn": "invalid",
  "price": -10,
  "publishedYear": 2050
}

Response 400:
{
  "errors": [
    {
      "code": "NotEmptyValidator",
      "description": "Title is required",
      "type": 2
    },
    {
      "code": "NotEmptyValidator",
      "description": "Author is required",
      "type": 2
    },
    {
      "code": "NotEmptyValidator",
      "description": "ISBN is required",
      "type": 2
    },
    {
      "code": "GreaterThanValidator",
      "description": "Published year must be a valid year",
      "type": 2
    }
  ],
  "code": "Validation.General",
  "description": "One or more validation errors occurred",
  "type": 2
}
```

## Global Level Exception Handling

Global exception handling ensures all unhandled exceptions are caught and returned as consistent API responses.

### Built-in Exception Handler Middleware

ASP.NET Core 10 provides `IExceptionHandler` for global exception handling:

```csharp
// Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler();
```

### Custom Global Exception Handler

Global exception handling can be implemented using `IExceptionHandler` to catch and format all unhandled exceptions:

```csharp
// Register in Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var app = builder.Build();
app.UseExceptionHandler();
```

### Exception Types & Status Codes

| Exception Type | Status Code | Example |
|---|---|---|
| `ValidationException` | 400 Bad Request | FluentValidation errors |
| `KeyNotFoundException` | 404 Not Found | Resource not found |
| `ArgumentException` | 400 Bad Request | Invalid input argument |
| `UnauthorizedAccessException` | 401 Unauthorized | Authentication failed |
| `InvalidOperationException` | 409 Conflict | Operation cannot be performed |
| Default Exception | 500 Internal Server Error | Unhandled errors |

### Registering Global Exception Handler

Global exception handlers are configured in `Program.cs` using `AddExceptionHandler<>()` and `UseExceptionHandler()` middleware.

### Example: Uncaught Exception Response

**Database Connection Error:**
```http
POST /books
{
  "title": "Test",
  "author": "Author",
  "isbn": "1234567890",
  "price": 19.99,
  "publishedYear": 2023
}

Response 500:
{
  "type": "InvalidOperationException",
  "title": "An error occurred",
  "status": 500,
  "detail": "An exception has been raised that is likely due to a transient failure.",
  "instance": "/books"
}
```

**Validation Error (caught by FluentValidation):**
```http
Response 400:
{
  "errors": [
    {
      "code": "NotEmptyValidator",
      "description": "Title is required",
      "type": 2
    },
    {
      "code": "GreaterThanValidator",
      "description": "Published year must be a valid year",
      "type": 2
    }
  ],
  "code": "Validation.General",
  "description": "One or more validation errors occurred",
  "type": 2
}
```

### Logging Exceptions

Exceptions should be logged with context (path, timestamp, exception type) for debugging and monitoring purposes.

### Environment-Specific Error Details

Stack traces and detailed error information should only be exposed in development environments to prevent information leakage in production.

### Best Practices for Global Exception Handling

✅ **Do:**
- Log all exceptions with context (path, user, timestamp)
- Return consistent error response format
- Include error codes for client handling
- Hide sensitive information in production
- Handle specific exceptions first, generic last
- Use proper HTTP status codes
- Return failed operations as JSON

❌ **Don't:**
- Expose stack traces to clients (except dev)
- Log sensitive data (passwords, tokens)
- Return 500 for business logic failures
- Suppress logging of exceptions
- Ignore validation errors
- Return HTML error pages from APIs

## Architecture Flow

### Request Processing Pipeline with Exception Handling

```
HTTP Request
    ↓
MinimalAPI Endpoint (Route binding)
    ↓
LoggingDecorator (Log request start)
    ↓
ValidationDecorator (FluentValidation)
    ↓
Handler (Business logic)
    ├─ IRepository (Data access)
    ├─ IUnitOfWork (Transaction)
    └─ Returns Result<T>
    ↓
LoggingDecorator (Log response, duration)
    ↓
Endpoint Result Mapping (Map to HTTP response)
    ↓
[Exception Caught?]
    ├─ Yes → GlobalExceptionHandler
    │        └─ Format & Log Error
    │           └─ Return Error Response
    └─ No → HTTP Response (200/400/404)
```

## Adding a New Feature

1. **Create Feature Folder** under `Features/Book/`:
   ```
   Features/Book/NewFeature/
   ├── NewFeatureRequest.cs (record)
   ├── NewFeatureResponse.cs (record)
   ├── NewFeatureHandler.cs
   ├── NewFeatureValidator.cs
   └── NewFeatureEndpoint.cs
   ```

2. **Implement Handler** (inherit `IHandler<TRequest, TResponse>`):
   ```csharp
   public sealed class NewFeatureHandler(
       IRepository<Book> _repo) : IHandler<NewFeatureRequest, Result<NewFeatureResponse>>
   {
       public async Task<Result<NewFeatureResponse>> HandleAsync(...) { }
   }
   ```

3. **Register Validator** (auto-discovered from assembly)

4. **Create Endpoint** (inherit `IApiEndpoint`):
   ```csharp
   public void MapEndpoint(WebApplication app)
   {
       app.MapPost("path", async (...) => { ... })
           .WithTags(ApiTags.Books)
           .Produces<NewFeatureResponse>(StatusCodes.Status200OK);
   }
   ```

5. **Optional: Publish a Domain Event** when the feature needs post-write behavior:
   ```csharp
   public sealed record NewFeatureCreatedEvent(Guid Id) : IDomainEvent;

   public sealed class NewFeatureCreatedEventHandler : IEventHandler<NewFeatureCreatedEvent>
   {
     public Task HandleAsync(NewFeatureCreatedEvent @event, CancellationToken cancellationToken)
     {
       return Task.CompletedTask;
     }
   }
   ```

## Best Practices

✅ **Do:**
- Keep each feature independent
- Use records for immutable DTOs
- Validate at the boundary (validators)
- Use dependency injection for all services
- Log important operations
- Handle errors with Result<T>
- Write handlers that do one thing well

❌ **Don't:**
- Put business logic in endpoints
- Modify requests/responses mid-pipeline
- Mix validators (create separate per feature)
- Expose entities in API responses
- Skip validation
- Ignore error codes from handlers

## Debugging

**Enable Debug Logging:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

**View Database Queries:**
```csharp
// In Program.cs
options.LogTo(Console.WriteLine, LogLevel.Information);
```

## License

MIT License - Use freely and commercially.

---

**Built with .NET 10 | Vertical Slice Architecture | CQRS Pattern**
