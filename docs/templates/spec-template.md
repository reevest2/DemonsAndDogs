# [Feature Title]

## Overview
<!-- What is this feature and why does it exist? What problem does it solve? -->

## Goals
<!-- What specific outcomes does this feature achieve? Use bullet points. -->
-

## Constraints
<!-- What must NOT change, what dependencies exist, what patterns must be followed? -->
- Follow existing patterns in `docs/core/best-practices.md`
- All constants must live in the `AppConstants` project (no magic strings)
- All async methods must accept `CancellationToken`

## Data Model
<!-- Any new or modified records, entities, or database changes. -->
<!-- If none, write "No data model changes." -->

## Flow
<!-- Step-by-step description of how this feature works end-to-end. -->
1.

## API Changes
<!-- New endpoints, modified contracts, new MediatR requests/handlers. -->
<!-- If none, write "No API changes." -->

## UI Changes
<!-- New or modified Blazor components, pages, or layouts. -->
<!-- If none, write "No UI changes." -->

## Test Cases
<!-- These drive TDD — tests are written BEFORE implementation. -->
<!-- Name format: MethodName_StateUnderTest_ExpectedBehavior -->

### Happy Path
- [ ] `MethodName_NormalCondition_ExpectedResult`

### Edge Cases
- [ ] `MethodName_BoundaryCondition_ExpectedResult`

### Error Cases
- [ ] `MethodName_InvalidInput_ThrowsOrReturnsError`

## Open Questions
<!-- Unresolved decisions that need answers before implementation begins. -->
<!-- Remove this section when all questions are answered. -->

## Implementation Notes
<!-- Added during or after implementation: decisions made, gotchas found, deviations from spec. -->
