using Newtonsoft.Json;
using Scheduler.Models;
using System.Collections.Generic;
using System.IO;

namespace Scheduler.Repositories
{
    public class RoomsScheduleRepository : IRoomsScheduleRepository
    {
        private string roomsScheduleFilePath = Path.Combine(Directory.GetCurrentDirectory(), "RoomsSchedule.json");

        public IEnumerable<RoomData> GetAllRoomSchedules()
        {
            if (!File.Exists(roomsScheduleFilePath))
            {
                return new RoomData[0];
            }

            string roomsScheduleJson = File.ReadAllText(roomsScheduleFilePath);
            IEnumerable<RoomData> roomsData = JsonConvert.DeserializeObject<IEnumerable<RoomData>>(roomsScheduleJson);
            return roomsData;
        }

        public void UpdateRoomsSchedule(IEnumerable<RoomData> rooms)
        {
            string roomsScheduleJson = JsonConvert.SerializeObject(rooms);
            File.WriteAllText(roomsScheduleFilePath, roomsScheduleJson);
        }
    }
}
