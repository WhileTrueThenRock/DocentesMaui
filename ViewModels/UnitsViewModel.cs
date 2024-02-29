using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using EFDocenteMAUI.Views.Popups;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EFDocenteMAUI.ViewModels
{
    [QueryProperty("User", "User")]
    internal partial class UnitsViewModel : ObservableObject
    {

        [ObservableProperty]
        private ObservableCollection<UnitModel> _unitList;

        [ObservableProperty]
        private UnitModel _unit;

        [ObservableProperty]
        private string _mode;

        [ObservableProperty]
        private string _image;

        [ObservableProperty]
        private string _image64;

        [ObservableProperty]
        private ImageSource _imageSource;
        [ObservableProperty]
        private ImageSource _pdfSource;
        [ObservableProperty]
        private string _pdf64;
        [ObservableProperty]
        private ImageSource _videoSource;
        [ObservableProperty]
        private string _video64;
        [ObservableProperty]
        private UnitsPopup _unitsPopup;

        [ObservableProperty]
        private string _resource;
        [ObservableProperty]
        private string _resourceToShow;

        [ObservableProperty]
        private UserModel _user;

        [ObservableProperty]
        private bool _modoCrear;
        [ObservableProperty]
        private ResourceModel _resourceModel;

        [ObservableProperty]
        private bool _webResourceVisible;

        [ObservableProperty]
        private string _mensajeError;
        public ICommand TapCommand => new Command<UnitModel>(async (selectedUnit) => await TapCommandExecute(selectedUnit)); //Para mostrar la info en el Popup usando Accordion.

        public UnitsViewModel()
        {
            UnitList = new ObservableCollection<UnitModel>();
            Unit = new UnitModel();
            ResourceModel = new ResourceModel();
            WebResourceVisible = false;
            GetUnits();
        }


        [RelayCommand]
        public void ShowResource(object image)
        {
            var uriURL = (UriImageSource)image;
            ResourceToShow = uriURL.Uri.AbsoluteUri;
        }

        [RelayCommand]
        public void ShowResourcePdf(object image)
        {
            ResourceToShow = image.ToString();
        }
        [RelayCommand]
        public void ShowResourceWeb(ResourceModel resource)
        {   
            ResourceToShow = resource.Contenido;
        }
        [RelayCommand]
        public void LoadWebResource()
        {
            ResourceModel = new ResourceModel();
            WebResourceVisible = true;
        }
        [RelayCommand]
        public async void AddWebResource()
        {
            if (ComprobarCampos())
            {
                Unit.WebResources.Add(ResourceModel);
                WebResourceVisible = false;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Info", MensajeError, "ACEPTAR");
            }
        }
        public bool ComprobarCampos()
        {
            bool todoOK = false;
            if(null == ResourceModel.Titulo || ResourceModel.Titulo.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Titulo, no puede estar vacio";
                todoOK = false;
            }
            else if(null == ResourceModel.Descripcion)
            {
                MensajeError = "El campo Descripción, no puede estar vacio";
                todoOK = false;
            }else if (null == ResourceModel.Contenido || ResourceModel.Contenido.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo URL, no puede estar vacio";
                todoOK = false;
            }
            else
            {
                todoOK = true;
            }
            return todoOK;
        }
        [RelayCommand]
        public async Task ShowUnitPopup(string opcion)
        {
            ModoCrear = bool.Parse(opcion);
            if (ModoCrear)
            {
                Unit = new UnitModel();
            }
            UnitsPopup = new UnitsPopup();
            await App.Current.MainPage.ShowPopupAsync(UnitsPopup);
        }

        public async Task TapCommandExecute(UnitModel selectedUnit) //Para abrir el popup sin perder el binding.
        {
            Unit = selectedUnit;
            UnitsPopup = new UnitsPopup();
            await App.Current.MainPage.ShowPopupAsync(UnitsPopup);
        }

        [RelayCommand]
        public async Task LoadImage()
        {
            var imagesDict = await ImageUtils.OpenImage();
            if (imagesDict != null)
            {
                ImageSource = (ImageSource)imagesDict["imageFromStream"];
                Image64 = (string)imagesDict["imageBase64"];
                await SaveImageAsync();
            }
        }
        [RelayCommand]
        public async Task LoadPDF()
        {
            var pdfDict = await PDFUtils.OpenPDF();
            if (pdfDict != null)
            {
                PdfSource = (ImageSource)pdfDict["pdfFromStream"];
                Pdf64 = (string)pdfDict["pdfBase64"];
                await SavePDFAsync();
            }
        }
        [RelayCommand]
        public async Task LoadVideo()
        {
            var videoDict = await VideoUtils.OpenVideo();
            if (videoDict != null)
            {
                VideoSource = (ImageSource)videoDict["videoFromStream"];
                Video64 = (string)videoDict["videoBase64"];
                await SaveVideoAsync();
            }
        }

        public async Task<bool> UpdateImage()
        {
            ImageModel imagen = new ImageModel();
            imagen.Id = ObjectId.GenerateNewId().ToString();
            imagen.Content = Image64;
            var request = new RequestModel(method: "POST", route: "/images/save", data: imagen, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            Unit.Images.Add(APIService.ImagenesServerUrl + "/images/" + imagen.Id.ToString());
            return response.Success == 0;
        }
        public async Task<bool> UpdatePDF()
        {
            PDFModel pdf = new PDFModel();
            pdf.Id = ObjectId.GenerateNewId().ToString();
            pdf.Content = Pdf64;
            var request = new RequestModel(method: "POST", route: "/pdfs/save", data: pdf, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                Unit.Pdfs.Add(APIService.ImagenesServerUrl + "/pdfs/" + pdf.Id.ToString());
            }
            return response.Success == 0;
        }
        public async Task<bool> UpdateVideo()
        {
            VideoModel video = new VideoModel();
            video.Id = ObjectId.GenerateNewId().ToString();
            video.Content = Video64;
            var request = new RequestModel(method: "POST", route: "/videos/save", data: video, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                Unit.Resources.Add(APIService.ImagenesServerUrl + "/videos/" + video.Id.ToString());
            }
            return response.Success == 0;
        }
        public async Task SaveImageAsync()
        {
            bool okSaveImage = await UpdateImage();
            if (okSaveImage)
            {
                await App.Current.MainPage.DisplayAlert("Temario", "Imagen añadida correctamente", "Aceptar");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Temario", "Error al añadir imagen", "Aceptar");
            }
        }
        public async Task SavePDFAsync()
        {
            bool okSavePDF = await UpdatePDF();
            if (okSavePDF)
            {
                await App.Current.MainPage.DisplayAlert("Temario", "PDF añadida correctamente", "Aceptar");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Temario", "Error al añadir PDF", "Aceptar");
            }
        }
        public async Task SaveVideoAsync()
        {
            bool okSaveVideo = await UpdateVideo();
            if (okSaveVideo)
            {
                await App.Current.MainPage.DisplayAlert("Temario", "Video añadida correctamente", "Aceptar");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Temario", "Error al añadir Video", "Aceptar");
            }
        }
        [RelayCommand]
        public async Task ExecuteRequest()
        {
            var request = new RequestModel(method: "POST",
                                          route: "/units/" + Mode,
                                          data: Unit,
                                          server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                App.Current.MainPage.DisplayAlert("Temario", response.Message, "Aceptar");
                Unit = new UnitModel();
                ClosePopUp();
                GetUnits();

            }
        }
        [RelayCommand]
        public async Task CreateUnit()
        {
            Mode = "create";
            ExecuteRequest();
        }

        [RelayCommand]
        public async Task UpdateUnit()
        {
            Mode = "update";
            ExecuteRequest();
        }

        [RelayCommand]
        public async Task DeleteUnit()
        {
            bool deleteUnit = await App.Current.MainPage.DisplayAlert("Confirmación",
                "¿Estás seguro de que quieres eliminar esta Unidad?", "Sí", "No");
            if (deleteUnit)
            {
                Mode = "delete";
                ExecuteRequest();
                ClosePopUp();
            }

        }
        [RelayCommand]
        public async Task ClosePopUp()
        {
            ModoCrear = false;
            WebResourceVisible = false;
            UnitsPopup.Close();
        }
        public async Task GetUnits()
        {
            try
            {
            UnitList = new ObservableCollection<UnitModel>();
            var request = new RequestModel(method: "GET",
                                           route: "/units/getUnits",
                                           data: string.Empty,
                                           server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                ObservableCollection<UnitModel> unitList =
                JsonConvert.DeserializeObject<ObservableCollection<UnitModel>>
                    (response.Data.ToString());
                foreach (UnitModel dem in unitList)
                {
                    if (!UnitList.Contains(dem))
                    {
                        UnitList.Add(dem);

                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        [RelayCommand]
        public async Task LoadRegisterUserPage()
        {

            await Shell.Current.GoToAsync("//RegisterUserPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadCalendarPage()
        {
            await Shell.Current.GoToAsync("//CalendarPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadChatPage()
        {
            await Shell.Current.GoToAsync("//MainPage", new Dictionary<string, object>() { ["User"] = User });
        }
    }
}
