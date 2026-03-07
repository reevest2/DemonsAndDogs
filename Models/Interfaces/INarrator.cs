using Models.Narration;

namespace Models.Interfaces;

public interface INarrator
{
    Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? Tone = "neutral");
}
