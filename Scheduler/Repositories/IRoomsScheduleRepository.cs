using Scheduler.Models;
using System.Collections.Generic;

namespace Scheduler.Repositories
{
    public interface IRoomsScheduleRepository
    {
        IEnumerable<RoomData> GetAllRoomSchedules();

        void UpdateRoomsSchedule(IEnumerable<RoomData> rooms);
    }
}
