
using System;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Plugins.AutomuteUs.AmongUsCapture;

namespace Impostor.Plugins.AutomuteUs.Handlers
{
    public class MeetingEventListener : IEventListener
    {
        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            GameReader.GameStateChanged(e.Game.Code, GameState.DISCUSSION);
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent e)
        {
            // listening OnGameStarted
        }
    }
}
