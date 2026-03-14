# Session Start

Open a working session for Demons and Dogs. Run this at the beginning of every coding session.

## Steps

1. **Load project context** — Call the `GetStarted` MCP tool. This returns the doc index and roadmap. Do not skip this step.

2. **Read session memory** — Read the file at `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md`. This tells you what feature is active, what's been done, and what the next task is.

3. **Confirm baseline** — Run `.\scripts\Run-Tests.ps1` from the project root. Report the pass/fail count. If tests are failing, stop and report which ones before proceeding.

4. **Load active spec** — Based on the active feature in memory, call `GetDoc` to load the feature spec from `docs/features/`. If no active feature is recorded in memory, ask the user which feature to work on.

5. **Report session state** — Output a brief session brief:

   ## Session Ready
   - **Active feature**: [spec name]
   - **Tests baseline**: [N] passing
   - **Next task**: [the next unchecked item from current-session.md]

If `$ARGUMENTS` is provided, treat it as the name of the spec to load (overrides the active feature in memory).
