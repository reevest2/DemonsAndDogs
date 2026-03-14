using AppConstants;
using Models.Narration;

namespace Models.Interfaces;

public interface INarrator
{
    Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? Tone = NarrationTones.Neutral, CancellationToken cancellationToken = default);
}
