---
name: new-feature
description: Start a new feature — interviews user, writes spec, creates feature branch
disable-model-invocation: true
argument-hint: "[feature name or description]"
---

You are starting a new feature for DemonsAndDogs. $ARGUMENTS is the feature hint (optional).

## Step 1 — Context

Read:
- `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md`
- `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\project_milestone_state.md`
- `docs/specs/_template.md`

## Step 2 — Interview

Use AskUserQuestion to gather requirements. Ask about:
- Core behavior and user value
- Acceptance criteria (what does "done" look like?)
- Edge cases and error handling
- Constraints (performance, backwards-compat)

Do NOT ask obvious questions. Focus on the hard parts. Use a subagent to explore which existing services/models are relevant if needed.

## Step 3 — Write Spec

Create `docs/specs/{kebab-case-name}.md` using the template. The spec MUST include:
- Summary (1 paragraph)
- Acceptance criteria (numbered, each testable)
- Technical approach (which files change and how)
- Test plan (what tests to write)
- Out of scope

## Step 4 — Branch and Commit

```bash
git checkout -b feature/{kebab-case-name}
git add docs/specs/{kebab-case-name}.md
git commit -m "spec: {feature-name} — acceptance criteria and technical approach"
```

## Step 5 — Update Memory

Update `current-session.md` with:
- active_feature: {name}
- active_spec: docs/specs/{file}.md
- phase: spec
- branch: feature/{kebab-name}
- next_step: Run `/implement` to begin TDD

Report: "Spec committed. Run `/implement` to begin TDD."
