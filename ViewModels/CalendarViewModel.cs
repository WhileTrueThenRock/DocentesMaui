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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace EFDocenteMAUI.ViewModels
{
    [QueryProperty("User", "User")]
    internal partial class CalendarViewModel : ObservableObject, INotifyPropertyChanged
    {

        private UserModel _user;
        public UserModel User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private CultureInfo _culture;

        [ObservableProperty]
        private EventCollection _events = new();

        private ObservableCollection<EventModel> _eventsList;
        public ObservableCollection<EventModel> EventsList
        {
            get
            {
                return _eventsList;
            }
            set
            {
                _eventsList = value;
                OnPropertyChanged();
            }

        }

        private EventModel _selectedEvent;
        public EventModel SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
            }
        }

        private string _eventHeader;
        public string EventHeader
        {
            get { return _eventHeader; }
            set
            {
                _eventHeader = value; OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsDateFilter = new ();

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsTypeFilter = new();

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsDescriptionFilter = new();



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
        private bool _eventExpander;

        [ObservableProperty]
        private string _resultadoFecha;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _fechaIni;

        public string FechaIni
        {
            set
            {
                _fechaIni = value.Replace(" 0:00:00", "");
                OnPropertyChanged();
            }
            get
            {
                return _fechaIni;
            }
        }

        private string _fechaFin;

        public string FechaFin
        {
            set
            {
                _fechaFin = value.Replace(" 0:00:00", "");
                OnPropertyChanged();
            }
            get
            {
                return _fechaFin;
            }
        }

        [ObservableProperty]
        private bool _isCreateVisible;

        [ObservableProperty]
        private bool _isUpdateVisible;

        [ObservableProperty]
        private bool _isDeleteVisible;

        [ObservableProperty]
        private string _headerTabName;

        [ObservableProperty]
        private string _filtro;

        public CalendarViewModel()
        {

            Culture = CultureInfo.CurrentCulture;
            EventsList = new ObservableCollection<EventModel>();
            SelectedEvent = new EventModel();
            GetEvents();
            EventHeader = "Actividades de clase";
            FechaIni = new DateTime(2023, 1, 1).ToString("dd-MM-yyyy");
            FechaFin = DateTime.Now.ToString("dd-MM-yyyy");

        }


        [RelayCommand]
        public async Task GetUsersByFiltroType(string type)
        {

            if (null == type || type.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Info", "El campo de busqueda no puede estar vacio", "ACEPTAR");
            }
            else
            {

                var request = new RequestModel(method: "GET",
                                                route: "/events/getEventsByFilter/" + type.ToLower(),
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);
                if (response.Success == 0)
                {
                    ObservableCollection<DayEventsModel> eventsList =
                    JsonConvert.DeserializeObject<ObservableCollection<DayEventsModel>>
                        (response.Data.ToString());
                    EventsTypeFilter.Clear();
                    foreach (DayEventsModel dem in eventsList)
                    {
                        if (dem.Events.Count > 0)
                        {
                            foreach (var eventModel in dem.Events)
                            {
                                EventsTypeFilter.Add(eventModel);
                            }
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No se han encontrado resultados", "ACEPTAR");
                }
            }
        }

        [RelayCommand]
        public async Task GetUsersByFiltroDescription(string descripcion)
        {

            if (null == descripcion || descripcion.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Info", "El campo de busqueda no puede estar vacio", "ACEPTAR");
            }
            else
            {

                var request = new RequestModel(method: "GET",
                                                route: "/events/getEventsByDescription/" + descripcion.ToLower(),
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);
                if (response.Success == 0)
                {
                    ObservableCollection<DayEventsModel> eventsList =
                    JsonConvert.DeserializeObject<ObservableCollection<DayEventsModel>>
                        (response.Data.ToString());
                    EventsDescriptionFilter.Clear();
                    foreach (DayEventsModel dem in eventsList)
                    {
                        if (dem.Events.Count > 0)
                        {
                            foreach (var eventModel in dem.Events)
                            {
                                EventsDescriptionFilter.Add(eventModel);
                            }
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No se han encontrado resultados", "ACEPTAR");
                }
            }
        }


        [RelayCommand]
        public async Task FiltrarFecha()
        {
            if (!DateTime.TryParse(FechaIni, out DateTime fechaInicio) ||
         !DateTime.TryParse(FechaFin, out DateTime fechaFin))
            {
                return;
            }

            string[] dateArray = FechaIni.Split(' ');
            FechaIni = dateArray[0];

            string[] dateArray2 = FechaFin.Split(' ');
            FechaFin = dateArray2[0];

            FechaIni = FechaIni.Replace("/", "-");
            FechaFin = FechaFin.Replace("/", "-");
            var request = new RequestModel(method: "GET",
                                           route: "/events/getEventsByDate/" + FechaIni + "/" + FechaFin, //No filtra si es otro mes
                                           data: string.Empty,
                                           server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);

            if (response.Success == 0)
            {
                ObservableCollection<DayEventsModel> eventsList =
                JsonConvert.DeserializeObject<ObservableCollection<DayEventsModel>>
                    (response.Data.ToString());
                EventsDateFilter.Clear();
                foreach (DayEventsModel dem in eventsList)
                {
                    if (dem.Events.Count > 0)
                    {
                        //Events.Add(DateTime.Parse(dem.EventDate), dem.Events);

                        foreach (var eventModel in dem.Events)
                        {
                            EventsDateFilter.Add(eventModel);
                        }
                    }
                }
            }

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
                SelectedEvent.Image = APIService.ImagenesServerUrl + "/images/" + SelectedEvent.Id.ToString();
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
        public async Task UpdateEvent()
        {
            Mode = "update";
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
            imagen.Id = SelectedEvent.Id.ToString();
            imagen.Content = AvatarImage64;
            var request = new RequestModel(method: "POST", route: "/images/save", data: imagen, server: APIService.ImagenesServerUrl);


            ResponseModel response = await APIService.ExecuteRequest(request);

            return response.Success == 0;
        }


        [RelayCommand]
        public async Task ShowEventPopup(string mode)
        {
            Mode = mode;
            if (Mode.Equals("create"))
            {
                HeaderTabName = "Crear Evento";
                IsCreateVisible = true;
                IsUpdateVisible = false;
                IsDeleteVisible = false;
                AvatarImage = SelectedEvent.Image;


                DayEvents.Events.Clear();
                DayEvents.Id = ObjectId.GenerateNewId().ToString();
                SelectedEvent = new EventModel();
            }
            else if (Mode.Equals("update"))
            {
                HeaderTabName = "Editar Evento";
                IsCreateVisible = false;
                IsUpdateVisible = true;
                IsDeleteVisible = true;
                EventHeader = SelectedEvent.Type;
                AvatarImage = SelectedEvent.Image;
            }
            CalendarPopup = new CalendarPopup();
            await App.Current.MainPage.ShowPopupAsync(CalendarPopup);
        }

        [RelayCommand]
        public async Task ClosePopUp()
        {
            CalendarPopup.Close();

        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        [RelayCommand]
        public async Task LoadRegisterUserPage()
        {

            await Shell.Current.GoToAsync("//RegisterUserPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadUnitsPage()
        {
            await Shell.Current.GoToAsync("//UnitsPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadChatPage()
        {
            await Shell.Current.GoToAsync("//MainPage", new Dictionary<string, object>() { ["User"] = User });
        }
    }
}
