using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok
{
    public class PustokHub: Hub
    {
        private readonly UserManager<AppUser> _userManager;
        public PustokHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
               // user.LastConnectDate = DateTime.UtcNow.AddHours(4);
                //user.ConnectionId = Context.ConnectionId;

                var result = _userManager.UpdateAsync(user).Result;
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
               // user.LastConnectDate = DateTime.UtcNow.AddHours(4);
               // user.ConnectionId = null;

                var result = _userManager.UpdateAsync(user).Result;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
