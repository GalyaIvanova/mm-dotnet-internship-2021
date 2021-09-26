using Scheduler.Models;
using System;
using System.Collections.Generic;

namespace Scheduler.Services
{
    public interface IRoomsScheduleService
    {
        IEnumerable<RoomSearchAvailableSlotsResult> Search(SearchAvailableRoomsFilter filter);

        void AddScheduleToRoom(string roomName, DateTime from, DateTime to);
    }
}
