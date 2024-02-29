using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserModel user;
        public static string UserName { get; set; }

        public LoginViewModel()
        {
            User = new UserModel();
            User.UserName = "dam08";
            User.Password = "1234";
            SecureStorage.Default.RemoveAll();
        }

        [RelayCommand]
        public async Task Login()
        {
            if (await ComprobarCamposAsync())
            {
                var request = new RequestModel(route: "/auth/login",
                                           method: "POST",
                                           data: User,
                                           server: APIService.GestionServerUrl);
                var response = await APIService.ExecuteRequest(request);
                if (response.Success.Equals(0))
                {
                    await SecureStorage.Default.SetAsync("token", response.Data.ToString());
                    UserName = User.UserName;
                    await LoadMainPage();
                    await App.Current.MainPage.DisplayAlert("Login",
                    response.Message+" "+User.Nombre, "ACEPTAR");
                }
                else if (response.Success.Equals(2))
                {
                    await App.Current.MainPage.DisplayAlert("Login",
                    response.Message, "ACEPTAR");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Login",
                        "Error de conexión, intentelo más tarde", "ACEPTAR");
                }

                User = new UserModel();
            }
            



        }

        [RelayCommand]
        public async Task LoadMainPage()
        {
            User = await GetUserByUserName();
            await Shell.Current.GoToAsync("//MainPage",new Dictionary<string, object>(){["User"] = User});
        }

        [RelayCommand]
        public async Task LoadRegisterPage()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        public async Task<bool> ComprobarCamposAsync()
        {
            bool todoOK = false;
            if (User.UserName == null || User.UserName.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Login",
                    "El campo Nombre de Usuario no puede esta vacio", "ACEPTAR");
            }
            else if(User.Password == null || User.Password.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Login",
                    "El campo Contraseña no puede esta vacio", "ACEPTAR");
            }
            else
            {
                todoOK = true;
            }
            return todoOK;
        }
        public async Task<UserModel> GetUserByUserName()
        {
            UserModel user = new UserModel();
            var request = new RequestModel(route: "/users/byname/"+User.UserName,
                                           method: "GET",
                                           data: User,
                                           server: APIService.GestionServerUrl);
            var response = await APIService.ExecuteRequest(request);
            user = JsonConvert.DeserializeObject<UserModel>(response.Data.ToString());
            return user;
        }
    }
}
