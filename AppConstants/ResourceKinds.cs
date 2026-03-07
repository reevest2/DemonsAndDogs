namespace AppConstants;

public static class ResourceKinds
{
    public const string Schema = "schema";
    public const string Character = "character";
    public const string Consumable = "consumable";
    public const string Game = "game";
    public const string DocumentDefinition = "document_definition";
    public const string Document = "document";
    public const string Campaign = "campaign";

    public static readonly string[] All =
    [
        Schema,
        Character,
        Consumable,
        Game,
        DocumentDefinition,
        Document,
        Campaign
    ];
}
