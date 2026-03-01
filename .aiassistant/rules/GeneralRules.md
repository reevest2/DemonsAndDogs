---
apply: always
---

# Junie Assistant Rules for DemonsAndDogs

## Summary
This document defines **non-negotiable engineering rules** for the DemonsAndDogs solution, plus an explanation of the **JsonResource-based resource pattern** used in the database layer.

---

## Core Rules

### 1) Resources come only from JsonResource (no new resource models)
- **Do not create new resource model types** (no new `Resource<T>` specializations beyond what already exists).
- Resources must be represented using **`JsonResource`** and persisted in the **`JsonResources`** table via:
    - `DbSet<Resource<JsonResource>> JsonResources`
- If new resource shapes are needed, they must be expressed as **JSON inside `JsonResource`**, not new tables/models.

---

### 2) Pages must be parameterized components; data fetched via Mediator queries to the API
- Each UI “page” in **DemonsAndDogs** must be a **component** that accepts parameters for its data needs (ids, filters, paging, etc.).
- The component **must not** directly embed data access logic.
- UIComponents should live in the UIComponentsLibrary
- Data must be retrieved by:
    - issuing **Mediator queries**
    - which call the **API** (not direct DB access from UI)

---

### 3) Prefer components as much as possible
- UI should be built from reusable components first.
- Avoid duplicating layout, form fields, grids, dialogs, and validation across multiple pages.
- If something appears more than once, it should become a component.

---

### 4) Controller method parameters must use records
- When passing parameters into controller actions, use a **single record** type rather than multiple primitive parameters.
- Applies to both query and body payloads where appropriate.
- This includes filters, paging, search criteria, and command payloads.

---

### 5) One type per file (services, repositories, contracts, handlers)
- Each **service**, **repository**, **contract**, and **handler** must live in its **own file**.
- No “grouped” files containing multiple handlers/contracts/types.

---

### 6) No magic strings; use AppConstants class library
- Do not introduce hard-coded strings for:
    - routes, policy names, claim types, cache keys
    - resource keys, table names (where applicable), header names
    - any cross-cutting identifiers used in more than one place
- Use the **AppConstants** class library for these values.

### 7) Ensure that unit tests in the project all pass.

---