# DemonsAndDogs — Generic Resource Framework

A generic, convention-based resource framework built on ASP.NET Core 8, Entity Framework Core, and Blazor WebAssembly. Define a model, register it in one line, and get a full set of CRUD API endpoints **and** UI components automatically.

---

## NuGet Packages

The framework is distributed as a set of NuGet packages that you can install independently:

| Package | Description | Install In |
|---|---|---|
| **ResourceFramework.Models** | Shared models: `ResourceBase`, `Resource<T>` | All projects that define or use resources |
| **ResourceFramework.DataAccess** | EF Core context, `ResourceRepository<T>`, `ResourceRegistry` | Server / API projects |
| **ResourceFramework.Services** | `IResourceService<T>` / `ResourceService<T>` | Server / API projects |
| **ResourceFramework.Server** | All-in-one server package — includes Models, DataAccess, Services, plus generic controllers and one-line DI setup | ASP.NET Core Web API projects |
| **ResourceFramework.UI** | Blazor WASM components — Radzen DataGrid with CRUD, MediatR handlers, `ResourceUIRegistry` | Blazor WebAssembly projects |

### Package Dependency Graph

```
ResourceFramework.Models          (standalone, no dependencies)
       │
       ├── ResourceFramework.DataAccess   (+ EF Core, Identity)
       │          │
       │          └── ResourceFramework.Services
       │                     │
       │                     └── ResourceFramework.Server  (+ ASP.NET Core MVC)
       │
       └── ResourceFramework.UI           (+ Radzen.Blazor, MediatR)
```

---

## Quick Start — Server (API)

### 1. Install the package

```bash
dotnet add package ResourceFramework.Server
```

Or reference the project directly:

```xml
<ProjectReference Include="..\ResourceFramework.Server\ResourceFramework.Server.csproj" />
```

### 2. Define a resource model

```csharp
using Models.Resources;

namespace MyApp;

public class TodoItem : ResourceBase
{
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}
```

### 3. Register it in `Program.cs` (one line!)

```csharp
using ResourceFramework.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// One call registers: repository, service, EF table mapping, and API controller
builder.Services.AddResourceFramework(registry =>
{
    registry.AddResource<TodoItem>("TodoItems");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure your DbContext as usual (PostgreSQL example)
builder.Services.AddDbContext<DataAccess.DbContext>(options =>
    options.UseNpgsql(dataSource));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
```

**That's it!** You now have these endpoints:

| Method | Route | Description |
|---|---|---|
| `GET` | `api/TodoItem/{id}` | Get by ID |
| `GET` | `api/TodoItem/filter?key1=...&key2=...&key3=...` | Filtered list |
| `GET` | `api/TodoItem` | Get all (non-deleted) |
| `POST` | `api/TodoItem` | Upsert (create or update) |
| `DELETE` | `api/TodoItem/{id}` | Soft-delete |

---

## Quick Start — Client (Blazor WASM)

### 1. Install the package

```bash
dotnet add package ResourceFramework.UI
```

### 2. Register resources in `Program.cs`

```csharp
using UI.Component.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddResourceUI(registry =>
{
    registry.AddResource<TodoItem>();
});

await builder.Build().RunAsync();
```

### 3. Add imports to `_Imports.razor`

```razor
@using Radzen
@using Radzen.Blazor
@using UI.Component.Components
@using UI.Component.Services
```

### 4. Add Radzen to `index.html`

```html
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material-base.css">
<script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
```

### 5. Use the components

**Auto-generated navigation links:**
```razor
<!-- In NavMenu.razor -->
<ResourceNavLinks />
```

**Resource page (dynamic, route-based):**
```razor
@page "/resources/{ResourceName}"
@inject ResourceUIRegistry Registry

@if (_resourceType != null)
{
    <RadzenNotification />
    @_gridFragment
}

@code {
    [Parameter] public string ResourceName { get; set; } = string.Empty;
    private Type? _resourceType;
    private RenderFragment? _gridFragment;

    protected override void OnParametersSet()
    {
        _resourceType = Registry.GetResourceType(ResourceName);
        if (_resourceType != null)
        {
            var gridType = typeof(ResourceGrid<>).MakeGenericType(_resourceType);
            _gridFragment = builder =>
            {
                builder.OpenComponent(0, gridType);
                builder.AddAttribute(1, "Title", ResourceName);
                builder.CloseComponent();
            };
        }
    }
}
```

**Or use the grid directly for a specific type:**
```razor
<ResourceGrid TResource="TodoItem" Title="My Todos" />
```

---

## Architecture Overview

| Project / Package | Responsibility |
|---|---|
| **Models** (`ResourceFramework.Models`) | Resource data models — inherit from `ResourceBase` |
| **DataAccess** (`ResourceFramework.DataAccess`) | EF Core context, generic `ResourceRepository<T>`, and `ResourceRegistry` |
| **API.Services** (`ResourceFramework.Services`) | Generic `IResourceService<T>` / `ResourceService<T>` |
| **ResourceFramework.Server** | All-in-one: generic `ResourceController<T>`, feature provider, and `AddResourceFramework()` |
| **UI.Component** (`ResourceFramework.UI`) | Blazor WASM: Radzen grid, edit dialog, nav links, MediatR handlers |
| **AppConstants** | Shared constants (table names, config keys) |

