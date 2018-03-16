using Microsoft.Extensions.Configuration;
using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Services
{
    public class PusherService
    {
        private IConfiguration _configuration;
        private Pusher _pusher;

        public PusherService(
            IConfiguration configuration
        )
        {
            _configuration = configuration;

            _pusher =
                new Pusher(
                    _configuration.GetValue<string>("Data:PusherAppId"),
                    _configuration.GetValue<string>("Data:PusherPublicApiKey"),
                    _configuration.GetValue<string>("Data:PusherPrivateApiKey"),
                    new PusherOptions { Cluster = "eu", Encrypted = true }
                );
        }

        public async Task Send(object msgObj, string callerSocketId = null)
        {
            await _pusher.TriggerAsync(
                _configuration.GetValue<string>("Data:PusherChannelName"),
                _configuration.GetValue<string>("Data:PusherEventName"),
                msgObj,
                new TriggerOptions { SocketId = callerSocketId }
            );
        }
    }
}
