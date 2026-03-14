# Session End

Close out a working session cleanly. Run this at the end of every coding session — even if the feature is not complete.

## Steps

1. **Build check** — Run `dotnet build` from the project root. If there are errors, stop and fix them before continuing. The build must be clean before committing.

2. **Test check** — Run `.\scripts\Run-Tests.ps1`. Record the pass count. If tests are failing that were previously passing, stop and fix regressions before continuing.

3. **Doc validation** — Run `.\scripts\Validate-Docs.ps1`. Report any broken links or unindexed files, but do not block the commit on doc warnings.

4. **Update state docs** — Make the following edits:
   - `docs/state/current-state.md` — If any features moved from Mocked/Stub to Working, update the relevant sections. Add a brief "In Progress" note if a feature is partially done.
   - `docs/state/roadmap.md` — Mark any completed roadmap items as `Done`.

5. **Update session memory** — Rewrite `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md` to reflect end-of-session state:
   - Mark completed sub-tasks with `[x]`
   - Set the next uncompleted task as the first `[ ]` item
   - Record any key decisions made this session

6. **Commit** — Stage and commit all modified files (excluding secrets or build artifacts). Use this commit message format:
   ```
   wip: <feature-name> - <one sentence on what's done>

   - <bullet: what was completed>
   - <bullet: what remains>
   - Tests: <N> passing
   ```
   If the feature is fully complete, use `feat:` instead of `wip:`.

7. **Report close-out summary**:

   ## Session Closed
   - **Build**: clean / errors (list)
   - **Tests**: [N] passing, [N] failing
   - **Committed**: yes/no
   - **Next task**: [first pending item in current-session.md]
   - **Resume command**: `Call GetStarted. Then read docs/features/<spec>. Context: [paste commit message].`

If `$ARGUMENTS` is provided, treat it as extra context for the commit message.
