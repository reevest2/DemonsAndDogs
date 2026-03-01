using System.Text.Json;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

// --- Schema Handlers ---

public class GetSchemasListHandler(ApiClient apiClient) : IRequestHandler<GetSchemasListQuery, List<SchemaResource>>
{
    public async Task<List<SchemaResource>> Handle(GetSchemasListQuery request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<List<SchemaResource>>($"api/SchemaResource/GetAllByOwnerId/User/{request.OwnerId}", cancellationToken);
    }
}

public class GetSchemaEditorHandler(ApiClient apiClient) : IRequestHandler<GetSchemaEditorQuery, SchemaResource>
{
    public async Task<SchemaResource> Handle(GetSchemaEditorQuery request, CancellationToken cancellationToken)
    {
        // For simplicity in MVP, we might just get by ID if we have it, 
        // but the query asks for SchemaKey. Let's assume we fetch all and filter or add a specialized endpoint.
        // Given the existing ResourceControllerBase, we'll get all by owner and filter by SchemaKey.
        var schemas = await apiClient.Get<List<SchemaResource>>($"api/SchemaResource/GetAllByOwnerId/User/{request.OwnerId}", cancellationToken);
        return schemas.FirstOrDefault(s => s.SchemaKey == request.SchemaKey);
    }
}

public class CreateSchemaHandler(ApiClient apiClient) : IRequestHandler<CreateSchemaCommand, SchemaResource>
{
    public async Task<SchemaResource> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
    {
        var schema = new SchemaResource
        {
            OwnerId = request.OwnerId,
            SchemaKey = request.SchemaKey,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            DraftJson = JsonSerializer.SerializeToElement(new { type = "object", properties = new { } })
        };
        return await apiClient.Post<SchemaResource, SchemaResource>($"api/SchemaResource/Create/User/{request.OwnerId}", schema, cancellationToken);
    }
}

public class SaveSchemaDraftHandler(ApiClient apiClient) : IRequestHandler<SaveSchemaDraftCommand, SchemaResource>
{
    public async Task<SchemaResource> Handle(SaveSchemaDraftCommand request, CancellationToken cancellationToken)
    {
        var schema = await apiClient.Get<SchemaResource>($"api/SchemaResource/GetById/User/{request.OwnerId}/{request.SchemaId}", cancellationToken);
        schema.DraftJson = JsonDocument.Parse(request.DraftJson).RootElement.Clone();
        schema.UpdatedAt = DateTime.UtcNow;
        await apiClient.Put<SchemaResource>($"api/SchemaResource/Update/User/{request.OwnerId}/{request.SchemaId}", schema, cancellationToken);
        return schema;
    }
}

public class PublishSchemaHandler(ApiClient apiClient) : IRequestHandler<PublishSchemaCommand, PublishedSchemaResource>
{
    public async Task<PublishedSchemaResource> Handle(PublishSchemaCommand request, CancellationToken cancellationToken)
    {
        var schema = await apiClient.Get<SchemaResource>($"api/SchemaResource/GetById/User/{request.OwnerId}/{request.SchemaId}", cancellationToken);
        
        int nextVersion = (schema.LatestPublishedVersion ?? 0) + 1;
        
        var published = new PublishedSchemaResource
        {
            OwnerId = request.OwnerId,
            SchemaKey = schema.SchemaKey,
            Version = nextVersion,
            PublishedJson = schema.DraftJson.Clone(),
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await apiClient.Post<PublishedSchemaResource, PublishedSchemaResource>($"api/PublishedSchemaResource/Create/User/{request.OwnerId}", published, cancellationToken);
        
        schema.LatestPublishedVersion = nextVersion;
        await apiClient.Put<SchemaResource>($"api/SchemaResource/Update/User/{request.OwnerId}/{request.SchemaId}", schema, cancellationToken);
        
        return result;
    }
}

// --- Resource Handlers ---

public class GetResourcesListHandler(ApiClient apiClient) : IRequestHandler<GetResourcesListQuery, List<DataResource>>
{
    public async Task<List<DataResource>> Handle(GetResourcesListQuery request, CancellationToken cancellationToken)
    {
        var resources = await apiClient.Get<List<DataResource>>($"api/DataResource/GetAllByOwnerId/User/{request.OwnerId}", cancellationToken);
        if (!string.IsNullOrEmpty(request.SchemaKey))
        {
            return resources.Where(r => r.SchemaKey == request.SchemaKey).ToList();
        }
        return resources;
    }
}

public class GetResourceEditorHandler(ApiClient apiClient) : IRequestHandler<GetResourceEditorQuery, DataResource>
{
    public async Task<DataResource> Handle(GetResourceEditorQuery request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<DataResource>($"api/DataResource/GetById/User/{request.OwnerId}/{request.ResourceId}", cancellationToken);
    }
}

public class SaveResourceHandler(ApiClient apiClient) : IRequestHandler<SaveResourceCommand, DataResource>
{
    public async Task<DataResource> Handle(SaveResourceCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Resource.Id) || request.Resource.Id == Guid.Empty.ToString())
        {
            request.Resource.CreatedAt = DateTime.UtcNow;
            return await apiClient.Post<DataResource, DataResource>($"api/DataResource/Create/User/{request.OwnerId}", request.Resource, cancellationToken);
        }
        else
        {
            request.Resource.UpdatedAt = DateTime.UtcNow;
            await apiClient.Put<DataResource>($"api/DataResource/Update/User/{request.OwnerId}/{request.Resource.Id}", request.Resource, cancellationToken);
            return request.Resource;
        }
    }
}

