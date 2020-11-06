using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using System;
using System.Threading.Tasks;

namespace Impostor.Plugins.AutomuteUs.Handlers
{
	class GameEventListener: IEventListener
    {
        public const string TAG = "GameEvent";

		/*[EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > created");
            GamesManager.GameStateChanged(e.Game.Code, GameState.LOBBY);
        }*/

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnGameClose(e);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnGameStarting(e);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnGameStarted(e);
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnGameEnded(e);
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            Game game = AutomuteUsPlugin.gamesManager.GetGame(e);
            if (game == null)
            {
                    _ = ChatManager.SendServerMessage(e.PlayerControl, "We play by [add8e6ff]Discord [ffffffff]with [008080ff]AutometeUs");
                if (e.ClientPlayer.IsHost)
                {
                    _ = ChatManager.SendServerMessage(e.PlayerControl, "Type [fefeeffe]/discord [ffffffff]command to create a new game on the Discord server channel...");
                }

                // ...
            }
            else
            {
                game.OnPlayerSpawned(e);
            }
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnPlayerDestroyed(e);
        }

        [EventListener]
        public void OnPlayerExile(IPlayerExileEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnPlayerExile(e);
        }

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnPlayerMurder(e);
        }

        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnMeetingStarted(e);
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent e)
        {
            AutomuteUsPlugin.gamesManager.GetGame(e)?.OnMeetingEnded(e);
        }
    }
}
