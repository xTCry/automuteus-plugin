using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Exceptions;

namespace Impostor.Plugins.AutomuteUs.AmongUsCapture
{
	public class ClientSocket
	{
		private SocketIO socket;
		private string ConnectCode;

		public void Init()
		{
			socket = new SocketIO();

            socket.OnConnected += async (sender, e) =>
			{
				AutomuteUsPlugin.Log("ClientSocket", "Connected successfully! => " + socket.ServerUri.ToString());

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
			try
			{
				AutomuteUsPlugin.Log("ClientSocket", "Try send ConnectCode: " + connectCode);
				ConnectCode = connectCode;
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
				AutomuteUsPlugin.Log("ClientSocket", "Fail! Invalid bot host, not connecting. " + e.Message);
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
			AutomuteUsPlugin.Log("ClientSocket", "Error: " + message);
		}
	}
}
