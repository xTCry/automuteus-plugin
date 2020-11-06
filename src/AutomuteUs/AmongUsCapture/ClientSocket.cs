using System;
using System.Threading.Tasks;
using System.Text.Json;
using SocketIOClient;

namespace Impostor.Plugins.AutomuteUs.AmongUsCapture
{
	public class ClientSocket
	{
		private readonly SocketIO socket = new SocketIO();
		private string SecretKey;

		public void Init()
		{
			// Register handlers for game-state change events.
			GamesManager.OnNewGameEvent += NewGameHandler;
			GamesManager.OnEndGameEvent += EndGameHandler;
			GamesManager.OnGameStateChangedEvent += GameStateChangedHandler;
			GamesManager.OnPlayerChangedEvent += PlayerChangedHandler;
			GamesManager.OnJoinedLobbyEvent += JoinedLobbyHandler;

			socket.On("gameAdded", (response) => {
				AutomuteUsPlugin.Log("ClientSocket", $"New game successfully added! => {response.GetValue<string>()}");
				AutomuteUsPlugin.gamesManager.GetGame(response.GetValue<DiscordGameEventArgs>().LobbyCode)?.OnBotConnected();
			});

			socket.OnConnected += async (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", $"Connected successfully! => {socket.ServerUri}");

				await socket.EmitAsync("secretKey", SecretKey);
	
				AutomuteUsPlugin.Log("ClientSocket", $"Connection SecretKey ({SecretKey}) sent to server.");
			};

			// Handle socket disconnection events.
			socket.OnReconnecting += (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", "Reconnecting...");
			};

			// Handle socket disconnection events.
			socket.OnDisconnected += (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", "Lost connection!");

				// TODO: cath this...
			};
		}

		public async ValueTask<bool> Connect(string url, string secretKey)
		{
			this.SecretKey = secretKey;

			try
			{
				socket.ServerUri = new Uri(url);
				socket.Options.AllowedRetryFirstConnection = true;
				socket.Options.ConnectionTimeout = TimeSpan.FromSeconds(60);

				if (socket.Connected) await socket.DisconnectAsync();

				Task t = socket.ConnectAsync();
				await t;

				if (t.IsCompletedSuccessfully)
				{
					return true;
				}

				OnConnectionFailure(t.Exception);
			}
			catch (ArgumentNullException e)
			{
				AutomuteUsPlugin.Log("ClientSocket", "Fail! " + e.Message);
			}
			catch (UriFormatException e)
			{
				AutomuteUsPlugin.Log("ClientSocket", "Fail! " + e.Message);
			}
			catch (Exception e)
			{
				AutomuteUsPlugin.Log("ClientSocket", $"Fail! Invalid bot host, not connecting. {e.Message}");
			}
			return false;
		}

		public async Task DisconnectAsync()
		{
			if (socket.Connected) await socket?.DisconnectAsync();
		}

		private void OnConnectionFailure(AggregateException e = null)
		{
			var message = e != null ? e.Message : "A generic connection error occured.";
			AutomuteUsPlugin.Log("ClientSocket", $"Error: {message}");
		}

		private void Emit(string eventName, params object[] data)
		{
			if (!socket.Connected) return;
			socket.EmitAsync(eventName, data);
		}

		private void NewGameHandler(object sender, DiscordGameEventArgs e)
		{
			Emit("newGame", JsonSerializer.Serialize(e));
			AutomuteUsPlugin.Log("ClientSocket", $"New game by code ({e.LobbyCode}) sent to server.");
		}

		private void EndGameHandler(object sender, DiscordGameEventArgs e)
		{
			Emit("endGame", JsonSerializer.Serialize(e));
			AutomuteUsPlugin.Log("ClientSocket", $"End game by code ({e.LobbyCode}) sent to server.");
		}

		private void GameStateChangedHandler(object sender, GameStateChangedEventArgs e)
		{
			Emit("state", JsonSerializer.Serialize(e.NewState));
		}

		private void PlayerChangedHandler(object sender, PlayerChangedEventArgs e)
		{
			Emit("player", JsonSerializer.Serialize(e));
		}

		private void JoinedLobbyHandler(object sender, LobbyEventArgs e)
		{
			Emit("lobby", JsonSerializer.Serialize(e));
			AutomuteUsPlugin.Log("ClientSocket", $"Room code ({e.LobbyCode}) sent to server.");
		}
	}
}
