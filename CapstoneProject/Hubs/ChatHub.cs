using CapstoneProject.Models;
using Microsoft.AspNetCore.SignalR;

namespace CapstoneProject.NewFolder
{
    public class ChatHub : Hub
    {
        private readonly UserAccessLayer _userDAL;

        public ChatHub(UserAccessLayer userDAL)
        {
            _userDAL = userDAL;
        }

        private string GetUsername()
        {
            return Context.GetHttpContext()?.Request.Query["username"].ToString() ?? "Unknown";
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task SendMessage(string conversationId, string message)
        {
            string sender = GetUsername();

            // conversationId is "smallerID_largerID" e.g. "3_7"
            var parts = conversationId.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[0], out int id1) && int.TryParse(parts[1], out int id2))
            {
                int? senderId = _userDAL.GetUserID(sender);
                if (senderId.HasValue)
                {
                    int receiverId = senderId.Value == id1 ? id2 : id1;
                    _userDAL.SaveMessage(senderId.Value, receiverId, message);
                }
            }

            await Clients.Group(conversationId)
                .SendAsync("ReceiveMessage", conversationId, sender, message);
        }
    }
}