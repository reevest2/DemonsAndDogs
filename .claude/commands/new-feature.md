# New Feature

Scaffold a new feature spec and prepare the TDD structure. Run this when starting work on any feature that doesn't have a spec yet.

## Steps

1. **Confirm no spec exists** — Call `GetDoc` with the feature name from `$ARGUMENTS`. If a spec already exists, stop and tell the user to use `/session-start` instead.

2. **Create the spec** — Run the scaffold script:
   ```powershell
   .\scripts\New-Spec.ps1 -Name "<feature-name>" -Title "<Feature Title>" -Milestone "<Milestone Name>"
   ```
   Use `$ARGUMENTS` to determine the feature name and title. If milestone is not provided in `$ARGUMENTS`, ask the user before running the script.

3. **Draft the spec content** — Open the newly created spec file and fill in these sections based on what you know from the roadmap and architecture docs:
   - **Goal**: one sentence
   - **Constraints**: list 3–5 project-specific rules that apply (no magic strings, records only, no DbContext outside DataAccess/, etc.)
   - **Flow**: numbered steps from controller to database (or component to service)
   - **Test Cases**: 8–15 test names in `MethodName_StateUnderTest_ExpectedBehavior` format. Write the happy-path cases first, then edge cases, then error cases.
   - **API Changes**: list every new MediatR request record by name and signature

   Do NOT include: C# code snippets, architecture explanations (link to docs/core/ instead), or scope beyond this feature.

4. **Create test file skeleton** — In the appropriate test project, create a new test file with stubbed test methods matching every test case in the spec. Do not write assertions yet — just the method signatures with `// Arrange / Act / Assert` comments and `throw new NotImplementedException()` bodies. This confirms the spec compiles.
   - API/service logic → `DemonsAndDogs.API.Tests/<FeatureName>/`
   - Blazor components → `DemonsAndDogs.Tests/<FeatureName>/`

5. **Update session memory** — Write to `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md`:
   - Active feature: the new spec name
   - Progress: all test cases as `[ ]` unchecked items
   - Key decisions: any scope constraints decided in step 3

6. **Run build check** — Run `dotnet build`. The test skeleton must compile (stubs are allowed to throw `NotImplementedException`). Fix any compile errors before declaring the scaffold complete.

7. **Report ready state**:

   ## Feature Scaffolded
   - **Spec**: docs/features/<name>.md
   - **Test file**: <path to test file>
   - **Test cases**: [N] stubs created
   - **Next step**: Run `/session-start <feature-name>` to begin TDD implementation

`$ARGUMENTS` format: `<feature-name> "<Feature Title>" "<Milestone Name>"`
Example: `session-persistence "Session Persistence" "Milestone 5 — Real Data"`
