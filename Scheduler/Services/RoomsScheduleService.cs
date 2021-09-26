using Scheduler.Models;
using Scheduler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Services
{
    public class RoomsScheduleService : IRoomsScheduleService
    {
        private readonly IRoomsScheduleRepository roomsScheduleRepository;
        private const int meetingDurationStepInMinutes = 15;
        private readonly TimeSpan meetingDurationTimeInMinutes = TimeSpan.FromMinutes(meetingDurationStepInMinutes);

        public RoomsScheduleService(IRoomsScheduleRepository roomsScheduleRepository)
        {
            this.roomsScheduleRepository = roomsScheduleRepository;
        }

        public IEnumerable<RoomSearchAvailableSlotsResult> Search(SearchAvailableRoomsFilter filter)
        {
            IEnumerable<RoomData> rooms = roomsScheduleRepository.GetAllRoomSchedules();
            List<RoomSearchAvailableSlotsResult> searchResults = new List<RoomSearchAvailableSlotsResult>();

            foreach (RoomData room in rooms)
            {
                if (filter.NumberOfParticipants > room.Capacity)
                {
                    continue;
                }

                var availableSlots = ExtractAvailableSlotsForRoom(room, filter.Date);
                availableSlots = availableSlots.Where(slot => slot.From.Add(filter.MeetingDuration) <= slot.To).ToList();

                List<RoomSearchAvailableSlotsResult> updatedSlots = new List<RoomSearchAvailableSlotsResult>();

                foreach (var slot in availableSlots)
                {
                    int numberOfShifts = (int)((slot.Duration - filter.MeetingDuration).TotalMinutes / meetingDurationStepInMinutes);
                    TimeSpan startTime = slot.From;
                    TimeSpan endTime = startTime.Add(filter.MeetingDuration);

                    for (int i = 0; i <= numberOfShifts; i++)
                    {
                        updatedSlots.Add(new RoomSearchAvailableSlotsResult(
                            roomName: room.RoomName,
                            duration: endTime - startTime,
                            from: startTime,
                            to: endTime));

                        startTime = startTime.Add(meetingDurationTimeInMinutes);
                        endTime = startTime.Add(filter.MeetingDuration);
                    }
                }

                if (updatedSlots.Any())
                {
                    searchResults.AddRange(updatedSlots);
                }
            }

            return searchResults;
        }

        public void AddScheduleToRoom(string roomName, DateTime from, DateTime to)
        {
            if (string.IsNullOrWhiteSpace(roomName))
            {
                throw new ArgumentNullException("Room name cannot be empty");
            }

            IEnumerable<RoomData> rooms = roomsScheduleRepository.GetAllRoomSchedules();

            foreach (var room in rooms)
            {
                if (room.RoomName == roomName)
                {
                    // Prevent saving the the schedule if such already exists.
                    if (!room.Schedule.Any(s => s.From.Date == from))
                    {
                        room.Schedule.Add(new RoomScheduleData(from, to));
                        room.Schedule = room.Schedule.OrderBy(s => s.From).ToList();
                    }

                    break;
                }
            }

            roomsScheduleRepository.UpdateRoomsSchedule(rooms);
        }

        private List<RoomSearchAvailableSlotsResult> ExtractAvailableSlotsForRoom(RoomData room, DateTime date)
        {
            List<RoomSearchAvailableSlotsResult> availableSlots = new List<RoomSearchAvailableSlotsResult>();

            var schedulesForDate = room.Schedule.Where(s => s.From.Date == date).ToList();
            if (schedulesForDate.Any())
            {
                availableSlots = CalculateAvailableSlotsForRoom(room, schedulesForDate);
            }
            else
            {
                availableSlots = GenerateEmptySlotsForRoom(room);
            }

            return availableSlots;
        }

        private List<RoomSearchAvailableSlotsResult> CalculateAvailableSlotsForRoom(RoomData room, List<RoomScheduleData> schedulesForDate)
        {
            List<RoomSearchAvailableSlotsResult> availableSlots = new List<RoomSearchAvailableSlotsResult>();

            RoomScheduleData firstSchedule = schedulesForDate[0];
            if (room.AvailableFrom < firstSchedule.From.TimeOfDay)
            {
                availableSlots.Add(new RoomSearchAvailableSlotsResult(
                    roomName: room.RoomName,
                    duration: firstSchedule.From.TimeOfDay - room.AvailableFrom,
                    from: room.AvailableFrom,
                    to: firstSchedule.From.TimeOfDay));
            }

            for (int i = 1; i < schedulesForDate.Count; i++)
            {
                var currentSchedule = schedulesForDate[i];
                var previousSchedule = schedulesForDate[i - 1];
                TimeSpan duration = currentSchedule.From - previousSchedule.To;

                if (duration > TimeSpan.Zero)
                {
                    availableSlots.Add(new RoomSearchAvailableSlotsResult(
                        roomName: room.RoomName,
                        duration: duration,
                        from: previousSchedule.To.TimeOfDay,
                        to: currentSchedule.From.TimeOfDay));
                }
            }

            RoomScheduleData lastSchedule = schedulesForDate[schedulesForDate.Count - 1];
            if (room.AvailableTo > lastSchedule.To.TimeOfDay)
            {
                availableSlots.Add(new RoomSearchAvailableSlotsResult(
                    roomName: room.RoomName,
                    duration: room.AvailableTo - lastSchedule.To.TimeOfDay,
                    from: lastSchedule.To.TimeOfDay,
                    to: room.AvailableTo));
            }

            return availableSlots;
        }

        private List<RoomSearchAvailableSlotsResult> GenerateEmptySlotsForRoom(RoomData room)
        {
            return new List<RoomSearchAvailableSlotsResult>
            {
                new RoomSearchAvailableSlotsResult(room.RoomName, room.AvailableTo - room.AvailableFrom, room.AvailableFrom, room.AvailableTo)
            };
        }
    }
}
