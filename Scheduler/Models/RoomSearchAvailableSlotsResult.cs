using System;

namespace Scheduler.Models
{
    public class RoomSearchAvailableSlotsResult
    {
        public RoomSearchAvailableSlotsResult(string roomName, TimeSpan duration, TimeSpan from, TimeSpan to)
        {
            RoomName = roomName;
            Duration = duration;
            From = from;
            To = to;
        }

        public string RoomName { get; }
        public TimeSpan Duration { get; }
        public TimeSpan From { get; }
        public TimeSpan To { get; }

        public string AvailabilityDisplayText => $"{From.ToString("hh\\:mm")} - {To.ToString("hh\\:mm")}";
    }
}
