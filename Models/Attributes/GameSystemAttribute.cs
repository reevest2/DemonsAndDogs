namespace Models.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GameSystemAttribute(string systemId) : Attribute
{
    public string SystemId { get; } = systemId;
}
