using OnlineClassifieds.DAL.Data;
using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly DataContext _db;

        public CategoryRepository(DataContext db) : base(db)
        {
            _db = db;
        }
    }
}
