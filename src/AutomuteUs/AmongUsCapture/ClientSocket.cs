using System;
using System.Threading.Tasks;
using System.Text.Json;
using SocketIOClient;

namespace Impostor.Plugins.AutomuteUs.AmongUsCapture
{
	public class ClientSocket
	{
		private SocketIO socket;
		private string ConnectCode;

		public void Init()
		{
			socket = new SocketIO();

			// Register handlers for game-state change events.
			GameReader.OnGameStateChanged += GameStateChangedHandler;
			GameReader.OnPlayerChanged += PlayerChangedHandler;
			GameReader.OnJoinedLobby += JoinedLobbyHandler;

			socket.OnConnected += async (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", $"Connected successfully! => {socket.ServerUri.ToString()}");

				await socket.EmitAsync("connectCode", ConnectCode);
	
				AutomuteUsPlugin.Log("ClientSocket", $"Connection code ({ConnectCode}) sent to server.");
			};

			// Handle socket disconnection events.
			socket.OnDisconnected += (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", "Lost connection!");

				// TODO: cath this...
			};
		}

		/**
		 * TODO: change connectCode to secretKey
		 */
		public async ValueTask<bool> Connect(string url, string connectCode)
		{
			ConnectCode = connectCode;

			try
			{
				socket.ServerUri = new Uri(url);

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
