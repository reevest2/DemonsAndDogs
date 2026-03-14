using System.Text.Json;
using AppConstants;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace DataAccess;

public static class DbSeeder
{
    public static async Task SeedAsync(DbContext context)
    {
        if (await context.JsonResources.OfType<CampaignResource>().AnyAsync())
            return;

        context.JsonResources.AddRange(
            new CampaignResource
            {
                Id = "seed-campaign-1",
                EntityId = "Lost Mine of Phandelver",
                GameId = GameSystemIds.DnD5e,
                ResourceKind = ResourceKinds.Campaign,
                Theme = "fantasy",
                Data = JsonSerializer.Deserialize<JsonElement>(
                    """{ "name": "Lost Mine of Phandelver", "description": "A classic D&D 5e starter adventure." }""")
            },
            new CharacterResource
            {
                Id = "seed-char-1",
                EntityId = "Gimli",
                GameId = GameSystemIds.DnD5e,
                ResourceKind = ResourceKinds.Character,
                Data = JsonSerializer.Deserialize<JsonElement>(
                    """{ "name": "Gimli", "race": "Dwarf", "class": "Fighter", "stats": { "str": 18, "dex": 12, "con": 16, "int": 10, "wis": 12, "cha": 8 } }""")
            },
            new CharacterResource
            {
                Id = "seed-char-2",
                EntityId = "Legolas",
                GameId = GameSystemIds.DnD5e,
                ResourceKind = ResourceKinds.Character,
                Data = JsonSerializer.Deserialize<JsonElement>(
                    """{ "name": "Legolas", "race": "Elf", "class": "Ranger", "stats": { "str": 12, "dex": 18, "con": 14, "int": 12, "wis": 14, "cha": 12 } }""")
            }
        );

        await context.SaveChangesAsync();
    }
}
