using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace web_luat.Models
{
    public class AuthHub : Hub
    {
        // userId -> connectionId
        public static ConcurrentDictionary<int, string> UserConnections
            = new ConcurrentDictionary<int, string>();

        public void Register(int userId)
        {
            UserConnections.AddOrUpdate(userId, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (item.Key != 0)
            {
                UserConnections.TryRemove(item.Key, out _);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}