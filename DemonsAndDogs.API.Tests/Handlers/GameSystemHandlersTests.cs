using System.Text.Json;
using API.Services.Abstraction;
using Mediator.Mediator.Contracts.GameSystems;
using Mediator.Mediator.Handlers.GameSystems;
using Models.GameSystems;
using Models.Interfaces;

namespace DemonsAndDogs.API.Tests.Handlers;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeRuleBook : IRuleBook
{
    public string SystemId => "test-system";
    public string DisplayName => "Test System";

    public CheckResult ResolveSkillCheck(SkillCheckContext context) =>
        new(10, 15, true, "Passed!");

    public AttackResult ResolveAttack(AttackContext context) =>
        new(15, 17, true, false, 8, "slashing", "Hit!");

    public CharacterSheetSchema GetCharacterSheetSchema() =>
        new(SystemId, [new("abilities", "Abilities", [new("strength", "Strength", "number", true, 10)])]);

    public IReadOnlyDictionary<string, int> ExtractStats(JsonElement data) =>
        new Dictionary<string, int> { ["strength"] = 18 };
}

file class FakeRegistry : IGameSystemRegistry
{
    private readonly FakeRuleBook _ruleBook = new();

    public IRuleBook Get(string systemId) =>
        systemId == "test-system" ? _ruleBook : throw new KeyNotFoundException(systemId);

    public IEnumerable<IRuleBook> GetAll() => [_ruleBook];
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

/// <summary>
/// Unit tests for game system MediatR handlers.
/// These handlers bridge MediatR requests to the IGameSystemRegistry and IRuleBook interfaces.
/// </summary>
public class GameSystemHandlersTests
{
    // -----------------------------------------------------------------------
    // GetGameSystemsHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetGameSystems_ReturnsDescriptorsFromRegistry()
    {
        var handler = new GetGameSystemsHandler(new FakeRegistry());

        var result = await handler.Handle(new GetGameSystemsRequest(), default);

        var systems = result.ToList();
        Assert.Single(systems);
        Assert.Equal("test-system", systems[0].SystemId);
        Assert.Equal("Test System", systems[0].DisplayName);
    }

    // -----------------------------------------------------------------------
    // GetCharacterSheetSchemaHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCharacterSheetSchema_ValidSystemId_ReturnsSchemaFromRuleBook()
    {
        var handler = new GetCharacterSheetSchemaHandler(new FakeRegistry());

        var result = await handler.Handle(new GetCharacterSheetSchemaRequest("test-system"), default);

        Assert.Equal("test-system", result.SystemId);
        Assert.NotEmpty(result.Sections);
    }

    [Fact]
    public async Task GetCharacterSheetSchema_UnknownSystemId_Throws()
    {
        var handler = new GetCharacterSheetSchemaHandler(new FakeRegistry());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetCharacterSheetSchemaRequest("unknown"), default));
    }

    // -----------------------------------------------------------------------
    // ResolveSkillCheckHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ResolveSkillCheck_ValidContext_ReturnsCheckResult()
    {
        var handler = new ResolveSkillCheckHandler(new FakeRegistry());
        var context = new SkillCheckContext("char-1", "stealth", 0, 0, 10);

        var result = await handler.Handle(new ResolveSkillCheckRequest("test-system", context), default);

        Assert.True(result.IsSuccess);
    }

    // -----------------------------------------------------------------------
    // ResolveAttackHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ResolveAttack_ValidContext_ReturnsAttackResult()
    {
        var handler = new ResolveAttackHandler(new FakeRegistry());
        var context = new AttackContext("sword", 0, 15);

        var result = await handler.Handle(new ResolveAttackRequest("test-system", context), default);

        Assert.True(result.IsHit);
    }
}
