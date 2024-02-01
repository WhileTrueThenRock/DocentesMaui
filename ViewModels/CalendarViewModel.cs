using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Views.Popups;
using GestorChat.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using Plugin.Maui.Calendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class CalendarViewModel : ObservableObject
    {
        [ObservableProperty]
        private CultureInfo _culture;

        [ObservableProperty]
        private EventCollection _events = new();

        [ObservableProperty]
        private ObservableCollection<EventModel> eventsList = new();

        [ObservableProperty]
        private EventModel selectedEvent;

        [ObservableProperty]
        private DayEventsModel dayEvents = new();

        [ObservableProperty]
        private CalendarPopup calendarPopup;

        [ObservableProperty]
        private string mode;

        public CalendarViewModel() 
        {
            Culture = CultureInfo.CurrentCulture;
            SelectedEvent = new EventModel();
            GetEvents();
        }

        [RelayCommand]
        public async Task GetEvents()
        {
            Events.Clear();
            var request = new RequestModel(method: "GET",
                                           route: "/events/getEvents",
                                           data: string.Empty,
                                           server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                ObservableCollection<DayEventsModel> eventList =
                JsonConvert.DeserializeObject<ObservableCollection<DayEventsModel>>
                    (response.Data.ToString());
                foreach (DayEventsModel dem in eventList)
                {
                    if (dem.Events.Count > 0)
                    {
                        Events.Add(DateTime.Parse(dem.EventDate), dem.Events);
                    }
                }
            }
        }

        [RelayCommand]
        public async Task ExecuteRequest()
        {
            DayEvents.Events.Clear();
            DayEvents.Events.Add(SelectedEvent); //selectedEvent es el popup
            string[] dateArray = DayEvents.EventDate.Split(' ');
            DayEvents.EventDate = dateArray[0];
            var request = new RequestModel(method: "POST",
                                          route: "/events/" + Mode,
                                          data: DayEvents,
                                          server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                App.Current.MainPage.DisplayAlert("Eventos", response.Message, "Aceptar");
                GetEvents();
                ClosePopUp();
            }
        }

        [RelayCommand]
        public async Task DeleteEvent()
        {
            Mode = "delete";
            ExecuteRequest();
            ClosePopUp();
        }


        [RelayCommand]
        public async Task ShowEventPopup(string mode)
        {
            Mode = mode;
            if (Mode.Equals("create"))
            {
                DayEvents.Events.Clear();
                DayEvents.Id = ObjectId.GenerateNewId().ToString();
                SelectedEvent = new EventModel();
            }
            CalendarPopup = new CalendarPopup();
            await App.Current.MainPage.ShowPopupAsync(CalendarPopup);
        }

        [RelayCommand]
        public async Task ClosePopUp()
        {
            CalendarPopup.Close();

        }

    }
}
