using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace NTK2024.BatSignalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BatSignalController(IHubContext<BatHub> batHub) : ControllerBase
    {
        public static bool IsOn;

        [SwaggerOperation(
                Summary = "Turns on the bat signa.",
                Description = "Turns on the bat signal."
            )
        ]
        [HttpPost("on", Name = "turn_batsignal_on")]
        public BatSignalState TurnOn()
        {
            IsOn = true;
            batHub.Clients.All.SendAsync("changeState", IsOn);
            return new BatSignalState(IsOn);
        }


        [SwaggerOperation(
                Summary = "Turns off the bat signal off.",
                Description = "Turns off the bat signal off."
            )
        ]
        [HttpPost("off", Name = "turn_batsignal_off")]
        public BatSignalState TurnOff()
        {
            IsOn = false;
            batHub.Clients.All.SendAsync("changeState", IsOn);
            return new BatSignalState(IsOn);
        }

        [SwaggerOperation(
            Summary = "Gets the current bat signal state, that can be on or off.",
            Description = "Gets the current bat signal state, that can be on or off.")]
        [HttpGet(Name = "batsignal_state")]
        public BatSignalState State()
        {
            return new BatSignalState(IsOn);
        }

        public record BatSignalState(bool IsOn);

    }
}
