using System;

namespace Scheduler.Models
{
    public class RoomScheduleData
    {
        public RoomScheduleData()
        {
        }

        public RoomScheduleData(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
