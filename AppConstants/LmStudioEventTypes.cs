namespace AppConstants;

public static class LmStudioEventTypes
{
    public const string MessageDelta = "message.delta";
    public const string ChatStart = "chat.start";
    public const string MessageEnd = "message.end";
    public const string ChatEnd = "chat.end";
    public const string PromptProcessingStart = "prompt_processing.start";
    public const string PromptProcessingEnd = "prompt_processing.end";
    
    // SSE field names
    public const string TypeField = "type";
    public const string ContentField = "content";

    // SSE stream markers
    public const string SsePrefix = "data: ";
    public const string SseDone = "data: [DONE]";

    public static readonly string[] All =
    [
        MessageDelta,
        ChatStart,
        MessageEnd,
        ChatEnd,
        PromptProcessingStart,
        PromptProcessingEnd
    ];
}
