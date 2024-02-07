using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using EFDocenteMAUI.Views.Popups;
using MongoDB.Bson;
using Newtonsoft.Json;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class CalendarViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserModel _user;

        [ObservableProperty]
        private CultureInfo _culture;

        [ObservableProperty]
        private EventCollection _events = new();

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsList = new();

        [ObservableProperty]
        private EventModel _selectedEvent;

        [ObservableProperty]
        private DayEventsModel _dayEvents = new();

        [ObservableProperty]
        private CalendarPopup _calendarPopup;

        [ObservableProperty]
        private string _mode;

        [ObservableProperty]
        private string avatarImage64;

        [ObservableProperty]
        private ImageSource avatarImage;

        [ObservableProperty]
        private string _eventHeader;

        [ObservableProperty]
        private bool _eventExpander;

        [ObservableProperty]
        private string _resultadoFecha;

        [ObservableProperty]
        private DateTime _fechaIni;

        [ObservableProperty]
        private DateTime _fechaFin;


        public CalendarViewModel()
        {
            User = new UserModel();
            Culture = CultureInfo.CurrentCulture;
            SelectedEvent = new EventModel();
            GetEvents();
            EventHeader = "Actividades de clase";
            FechaIni = new DateTime(2023, 1, 1);
            FechaFin = DateTime.Now;
            // ResultadoFecha = $"Años filtrados: {AnioMenor} - {AnioMayor}";
        }


        [RelayCommand]
        public async Task SelectedTypeEvent(string tipoEvento)
        {

            if (tipoEvento == "actividades")
            {
                EventHeader = "Actividades de clase";

            }

            else if (tipoEvento == "trabajos")
            {
                EventHeader = "Entrega de trabajos";
            }

            else if (tipoEvento == "examenes")
            {
                EventHeader = "Exámenes";
            }

            else if (tipoEvento == "vacaciones")
            {
                EventHeader = "Vacaciones";
            }
            EventExpander = false;
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
            bool okSaveImage = await UpdateImage();
            if (okSaveImage)
            {
                DayEvents.Events.Clear();
                DayEvents.Events.Add(SelectedEvent); //selectedEvent es el popup
                string[] dateArray = DayEvents.EventDate.Split(' ');
                DayEvents.EventDate = dateArray[0];
                User.Avatar = APIService.ImagenesServerUrl + "/images/" + User.Id.ToString();
                SelectedEvent.Type = EventHeader;
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

        }

        [RelayCommand]
        public async Task CreateEvent()
        {
            Mode = "create";
            ExecuteRequest();
        }

        [RelayCommand]
        public async Task DeleteEvent()
        {
            Mode = "delete";
            ExecuteRequest();
            ClosePopUp();
        }


        [RelayCommand]
        public async Task LoadImage()
        {
            //cambiar por isEnabled observable property
            var imagesDict = await ImageUtils.OpenImage();
            if (imagesDict != null)
            {
                AvatarImage = (ImageSource)imagesDict["imageFromStream"];
                AvatarImage64 = (string)imagesDict["imageBase64"];
            }
        }

        public async Task<bool> UpdateImage()
        {
            ImageModel imagen = new ImageModel();
            imagen.Id = SelectedEvent.Id.ToString(); //Linea modificada
            imagen.Content = AvatarImage64;
            var request = new RequestModel(method: "POST", route: "/images/save", data: imagen, server: APIService.ImagenesServerUrl);


            ResponseModel response = await APIService.ExecuteRequest(request);

            await App.Current.MainPage.DisplayAlert("Actualizar", response.Message, "Aceptar");
            return response.Success == 0;
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