public class GetNewResourceEditorHandler(ApiClient apiClient) : IRequestHandler<GetNewResourceEditorQuery, DataResource>
{
    public async Task<DataResource> Handle(GetNewResourceEditorQuery request, CancellationToken cancellationToken)
    {
        // Find the published schema version
        var publishedVersions = await apiClient.Get<List<PublishedSchemaResource>>($"api/PublishedSchemaResource/GetAllByOwnerId/User/{request.OwnerId}", cancellationToken);
        var version = request.Version ?? publishedVersions.Where(v => v.SchemaKey == request.SchemaKey).Max(v => (int?)v.Version) ?? 1;
        
        return new DataResource
        {
            OwnerId = request.OwnerId,
            SchemaKey = request.SchemaKey,
            SchemaVersion = version,
            JsonPayload = JsonSerializer.SerializeToElement(new { })
        };
    }
}

public class ValidateSchemaDraftHandler : IRequestHandler<ValidateSchemaDraftCommand, ValidationResult>
{
    public Task<ValidationResult> Handle(ValidateSchemaDraftCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        try
        {
            using var doc = JsonDocument.Parse(request.DraftJson);
            var root = doc.RootElement;
            
            // Basic validation: must be an object and have "type": "object"
            if (root.ValueKind != JsonValueKind.Object)
            {
                errors.Add(new ValidationError("$", "Schema must be a JSON object."));
            }
            else
            {
                if (!root.TryGetProperty("type", out var typeProp) || typeProp.GetString() != "object")
                {
                    errors.Add(new ValidationError("$.type", "Top-level schema must have 'type': 'object' for this MVP."));
                }
                
                if (root.TryGetProperty("properties", out var propertiesProp))
                {
                    if (propertiesProp.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add(new ValidationError("$.properties", "'properties' must be an object."));
                    }
                }
            }
        }
        catch (JsonException ex)
        {
            errors.Add(new ValidationError("$", $"Invalid JSON: {ex.Message}"));
        }

        return Task.FromResult(new ValidationResult(errors.Count == 0, errors));
    }
}

public class ValidateResourceHandler(ApiClient apiClient) : IRequestHandler<ValidateResourceCommand, ValidationResult>
{
    public async Task<ValidationResult> Handle(ValidateResourceCommand request, CancellationToken cancellationToken)
    {
        // In a real app, we'd use a JSON Schema validation library here.
        // For the MVP "Thin Slice", we'll do basic checks and assume the UI handles most via DynamicForm.
        // We'll just check if it's valid JSON for now.
        var errors = new List<ValidationError>();
        try
        {
            JsonDocument.Parse(request.JsonPayload);
        }
        catch (JsonException ex)
        {
            errors.Add(new ValidationError("$", $"Invalid JSON: {ex.Message}"));
        }
        
        return new ValidationResult(errors.Count == 0, errors);
    }
}
