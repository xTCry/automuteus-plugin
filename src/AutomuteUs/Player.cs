using Impostor.Api.Net;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using System;
using System.Threading.Tasks;

namespace Impostor.Plugins.AutomuteUs
{
	public class Player
	{
		private readonly Game Game;
		private readonly IClientPlayer ClientPlayer;
		private bool isWatthing;

		public Player(IClientPlayer player, Game game)
		{
			this.ClientPlayer = player;
			this.Game = game;

			if (Game.BotConnected)
			{
				WatchMe();
			}
		}

		public void WatchMe()
		{
			if (isWatthing) { return; }
			isWatthing = true;

			Task.Run(async () =>
			{
				AutomuteUsPlugin.Log(Game.TAG, $"Started watching player #{ClientPlayer.Client.Id}.");

				var lastColor = ClientPlayer.Character.PlayerInfo.ColorId;

				while (isWatthing && ClientPlayer.Client.Connection != null &&
					   ClientPlayer.Client.Connection.IsConnected)
				{
					if (ClientPlayer.Character.PlayerInfo.ColorId != lastColor)
					{
						GamesManager.OnPlayerChanged(Game.gameCode, ClientPlayer.Character.PlayerInfo, PlayerAction.ChangedColor);

						lastColor = ClientPlayer.Character.PlayerInfo.ColorId;
					}

					await Task.Delay(TimeSpan.FromMilliseconds(5000));
				}

				AutomuteUsPlugin.Log(Game.TAG, $"Stopped watching player #{ClientPlayer.Client.Id}.");
			});
		}
	}
}
