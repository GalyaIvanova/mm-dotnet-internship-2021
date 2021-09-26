using Scheduler.Models;
using Scheduler.Repositories;
using Scheduler.Services;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRoomsScheduleService roomsScheduleService;
        private IRoomsScheduleRepository roomsScheduleRepository;
        private DateTime meetingDate;

        public MainWindow()
        {
            InitializeComponent();

            roomsScheduleRepository = new RoomsScheduleRepository();
            roomsScheduleService = new RoomsScheduleService(roomsScheduleRepository);

            DatePicker.SelectedDate = DateTime.Today;
            meetingDate = DatePicker.SelectedDate.Value;

            //IEnumerable<RoomData> rooms = roomsScheduleRepository.GetAllRoomSchedules();

            //if (!rooms.Any())
            //{
            //    MessageBox.Show("No room schedules data source available");
            //    Environment.Exit(0);
            //}
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Typing(object sender, TextChangedEventArgs e)
        {
            TextBox sourceControl = (TextBox)sender;
            Regex regex = new Regex(@"^\d+$");
            bool isInputValid = regex.IsMatch(sourceControl.Text);

            if (!isInputValid)
            {
                sourceControl.Text = string.Empty;
            }

            if (sourceControl.Name == "MeetingDurationHours" || sourceControl.Name == "MeetingDurationMinutes")
            {
                HandleMeetingDurationInputValid();
            }
        }

        private void HandleMeetingDurationInputValid()
        {
            if (!int.TryParse(MeetingDurationHours?.Text, out int meetingDurationHours) ||
                !int.TryParse(MeetingDurationMinutes?.Text, out int meetingDurationMinutes))
            {
                return;
            }

            TimeSpan durationInMinutes = TimeSpan.FromMinutes(meetingDurationHours * 60 + meetingDurationMinutes);

            if (durationInMinutes.TotalHours > 8.0 || meetingDurationMinutes > 59)
            {
                MeetingDurationHours.Text = "00";
                MeetingDurationMinutes.Text = "00";
                return;
            }
        }

        private void Search()
        {
            if (!int.TryParse(NumberOfParticipants.Text, out int numberOfParticipants))
            {
                MessageBox.Show("Please enter a numeric value for number of participants");
                return;
            }

            meetingDate = DatePicker.SelectedDate.Value;

            var roomsAvailability = roomsScheduleService.Search(new SearchAvailableRoomsFilter(meetingDate, numberOfParticipants, new TimeSpan(int.Parse(MeetingDurationHours.Text), int.Parse(MeetingDurationMinutes.Text), 0)));
            ResultsGrid.ItemsSource = roomsAvailability;
        }

        private void SelectRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomSearchAvailableSlotsResult availability = (sender as Button).Tag as RoomSearchAvailableSlotsResult;
            roomsScheduleService.AddScheduleToRoom(availability.RoomName, meetingDate.Add(availability.From), meetingDate.Add(availability.To));

            Search();
        }
    }
}
