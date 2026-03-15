using AppConstants;
using MediatR;
using API.Services.Sessions.Contracts;
using Models;
using Models.Interfaces;
using Models.Narration;
using Models.Session;

namespace API.Services.Sessions.Handlers;

public class NarrateActionHandler(INarrator narrator, ISessionStore sessionStore)
    : IRequestHandler<NarrateActionRequest, Result<NarrationResult>>
{
    public async Task<Result<NarrationResult>> Handle(NarrateActionRequest request, CancellationToken cancellationToken)
    {
        if (!sessionStore.TryGet(request.SessionId, out var session))
            return Result<NarrationResult>.NotFound("Session", request.SessionId);

        var lastEvent = session!.EventLog.LastOrDefault();
        if (lastEvent == null)
            return Result<NarrationResult>.Ok(new NarrationResult("No events to narrate yet.", TokenStream: null));

        // Map SessionEvent to GameEvent for the narrator
        var gameEvent = new GameEvent(
            EventType: lastEvent.EventType,
            Description: lastEvent.Description,
            OccurredAt: lastEvent.Timestamp,
            SubjectId: session.CharacterName,
            Metadata: BuildMetadata(lastEvent)
        );

        var narrationResult = await narrator.NarrateAsync(gameEvent, NarrationTones.Dramatic, cancellationToken);
        return Result<NarrationResult>.Ok(narrationResult);
    }

    private Dictionary<string, string> BuildMetadata(SessionEvent sessionEvent)
    {
        var metadata = new Dictionary<string, string>();

        if (sessionEvent.CheckResult != null)
        {
            metadata["Success"] = sessionEvent.CheckResult.IsSuccess.ToString();
            metadata["Roll"] = sessionEvent.CheckResult.TotalResult.ToString();
        }
        else if (sessionEvent.AttackResult != null)
        {
            metadata["Success"] = sessionEvent.AttackResult.IsHit.ToString();
            metadata["Roll"] = sessionEvent.AttackResult.TotalAttackResult.ToString();
            metadata["Critical"] = sessionEvent.AttackResult.IsCriticalHit.ToString();
        }

        return metadata;
    }
}
