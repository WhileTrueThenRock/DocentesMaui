using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using EFDocenteMAUI.Views.Popups;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

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


        public UnitsViewModel() 
        {
            UnitList = new ObservableCollection<UnitModel>();
            Unit = new UnitModel();
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
                    GetUnits();
                    Unit = new UnitModel();
                    ClosePopUp();
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
            UnitsPopup.Close();

        }
        public async Task GetUnits()
        {
            UnitList.Clear();
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
                    UnitList.Add(dem);
                }
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
