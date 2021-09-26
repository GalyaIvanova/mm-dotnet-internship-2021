using System;

namespace Scheduler.Models
{
    public class SearchAvailableRoomsFilter
    {
        public SearchAvailableRoomsFilter(DateTime date, int numberOfParticipants, TimeSpan meetingDuration)
        {
            Date = date;
            NumberOfParticipants = numberOfParticipants;
            MeetingDuration = meetingDuration;
        }

        public DateTime Date { get; }

        public int NumberOfParticipants { get; }

        public TimeSpan MeetingDuration { get; }
    }
}
