# Current State

This document provides the authoritative source on what features are currently implemented, what is mocked, and what's planned next for the Demons and Dogs project.

## Working Features
- **Narration Streaming**: Mordecai narrator via LM Studio (local LLM).
- **Game Session**: In-memory session management.
- **D&D 5e Mechanics**: Skill checks and attacks implemented in `DnD5eRuleBook` with full unit test coverage.
- **UI Themes**: Support for Fantasy and Steampunk themes.
- **Character Sheet**: Dynamic display of character stats and fields.
- **Action Log**: Real-time display of game events and narration.
- **Documentation MCP Tools**: `GetStarted`, `ListDocs`, `GetDoc`, and `GetAllDocs` tools available for AI context.
- **Test Infrastructure**: Unit tests and data builders for DnD5e mechanics.
- **Session Persistence**: `ISessionPersistence` + `JsonSessionPersistence` wired into all session handlers. Sessions written to DB on start/action, loaded from DB on cache miss.
- **ICampaignService**: `JsonCampaignService` reads `CampaignResource` from DB via `IJsonResourceRepository`. Seeded with 1 campaign on startup.
- **ICharacterService**: `JsonCharacterService` reads `CharacterResource` from DB via `IJsonResourceRepository`. Seeded with 2 characters on startup.
- **Real Character Stats**: `IRuleBook.ExtractStats()` extracts stats from `CharacterResource.Data` using schema field keys. `SessionState.Stats` populated on session start. Character detail page calls `GET /api/character/{id}/stats`. Both views render real values.

## Mocked / Stub
- **Game Systems List**: Hardcoded to only show D&D 5e.

## In-Memory Cache (Write-Through to DB)
- **SessionStore**: `ISessionStore` singleton with `ConcurrentDictionary` backing acts as a write-through cache. Sessions are persisted to the DB via `ISessionPersistence` on every write and reloaded from DB on cache miss.

## Not Started
- **Art Generation**: AI-generated portraits and scene backgrounds.
- **Rulebook Upload**: PDF/Text parsing for new game systems.
- **Second Game System**: Integration of a non-dnd5e system (e.g., Call of Cthulhu).
- **Cloud Narrator Provider**: Integration with Azure OpenAI or Anthropic API.

## Known Issues
- No known issues. (All Milestone 1–4 items resolved.)

## Configuration Notes
- **LM Studio**: Must be running locally for narration to work.
- **Local API URL**: BaseUrl must be set to `http://127.0.0.1:1234/api` in settings.
- **Setup Guide**: Refer to [narration-lmstudio.md](narration-lmstudio.md) for full local LLM configuration instructions.
