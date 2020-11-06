﻿using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using Impostor.Plugins.AutomuteUs.Handlers;
using System;
using System.Collections.Concurrent;

namespace Impostor.Plugins.AutomuteUs
{
	public class Game
	{
		private readonly ConcurrentDictionary<int, Player> _players = new ConcurrentDictionary<int, Player>();

		public string TAG;
		public GameCode gameCode;
		public bool BotConnected;
		public bool needForceUpdate;

		public Game(GameCode gameCode)
		{
			this.gameCode = gameCode;
			TAG = gameCode;
		}

		public void OnBotConnected()
		{
			BotConnected = true;
			needForceUpdate = true;
		}

		public void CheckUpdate(IGameEvent e)
		{
			if (!needForceUpdate) { return; }
			needForceUpdate = false;

			// TODO: update all info from e.Game.Players or _players
			foreach(var player in _players)
			{
				player.Value.WatchMe();
			}
		}

		public void OnGameEnded(IGameEndedEvent e)
		{
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.LOBBY);
			GamesManager.OnJoinedLobby(e.Game.Code);

			CheckUpdate(e);
		}

		public void OnGameClose(IGameDestroyedEvent e)
		{
			AutomuteUsPlugin.Log(TAG, "Game > destroyed");

			BotConnected = false;
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.MENU);
			AutomuteUsPlugin.gamesManager.RemoveGame(e);
		}

		public void OnGameStarting(IGameStartingEvent e)
		{
			AutomuteUsPlugin.Log(TAG, "Game > starting");
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.TASKS);

			CheckUpdate(e);
		}

		public void OnGameStarted(IGameStartedEvent e)
		{
			AutomuteUsPlugin.Log(TAG, "Game > started");
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.TASKS);

			CheckUpdate(e);
		}

		public void OnPlayerSpawned(IPlayerSpawnedEvent e)
		{
			GamesManager.OnPlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Joined);
			_players.TryAdd(e.ClientPlayer.Client.Id, new Player(e.ClientPlayer, this));

			if (BotConnected)
			{
				_ = ChatManager.SendServerMessage(e.PlayerControl, $"Hi, {e.PlayerControl.PlayerInfo.PlayerName}, " +
					$"we're play by [add8e6ff]Discord [ffffffff]with [008080ff]AutometeUs!");
			}

			CheckUpdate(e);
		}

		public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
		{
			GamesManager.OnPlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Left);
			_players.TryRemove(e.ClientPlayer.Client.Id, out _);

			CheckUpdate(e);
		}

		public void OnPlayerExile(IPlayerExileEvent e)
		{
			GamesManager.OnPlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Died);

			CheckUpdate(e);
		}

		public void OnPlayerMurder(IPlayerMurderEvent e)
		{
			AutomuteUsPlugin.Log("PlayerMurder", $"Murder: ({e.PlayerControl.PlayerInfo.PlayerName}); Victim: ({e.Victim.PlayerInfo.PlayerName}); ");
			GamesManager.OnPlayerChanged(e.Game.Code, e.PlayerControl.PlayerInfo, PlayerAction.Died);

			CheckUpdate(e);
		}

		public void OnMeetingStarted(IMeetingStartedEvent e)
		{
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.DISCUSSION);

			CheckUpdate(e);
		}

		public void OnMeetingEnded(IMeetingEndedEvent e)
		{
			GamesManager.OnGameStateChanged(e.Game.Code, GameState.TASKS);

			CheckUpdate(e);
		}
	}
}
