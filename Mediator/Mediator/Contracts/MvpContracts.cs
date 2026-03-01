using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

// Schema Queries
public record GetSchemasListQuery(string OwnerId) : IRequest<List<SchemaResource>>;
public record GetSchemaEditorQuery(string OwnerId, string SchemaKey) : IRequest<SchemaResource>;

// Schema Commands
public record CreateSchemaCommand(string OwnerId, string SchemaKey, string Name) : IRequest<SchemaResource>;
public record SaveSchemaDraftCommand(string OwnerId, string SchemaId, string DraftJson) : IRequest<SchemaResource>;
public record ValidateSchemaDraftCommand(string DraftJson) : IRequest<ValidationResult>;
public record PublishSchemaCommand(string OwnerId, string SchemaId) : IRequest<PublishedSchemaResource>;

// Resource Queries
public record GetResourcesListQuery(string OwnerId, string? SchemaKey = null) : IRequest<List<DataResource>>;
public record GetResourceEditorQuery(string OwnerId, string ResourceId) : IRequest<DataResource>;
public record GetNewResourceEditorQuery(string OwnerId, string SchemaKey, int? Version = null) : IRequest<DataResource>;

// Resource Commands
public record ValidateResourceCommand(string SchemaKey, int Version, string JsonPayload) : IRequest<ValidationResult>;
public record SaveResourceCommand(string OwnerId, DataResource Resource) : IRequest<DataResource>;

public record ValidationResult(bool IsValid, List<ValidationError> Errors);
public record ValidationError(string Path, string Message);
