using System.ComponentModel.DataAnnotations;

namespace OnlineClassifieds.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
