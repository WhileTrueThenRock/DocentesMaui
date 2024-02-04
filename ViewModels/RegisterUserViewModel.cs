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
        [ObservableProperty]
        private string _telefonoString;
        [ObservableProperty]
        private int _numTelefono;
        [ObservableProperty]
        private string _cpString;
        [ObservableProperty]
        private string _mensajeError;
        
      

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
            if (null == User.Nombre || !User.Nombre.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Nombre, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateSurname()
        {
            if (null == User.Apellidos || !User.Apellidos.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Apellidos, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateDni()
        {
            if (null == User.Dni || !User.Dni.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo DNI no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateEmail()
        {
            if (null == User.Email || !User.Email.Equals(string.IsNullOrWhiteSpace))
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
            if (null == TelefonoString || !TelefonoString.Equals(string.IsNullOrWhiteSpace))
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
                User.Telefono = numericValue;
            }
            return true;
        }
        private bool ValidateGrade()
        {
            if (null == User.Curso || !User.Curso.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Curso, no puede estar vacio";
                return false;
            }
            return true;
        }

        private bool ValidateStreet()
        {
            if (null == User.Direccion.Calle || !User.Direccion.Calle.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Calle, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateNumber()
        {
            if (null == User.Direccion.Numero || !User.Direccion.Numero.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Número, no puede estar vacio";
                return false;
            }
            return true;
        }
        private bool ValidateCity()
        {
            if (null == User.Direccion.Poblacion || !User.Direccion.Poblacion.Equals(string.IsNullOrWhiteSpace))
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
            if (null == CpString || !CpString.Equals(string.IsNullOrWhiteSpace))
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
            
            if (null == User.UserName || !User.UserName.Equals(string.IsNullOrWhiteSpace))
            {
                MensajeError = "El campo Nombre de Usuario,\n no puede estar vacio";
                return false;
            }
            return true;
            
        }
        private bool ValidatePassword()
        {

            if (null == User.Password || !User.Password.Equals(string.IsNullOrWhiteSpace))
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
            IsCreateMode = false;
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
