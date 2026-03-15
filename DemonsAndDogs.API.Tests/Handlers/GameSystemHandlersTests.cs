using System.Text.Json;
using API.Controllers;
using API.Services.GameSystems;
using Microsoft.AspNetCore.Mvc;
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
/// Unit tests for game system controller endpoints and remaining MediatR handlers.
/// GetAll and GetSchema test the controller directly.
/// ResolveSkillCheck and ResolveAttack test the MediatR handlers (still in use).
/// </summary>
public class GameSystemHandlersTests
{
    // -----------------------------------------------------------------------
    // GameSystemController.GetAll
    // -----------------------------------------------------------------------

    [Fact]
    public void GetGameSystems_ReturnsDescriptorsFromRegistry()
    {
        var controller = new GameSystemController(new FakeRegistry());

        var result = controller.GetAll();

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var systems = (okResult.Value as IEnumerable<GameSystemDescriptor>)!.ToList();
        Assert.Single(systems);
        Assert.Equal("test-system", systems[0].SystemId);
        Assert.Equal("Test System", systems[0].DisplayName);
    }

    // -----------------------------------------------------------------------
    // GameSystemController.GetSchema
    // -----------------------------------------------------------------------

    [Fact]
    public void GetCharacterSheetSchema_ValidSystemId_ReturnsSchemaFromRuleBook()
    {
        var controller = new GameSystemController(new FakeRegistry());

        var result = controller.GetSchema("test-system");

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var schema = okResult.Value as CharacterSheetSchema;
        Assert.NotNull(schema);
        Assert.Equal("test-system", schema.SystemId);
        Assert.NotEmpty(schema.Sections);
    }

    [Fact]
    public void GetCharacterSheetSchema_UnknownSystemId_Throws()
    {
        var controller = new GameSystemController(new FakeRegistry());

        Assert.Throws<KeyNotFoundException>(() => controller.GetSchema("unknown"));
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
