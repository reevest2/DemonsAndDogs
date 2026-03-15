using Models.Interfaces;

namespace API.Services.GameSystems;

public interface IGameSystemRegistry
{
    IRuleBook Get(string systemId);
    IEnumerable<IRuleBook> GetAll();
}
