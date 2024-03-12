using OnlineClassifieds.DAL.Data;
using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly DataContext _db;

        public MessageRepository(DataContext db) : base(db)
        {
            _db = db;
        }

        public override async Task Remove(Message entity)
        {
            entity.DeleteDt = DateTime.Now;
            await base.Save();
        }

        public override async Task Remove(Guid id)
        {
            var announcement = await base.FirstOrDefault(a => a.Id == id);
            if (announcement is not null)
            {
                await Remove(announcement);
            }
        }
    }
}
