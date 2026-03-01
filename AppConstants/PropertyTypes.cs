namespace AppConstants;

public static class PropertyTypes
{
    public const string String = "string";
    public const string Int = "int";
    public const string Decimal = "decimal";
    public const string Bool = "bool";
    public const string Enum = "enum";
    public const string DateTime = "datetime";

    public static readonly string[] All =
    [
        String,
        Int,
        Decimal,
        Bool,
        Enum,
        DateTime
    ];
}
