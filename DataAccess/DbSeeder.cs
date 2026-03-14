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
                    """{ "name": "Gimli", "race": "Dwarf", "class": "Fighter", "strength": 18, "dexterity": 12, "constitution": 16, "intelligence": 10, "wisdom": 12, "charisma": 8, "hp": 45, "ac": 16 }""")
            },
            new CharacterResource
            {
                Id = "seed-char-2",
                EntityId = "Legolas",
                GameId = GameSystemIds.DnD5e,
                ResourceKind = ResourceKinds.Character,
                Data = JsonSerializer.Deserialize<JsonElement>(
                    """{ "name": "Legolas", "race": "Elf", "class": "Ranger", "strength": 12, "dexterity": 18, "constitution": 14, "intelligence": 12, "wisdom": 14, "charisma": 12, "hp": 38, "ac": 14 }""")
            }
        );

        await context.SaveChangesAsync();
    }
}
