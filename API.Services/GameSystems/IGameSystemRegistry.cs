using Models;
using Models.Interfaces;

namespace API.Services.GameSystems;

public interface IGameSystemRegistry
{
    Result<IRuleBook> Get(string systemId);
    IEnumerable<IRuleBook> GetAll();
}
