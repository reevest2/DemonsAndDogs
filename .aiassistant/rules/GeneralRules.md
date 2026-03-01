---
apply: always
---

# DemonsAndDogs – Rider Junie Enforcement Rules

This document defines non-negotiable architectural constraints for the DemonsAndDogs solution.

Rider Junie must treat these as hard constraints, not suggestions.

These rules apply to:

- All new code
- All refactored code
- All generated code
- All modifications to existing code

---

## 1. Resource Model Constraint (JsonResource Only)

### Forbidden

Rider Junie must NOT:

- Create new resource model types
- Create new Resource<T> specializations
- Create new resource tables
- Introduce alternative resource storage mechanisms

### Required

All resources MUST:

- Use JsonResource
- Be stored in the JsonResources table
- Be accessed via:

```csharp
DbSet<Resource<JsonResource>> JsonResources
```

### Extending Resource Shape

If a new resource structure is required:

- The structure MUST be represented as JSON inside JsonResource
- No database schema changes are allowed
- No additional EF entities are allowed

Database schema stability is mandatory.

---

## 2. UI Architecture Constraint

### Page Definition

Every UI page MUST:

- Be implemented as a parameterized component
- Accept required inputs via parameters (id, filters, paging, etc.)

A page MUST NOT:

- Access the database directly
- Reference repositories
- Contain embedded data access logic

### Required Data Flow

All data retrieval MUST follow this exact flow:

```
UI Component
→ Mediator Query
→ API Endpoint
→ Handler
→ Data Layer
```

Direct UI → Database access is forbidden.

### Component Location

Reusable UI components MUST exist in:

```
UIComponentsLibrary
```

---

## 3. Component-First Enforcement

Before creating new UI markup or logic:

1. Check for an existing reusable component.
2. If functionality appears more than once, it MUST be extracted into a component.

This applies to:

- Forms
- Grids
- Dialogs
- Layout sections
- Validation logic

Duplication is not permitted when componentization is possible.

---

## 4. Controller Parameter Constraint (Records Only)

Controller methods MUST:

- Accept a single record type as input
- Not use multiple primitive parameters

Applies to:

- Query parameters
- Request bodies
- Filters
- Paging
- Search criteria
- Command payloads

### Correct

```csharp
public async Task<IActionResult> Get(GetDogsQuery request)
```

### Incorrect

```csharp
public async Task<IActionResult> Get(Guid id, int page, string filter)
```

---

## 5. One Type Per File Rule

Each of the following MUST exist in its own file:

- Service
- Repository
- Contract
- Handler
- Record
- Interface

Forbidden:

- Multiple handlers in one file
- Grouped contracts
- Multiple services in one file

File name MUST match type name.

---

## 6. No Hard-Coded Strings

Rider Junie must NOT introduce hard-coded strings for:

- Routes
- Policy names
- Claim types
- Cache keys
- Resource keys
- Header names
- Cross-layer identifiers
- Any value reused across layers

All shared string values must come from the AppConstants class library.

If a string could be reused, it belongs in AppConstants.

---

## 7. Unit Test Integrity

All unit tests MUST pass.

Rider Junie must NOT:

- Introduce changes that break existing tests
- Leave failing tests unresolved
- Introduce functionality without appropriate tests (when applicable)

Test stability is mandatory.

---

## Enforcement Priority

If a requested implementation conflicts with these rules:

1. These rules take priority.
2. The architecture must not be violated.
3. An alternative compliant implementation must be used.

These constraints are architectural invariants of the DemonsAndDogs solution.