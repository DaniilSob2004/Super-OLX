using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineClassifieds.Services;
using OnlineClassifieds.ViewModels;

using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IChatRepository _chatRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly CurrentUserProvider _currentUserProvider;

        public MessageController(
            IChatRepository chatRepository,
            IAnnouncementRepository announcementRepository,
            IMessageRepository messageRepository,
            CurrentUserProvider currentUserProvider)
        {
            _chatRepository = chatRepository;
            _announcementRepository = announcementRepository;
            _messageRepository = messageRepository;
            _currentUserProvider = currentUserProvider;
        }


        public async Task<IActionResult> Index(string? activeChatId = null)
        {
            Chat? activeChat = null;
            if (Guid.TryParse(activeChatId, out Guid activeChatGuid))
            {
                activeChat = await _chatRepository.FirstOrDefault(
                    c => c.Id == activeChatGuid,
                    includeProps: "Announcement,Messages,UserOwner,UserBuyer"
                );
                if (activeChat is null) { return NotFound(); }
            }

            string? currUserId = _currentUserProvider.GetCurrentUserId();
            if (currUserId is null) { return NotFound(); }

            // находим все чаты данного пользователя
            var chats = await _chatRepository.GetAll(
                c => c.IdOwner!.Equals(currUserId) || c.IdBuyer!.Equals(currUserId),
                includeProps: "Announcement,Messages,UserOwner,UserBuyer"
            );

            UserChatVM userChatVM = new()
            {
                Chats = chats,
                CurrUserId = currUserId,
                ActiveChat = activeChat ?? (chats.Count() > 0 ? chats.First() : null)  // первый чат активный если не передали в параметр
            };

            return View(userChatVM);
        }

        public async Task<IActionResult> OpenChat(string idAnnouncement)
        {
            if (!Guid.TryParse(idAnnouncement, out Guid announIdGuid)) { return NotFound(); }

            string? currUserId = _currentUserProvider.GetCurrentUserId();
            if (currUserId is null) { return NotFound(); }

            var announcment = await _announcementRepository.Find(announIdGuid);
            if (announcment is null) { return NotFound(); }

            // если ли чат с этим объявлением у данного пользователя
            Chat? chat = await _chatRepository.FirstOrDefault(
                c => c.IdAnnouncement == announIdGuid && currUserId.Equals(c.IdBuyer)
            );
            if (chat is null)  // чата нет
            {
                Chat newChat = new()
                {
                    IdAnnouncement = announIdGuid,
                    IdBuyer = currUserId,
                    IdOwner = announcment.IdUser
                };
                await _chatRepository.Add(newChat);
                await _chatRepository.Save();

                // отправляем для вывода новый активный чат
                return RedirectToAction(nameof(Index), new { activeChatId = newChat.Id });
            }

            return RedirectToAction(nameof(Index), new { activeChatId = chat.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, string chatId)
        {
            if (!Guid.TryParse(chatId, out Guid chatGuid) || message is null) { return NotFound(); }

            string? currUserId = _currentUserProvider.GetCurrentUserId();
            if (currUserId is null) { return NotFound(); }

            Chat? chat = await _chatRepository.FirstOrDefault(
                c => c.Id == chatGuid,
                includeProps: "Announcement,UserOwner,UserBuyer"
            );
            if (chat is null) { return NotFound(); }

            Message mess = new()
            {
                Text = message,
                IdSenderUser = currUserId,
                // если текущий пользователь в чате это владелец, то получатель, это покупатель этого чата, иначе владелец
                IdReceiverUser = currUserId.Equals(chat.IdOwner) ? chat.IdBuyer : chat.IdOwner,
                IdChat = chatGuid
            };
            await _messageRepository.Add(mess);
            await _messageRepository.Save();

            return PartialView("ChatTemplate/_Message", mess);
        }

        [HttpPost]
        public async Task<IActionResult> SwitchChat(string chatId)
        {
            if (!Guid.TryParse(chatId, out Guid chatGuid)) { return NotFound(); }

            Chat? chat = await _chatRepository.FirstOrDefault(
                c => c.Id == chatGuid,
                includeProps: "Announcement,Messages,UserOwner,UserBuyer"
            );
            if (chat is null) { return NotFound(); }

            return PartialView("ChatTemplate/_ChatContent", chat);
        }

        [HttpPost]
        public async Task<IActionResult> ShowNewMessage(string message, string sendUserId, string chatId)
        {
            if (!Guid.TryParse(chatId, out Guid chatGuid)) { return NotFound(); }

            Chat? chat = await _chatRepository.FirstOrDefault(
                c => c.Id == chatGuid,
                includeProps: "Messages"
            );
            if (chat is null) { return NotFound(); }

            // находим последнее смс в чате 'chatId' отправленное пользователем 'sendUserId' и текстом 'message'
            Message? lastMessage = chat.Messages
                .Where(m => m.IdSenderUser!.Equals(sendUserId) && m.Text.Equals(message))
                .OrderByDescending(m => m.SendDt)
                .FirstOrDefault();

            return PartialView("ChatTemplate/_Message", lastMessage);
        }
    }
}
