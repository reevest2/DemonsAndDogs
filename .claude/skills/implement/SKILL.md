---
name: implement
description: Implement active feature using TDD — writes failing tests, then implements, then refactors
disable-model-invocation: true
---

You are implementing a feature for DemonsAndDogs using test-driven development.

## Step 1 — Load Context

Read `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md`.
Read the active spec file listed there. Verify you're on the correct branch; checkout if not.

## Step 2 — Explore

Use a subagent (Explore type) to investigate:
- Which existing services, models, and interfaces are relevant to this spec
- Existing test patterns in `DemonsAndDogs.API.Tests/` and `DemonsAndDogs.Tests/`
- Any similar features already implemented to follow as patterns

## Step 3 — RED (Write Failing Tests)

For each acceptance criterion in the spec:
- Write xunit `[Fact]` tests that will FAIL until implemented
- Follow existing patterns: AAA structure, NSubstitute for mocks, `Fakes/` for test doubles
- API/service changes → `DemonsAndDogs.API.Tests/`
- UI/component changes → `DemonsAndDogs.Tests/`

Run: `dotnet test DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -30`
Confirm new tests fail while existing tests still pass.
Update memory: phase → red

## Step 4 — GREEN (Implement)

Implement the minimum code to make ALL tests pass.
- Follow existing architecture (see CLAUDE.md for layer responsibilities)
- Register new services in `API.Configuration/`

Run: `dotnet test DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -30`
ALL tests must pass before proceeding.
Update memory: phase → green

## Step 5 — REFACTOR

- Eliminate duplication, ensure consistent naming
- Run full test suite one more time

Update memory: phase → refactor

## Step 6 — Commit

```bash
git add -A
git commit -m "feat: {feature-name}

- {bullet summary of changes}
- Tests: {N} new tests added

Spec: docs/specs/{spec-file}"
```

Update memory: phase → implemented.
Report: "Implementation complete. Run `/ship` to create PR."
