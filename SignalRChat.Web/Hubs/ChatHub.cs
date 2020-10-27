using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}
