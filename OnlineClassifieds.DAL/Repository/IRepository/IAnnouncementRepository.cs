using Microsoft.AspNetCore.Mvc.Rendering;

using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository.IRepository
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        void Update(Announcement announcement, Announcement oldAnnouncement);
        Task Deactivate(Guid id);
        Task Activate(Guid id);
        string GetImage(Announcement announcement);
        SelectList? GetAllDropDownList(string obj);
    }
}
