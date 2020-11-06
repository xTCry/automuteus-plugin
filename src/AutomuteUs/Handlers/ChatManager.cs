using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Impostor.Plugins.AutomuteUs.Handlers
{
	using ChatAction = Func<string[], IPlayerChatEvent, ValueTask<bool>>;

	public class ChatManager : IEventListener
	{
		public const string CommandPrefix = "/";
		private readonly Dictionary<string, ChatAction> _commands = new Dictionary<string, ChatAction>();

		public ChatManager()
		{
			AddCommand("discord", NewDiscordGame);
		}

		public void AddCommand(string command, ChatAction action)
		{
			_commands.Add(command, action);
		}

		[EventListener]
		public async ValueTask OnPlayerChat(IPlayerChatEvent e)
		{
			var msg = e.Message;
			if (!e.ClientPlayer.IsHost || msg.Length < 3 || !msg.StartsWith(CommandPrefix))
			{
				return;
			}

			msg = msg[CommandPrefix.Length..];

			string[] args = msg.Split(" ");

			if (_commands.TryGetValue(args[0], out var callback))
			{
				if (await callback(args[1..], e))
				{
					await e.Game.SyncSettingsAsync();
				}
			}
		}

		public static async ValueTask SendServerMessage(IInnerPlayerControl control, string message, string prefix = "[add8e6ff]AutometeUs")
		{
			await SendServerMessage(control, new string[] { message }, prefix);
		}

		public static async ValueTask SendServerMessage(IInnerPlayerControl control, string[] messages, string prefix = "[add8e6ff]AutometeUs")
		{
			var name = control.PlayerInfo.PlayerName;
			var colorId = control.PlayerInfo.ColorId;
			await control.SetNameAsync(prefix);
			await control.SetColorAsync(ColorType.Blue);
			foreach (var message in messages)
			{
				await control.SendChatAsync(message);
			}
			await control.SetNameAsync(name);
			await control.SetColorAsync(colorId);
		}

		private static async ValueTask<bool> NewDiscordGame(string[] args, IPlayerChatEvent e)
		{
			if (e.Game.GameState != Api.Innersloth.GameStates.NotStarted) { return false; }
			AutomuteUsPlugin.gamesManager.AddNewGame(e);
			GamesManager.OnNewGame(e.Game.Code);

			return true;
		}
	}
}
