using System.Linq.Expressions;

namespace OnlineClassifieds.DAL.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProps = null,
                        bool isTracking = false
        );
        Task<T?> FirstOrDefault(
            Expression<Func<T, bool>>? filter = null,
            string? includeProps = null,
            bool isTracking = false
        );
        Task<T?> Find(Guid id);
        Task Add(T entity);
        Task Remove(T entity);
        Task Remove(Guid id);
        Task Save();
    }
}
