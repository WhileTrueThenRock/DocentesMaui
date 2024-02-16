using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using EFDocenteMAUI.Views.Popups;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics.Text;
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
        private ObservableCollection<EventModel> _eventsDateFilter = new();

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsTypeFilter = new();

        [ObservableProperty]
        private ObservableCollection<EventModel> _eventsDescriptionFilter = new();

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
        private string _fechaIni;

        [ObservableProperty]
        private string _fechaFin;

        //[ObservableProperty]
        //private DateTime _fechaInicio = new DateTime(2023, 1, 1);

        //[ObservableProperty]
        //private DateTime _fechaFin = DateTime.Now;

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
            User = new UserModel();
            Culture = CultureInfo.CurrentCulture;
            SelectedEvent = new EventModel();
            GetEvents();
            EventHeader = "Actividades de clase";
            FechaIni = new DateTime(2023, 1, 1).ToString("dd-MM-yyyy");
            FechaFin = DateTime.Now.ToString("dd-MM-yyyy");
            // ResultadoFecha = $"Años filtrados: {AnioMenor} - {AnioMayor}";
        }


        [RelayCommand]
        public async Task GetUsersByFiltro(string type)
        {
            //if (null == Filtro)
            //{
            //    await App.Current.MainPage.DisplayAlert("Info", "Debes selecionar un Campo de busqueda", "ACEPTAR");

            //}
            if (null == type || type.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Info", "El campo de busqueda no puede estar vacio", "ACEPTAR");
            }
            else
            {
                //string filtro = descripcion.ToLower();
                //   IsListVisible = true;
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
            //if (null == Filtro)
            //{
            //    await App.Current.MainPage.DisplayAlert("Info", "Debes selecionar un Campo de busqueda", "ACEPTAR");

            //}
            if (null == descripcion || descripcion.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Info", "El campo de busqueda no puede estar vacio", "ACEPTAR");
            }
            else
            {
                //string filtro = descripcion.ToLower();
                //   IsListVisible = true;
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
                // Manejar el caso en que las fechas no sean válidas
                return;
            }
           
           // Events.Clear();
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
            imagen.Id = SelectedEvent.Id.ToString(); //Linea modificada
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
                AvatarImage = APIService.ImagenesServerUrl + "/images/default";


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

    }
}
