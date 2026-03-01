---
apply: always
---

# Junie Assistant Rules for DemonsAndDogs

## Summary
This document defines **non-negotiable engineering rules** for the DemonsAndDogs solution, plus an explanation of the **JsonResource-based resource pattern** used in the database layer.

---

## Core Rules

### 1) Resources come only from JsonResource (no new resource models)
- **Do not create new resource model types** (no new `Resource<T>` specializations beyond what already exists).
- Resources must be represented using **`JsonResource`** and persisted in the **`JsonResources`** table via:
    - `DbSet<Resource<JsonResource>> JsonResources`
- If new resource shapes are needed, they must be expressed as **JSON inside `JsonResource`**, not new tables/models.

---

### 2) Pages must be parameterized components; data fetched via Mediator queries to the API
- Each UI “page” in **DemonsAndDogs** must be a **component** that accepts parameters for its data needs (ids, filters, paging, etc.).
- The component **must not** directly embed data access logic.
- Data must be retrieved by:
    - issuing **Mediator queries**
    - which call the **API** (not direct DB access from UI)

---

### 3) Prefer components as much as possible
- UI should be built from reusable components first.
- Avoid duplicating layout, form fields, grids, dialogs, and validation across multiple pages.
- If something appears more than once, it should become a component.

---

### 4) Controller method parameters must use records
- When passing parameters into controller actions, use a **single record** type rather than multiple primitive parameters.
- Applies to both query and body payloads where appropriate.
- This includes filters, paging, search criteria, and command payloads.

---

### 5) One type per file (services, repositories, contracts, handlers)
- Each **service**, **repository**, **contract**, and **handler** must live in its **own file**.
- No “grouped” files containing multiple handlers/contracts/types.

---

### 6) No magic strings; use AppConstants class library
- Do not introduce hard-coded strings for:
    - routes, policy names, claim types, cache keys
    - resource keys, table names (where applicable), header names
    - any cross-cutting identifiers used in more than one place
- Use the **AppConstants** class library for these values.

---

## Resource Pattern Explanation (JsonResource)

### What this pattern does
The resource pattern stores typed resources using a generic `Resource<T>` wrapper, where the **payload is stored as JSON** in PostgreSQL using `jsonb`. In this solution:
- `T` is constrained to `ResourceBase`
- the only configured resource table is **`JsonResources`**
- the JSON payload is stored in a `Data` column typed as **`jsonb`**

### Why it exists
This approach centralizes “resource” storage into a single table and supports schema-flexible payloads without creating new tables/models for each resource variation.

### How the DbContext config works
- `OnModelCreating` calls `ConfigureResource<JsonResource>(..., "JsonResources")`
- The generic configuration maps:
    - `Resource<T>` to the specified table
    - the `Data` property to PostgreSQL `jsonb`

### Reference implementation
```csharp
public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureResource<JsonResource>(modelBuilder, "JsonResources");
    }

    public DbSet<Resource<JsonResource>> JsonResources { get; set; }

    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName) where T : ResourceBase
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);
            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}