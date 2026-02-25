using Microsoft.AspNetCore.SignalR;

namespace CapstoneProject.NewFolder
{
    public class ChatHub : Hub
    {
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task SendMessage(int conversationId, string sender, string message)
        {
            try
            {
                await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", conversationId, sender, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendMessage error: " + ex.Message);
                throw;
            }
        }
    }
}