### How It Fits Together

```
Model ──► ResourceRegistry ──► EF ModelBuilder (table + jsonb column)
                            ──► DI Container   (repository + service)
                            ──► MVC FeatureProvider (controller endpoints)

UI:  ResourceUIRegistry ──► MediatR Handlers ──► HttpClient ──► API endpoints
                        ──► ResourceGrid (Radzen DataGrid with CRUD)
                        ──► ResourceNavLinks (auto sidebar)
```

---

## ResourceBase — The Foundation

Every resource inherits from `ResourceBase`, which provides:

```csharp
public class ResourceBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? OwnerId { get; set; }   // optional owner
    public string? Key1 { get; set; }       // generic lookup key
    public string? Key2 { get; set; }       // generic lookup key
    public string? Key3 { get; set; }       // generic lookup key
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }     // soft-delete flag
}
```

Data is stored as a **JSON column** (`jsonb` in PostgreSQL) inside `Resource<T>`:

```csharp
public class Resource<T> : ResourceBase where T : ResourceBase
{
    [Column(TypeName = "jsonb")]
    public T Data { get; set; }
}
```

**Key1 / Key2 / Key3** are generic, optional filter keys. Use them however your domain requires (e.g., Key1 = TenantId, Key2 = CategoryId). **OwnerId** is also optional.

---

## Upsert Request Body

```json
{
  "data": { /* your resource fields */ },
  "id": null,
  "key1": "optional",
  "key2": "optional",
  "key3": "optional",
  "ownerId": "optional"
}
```

- **Create**: omit `id` (or set to `null`).
- **Update**: provide the existing `id`.

---

## Customizing a Resource Service

The default `ResourceService<T>` works for most cases. For custom business logic:

```csharp
public interface ITodoService : IResourceService<TodoItem>
{
    Task<TodoItem> MarkComplete(string id);
}

public class TodoService : ResourceService<TodoItem>, ITodoService
{
    public TodoService(
        IResourceRepository<TodoItem> repo,
        ILogger<ResourceService<TodoItem>> logger) : base(repo, logger) { }

    public async Task<TodoItem> MarkComplete(string id)
    {
        var item = await GetById(id);
        item.IsComplete = true;
        return await Update(id, item);
    }
}
```

Register with the three-type-parameter overload:

```csharp
registry.AddResource<TodoItem, ITodoService, TodoService>("TodoItems");
```

---

## Building NuGet Packages

All packageable projects have `GeneratePackageOnBuild` enabled. Simply build:

```bash
dotnet build
```

Packages are generated in each project's `bin/Debug/` (or `bin/Release/`) folder:

```
Models/bin/Debug/ResourceFramework.Models.1.0.0.nupkg
DataAccess/bin/Debug/ResourceFramework.DataAccess.1.0.0.nupkg
API.Services/bin/Debug/ResourceFramework.Services.1.0.0.nupkg
ResourceFramework.Server/bin/Debug/ResourceFramework.Server.1.0.0.nupkg
UI.Component/bin/Debug/ResourceFramework.UI.1.0.0.nupkg
```

To create Release packages:

```bash
dotnet pack -c Release
```

To publish to a NuGet feed:

```bash
dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY
```

Or push to a local feed for development:

```bash
# Create a local feed directory
mkdir C:\LocalNuGet

# Push all packages
Get-ChildItem -Recurse -Filter *.nupkg | ForEach-Object {
    dotnet nuget push $_.FullName --source C:\LocalNuGet
}

# Add the local feed to your NuGet sources
dotnet nuget add source C:\LocalNuGet --name LocalDev
```

---

## Project Setup (Development)

### Prerequisites

- .NET 8 SDK
- PostgreSQL (with `jsonb` support)

### Configuration

Set your connection string in `API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=demonsanddogs;Username=...;Password=..."
  }
}
```

### Running

```bash
dotnet run --project API
```

Swagger UI is available in Development mode at `/swagger`.

### EF Migrations

```bash
dotnet ef migrations add AddMyResource --project DataAccess --startup-project API
dotnet ef database update --project DataAccess --startup-project API
```

---

## Key Components Reference

### ResourceRegistry (`DataAccess/ResourceRegistry.cs`)

Central registration hub. `AddResource<T>(tableName)`:
- Configures EF `ModelBuilder` to map `Resource<T>` to the given table with `jsonb` Data column.
- Registers `IResourceRepository<T>` → `ResourceRepository<T>` in DI.
- Tracks the type for MVC feature provider.

### AddResourceFramework (`ResourceFramework.Server/Extensions/`)

Single extension method that wires everything together:
- Registers `IResourceService<T>` → `ResourceService<T>` (open generic).
- Calls `AddResources` (registry + repositories).
- Adds MVC controllers with naming convention.
- Adds feature provider for auto-generated controllers.

### ResourceUIRegistry (`UI.Component/Services/`)

Client-side registry mapping resource types to route names and display names. Used by `ResourceGrid`, `ResourceNavLinks`, and MediatR handlers to construct API URLs.

### AddResourceUI (`UI.Component/Extensions/`)

Registers `ResourceUIRegistry`, MediatR, `NotificationService`, and all open-generic MediatR handlers in one call.
