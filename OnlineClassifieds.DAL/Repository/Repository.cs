using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.DAL.Data;

namespace OnlineClassifieds.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _db;
        internal DbSet<T> dbSet;

        public Repository(DataContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProps = null,
            bool isTracking = false)
        {
            IQueryable<T> query = dbSet;  // коллекция сущностей 'T'
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (orderBy is not null)
            {
                query = orderBy(query);
            }
            if (includeProps is not null)
            {
                foreach (string property in includeProps
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<T?> FirstOrDefault(
            Expression<Func<T, bool>>? filter = null,
            string? includeProps = null,
            bool isTracking = false)
        {
            IQueryable<T> query = dbSet;  // коллекция сущностей 'T'
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (includeProps is not null)
            {
                foreach (string property in includeProps
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> Find(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        virtual public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        virtual public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        virtual public async Task Remove(Guid id)
        {
            var entity = await Find(id);
            if (entity is not null)
            {
                await Remove(entity);
            }
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
