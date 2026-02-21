using System.Net;
using System.Net.Http.Json;
using Mediator.Mediator.Records;
using MediatR;
using Models.Character;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class GetCharacterResourceHandler(HttpClient http)
    : IRequestHandler<GetCharacterResourceQuery, CharacterResource?>
{
    public async Task<CharacterResource?> Handle(GetCharacterResourceQuery request, CancellationToken ct)
    {
        var res = await http.GetAsync($"api/characterresources/{request.ResourceId}", ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CharacterResource>(cancellationToken: ct);
    }
}

public class GetCharacterResourcesHandler(HttpClient http)
    : IRequestHandler<GetCharacterResourcesQuery, List<CharacterResource>>
{
    public async Task<List<CharacterResource>> Handle(GetCharacterResourcesQuery request, CancellationToken ct)
    {
        return await http.GetFromJsonAsync<List<CharacterResource>>("api/characterresources", ct) ?? new();
    }
}

public class CreateCharacterResourceHandler(HttpClient http)
    : IRequestHandler<CreateCharacterResourceCommand, CharacterResource>
{
    public async Task<CharacterResource> Handle(CreateCharacterResourceCommand request, CancellationToken ct)
    {
        var res = await http.PostAsJsonAsync("api/characterresources", request.Resource, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CharacterResource>(cancellationToken: ct))!;
    }
}

public class UpdateCharacterResourceHandler(HttpClient http)
    : IRequestHandler<UpdateCharacterResourceCommand, CharacterResource>
{
    public async Task<CharacterResource> Handle(UpdateCharacterResourceCommand request, CancellationToken ct)
    {
        var res = await http.PutAsJsonAsync($"api/characterresources/{request.ResourceId}", request.Resource, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CharacterResource>(cancellationToken: ct))!;
    }
}

public class DeleteCharacterResourceHandler(HttpClient http)
    : IRequestHandler<DeleteCharacterResourceCommand>
{
    public async Task Handle(DeleteCharacterResourceCommand request, CancellationToken ct)
    {
        var res = await http.DeleteAsync($"api/characterresources/{request.ResourceId}", ct);
        res.EnsureSuccessStatusCode();
    }
}
