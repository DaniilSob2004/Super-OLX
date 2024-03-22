using OnlineClassifieds.DAL.Data;
using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        private readonly DataContext _db;

        public ChatRepository(DataContext db) : base(db)
        {
            _db = db;
        }
    }
}
