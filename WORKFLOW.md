# Developer Workflow — Demons and Dogs

This is the human-readable guide for how to work on this project.
It describes the spec-driven, AI-assisted development loop used for every feature.

---

## The Core Loop

```
Plan with Claude
     |
     v
Claude writes Junie prompts
     |
     v
Paste prompts into Junie
     |
     v
Paste Junie output back into Claude for review
     |
     v
Repeat until roadmap item is complete
     |
     v
Move to next item
```

During this process, Claude will prompt you to keep docs and context up to date.

---

## Step by Step

### Step 1 — Plan with Claude

Start a conversation with Claude and upload `claude-context.txt` from the project root.

Claude will help you:
- Understand what needs to be built next (check `docs/state/roadmap.md`)
- Write or review the feature spec in `docs/features/`
- Identify gaps, constraints, and edge cases before any code is written
- Break the work into clear, Junie-sized tasks

**Tools to run before starting:**
```powershell
# Export current project state for Claude
.\scripts\Export-ClaudeContext.ps1

# Validate docs are in sync
.\scripts\Validate-Docs.ps1
```

---

### Step 2 — Create a Spec (if one doesn't exist)

Every feature needs a spec before Junie writes any code.

```powershell
.\scripts\New-Spec.ps1 -Name "feature-name" -Title "Feature Title" -Milestone "Milestone X"
```

This creates `docs/features/feature-name.md` and registers it in `index.md` and `roadmap.md`.

Fill in the spec with Claude's help. The most important sections are:
- **Flow** — end to end description of how the feature works
- **Test Cases** — these drive TDD, Junie writes these tests first
- **Constraints** — what must not change, what patterns must be followed

A spec is ready for Junie when all sections are filled in and Test Cases are defined.

---

### Step 3 — Claude Writes Junie Prompts

Once the spec is solid, ask Claude to write the Junie prompts.

Every prompt follows this format:
```
Context:   What exists, what the current state is
Goal:      What needs to be built, specific and measurable
Constraints: What must not change, what patterns to follow
Output:    Exact files expected
```

Claude will write one prompt per logical task. For a typical feature this is 2-4 prompts
run in sequence, where each builds on the last.

See `docs/state/index.md` for the example prompt format.

---

### Step 4 — Paste Prompts into Junie

Copy each prompt from Claude and paste it into Junie.

Junie will:
1. Call `GetStarted` from the MCP server to load project context
2. Read the relevant spec doc
3. Write tests first (from the Test Cases section of the spec)
4. Write implementation to make the tests pass
5. Report what changed

---

### Step 5 — Paste Junie Output Back into Claude

Copy Junie's summary of changes and paste it back into Claude.

Claude will:
- Review whether the output matches the spec
- Identify any issues or missing pieces
- Write the next prompt if more work is needed
- Tell you when the roadmap item is complete

Repeat Steps 3-5 until the feature is done.

---

### Step 6 — Close Out the Feature

When a feature is complete:

1. **Update docs:**
   - Mark the item as `Done` in `docs/state/roadmap.md`
   - Update `docs/state/current-state.md` to reflect the new state
   - Fill in `## Implementation Notes` in the feature spec

2. **Validate:**
   ```powershell
   .\scripts\Validate-Docs.ps1
   ```

3. **Re-export context:**
   ```powershell
   .\scripts\Export-ClaudeContext.ps1
   ```

4. Upload the new `claude-context.txt` to the Claude Project to keep context synced.

---

## Scripts Reference

| Script | Purpose | When to Run |
|---|---|---|
| `scripts/Export-ClaudeContext.ps1` | Exports all source files into a single Claude context file | Start of each Claude session, end of each feature |
| `scripts/Validate-Docs.ps1` | Checks all docs are indexed, links are valid, roadmap is current | Before and after any doc changes |
| `scripts/New-Spec.ps1` | Scaffolds a new feature spec and registers it in index and roadmap | When starting a new feature |

---

## Key Docs Reference

| Doc | Purpose |
|---|---|
| `docs/state/roadmap.md` | What's planned, in progress, and done |
| `docs/state/current-state.md` | What's working, what's mocked, what's not started |
| `docs/state/index.md` | Master entry point — links to all docs |
| `docs/core/best-practices.md` | Coding patterns Junie must follow |
| `docs/core/architecture.md` | Project structure and key design decisions |
| `docs/features/` | One spec per feature — read before writing any code |

---

## Rules

- **No code without a spec.** If a spec doesn't exist, create one first.
- **No magic strings.** All constants go in `AppConstants/`.
- **Tests before implementation.** Junie writes test cases from the spec first.
- **Keep docs current.** Update `current-state.md` and `roadmap.md` when features complete.
- **Re-export context** after significant changes so Claude stays synced.
