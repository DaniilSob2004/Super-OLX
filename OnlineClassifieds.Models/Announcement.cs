using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace OnlineClassifieds.Models
{
    public class Announcement
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Category")]
        [Required(ErrorMessage = "Required category")]
        public Guid? IdCat { get; set; }

        public string? IdUser { get; set; } = null!;

        [DisplayName("Title")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Title must be from {1} to {2}")]
        public string Title { get; set; } = null!;

        [DisplayName("Price")]
        [Range(1, 1000000, ErrorMessage = "Price must be from {1} to {2}")]
        public float Price { get; set; }

        [DisplayName("Description")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Description must be from {1} to {2}")]
        public string Description { get; set; } = null!;

        [DisplayName("Image")]
        public string? Image { get; set; } = null!;

        [DisplayName("City")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "City must be from {1} to {2}")]
        public string City { get; set; } = null!;

        public bool IsActive { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }


        [ForeignKey("IdCat")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("IdUser")]
        public virtual User User { get; set; } = null!;

        [InverseProperty("Announcement")]
        public ICollection<Chat> Chats { get; set; } = null!;
    }
}
