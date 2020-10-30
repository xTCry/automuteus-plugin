using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;

namespace Impostor.Plugins.AutomuteUs.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e) {
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Joined);
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Left);
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            //Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " said " + e.Message);

            if (e.Message == "test")
            {
                // ...
            }
        }
    }
}
