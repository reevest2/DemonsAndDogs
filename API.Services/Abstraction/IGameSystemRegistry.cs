using Models.Interfaces;

namespace API.Services.Abstraction;

public interface IGameSystemRegistry
{
    IRuleBook Get(string systemId);
    IEnumerable<IRuleBook> GetAll();
}
