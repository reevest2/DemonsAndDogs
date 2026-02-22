using System.Net;
using System.Net.Http.Json;
using Mediator.Mediator.Records;
using MediatR;
using Models.Character;
using Models.Resources;
using Models.Resources.Character;

namespace Mediator.Mediator.Handlers;

public class GetCharacterResourceHandler(HttpClient http)
    : IRequestHandler<GetCharacterResourceQuery, CharacterData?>
{
    public async Task<CharacterData?> Handle(GetCharacterResourceQuery request, CancellationToken ct)
    {
        var res = await http.GetAsync($"api/characterresources/{request.ResourceId}", ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CharacterData>(cancellationToken: ct);
    }
}

public class GetCharacterResourcesHandler(HttpClient http)
    : IRequestHandler<GetCharacterResourcesQuery, List<CharacterData>>
{
    public async Task<List<CharacterData>> Handle(GetCharacterResourcesQuery request, CancellationToken ct)
    {
        return await http.GetFromJsonAsync<List<CharacterData>>("api/characterresources", ct) ?? new();
    }
}

public class CreateCharacterResourceHandler(HttpClient http)
    : IRequestHandler<CreateCharacterResourceCommand, CharacterData>
{
    public async Task<CharacterData> Handle(CreateCharacterResourceCommand request, CancellationToken ct)
    {
        var res = await http.PostAsJsonAsync("api/characterresources", request.Resource, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CharacterData>(cancellationToken: ct))!;
    }
}

public class UpdateCharacterResourceHandler(HttpClient http)
    : IRequestHandler<UpdateCharacterResourceCommand, CharacterData>
{
    public async Task<CharacterData> Handle(UpdateCharacterResourceCommand request, CancellationToken ct)
    {
        var res = await http.PutAsJsonAsync($"api/characterresources/{request.ResourceId}", request.Resource, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CharacterData>(cancellationToken: ct))!;
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

public class GetCharacterTemplateResourceHandler(HttpClient http)
    : IRequestHandler<GetCharacterTemplateResourceQuery, CharacterTemplateData?>
{
    public async Task<CharacterTemplateData?> Handle(GetCharacterTemplateResourceQuery request, CancellationToken ct)
    {
        var res = await http.GetAsync($"api/charactertemplateresources", ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CharacterTemplateData>(cancellationToken: ct);
    }
}

public class GetCharacterTemplateResourcesHandler(HttpClient http)
    : IRequestHandler<GetCharacterTemplateResourcesQuery, List<CharacterTemplateData>>
{
    public async Task<List<CharacterTemplateData>> Handle(GetCharacterTemplateResourcesQuery request, CancellationToken ct)
    {
        return await http.GetFromJsonAsync<List<CharacterTemplateData>>("api/charactertemplateresources", ct) ?? new();
    }
}

public class CreateCharacterTemplateResourceHandler(HttpClient http)
    : IRequestHandler<CreateCharacterTemplateResourceCommand, CharacterTemplateData>
{
    public async Task<CharacterTemplateData> Handle(CreateCharacterTemplateResourceCommand request, CancellationToken ct)
    {
        var res = await http.PostAsJsonAsync("api/charactertemplateresources", request.Resource, ct);

        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"{(int)res.StatusCode} {res.ReasonPhrase} Body={body}");
        }

        return (await res.Content.ReadFromJsonAsync<CharacterTemplateData>(cancellationToken: ct))!;
    }
}

public class UpdateCharacterTemplateResourceHandler(HttpClient http)
    : IRequestHandler<UpdateCharacterTemplateResourceCommand, CharacterTemplateData>
{
    public async Task<CharacterTemplateData> Handle(UpdateCharacterTemplateResourceCommand request, CancellationToken ct)
    {
        var res = await http.PutAsJsonAsync($"api/charactertemplateresources/{request.ResourceId}", request.Resource, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CharacterTemplateData>(cancellationToken: ct))!;
    }
}

public class DeleteCharacterTemplateResourceHandler(HttpClient http)
    : IRequestHandler<DeleteCharacterTemplateResourceCommand>
{
    public async Task Handle(DeleteCharacterTemplateResourceCommand request, CancellationToken ct)
    {
        var res = await http.DeleteAsync($"api/charactertemplateresources/{request.ResourceId}", ct);
        res.EnsureSuccessStatusCode();
    }
}
