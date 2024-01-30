using CommunityToolkit.Mvvm.ComponentModel;
using GestorChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class RegisterUserViewModel : ObservableObject
    {
        [ObservableProperty]
        public UserModel _user;
        public RegisterUserViewModel()
        {
            User = new UserModel();
        }
    }
}
