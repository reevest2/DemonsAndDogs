using API.Services.Abstraction;
using Models.Common;
using System.Text.Json;
using AppConstants;

namespace API.Services.Mock;

public class MockCharacterService : ICharacterService
{
    private static readonly List<JsonResource> _characters = new()
    {
        new CharacterResource
        {
            Id = "mock-char-1",
            EntityId = "Gimli",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Character,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ 
                ""name"": ""Gimli"", 
                ""race"": ""Dwarf"", 
                ""class"": ""Fighter"", 
                ""stats"": { ""str"": 18, ""dex"": 12, ""con"": 16, ""int"": 10, ""wis"": 12, ""cha"": 8 } 
            }")
        },
        new CharacterResource
        {
            Id = "mock-char-2",
            EntityId = "Legolas",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Character,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ 
                ""name"": ""Legolas"", 
                ""race"": ""Elf"", 
                ""class"": ""Ranger"", 
                ""stats"": { ""str"": 12, ""dex"": 18, ""con"": 14, ""int"": 12, ""wis"": 14, ""cha"": 12 } 
            }")
        }
    };

    public Task<IEnumerable<JsonResource>> GetAllAsync() => Task.FromResult<IEnumerable<JsonResource>>(_characters);

    public Task<JsonResource?> GetByIdAsync(string id) => Task.FromResult(_characters.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<JsonResource>> GetBySystemIdAsync(string systemId) => 
        Task.FromResult(_characters.Where(c => c.GameId == systemId));
}
