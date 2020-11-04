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

        public bool inGame;

		[EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > created");
            GameReader.GameStateChanged(e.Game.Code, GameState.LOBBY);
            // inGame = true;
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > destroyed");
            GameReader.GameStateChanged(e.Game.Code, GameState.MENU);
            inGame = false;
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > starting");
            GameReader.GameStateChanged(e.Game.Code, GameState.TASKS);
        }

        /*[EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.TASKS);

            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;

                Console.WriteLine($"- {info.PlayerName} {info.IsImpostor}");
            }
        }*/

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.LOBBY);
            GameReader.JoinedLobby(e.Game.Code);
        }

        /*[EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Player joined a game.");
        }

        [EventListener]
        public void OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Player left a game.");
        }*/

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Joined);

            if (!inGame)
			{
                var playerName = e.PlayerControl.PlayerInfo.PlayerName;
                e.PlayerControl.SetNameAsync("[008080ff]AutometeUs");
                e.PlayerControl.SendChatAsync("We play by [008080ff]Discord with AutometeUs");
                e.PlayerControl.SetNameAsync(playerName);

                inGame = true;
            }

            // Need to make a local copy because it might be possible that
            // the event gets changed after being handled.
            var clientPlayer = e.ClientPlayer;
            var playerControl = e.PlayerControl;

            Task.Run(async () =>
            {
                Console.WriteLine($"Starting player #{clientPlayer.Client.Id} watcher.");

                var lastColor = playerControl.PlayerInfo.ColorId;

                while (clientPlayer.Client.Connection != null &&
                       clientPlayer.Client.Connection.IsConnected)
                {
                    if (e.PlayerControl.PlayerInfo.ColorId != lastColor)
					{
                        GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.ChangedColor);

                        lastColor = e.PlayerControl.PlayerInfo.ColorId;
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(5000));
                }

                Console.WriteLine($"Stopping player #{clientPlayer.Client.Id} watch.");
            });
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Left);
        }

        [EventListener]
        public void OnPlayerExile(IPlayerExileEvent e)
        {
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Died);
        }

        [EventListener]
        public void OnPlayerMurder(IPlayerMurderEvent e)
        {
            AutomuteUsPlugin.Log("PlayerMurder", $"Murder: ({e.PlayerControl.PlayerInfo.PlayerName}); Victim: ({e.Victim.PlayerInfo.PlayerName}); ");
            GameReader.PlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Died);
        }

        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.DISCUSSION);
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.TASKS);
        }

        /*[EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            AutomuteUsPlugin.Log("HELLOW", e.PlayerControl.PlayerInfo.PlayerName + " said " + e.Message);

            if (e.Message == "qq")
            {
                e.Cancel = true;
                // ...
            }
        }*/
    }
}
