using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Models.Resources.Abstract;

namespace DataAccess.Abstraction;

public interface IResourceRepository
{
    public interface IResourceRepository<T>
    {
        DbSet<Resource<T>> GetDbSet();
        IQueryable<Resource<T>> GetQuery();
        Task<List<Resource<T>>> GetAllAsync(params Expression<Func<Resource<T>, bool>>[] filters);
        Task<T> GetByIdAsync(string id);
        Task<T> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<T> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<List<T>> GetListByOwnerAsync(string ownerId);
        Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false);
        Task<List<T>> GetBySubjectId(string subjectId);
        Task<T> GetByOwnerIdAndSubjectId(string ownerId, string subjectId);
        Task<T> CreateResourceAsync(T data, string ownerId, string subjectId, string entityId);
        Task<T> UpdateResourceAsync(string id, T data, bool isDeleted = false);
        Task DeleteResourceAsync(string id);
    }
}