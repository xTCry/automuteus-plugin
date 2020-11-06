using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.AutomuteUs.Handlers;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using Impostor.Api.Games.Managers;

namespace Impostor.Plugins.AutomuteUs
{
    [ImpostorPlugin(
        package: "us.automute.impostor",
        name: "Discord AutomuteUs",
        author: "xTCry",
        version: "0.0.1")]
    public class AutomuteUsPlugin : PluginBase
    {
        public const string TAG = "AutomuteUsPlugin";
        private static ILogger<AutomuteUsPlugin> _logger;
        private readonly IEventManager _eventManager;

        public static IGameManager gameManager;
        public static readonly ClientSocket socket = new ClientSocket();
        public static readonly GamesManager gamesManager = new GamesManager();

        public AutomuteUsPlugin(ILogger<AutomuteUsPlugin> logger, IEventManager eventManager, IGameManager _gameManager)
        {
            _logger = logger;
            _eventManager = eventManager;
            gameManager = _gameManager;
        }

        public override async ValueTask EnableAsync()
        {
            Log("Is being enabled.");
            Log($"[PluginConfig] Host: {PluginConfig.config.Host}; SecretKey: {PluginConfig.config.SecretKey}");

            if (PluginConfig.config.SecretKey == "")
            {
                Log("The secret key is missing! Launch aborted.");
                return;
            }

            _eventManager.RegisterListener(new ChatManager());
            _eventManager.RegisterListener(new GameEventListener());

            // Run socket in background.
            // Important to wait for init to have actually finished before continuing
            await Task.Factory.StartNew(() => socket.Init());

            await socket.Connect(PluginConfig.config.Host, PluginConfig.config.SecretKey);
        }

        public override async ValueTask DisableAsync()
        {
            socket?.DisconnectAsync();
            await Task.Delay(500);
            Log("Is being disabled.");
        }

        public static void Log(string prefix, string str = null)
        {
            if (str == null)
            {
                _logger.LogInformation($"[{TAG}] {prefix}");
                return;
            }
            _logger.LogInformation($"[{TAG}] [{prefix}] {str}");
        }
    }
}
