# DnD5e RuleBook Unit Tests

## Overview
The `DnD5eRuleBook` is the core game mechanics implementation for D&D 5th Edition.
It currently has no unit tests, meaning rule resolution logic (skill checks, attacks,
critical hits) could silently break with any change. This spec covers adding full unit
test coverage for all `DnD5eRuleBook` mechanics.

## Goals
- Lock down D&D 5e mechanics so regressions are caught immediately
- Establish the test patterns and builder helpers that future game systems will follow
- First real TDD spec run through the full workflow loop

## Constraints
- Tests go in `DemonsAndDogs.API.Tests`
- Follow `MethodName_StateUnderTest_ExpectedBehavior` naming from `docs/core/testing.md`
- No changes to `DnD5eRuleBook` implementation — tests must pass against existing code
- xUnit only, no additional test frameworks

## Data Model
No changes.

## Flow
No runtime flow — these are unit tests executed by the test runner.

## API Changes
None.

## UI Changes
None.
~~~~
## Test Cases

### Skill Checks
- [ ] ResolveSkillCheck_RollPlusModifierMeetsDC_ReturnsSuccess
- [ ] ResolveSkillCheck_RollPlusModifierBelowDC_ReturnsFailure
- [ ] ResolveSkillCheck_NaturalTwenty_ReturnsSuccessRegardlessOfDC
- [ ] ResolveSkillCheck_NaturalOne_ReturnsFailureRegardlessOfModifiers
- [ ] ResolveSkillCheck_ProficiencyBonusIsApplied_TotalIsCorrect
- [ ] ResolveSkillCheck_AdditionalModifiersApplied_TotalIsCorrect

### Attacks
- [ ] ResolveAttack_RollMeetsOrExceedsAC_ReturnsHit
- [ ] ResolveAttack_RollBelowAC_ReturnsMiss
- [ ] ResolveAttack_NaturalTwenty_ReturnsCriticalHit
- [ ] ResolveAttack_NaturalOne_ReturnsCriticalMiss
- [ ] ResolveAttack_NoTargetAC_ReturnsHit
- [ ] ResolveAttack_OnHit_DamageDealtIsReturned

### Registry
- [ ] GameSystemRegistry_DiscoversDnD5eRuleBook_ViaReflection
- [ ] GameSystemRegistry_UnknownSystemId_ThrowsKeyNotFoundException

### Error Cases
- [ ] ResolveSkillCheck_NullAdditionalModifiers_DoesNotThrow
- [ ] ResolveAttack_NullAdditionalModifiers_DoesNotThrow

## Open Questions
None — MVP scope, close enough to real D&D 5e mechanics.

## Implementation Notes
<!-- Fill in after Junie completes implementation -->