# Narration: LM Studio Integration

## Overview

AI narration is provided by a local LLM running in LM Studio. The narrator is abstracted
behind `INarrator` in `Models/Interfaces/`. The concrete implementation is `LocalLlmNarrator`
in `API.Services/Narration/`. Configuration lives in `appsettings.Development.json` under
the `LocalLlm` key.

## Endpoint

LM Studio exposes a **non-OpenAI-compatible** chat endpoint. Use:

```
POST http://127.0.0.1:1234/api/v1/chat
```

Do **not** use `/v1/chat/completions` — that is the OpenAI-compatible endpoint and is not
supported in the version of LM Studio used by this project.

## Configuration

`appsettings.Development.json`:

```json
"LocalLlm": {
  "BaseUrl": "http://127.0.0.1:1234/api",
  "ModelId": "google/gemma-3-4b",
  "SystemPrompt": "You are Mordecai..."
}
```

- `BaseUrl` must be the host + `/api` with no trailing slash. The narrator appends `/v1/chat` itself.
- `ModelId` must match exactly the model identifier shown in LM Studio's model list.
- Do not put a path suffix in `BaseUrl` — the narrator always appends `/v1/chat`.

## Request Format

LM Studio's `/api/v1/chat` uses `input` (a plain string) rather than a `messages` array:

```json
{
  "model": "google/gemma-3-4b",
  "input": "Character: Gimli\nAction: Attack\nOutcome: ...\nTone: dramatic",
  "stream": true
}
```

The system prompt is configured separately in `LocalLlmOptions` and prepended by LM Studio
when the model is loaded. The `input` field contains only the user-facing prompt.

## SSE Response Format

When `stream: true`, LM Studio returns a sequence of SSE `data:` lines. Each line carries
a JSON object with a `type` field. Only `message.delta` events carry tokens:

```
data: {"type":"chat.start","model_instance_id":"google/gemma-3-4b"}
data: {"type":"prompt_processing.start"}
data: {"type":"prompt_processing.progress","progress":0}
data: {"type":"prompt_processing.progress","progress":1}
data: {"type":"prompt_processing.end"}
data: {"type":"message.start"}
data: {"type":"message.delta","content":"Gimli"}
data: {"type":"message.delta","content":" swings"}
data: {"type":"message.delta","content":" his axe..."}
data: {"type":"message.end"}
data: {"type":"chat.end"}
data: [DONE]
```

The parser in `StreamTokensAsync` must:
1. Strip the `data: ` prefix
2. Skip `[DONE]`
3. Parse as JSON
4. Only yield `content` when `type == "message.delta"`
5. Ignore all other event types (`chat.start`, `chat.end`, `message.start`, `message.end`, `prompt_processing.*`)

## Stream Lifetime (Important)

`LocalLlmNarrator` uses `HttpCompletionOption.ResponseHeadersRead` so the response body
is not buffered. The `HttpRequestMessage`, `HttpResponseMessage`, `Stream`, and `StreamReader`
are all owned inside the `StreamTokensAsync` async iterator via `using` declarations.

They must **not** be disposed before iteration completes. Do not move the HTTP call back
into `NarrateAsync` and pass the stream out — the response will be disposed before the
controller can iterate it, causing a 500.

## Blazor WASM Streaming (Important)

`ApiClient.GetStream` runs in Blazor WASM (browser/WebAssembly). The browser's HTTP stack
does **not** support synchronous reads. This means:

- **Never use `StreamReader.EndOfStream`** — it performs a synchronous read and throws
  `net_http_synchronous_reads_not_supported`
- Use `while (true)` and break on `null` from `ReadLineAsync` instead:

```csharp
// WRONG - throws in Blazor WASM
while (!reader.EndOfStream)

// CORRECT
while (true)
{
    var line = await reader.ReadLineAsync(ct);
    if (line == null) break;
    ...
}
```

## Flow

```
NarrationPanel.razor
  -> GET api/narration/stream/{sessionId}
  -> NarrationController
  -> mediator.Send(NarrateActionRequest)
  -> NarrateActionHandler
      - looks up session from SessionStore
      - maps last SessionEvent -> GameEvent
  -> LocalLlmNarrator.NarrateAsync(gameEvent, "dramatic")
      - builds prompt string
      - returns NarrationResult with TokenStream = StreamTokensAsync(...)
  -> NarrationController iterates TokenStream
      - writes each token as SSE: data: {token}\n\n
  -> NarrationPanel.razor receives tokens via ApiClient.GetStream()
      - appends character by character with 15ms delay for typewriter effect
```

## Narrator Persona

Mordecai is the configured narrator persona. System prompt characteristics:
- Dramatic, theatrical, slightly unhinged
- Describes action outcomes in 2-3 vivid sentences
- Rich sensory detail, occasional fourth-wall breaks and dry wit
- Celebrates successes, makes failures entertainingly catastrophic
- Never explains game mechanics, never breaks character

The system prompt is stored in `LocalLlmOptions.SystemPrompt` and configured per-environment
in `appsettings.Development.json`.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| 404 from LM Studio | Wrong endpoint path | Ensure `BaseUrl` ends with `/api`, narrator appends `/v1/chat` |
| 500 from API | Stream disposed early | Ensure HTTP call is inside the iterator, not in `NarrateAsync` |
| `net_http_synchronous_reads_not_supported` | `StreamReader.EndOfStream` used in Blazor WASM | Replace `while (!reader.EndOfStream)` with `while (true)` and break on null from `ReadLineAsync` |
| "The shadows grow silent..." with no error | SSE parse failure | Check `type` field - only yield `content` on `message.delta` events |
| "The shadows grow silent..." after working | `ApiClient.GetStream` parsing wrong field | Ensure no JSON parsing in `GetStream` - yield raw `data:` value directly |
| LM Studio logs show request but no tokens | Wrong request shape | Use `input` string, not `messages` array |
| LM Studio returns 404 | Wrong endpoint | Use `/api/v1/chat` not `/api/v1/chat/completions` |
| Model not found | Wrong `ModelId` | Match exactly to LM Studio's loaded model identifier |
