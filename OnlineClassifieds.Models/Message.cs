using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineClassifieds.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string? IdSenderUser { get; set; }
        public string? IdReceiverUser { get; set; }
        public string Text { get; set; } = null!;
        public Guid? IdChat { get; set; }
        public DateTime SendDt { get; set; }
        public DateTime? DeleteDt { get; set; }


        [ForeignKey("IdSenderUser")]
        public virtual User SenderUser { get; set; } = null!;

        [ForeignKey("IdReceiverUser")]
        public virtual User ReceiverUser { get; set; } = null!;

        [ForeignKey("IdChat")]
        public virtual Chat Chat { get; set; } = null!;
    }
}
