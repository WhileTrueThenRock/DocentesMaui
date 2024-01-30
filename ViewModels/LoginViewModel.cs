using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestorChat.Models;
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
    }
}
