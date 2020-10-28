using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.AutomuteUs.Handlers;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using System.Threading;
using Impostor.Api.Net.Manager;
using System.Linq;

namespace Impostor.Plugins.AutomuteUs
{
    [ImpostorPlugin(
        package: "us.automute",
        name: "Discord AutomuteUs",
        author: "xTCry",
        version: "0.0.1")]
    public class AutomuteUsPlugin : PluginBase
    {
        public const string TAG = "[AutomuteUsPlugin] ";
        public static ILogger<AutomuteUsPlugin> _logger;
        private readonly IEventManager _eventManager;

        public static readonly ClientSocket socket = new ClientSocket();

        public AutomuteUsPlugin(ILogger<AutomuteUsPlugin> logger, IEventManager eventManager, IClientManager clientManager)
        {
            _logger = logger;
            _eventManager = eventManager;

            Log("PluginConfig Host: " + PluginConfig.config.Host);
            Log("Clients: " + clientManager.Clients.ToArray().Length);
        }

        public override async ValueTask EnableAsync()
        {
            Log("Is being enabled.");

            if (PluginConfig.config.ConnectCode == "")
                return;

            // Run socket in background.
            // Important to wait for init to have actually finished before continuing
            await Task.Factory.StartNew(() => socket.Init());
            bool connected = await socket.Connect(PluginConfig.config.Host, PluginConfig.config.ConnectCode);
            if (!connected)
                return;

            _eventManager.RegisterListener(new GameEventListener());
        }

        public override async ValueTask DisableAsync()
        {
            await socket?.DisconnectAsync();
            Log("Is being disabled.");
        }

        public static void Log(string prefix, string str = null)
        {
            if (str == null)
            {
                _logger.LogInformation(TAG + prefix);
                return;
            }
            _logger.LogInformation(TAG + "[" + prefix + "] " + str);
        }
    }
}
