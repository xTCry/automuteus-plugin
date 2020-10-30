using Impostor.Api.Events;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using System;

namespace Impostor.Plugins.AutomuteUs.Handlers
{
	class GameEventListener: IEventListener
    {
        public const string TAG = "GameEvent";

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > created");
            GameReader.GameStateChanged(e.Game.Code, GameState.LOBBY);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Game > starting");
            GameReader.GameStateChanged(e.Game.Code, GameState.TASKS);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.TASKS);

            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;

                Console.WriteLine($"- {info.PlayerName} {info.IsImpostor}");
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.LOBBY);
            GameReader.JoinedLobby(e.Game.Code);
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.MENU);
            AutomuteUsPlugin.Log(TAG, "Game > destroyed");
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Player joined a game.");
        }

        [EventListener]
        public void OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            AutomuteUsPlugin.Log(TAG, "Player left a game.");
        }
    }
}
