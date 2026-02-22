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
        Task<Resource<T>> GetByIdAsync(string id);
        Task<Resource<T>> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<Resource<T>> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<T>, bool>>[] filters);
        Task<List<Resource<T>>> GetListByOwnerAsync(string ownerId);
        Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false);
        Task<List<Resource<T>>> GetBySubjectId(string subjectId);
        Task<Resource<T>> GetByOwnerIdAndSubjectId(string ownerId, string subjectId);
        Task<Resource<T>> CreateResourceAsync(T data, string ownerId, string subjectId, string entityId);
        Task<Resource<T>> UpdateResourceAsync(string id, T data, bool isDeleted = false);
        Task DeleteResourceAsync(string id);
    }
}