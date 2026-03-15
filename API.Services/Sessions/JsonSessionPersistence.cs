using System.Text.Json;
using DataAccess.Abstraction;
using Models.Common;
using Models.Interfaces;
using Models.Session;

namespace API.Services.Sessions;

public class JsonSessionPersistence(IJsonResourceRepository repository) : ISessionPersistence
{
    public async Task SaveAsync(SessionState state, CancellationToken ct)
    {
        var data = JsonSerializer.SerializeToElement(state);

        var existing = (await repository.QueryAsync(q =>
            q.Where(r => r.EntityId == state.SessionId))).FirstOrDefault();

        if (existing != null)
        {
            var result = await repository.UpdateAsync(existing with { Data = data });
            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to persist session {state.SessionId}: {result.Error!.Message}");
        }
        else
        {
            await repository.CreateAsync(new SessionResource
            {
                EntityId = state.SessionId,
                Data = data
            });
        }
    }

    public async Task<SessionState?> LoadAsync(string sessionId, CancellationToken ct)
    {
        var results = await repository.QueryAsync(q =>
            q.Where(r => r.EntityId == sessionId));

        var resource = results.FirstOrDefault();
        if (resource == null) return null;

        return resource.Data.Deserialize<SessionState>();
    }
}
