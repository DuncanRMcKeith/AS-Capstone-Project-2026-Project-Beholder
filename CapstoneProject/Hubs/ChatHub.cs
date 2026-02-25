using Microsoft.AspNetCore.SignalR;

namespace CapstoneProject.NewFolder
{
    public class ChatHub : Hub
    {
        private string GetUsername()
        {
            return Context.GetHttpContext()?.Request.Query["username"].ToString() ?? "Unknown";
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task SendMessage(int conversationId, string message)
        {
            string sender = GetUsername();
            await Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", conversationId, sender, message);
        }
    }
}
