using OnlineClassifieds.Models;

namespace OnlineClassifieds.ViewModels
{
    public class UserChatVM
    {
        public string CurrUserId { get; set; } = null!;
        public IEnumerable<Chat> Chats { get; set; } = null!;
        public Chat? ActiveChat { get; set; } = null!;
    }
}
