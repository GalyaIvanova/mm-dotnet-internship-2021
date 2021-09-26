using System;
using System.Collections.Generic;

namespace Scheduler.Models
{
    public class RoomData
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
        public IList<RoomScheduleData> Schedule { get; set; }
    }
}
