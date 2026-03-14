Review the recent code changes against the project's coding standards.

1. Run `git diff` to see all uncommitted changes. If there are no uncommitted changes, run `git diff HEAD~1` to review the last commit.

2. Read the coding standards from docs/standards/ (architecture.md, naming-conventions.md, error-handling.md, testing-strategy.md).

3. Analyze every changed file against the standards. Check for:
   - Architecture violations (wrong layer dependencies, missing interfaces at boundaries)
   - Naming convention violations (casing, file naming, method naming)
   - Error handling issues (exceptions for control flow, missing guard clauses, Console.WriteLine)
   - Missing or inadequate tests for new/changed code

4. Output a structured review:

   ## Review Summary
   - Files reviewed: [count]
   - Issues found: [count]

   ## Issues
   For each issue:
   - **File**: path:line
   - **Standard**: which standard is violated
   - **Issue**: what's wrong
   - **Fix**: how to fix it

   ## Positive Notes
   Highlight anything done well.

If $ARGUMENTS is provided, focus the review on those specific files or topics.
