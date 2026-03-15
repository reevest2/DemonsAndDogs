using System.Text.Json;
using API.Controllers;
using API.Services.GameSystems;
using Microsoft.AspNetCore.Mvc;
using API.Services.GameSystems.Contracts;
using API.Services.GameSystems.Handlers;
using Models;
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

    public Result<IRuleBook> Get(string systemId) =>
        systemId == "test-system"
            ? Result<IRuleBook>.Ok(_ruleBook)
            : Result<IRuleBook>.NotFound("GameSystem", systemId);

    public IEnumerable<IRuleBook> GetAll() => [_ruleBook];
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

public class GameSystemHandlersTests
{
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
    public void GetCharacterSheetSchema_UnknownSystemId_ReturnsNotFound()
    {
        var controller = new GameSystemController(new FakeRegistry());

        var result = controller.GetSchema("unknown");

        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact]
    public async Task ResolveSkillCheck_ValidContext_ReturnsCheckResult()
    {
        var handler = new ResolveSkillCheckHandler(new FakeRegistry());
        var context = new SkillCheckContext("char-1", "stealth", 0, 0, 10);

        var result = await handler.Handle(new ResolveSkillCheckRequest("test-system", context), default);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsSuccess);
    }

    [Fact]
    public async Task ResolveAttack_ValidContext_ReturnsAttackResult()
    {
        var handler = new ResolveAttackHandler(new FakeRegistry());
        var context = new AttackContext("sword", 0, 15);

        var result = await handler.Handle(new ResolveAttackRequest("test-system", context), default);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsHit);
    }
}
