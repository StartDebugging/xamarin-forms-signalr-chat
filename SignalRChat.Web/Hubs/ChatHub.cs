using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string from, string message)
        {
            await Clients.Others.SendAsync("broadcastMessage", from, message);
        }
    }
}
