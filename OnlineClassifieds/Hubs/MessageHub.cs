using Microsoft.AspNetCore.SignalR;

namespace OnlineClassifieds.Hubs
{
    public class MessageHub : Hub
    {
        public async void SendMessage(string chatId, string senderUserId, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", chatId, senderUserId, message);
        }
    }
}
