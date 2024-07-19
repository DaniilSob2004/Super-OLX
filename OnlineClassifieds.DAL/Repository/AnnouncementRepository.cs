using Microsoft.AspNetCore.Mvc.Rendering;

using OnlineClassifieds.DAL.Data;
using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        private readonly DataContext _db;

        public AnnouncementRepository(DataContext db) : base(db)
        {
            _db = db;
        }

        public override async Task Add(Announcement entity)
        {
            entity.IsActive = true;
            entity.CreateDt = DateTime.Now;
            await base.Add(entity);
        }

        public override async Task Remove(Announcement entity)
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

        public void Update(Announcement announcement, Announcement oldAnnouncement)
        {
            announcement.Title = oldAnnouncement.Title;
            announcement.IdCat = oldAnnouncement.IdCat;
            announcement.Price = oldAnnouncement.Price;
            announcement.Description = oldAnnouncement.Description;
            announcement.City = oldAnnouncement.City;
            announcement.Image = oldAnnouncement.Image;
            _db.Update(announcement);
        }

        public async Task Deactivate(Guid id)
        {
            var announ = await FirstOrDefault(a => a.Id.Equals(id));
            if (announ is not null)
            {
                announ.IsActive = false;
                _db.Update(announ);
            }
        }

        public async Task Activate(Guid id)
        {
            var announ = await FirstOrDefault(a => a.Id.Equals(id));
            if (announ is not null)
            {
                announ.IsActive = true;
                _db.Update(announ);
            }
        }

        public string GetImage(Announcement announcement)
        {
            return (announcement.Image is not null) ?
                announcement.Image :
                "default_announcement.png";
        }

        public SelectList? GetAllDropDownList(string obj)
        {
            if (obj.Equals(WC.DropDownListCategory))
            {
                return new SelectList(_db.Categories, "Id", "Name");
            }
            return null;
        }
    }
}
