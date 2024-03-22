using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineClassifieds.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? IdAnnouncement { get; set; }
        public string? IdOwner { get; set; }
        public string? IdBuyer { get; set; }


        [ForeignKey("IdAnnouncement")]
        public virtual Announcement Announcement { get; set; } = null!;

        [InverseProperty("Chat")]
        public ICollection<Message> Messages { get; set; } = null!;

        [ForeignKey("IdOwner")]
        public virtual User UserOwner { get; set; } = null!;

        [ForeignKey("IdBuyer")]
        public virtual User UserBuyer { get; set; } = null!;
    }
}
