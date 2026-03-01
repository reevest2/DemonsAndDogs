using System.Text.Json;
using AppConstants;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Models.Resources.Abstract;
using DbContext = DataAccess.DbContext;

namespace DatabaseSeedTool.Services;

public class DatabaseSeedService
{
    private readonly IDbContextFactory<DbContext> _contextFactory;

    public DatabaseSeedService(IDbContextFactory<DbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string> SeedDatabaseAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existingCount = await context.JsonResources.CountAsync();
        if (existingCount > 0)
            return $"Database already contains {existingCount} resource(s). Use Reload to replace existing data.";

        var schemas = CreateSchemaResources();
        foreach (var schema in schemas)
            context.JsonResources.Add(schema);

        await context.SaveChangesAsync();
        return $"Seeded {schemas.Count} schema resource(s) into the database.";
    }

    public async Task<string> SaveStateAsync(string? filePath = null)
    {
        filePath ??= GetDefaultSeedFile();

        await using var context = await _contextFactory.CreateDbContextAsync();

        var resources = await context.JsonResources.AsNoTracking().ToListAsync();

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(resources, options);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, json);
        return $"Saved {resources.Count} resource(s) to {filePath}";
    }

    public async Task<string> ReloadStateAsync(string? filePath = null)
    {
        filePath ??= GetDefaultSeedFile();

        if (!File.Exists(filePath))
            return $"File not found: {filePath}. Run Save first to create a state file.";

        var json = await File.ReadAllTextAsync(filePath);
        var resources = JsonSerializer.Deserialize<List<Resource<JsonResource>>>(json);

        if (resources == null || resources.Count == 0)
            return "No resources found in the state file.";

        await using var context = await _contextFactory.CreateDbContextAsync();

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var existing = await context.JsonResources.ToListAsync();
            context.JsonResources.RemoveRange(existing);
            await context.SaveChangesAsync();

            context.JsonResources.AddRange(resources);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            return $"Reloaded {resources.Count} resource(s) from {filePath}";
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static string GetDefaultSeedFile() =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData", "seed.json");

    private static List<Resource<JsonResource>> CreateSchemaResources()
    {
        var timestamp = DateTime.UtcNow;

        var characterSchema = new Resource<JsonResource>
        {
            Id = Guid.NewGuid().ToString(),
            OwnerId = "system",
            ResourceKind = ResourceKinds.Schema,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Version = 1,
            IsDeleted = false,
            Data = new JsonResource
            {
                Id = Guid.NewGuid().ToString(),
                ResourceKind = ResourceKinds.Schema,
                Data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
                {
                    name = "Character",
                    sections = new[]
                    {
                        new
                        {
                            name = "Basic Info",
                            properties = new[]
                            {
                                new { name = "Name", type = PropertyTypes.String },
                                new { name = "Level", type = PropertyTypes.Int },
                                new { name = "IsActive", type = PropertyTypes.Bool }
                            }
                        }
                    }
                }))
            }
        };

        var itemSchema = new Resource<JsonResource>
        {
            Id = Guid.NewGuid().ToString(),
            OwnerId = "system",
            ResourceKind = ResourceKinds.Schema,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Version = 1,
            IsDeleted = false,
            Data = new JsonResource
            {
                Id = Guid.NewGuid().ToString(),
                ResourceKind = ResourceKinds.Schema,
                Data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
                {
                    name = "Item",
                    sections = new[]
                    {
                        new
                        {
                            name = "Item Details",
                            properties = new[]
                            {
                                new { name = "Name", type = PropertyTypes.String },
                                new { name = "Weight", type = PropertyTypes.Decimal },
                                new { name = "Description", type = PropertyTypes.String }
                            }
                        }
                    }
                }))
            }
        };

        var spellSchema = new Resource<JsonResource>
        {
            Id = Guid.NewGuid().ToString(),
            OwnerId = "system",
            ResourceKind = ResourceKinds.Schema,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Version = 1,
            IsDeleted = false,
            Data = new JsonResource
            {
                Id = Guid.NewGuid().ToString(),
                ResourceKind = ResourceKinds.Schema,
                Data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
                {
                    name = "Spell",
                    sections = new[]
                    {
                        new
                        {
                            name = "Spell Details",
                            properties = new[]
                            {
                                new { name = "Name", type = PropertyTypes.String },
                                new { name = "Level", type = PropertyTypes.Int },
                                new { name = "CastTime", type = PropertyTypes.String }
                            }
                        }
                    }
                }))
            }
        };

        return [characterSchema, itemSchema, spellSchema];
    }
}
