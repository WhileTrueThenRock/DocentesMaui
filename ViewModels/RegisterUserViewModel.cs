using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class RegisterUserViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserModel _user;

        [ObservableProperty]
        private bool _isCreateMode;
        [ObservableProperty]
        private bool _isEditMode;
        [ObservableProperty]
        private bool _isDataEnabled;
        [ObservableProperty]
        private bool _isListVisible;
        [ObservableProperty]
        private bool _isSelectedUser;
        [ObservableProperty]
        private ObservableCollection<string> _parametrosBusqueda;

        [ObservableProperty]
        private string _busqueda;
        [ObservableProperty]
        private bool _isSelectedMode;

        [ObservableProperty]
        private ObservableCollection<UserModel> _userList;
        [ObservableProperty]
        private string avatarImage64;

        [ObservableProperty]
        private ImageSource avatarImage;
        public RegisterUserViewModel()
        {
            IsListVisible = false;
            IsSelectedUser = false;
            IsEditMode = false;
            ParametrosBusqueda = new ObservableCollection<string>();
            ParametrosBusqueda = ["Nombre","Apellidos", "Grupo","Poblacion","Nombre de Usuario"];
        }

        [RelayCommand]
        public async Task UpdateUser()
        {
            bool okSaveImage = await UpdateImage();
            if (okSaveImage)
            {
                string[] fechamodificada = User.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                User.FechaNacimiento = fechamodificada[0];
                User.Avatar = APIService.ImagenesServerUrl + "/avatars/" + User.Id.ToString();
                var request = new RequestModel(method: "POST",
                                                route: "/users/update",
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);

                //await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                await GetUsers();
                ShowUserInfo();
            }

        }
        [RelayCommand]
        public async Task GetUsers()
        {
            IsListVisible = true;
            var request = new RequestModel(method: "GET",
                                            route: "/users/all",
                                            data: User,
                                            server: APIService.GestionServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                UserList = JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(response.Data.ToString());
            }
        }
        [RelayCommand]
        public async Task CreateUser()
        {
            
            if (IsEditMode)
            {
                UpdateUser();
            }
            else
            {
                bool okSaveImage = await UpdateImage();
                if(okSaveImage)
                {
                    string[] fechamodificada = User.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                    User.FechaNacimiento = fechamodificada[0];
                    User.Avatar = APIService.ImagenesServerUrl + "/avatars/" + User.Id.ToString();
                    var request = new RequestModel(method: "POST",
                                                    route: "/auth/register",
                                                    data: User,
                                                    server: APIService.GestionServerUrl);
                    ResponseModel response = await APIService.ExecuteRequest(request);

                    await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                    User = new UserModel();
                    GetUsers();
                }
                
            }
            
        }
        [RelayCommand]
        public async Task DeleteUser()
        {
            bool DeleteUser = await App.Current.MainPage.DisplayAlert("Confirmación", 
                "¿Estás seguro de que quieres eliminar este estudiante?", "Sí", "No");
            if (DeleteUser)
            {
                string[] fechamodificada = User.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                User.FechaNacimiento = fechamodificada[0];
                var request = new RequestModel(method: "POST",
                                                route: "/users/delete",
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);

                await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                GetUsers();
            }
            
        }
        [RelayCommand]
        public void EnableCreateUser()
        {
            User = new UserModel();
            IsSelectedUser = true;
            IsCreateMode = true;
            IsEditMode = false;
            IsDataEnabled = true;
            AvatarImage = User.Avatar;

        }
        [RelayCommand]
        public void ShowUserInfo()
        {
            IsSelectedUser = true;
            IsEditMode = true;
            IsDataEnabled = false;
            AvatarImage = User.Avatar;
        }
        [RelayCommand]
        public void EditEnabled()
        {
            IsDataEnabled=true;
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
            imagen.Id = User.Id.ToString();
            imagen.Content = AvatarImage64;
            var request = new RequestModel(method: "POST", route: "/avatars/save", data: imagen, server: APIService.ImagenesServerUrl);
            

            ResponseModel response = await APIService.ExecuteRequest(request);

            //await App.Current.MainPage.DisplayAlert("Actualizar", response.Message, "Aceptar");
            return response.Success == 0;
        }
    }
}
