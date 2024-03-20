using OnlineClassifieds.Helpers;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.ViewModels
{
    public class AnnouncementVM
    {
        public IEnumerable<Announcement> Announcements { get; set; } = null!;
        public Pager Pager { get; set; } = null!;
    }
}
