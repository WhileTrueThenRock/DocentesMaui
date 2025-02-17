﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Numerics;

namespace EFDocenteMAUI.ViewModels
{
    [QueryProperty("User", "User")]
    internal partial class RegisterUserViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserModel _user;
        [ObservableProperty]
        private UserModel _userSelected;
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
        [ObservableProperty]
        private string _telefonoString;
        [ObservableProperty]
        private int _numTelefono;
        [ObservableProperty]
        private string _cpString;
        [ObservableProperty]
        private string _mensajeError;

        [ObservableProperty]
        private bool _imageChanged;

        [ObservableProperty]
        private string _filtro;
        
      

        public RegisterUserViewModel()
        {
            IsListVisible = false;
            IsSelectedUser = false;
            IsEditMode = false;
            ParametrosBusqueda = new ObservableCollection<string>();
            ParametrosBusqueda = ["Nombre","Correo","Curso","Poblacion"];
        }

        [RelayCommand]
        public async Task UpdateUser()
        {
            bool okSaveImage = await UpdateImage();
            if (okSaveImage)
            {   
                string[] fechamodificada = UserSelected.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                UserSelected.FechaNacimiento = fechamodificada[0];
                UserSelected.Direccion.Cp = int.Parse(CpString);
                UserSelected.Avatar = APIService.ImagenesServerUrl + "/avatars/" + UserSelected.Id.ToString();
                var request = new RequestModel(method: "POST",
                                                route: "/users/update",
                                                data: UserSelected,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);
                AvatarImage64 = null;
                await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
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
                                            data: UserSelected,
                                            server: APIService.GestionServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
                UserList = JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(response.Data.ToString());
            }
        }

