using Microsoft.AspNetCore.SignalR;
using NTK2024.BatSignalAPI.Controllers;

namespace NTK2024.BatSignalAPI
{
    public class BatHub : Hub
    {
        public async Task GetState()
        {
            await Clients.Caller.SendAsync("changeState", BatSignalController.IsOn);
        }
    }
}
