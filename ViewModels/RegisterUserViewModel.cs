using CommunityToolkit.Mvvm.ComponentModel;
using GestorChat.Models;

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
