# Data Model

## Design Decision: Hybrid Typed Shell + JSON Stats

### The Problem
Different TTRPG game systems have completely incompatible data shapes:
- D&D 5e: `Strength`, `Dexterity`, `SpellSlots`, `HP`
- Call of Cthulhu: `Sanity`, `Luck`, `Mythos`, `Occupation`
- Pathfinder: `BAB`, `CMB`, `CMD`, `Feats`

A fixed relational schema can't support arbitrary game systems without
constant migrations. But fully untyped JSON loses queryability and type safety.

### The Solution: Hybrid Entities
Fixed columns for common fields, `JsonDocument` column for system-specific data.

```csharp
public class CharacterEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string SystemId { get; set; }      // "dnd5e", "cthulhu", "pathfinder"
    public string CampaignId { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public JsonDocument Stats { get; set; }   // system-specific stats blob
    public string SchemaVersion { get; set; } // for future migrations
}
```

### Why This Works
- Queryable on common fields: `WHERE SystemId = 'dnd5e' AND CampaignId = '...'`
- Flexible stats: any game system works with zero schema migrations
- PDF export: stats are already JSON, map to sheet template via CharacterSheetSchema
- Type-safe at API boundary: IRuleBook validates and interprets the stats blob

### IRuleBook as Stats Interpreter
IRuleBook is the bridge between raw JSON stats and typed game logic:

```csharp
public interface IRuleBook
{
    // Validates a stats blob is well-formed for this system
    bool ValidateStats(JsonDocument stats);

    // Builds a typed context from raw stats for resolution
    SkillCheckContext BuildSkillCheckContext(JsonDocument stats, string skillId);
    AttackContext BuildAttackContext(JsonDocument stats, string weaponId);
}
```

D&D 5e knows: Strength modifier = (strength - 10) / 2
Call of Cthulhu knows: skill check = roll under skill value
The database just stores the blob. Each rulebook owns interpretation.

### API Boundary
Controllers return concrete resource types, never the abstract JsonResource base:

```csharp
// ✅ Correct
IEnumerable<CampaignResource> GetCampaigns()

// ❌ Wrong — client can't deserialize abstract type
IEnumerable<JsonResource> GetCampaigns()
```

JsonResource uses [JsonPolymorphic] + [JsonDerivedType] as a safety net
for any endpoints that must return mixed resource types.

## Entity Summary

| Entity | Fixed Fields | Stats Blob |
|---|---|---|
| `CharacterEntity` | Id, Name, SystemId, CampaignId, OwnerId | Ability scores, skills, equipment |
| `CampaignEntity` | Id, Name, SystemId, Description, OwnerId | Campaign-specific settings |
| `SessionEntity` | Id, CampaignId, CharacterId, StartedAt | Session state, current scene |
| `SessionEventEntity` | Id, SessionId, EventType, Timestamp | Event-specific data |
| `GameWorldEntity` | Id, CampaignId, EntityType, Name | NPC/monster/location stats |
| `RulebookEntity` | Id, SystemId, Name, Version | Parsed rulebook definition |

## PDF Export Strategy
Character sheet PDF generation flow:
```
CharacterEntity.Stats (JsonDocument)
  → IRuleBook.GetCharacterSheetSchema()  (field labels + layout)
    → PDF template filled with stats values
      → exported as .pdf
```

## Implementation Note
Mock data services are in place for MVP. Real persistence using this
hybrid model will be implemented post-MVP in the DataAccess project.
Switch from mock to real by flipping UseMockData in appsettings.
