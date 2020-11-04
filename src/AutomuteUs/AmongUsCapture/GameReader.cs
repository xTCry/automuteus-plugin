using Impostor.Api.Net.Inner.Objects;
using System;

namespace Impostor.Plugins.AutomuteUs.AmongUsCapture
{
	public class GameReader
	{
        public static event EventHandler<GameStateChangedEventArgs> OnGameStateChanged;

        public static event EventHandler<PlayerChangedEventArgs> OnPlayerChanged;

        public static event EventHandler<LobbyEventArgs> OnJoinedLobby;

        public static void GameStateChanged(string code, GameState state)
		{
            OnGameStateChanged.Invoke(null, new GameStateChangedEventArgs { NewState = state });
        }

        public static void JoinedLobby(string code)
		{
            OnJoinedLobby.Invoke(null, new LobbyEventArgs()
            {
                LobbyCode = code,
                Region = PlayRegion.Custom
            });
        }

        public static void PlayerChanged(string code, IInnerPlayerInfo info, PlayerAction action)
        {
            OnPlayerChanged.Invoke(null, new PlayerChangedEventArgs()
            {
                Action = action,
                Name = info.PlayerName,
                IsDead = info.IsDead,
                Disconnected = false,
                Color = (PlayerColor)info.ColorId
            });
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

    public enum PlayerColor
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Pink = 3,
        Orange = 4,
        Yellow = 5,
        Black = 6,
        White = 7,
        Purple = 8,
        Brown = 9,
        Cyan = 10,
        Lime = 11
    }

    public enum PlayRegion
    {
        NorthAmerica = 0,
        Asia = 1,
        Europe = 2,
        Custom = 4
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public GameState NewState { get; set; }
    }

    public class PlayerChangedEventArgs : EventArgs
    {
        public PlayerAction Action { get; set; }
        public string Name { get; set; }
        public bool IsDead { get; set; }
        public bool Disconnected { get; set; }
        public PlayerColor Color { get; set; }
    }

    public class LobbyEventArgs : EventArgs
    {
        public string LobbyCode { get; set; }
        public PlayRegion Region { get; set; }
    }
}
