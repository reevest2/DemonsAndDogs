using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Models.Resources;

namespace DataAccess.Abstraction;

public interface IResourceRepository
{
    public interface IResourceRepository<T> where T : ResourceBase
    {
        DbSet<T> GetDbSet();
        IQueryable<Resource<T>> GetQuery();
        Task<List<T>> GetAllAsync(params Expression<Func<Resource<T>, bool>>[] filters);
        Task<T> GetByIdAsync(string id);
        Task<T> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<T> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<List<T>> GetListByOwnerAsync(string ownerId);
        Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false);
        Task<List<T>> GetBySubjectId(string subjectId);
        Task<T> GetByOwnerIdAndSubjectId(string ownerId, string subjectId);
        Task<T> CreateResourceAsync(T data);
        Task<T> UpdateResourceAsync(T data);
        Task DeleteResourceAsync(T data);
    }
}