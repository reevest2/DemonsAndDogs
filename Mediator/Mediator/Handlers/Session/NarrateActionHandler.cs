using MediatR;
using Mediator.Mediator.Contracts.Session;
using Models.Interfaces;
using Models.Narration;
using Models.Session;
using System.Linq;

namespace Mediator.Mediator.Handlers.Session;

public class NarrateActionHandler(INarrator narrator) : IRequestHandler<NarrateActionRequest, NarrationResult>
{
    public async Task<NarrationResult> Handle(NarrateActionRequest request, CancellationToken cancellationToken)
    {
        if (!SessionStore.Sessions.TryGetValue(request.SessionId, out var session))
        {
            throw new KeyNotFoundException($"Session {request.SessionId} not found.");
        }

        var lastEvent = session.EventLog.LastOrDefault();
        if (lastEvent == null)
        {
            return new NarrationResult("No events to narrate yet.", TokenStream: null);
        }

        // Map SessionEvent to GameEvent for the narrator
        var gameEvent = new GameEvent(
            EventType: lastEvent.EventType,
            Description: lastEvent.Description,
            OccurredAt: lastEvent.Timestamp,
            SubjectId: session.CharacterName,
            Metadata: BuildMetadata(lastEvent)
        );

        return await narrator.NarrateAsync(gameEvent, "dramatic");
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