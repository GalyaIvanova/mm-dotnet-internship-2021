using System.Collections.Generic;

namespace Scheduler.Models
{
    public class RoomSearchResult
    {
        public string RoomName { get; set; }

        public IEnumerable<RoomSearchAvailableSlotsResult> AvailableSlots { get; set; } = new List<RoomSearchAvailableSlotsResult>();
    }
}
