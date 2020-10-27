using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Impostor.Plugins.AutomuteUs.Handlers;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.AutomuteUs
{
    [ImpostorPlugin(
        package: "us.automute",
        name: "Discord AutomuteUs",
        author: "xTCry",
        version: "0.0.1")]
    public class AutomuteUsPlugin : PluginBase
    {
        private readonly ILogger<AutomuteUsPlugin> _logger;


        public AutomuteUsPlugin(ILogger<AutomuteUsPlugin> logger, IEventManager eventManager)
        {
            _logger = logger;

            eventManager.RegisterListener(new GameEventListener());
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("AutomuteUs is being enabled.");
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("AutomuteUs is being disabled.");
            return default;
        }
    }
}
