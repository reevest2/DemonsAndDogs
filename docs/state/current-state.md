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

## Mocked / Stub
- **ICampaignService**: Returns hardcoded campaign lists.
- **ICharacterService**: Returns hardcoded character data.
- **Data Access**: No real database; all persistence is currently mocked or in-memory.
- **Character Stats**: All characters currently default to stats of 10.
- **Game Systems List**: Hardcoded to only show D&D 5e.

## In-Memory Only (Not Persisted)
- **SessionStore**: `ISessionStore` singleton with `ConcurrentDictionary` backing. DI-injected into all handlers.
- **Wiped on API Restart**: All session progress is lost when the API project restarts.

## Not Started
- **Session Persistence**: Saving sessions to a database (EF Core).
- **Real Character Stats**: System-specific stat generation and storage.
- **Art Generation**: AI-generated portraits and scene backgrounds.
- **Rulebook Upload**: PDF/Text parsing for new game systems.
- **Second Game System**: Integration of a non-dnd5e system (e.g., Call of Cthulhu).
- **Cloud Narrator Provider**: Integration with Azure OpenAI or Anthropic API.

## Known Issues
- **Magic Strings**: Some narration code still uses hardcoded strings instead of `AppConstants`.
- **Async Patterns**: `CancellationToken` is missing from several async method signatures.
- **SessionStore Design**: ~~Resolved~~ — `SessionStore` is now an `ISessionStore` singleton, injected via DI.

## Configuration Notes
- **LM Studio**: Must be running locally for narration to work.
- **Local API URL**: BaseUrl must be set to `http://127.0.0.1:1234/api` in settings.
- **Setup Guide**: Refer to [narration-lmstudio.md](narration-lmstudio.md) for full local LLM configuration instructions.
