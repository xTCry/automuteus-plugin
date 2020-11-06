using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner.Objects;
using System;
using System.Collections.Concurrent;

namespace Impostor.Plugins.AutomuteUs.AmongUsCapture
{
    public class GamesManager
    {
        //private static GamesManager instance;

        private readonly ConcurrentDictionary<GameCode, Game> _games = new ConcurrentDictionary<GameCode, Game>();

        public const int RegionImpostorServer = 10;
        public static event EventHandler<DiscordGameEventArgs> OnNewGameEvent;
        public static event EventHandler<DiscordGameEventArgs> OnEndGameEvent;
        public static event EventHandler<GameStateChangedEventArgs> OnGameStateChangedEvent;
        public static event EventHandler<PlayerChangedEventArgs> OnPlayerChangedEvent;
        public static event EventHandler<LobbyEventArgs> OnJoinedLobbyEvent;

        /*public static GamesManager GetInstance()
        {
            if (instance == null)
            {
                instance = new GamesManager();
            }
            return instance;
        }*/

        public static void OnGameStateChanged(string code, GameState state)
        {
            OnGameStateChangedEvent.Invoke(null, new GameStateChangedEventArgs
            {
                LobbyCode = code,
                NewState = state
            });
        }

        public static void OnNewGame(string code)
        {
            OnNewGameEvent.Invoke(null, new DiscordGameEventArgs()
            {
                LobbyCode = code,
            });
        }

        public static void OnEndGame(string code)
        {
            OnEndGameEvent.Invoke(null, new DiscordGameEventArgs()
            {
                LobbyCode = code,
            });
        }

        public static void OnJoinedLobby(string code)
        {
            OnJoinedLobbyEvent.Invoke(null, new LobbyEventArgs()
            {
                LobbyCode = code,
                Region = RegionImpostorServer,
            });
        }

        public static void OnPlayerChanged(string code, IInnerPlayerInfo info, PlayerAction action)
        {
            OnPlayerChangedEvent.Invoke(null, new PlayerChangedEventArgs()
            {
                LobbyCode = code,
                Action = action,
                Name = info.PlayerName,
                IsDead = info.IsDead,
                Disconnected = false,
                Color = (ColorType)info.ColorId
            });
        }

        public bool AddNewGame(IGameEvent e)
        {
            return _games.TryAdd(e.Game.Code, new Game(e.Game.Code));
        }

        public Game GetGame(IGameEvent e)
        {
            _games.TryGetValue(e.Game.Code, out var game);
            return game;
        }

        public Game GetGame(string code)
        {
            _games.TryGetValue(GameCode.From(code), out var game);
            return game;
        }

        public bool HasGame(IGameEvent e)
        {
            return _games.ContainsKey(e.Game.Code);
        }

        public bool HasGame(string code)
        {
            return _games.ContainsKey(GameCode.From(code));
        }

        public bool RemoveGame(IGameEvent e)
        {
            return _games.TryRemove(e.Game.Code, out _);
        }

        public bool RemoveGame(string code)
        {
            return _games.TryRemove(GameCode.From(code), out _);
        }
    }

    public enum GameState
    {
        LOBBY,
        TASKS,
        DISCUSSION,
        MENU, // ! it can't be
        UNKNOWN
    }

    public enum PlayerAction
    {
        Joined,
        Left,
        Died,
        ChangedColor,
        ForceUpdated,
        Disconnected,
        Exiled
    }

    public class DiscordGameEventArgs : EventArgs
    {
        public string LobbyCode { get; set; }
    }

    public class GameStateChangedEventArgs : DiscordGameEventArgs
    {
        public GameState NewState { get; set; }
    }

    public class PlayerChangedEventArgs : DiscordGameEventArgs
    {
        public PlayerAction Action { get; set; }
        public string Name { get; set; }
        public bool IsDead { get; set; }
        public bool Disconnected { get; set; }
        public ColorType Color { get; set; }
    }

    public class LobbyEventArgs : DiscordGameEventArgs
    {
        public int Region { get; set; }
    }
}
