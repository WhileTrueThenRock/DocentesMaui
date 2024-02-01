using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
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
            //bool okSaveImage = await UpdateImage();
            //if (okSaveImage)
            //{
                User.FechaNacimiento = User.FechaNacimiento.Replace(" 0:00:00", " ");
                //Usuario.Avatar = APIService.ImagenesServerUrl + "/imagenes/" + Usuario.Id.ToString();
                var request = new RequestModel(method: "POST",
                                                route: "/users/update",
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);

                await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                //await GetUsers();
                //ShowUserInfo();
           // }

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
                string[] fechamodificada = User.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                User.FechaNacimiento = fechamodificada[0];
                var request = new RequestModel(method: "POST",
                                                route: "/users/update",
                                                data: User,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);

                await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                GetUsers();
                
            }
            else
            {
                string[] fechamodificada = User.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                User.FechaNacimiento = fechamodificada[0];
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
        [RelayCommand]
        public async Task DeleteUser()
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
        [RelayCommand]
        public void EnableCreateUser()
        {
            User = new UserModel();
            IsSelectedUser = true;
            IsCreateMode = true;
            IsEditMode = false;
            IsDataEnabled = true;

        }
        [RelayCommand]
        public void ShowUserInfo()
        {
            IsSelectedUser = true;
            IsEditMode = true;
            IsDataEnabled = false;
        }
        [RelayCommand]
        public void EditEnabled()
        {
            IsDataEnabled=true;
        }
    }
}
