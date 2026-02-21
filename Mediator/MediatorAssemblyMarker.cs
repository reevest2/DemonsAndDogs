namespace Mediator;

// Add the following to Program.cs 
// builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));

/// <summary>
/// Used to register mediator services
/// </summary>
public sealed class MediatorAssemblyMarker;