using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserModel user;

        public LoginViewModel()
        {
            User = new UserModel();
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
                    await LoadMainPage();
                    await App.Current.MainPage.DisplayAlert("Login",
                    response.Message, "ACEPTAR");
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
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        public async Task LoadRegisterPage()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        public async Task<bool> ComprobarCamposAsync()
        {
            bool todoOK = false;
            if (User.UserName == null)
            {
                await App.Current.MainPage.DisplayAlert("Login",
                    "El campo Nombre de Usuario no puede esta vacio", "ACEPTAR");
            }
            else if(User.Password == null)
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
    }
}