        [RelayCommand]
        public async Task GetUsersByFiltro(string name)
        {
            if (null == Filtro)
            {
                await App.Current.MainPage.DisplayAlert("Info", "Debes selecionar un Campo de busqueda", "ACEPTAR");
                
            }else if (null == name || name.Any(Char.IsWhiteSpace))
            {
                await App.Current.MainPage.DisplayAlert("Info", "El campo de busqueda no puede estar vacio", "ACEPTAR");
            }
            else
            {
                string filtro = Filtro.ToLower();
                IsListVisible = true;
                var request = new RequestModel(method: "GET",
                                                route: "/users/" + filtro + "/" + name,
                                                data: UserSelected,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);
                if (response.Success == 0)
                {
                    UserList = JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(response.Data.ToString());
                    if (UserList.Count == 0)
                    {
                        await App.Current.MainPage.DisplayAlert("Info", "No se han encontrado resultados", "ACEPTAR");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No se han encontrado resultados", "ACEPTAR");
                }
            }
        }
        private bool ValidateFields()
        {
            bool validarCampos;
            if (!ValidateName() || !ValidateSurname() || !ValidateSurname() || !ValidateDni() || !ValidateEmail() || !ValidateTelefono() || !ValidateGrade()
                || !ValidateStreet() || !ValidateNumber() || !ValidateCity() || !ValidateCP() || !ValidateUserName() || !ValidatePassword()      )
            {
                validarCampos = false;
            }
            else
            {
                validarCampos = true;
            }

            return validarCampos;
        }
        private bool ValidateName()
        {
            if (null == UserSelected.Nombre || UserSelected.Nombre.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Nombre, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateSurname()
        {
            if (null == UserSelected.Apellidos)
            {
                MensajeError = "El campo Apellidos, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateDni()
        {
            if (null == UserSelected.Dni || UserSelected.Dni.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo DNI no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateEmail()
        {
            if (null == UserSelected.Email || UserSelected.Email.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo E-mail, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateTelefono()
        {
            int numericValue;
            bool isValid = int.TryParse(TelefonoString, out numericValue)
                           && numericValue >= 600000000
                           && numericValue <= 999999999;
            if (null == TelefonoString || TelefonoString.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Teléfono, no puede estar vacio";
                return false;
            }else if (!isValid)
            {
                MensajeError = "Formato de Teléfono incorrecto";
                return false;
            }
            else
            {
                UserSelected.Telefono = numericValue;
            }
            return true;
        }
        private bool ValidateGrade()
        {
            if (null == UserSelected.Curso || UserSelected.Curso.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Curso, no puede estar vacio";
                return false;
            }
            return true;
        }

        private bool ValidateStreet()
        {
            if (null == UserSelected.Direccion.Calle)
            {
                MensajeError = "El campo Calle, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateNumber()
        {
            if (null == UserSelected.Direccion.Numero || UserSelected.Direccion.Numero.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Número, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateCity()
        {
            if (null == UserSelected.Direccion.Poblacion || UserSelected.Direccion.Poblacion.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Población, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateCP()
        {
            int numericValue;
            bool isValid = int.TryParse(CpString, out numericValue)
                           && numericValue >= 0
                           && numericValue <= 99999;
            if (null == CpString || CpString.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo CP, no puede estar vacio";
                return false;
            }else if (!isValid)
            {
                MensajeError = "Formato CP incorrecto";
                return false;
            }
            return true;
        }
        private bool ValidateUserName()
        {
            
            if (null == UserSelected.UserName || UserSelected.UserName.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Nombre de Usuario,\n no puede estar vacio";
                return false;
            }
            return true;
            
        }
        private bool ValidatePassword()
        {

            if (null == User.Password || User.Password.Any(Char.IsWhiteSpace))
            {
                MensajeError = "El campo Contraseña,\n no puede estar vacio";
                return false;
            }
            return true;

        }

        [RelayCommand]
        public async Task CreateUser()
        {
            
            if (ValidateFields())
            {
                if (IsEditMode)
                {
                    UpdateUser();
                }
                else
                {
                    bool okSaveImage = await UpdateImage();
                    if (okSaveImage)
                    {
                        string[] fechamodificada = UserSelected.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                        UserSelected.FechaNacimiento = fechamodificada[0];
                        UserSelected.Direccion.Cp = int.Parse(CpString);
                        UserSelected.Avatar = APIService.ImagenesServerUrl + "/avatars/" + UserSelected.Id.ToString();
                        var request = new RequestModel(method: "POST",
                                                        route: "/auth/register",
                                                        data: UserSelected,
                                                        server: APIService.GestionServerUrl);
                        ResponseModel response = await APIService.ExecuteRequest(request);

                        await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                        UserSelected = new UserModel();
                        AvatarImage64 = null;
                        GetUsers();
                    }

                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Info", MensajeError, "ACEPTAR");
            }
            
            
            
        }
        [RelayCommand]
        public async Task DeleteUser()
        {
            bool DeleteUser = await App.Current.MainPage.DisplayAlert("Confirmación", 
                "¿Estás seguro de que quieres eliminar este estudiante?", "Sí", "No");
            if (DeleteUser)
            {
                string[] fechamodificada = UserSelected.FechaNacimiento.Replace(" 0:00:00", " ").Split(' ');
                UserSelected.FechaNacimiento = fechamodificada[0];
                var request = new RequestModel(method: "POST",
                                                route: "/users/delete",
                                                data: UserSelected,
                                                server: APIService.GestionServerUrl);
                ResponseModel response = await APIService.ExecuteRequest(request);

                await App.Current.MainPage.DisplayAlert("Info", response.Message, "ACEPTAR");
                GetUsers();
            }
            
        }
        [RelayCommand]
        public void EnableCreateUser()
        {
            UserSelected = new UserModel();
            IsSelectedUser = true;
            IsCreateMode = true;
            IsEditMode = false;
            IsDataEnabled = true;
            AvatarImage = UserSelected.Avatar;
            ImageChanged = false;

        }
        [RelayCommand]
        public void ShowUserInfo()
        {
            IsSelectedUser = true;
            IsEditMode = true;
            IsCreateMode = false;
            IsDataEnabled = false;
            AvatarImage = UserSelected.Avatar;
            TelefonoString = UserSelected.Telefono.ToString();
            CpString = UserSelected.Direccion.Cp.ToString();
            ImageChanged = false;
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
                ImageChanged = true;
            }
        }
        public async Task<bool> UpdateImage()
        {
            ResponseModel response = new ResponseModel();
            if (ImageChanged)
            {
                ImageModel imagen = new ImageModel();
                imagen.Id = UserSelected.Id.ToString();
                imagen.Content = AvatarImage64;
                var request = new RequestModel(method: "POST", route: "/avatars/save", data: imagen, server: APIService.ImagenesServerUrl);


                response = await APIService.ExecuteRequest(request);
            }
            response.Success = 0;

            return response.Success == 0;
        }
        [RelayCommand]
        public async Task LoadCalendarPage()
        {

            await Shell.Current.GoToAsync("//CalendarPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadUnitsPage()
        {
            await Shell.Current.GoToAsync("//UnitsPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadChatPage()
        {
            await Shell.Current.GoToAsync("//MainPage", new Dictionary<string, object>() { ["User"] = User });
        }
    }
    
}
