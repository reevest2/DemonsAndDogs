namespace Models.Resources.Abstract;

public interface IResourceDefinition {
    string Key { get; }
    string Title { get; }
    Type ModelType { get; }
    Type EditorComponentType { get; }

    Func<object> NewModel { get; }
    Func<CancellationToken, Task<IReadOnlyList<object>>> LoadAll { get; }
    Func<string, CancellationToken, Task<object>> LoadOne { get; }
    Func<object, CancellationToken, Task> Create { get; }
    Func<string, object, CancellationToken, Task> Update { get; }

    Func<object, string> GetId { get; }
    IReadOnlyList<GridColumnDefinition> Columns { get; }
}

public record GridColumnDefinition(string Title, Func<object, object> Value);