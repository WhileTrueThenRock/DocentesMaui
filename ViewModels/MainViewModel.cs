using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using EFDocenteMAUI.Utils;
using EFDocenteMAUI.Views.Popups;
using GestorChat.Views.Popups;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace EFDocenteMAUI.ViewModels
{
    [QueryProperty("User","User")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> _userList;

        [ObservableProperty]
        private ObservableCollection<UserModel> _users;

        [ObservableProperty]
        private Dictionary<string, string> _messagesDict;

        [ObservableProperty]
        private ObservableCollection<string> _emojis;

        [ObservableProperty]
        private UserModel _selectedUser;

        [ObservableProperty]
        private string _privateMessagesReceived;

        [ObservableProperty]
        private string _privateMessageToSend;

        [ObservableProperty]
        private string _messageToSend;

        [ObservableProperty]
        private ObservableCollection<string> _messagesReceived;  // Almacena los mensajes recibidos.

        public ObservableCollection<FileManager> ImageNodeInfo { get; set; }

        [ObservableProperty]
        private string _itemName;

        [ObservableProperty]
        private ImageSource _imageIcon;

        [ObservableProperty]
        private ObservableCollection<FileManager> _subFiles;
        [ObservableProperty]
        private string _userName;
        [ObservableProperty]
        private string _connectionState;
        [ObservableProperty]
        private string _chatSelection;
        [ObservableProperty]
        private ObservableCollection<string> _notificationMessagesReceived;

        [ObservableProperty]
        private ObservableCollection<MessageMediaModel> _showMessagesList;

        [ObservableProperty]
        private bool _notificationMessage;

        
        [ObservableProperty]
        private PrivateMessagePopup privateMessagePopup;
        ClientWebSocket ClientWebSocket { get; set; }
        [ObservableProperty]
        private string _messagesPrivateReceived;
        [ObservableProperty]
        private bool _notificacionChatGeneral;
        [ObservableProperty]
        private bool _notificacionChatNotificaciones;
        [ObservableProperty]
        private ImageSource _imageMainChat;
        [ObservableProperty]
        private string _imageNotificationChat;
        [ObservableProperty]
        private bool _imMainChat;
        [ObservableProperty]
        private bool _imNotificationChat;
        [ObservableProperty]
        private bool _privateNotification;

        [ObservableProperty]
        private Color _notificationColor;
        [ObservableProperty]
        private UserModel _user;
        [ObservableProperty]
        private bool _profesor;
        [ObservableProperty]
        private ObservableCollection<MessageMediaModel> _listMediaMessages;
        [ObservableProperty]
        private ObservableCollection<MessageMediaModel> _listNotificationMediaMessages;
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
        private string _resourceToShow;
        [ObservableProperty]
        private MessageMediaModel _messageMedia;
        [ObservableProperty]
        private VisorArchivosPopup _visorPopup;
        [ObservableProperty]
        private Color _colorBotonSala;
        [ObservableProperty]
        private Color _colorBotonNotificacion;
        public MainViewModel()
        {
            Inicio();
        }
        public void Inicio()
        {
            NotificacionChatNotificaciones = false;
            NotificacionChatGeneral = false;
            MessagesReceived = new ObservableCollection<string>();
            NotificationMessagesReceived = new ObservableCollection<string>();
            ShowMessagesList = new ObservableCollection<MessageMediaModel>();
            MessagesDict = new Dictionary<string, string>();
            Emojis = new ObservableCollection<string>() { "🍻", "🙋‍", "♂️", "💁‍", "♀️", "😍" };
            UserList = new ObservableCollection<string>();
            ConnectionState = "DESCONECTADO";
            ChatSelection = "SALA PRINCIPAL";
            UserName = LoginViewModel.UserName;
            ImageNodeInfo = new ObservableCollection<FileManager>();
            FileManager fileManager = new FileManager()
            {
                ItemName = "Carpeta",
                ImageIcon = "ricardo.jpg"

            };
            ImageNodeInfo.Add(fileManager);
            ImMainChat = true;
            ImNotificationChat = false;
            PrivateNotification = false;
            ImageMainChat = "botonmain.png";
            ImageNotificationChat = "botonnotification.png";
            Users = new ObservableCollection<UserModel>();
            ListMediaMessages = new ObservableCollection<MessageMediaModel>();
            ListNotificationMediaMessages = new ObservableCollection<MessageMediaModel>();
            Conectar();
            ColorBotonSala = Colors.Transparent;
            ColorBotonNotificacion= Colors.Transparent;
            ShowMainMsg();
        }
        [RelayCommand]
        public void AddEmoji(string emoji)
        {
            MessageToSend += emoji;
        }
        
        public async Task Conectar()
        {
            ClientWebSocket = new ClientWebSocket();  // Crear una nueva instancia de ClientWebSocket.

            // Construir la URI para la conexión WebSocket con el identificador del usuario. 192.168.20.12
            Uri uri = new Uri($"ws://192.168.20.132:5000/chat-websocket?userId={UserName}");
            ClientWebSocket.Options.SetRequestHeader("UserId", UserName);  // Configurar el encabezado UserId.
            string token = await SecureStorage.Default.GetAsync("token");
            ClientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
            try
            {
                await ClientWebSocket.ConnectAsync(uri, CancellationToken.None);  // Intentar conectar al servidor.

                // Si la conexión se establece correctamente, cambiar ConnectionState a "CONECTADO".
                if (ClientWebSocket.State == WebSocketState.Open)
                {
                    ConnectionState = "CONECTADO";
                    // Iniciar un hilo para recibir mensajes mientras la conexión esté abierta.
                    _ = Task.Run(async () => await ReceiveMessage(ClientWebSocket));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);  // Registrar cualquier excepción durante la conexión.
            }
            //User = await GetUserByUserName();
        }


        [RelayCommand]
        public async Task ShowPrivateMessagePopup()
        {
            if(UserName.Equals(SelectedUser.UserName))
            {
                return;
            }
            var tempUsers = new ObservableCollection<UserModel>(Users);
            foreach (var user in tempUsers)
            {
                if (user.UserName.Equals(SelectedUser.UserName))
                {
                    user.IsNotificationEnabled = false;
                }
            }
            Users = new ObservableCollection<UserModel>(tempUsers);
            NotificationColor = Colors.Black;
            PrivateMessagePopup = new PrivateMessagePopup();
            MessagesPrivateReceived = string.Empty;
            string sessionMessage = null;
            MessagesDict.TryGetValue(SelectedUser.UserName, out sessionMessage);
            if (sessionMessage != null)
            {
                MessagesPrivateReceived = sessionMessage;
            }
            else
            {
                MessagesPrivateReceived = string.Empty;
            }
            await App.Current.MainPage.ShowPopupAsync(PrivateMessagePopup);
        }

        public void LoadMessagesList(ObservableCollection<MessageMediaModel> lista)
        {
            ShowMessagesList = lista;
        }
        [RelayCommand]
        public void ShowMainMsg()
        {
            ChatSelection = "SALA PRINCIPAL";
            LoadMessagesList(ListMediaMessages);
            NotificationMessage = false;
            ImMainChat = true;
            ImNotificationChat = false;
            Profesor = true;
            ColorBotonSala = Colors.Transparent;
        }
        [RelayCommand]
        public void ShowNotificationMsg()
        {
            ChatSelection = "NOTIFICACIONES";
            LoadMessagesList(ListNotificationMediaMessages);
            NotificationMessage = true;
            ImMainChat = false;
            ImNotificationChat = true;
            Profesor = User.RolProfesor;
            ColorBotonNotificacion = Colors.Transparent;
        }
        [RelayCommand]
        private async Task ShowVisorPopup()
        {
            VisorPopup = new VisorArchivosPopup();
            if (null!=MessageMedia.Pdf && MessageMedia.Pdf.Contains("/pdfs"))
            {
                ResourceToShow = MessageMedia.Pdf;
                await App.Current.MainPage.ShowPopupAsync(VisorPopup);
            }
            else if (null != MessageMedia.Imagen && MessageMedia.Imagen.Contains("/images"))
            {
                ResourceToShow = MessageMedia.Imagen;
                await App.Current.MainPage.ShowPopupAsync(VisorPopup);
            }
            else if (null != MessageMedia.Video && MessageMedia.Video.Contains("/videos"))
            {
                ResourceToShow = MessageMedia.Video;
                await App.Current.MainPage.ShowPopupAsync(VisorPopup);
            }
            else
            {
                return;
            }
            
            
        }
        [RelayCommand]
        public async Task CloseVisorPopUp()
        {
            VisorPopup.Close();
        }

        [RelayCommand]
        public async Task ClosePopUp()
        {
            PrivateMessagePopup.Close();
        }

        [RelayCommand]
        public void AddEmojy(string emojy)
        {
            MessageToSend += emojy;
            PrivateMessageToSend += emojy;
        }

        [RelayCommand]
        public async Task LoadCalendarPage()
        {
            
            await Shell.Current.GoToAsync("//CalendarPage", new Dictionary<string, object>() { ["User"] = User });
        }

        [RelayCommand]
        public async Task LoadRegisterPage()
        {
            await Shell.Current.GoToAsync("//RegisterUserPage", new Dictionary<string, object>() { ["User"] = User });
        }
        [RelayCommand]
        public async Task LoadUnitsPage()
        {
            await Shell.Current.GoToAsync("//UnitsPage", new Dictionary<string, object>() { ["User"] = User });
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
        public async Task SaveImageAsync()
        {
            bool okSaveImage = await UpdateImage();
            if (okSaveImage)
            {
                await SendMessage("BroadCast");
            }
            else
            {
                MessageToSend = "";
                await App.Current.MainPage.DisplayAlert("ERROR", "Error al enviar imagen", "Aceptar");
            }
        }
        public async Task<bool> UpdateImage()
        {
            ImageModel imagen = new ImageModel();
            imagen.Id = ObjectId.GenerateNewId().ToString();
            imagen.Content = Image64;
            var request = new RequestModel(method: "POST", route: "/images/save", data: imagen, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            MessageToSend =APIService.ImagenesServerUrl + "/images/" + imagen.Id.ToString();
            return response.Success == 0;
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
        public async Task SavePDFAsync()
        {
            bool okSavePDF = await UpdatePDF();
            if (okSavePDF)
            {
                await SendMessage("BroadCast");
            }
            else
            {
                MessageToSend = "";
                await App.Current.MainPage.DisplayAlert("ERROR", "Error al enviar pdf", "Aceptar");
            }
        }
        public async Task<bool> UpdatePDF()
        {
            PDFModel pdf = new PDFModel();
            pdf.Id = ObjectId.GenerateNewId().ToString();
            pdf.Content = Pdf64;
            var request = new RequestModel(method: "POST", route: "/pdfs/save", data: pdf, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            MessageToSend = APIService.ImagenesServerUrl + "/pdfs/" + pdf.Id.ToString();
            return response.Success == 0;
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
        public async Task SaveVideoAsync()
        {
            bool okSaveVideo = await UpdateVideo();
            if (okSaveVideo)
            {
                await SendMessage("BroadCast");
            }
            else
            {
                MessageToSend = "";
                await App.Current.MainPage.DisplayAlert("ERROR", "Error al enviar video", "Aceptar");
            }
        }
        public async Task<bool> UpdateVideo()
        {
            VideoModel video = new VideoModel();
            video.Id = ObjectId.GenerateNewId().ToString();
            video.Content = Video64;
            var request = new RequestModel(method: "POST", route: "/videos/save", data: video, server: APIService.ImagenesServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            MessageToSend = APIService.ImagenesServerUrl + "/videos/" + video.Id.ToString();
            return response.Success == 0;
        }
        [RelayCommand]
        public async Task SendMessage(string purpose)
        {
            // Crear una nueva instancia del modelo de mensaje de chat.
            var messageChat = new MessageChatModel();

            // Asignar el identificador de usuario y el contenido del mensaje.
            messageChat.UserId = User.UserName;
            messageChat.Content = MessageToSend;
            messageChat.Purpose = purpose;
            if (NotificationMessage && !purpose.Equals("Private"))
            {
                messageChat.Purpose = "Notification";
            }
            if (purpose.Equals("Private"))
            {
                string sessionMessages = string.Empty;
                MessagesDict.TryGetValue(SelectedUser.UserName, out sessionMessages);
                if (sessionMessages != null)
                {
                    sessionMessages += messageChat.Content + "\n";
                    MessagesDict.Remove(SelectedUser.UserName);
                    MessagesDict.Add(SelectedUser.UserName, sessionMessages);
                }
                else
                {
                    MessagesDict.Add(SelectedUser.UserName, PrivateMessageToSend + "\n");
                }
                messageChat.Content = PrivateMessageToSend;
                messageChat.TargetUserID = SelectedUser.UserName;
              
                MessagesPrivateReceived += PrivateMessageToSend + "\n";
                PrivateMessageToSend = "";
            }
            // Serializar el objeto MessageChatModel a una cadena JSON.
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();  // Utilizar el resolver de nombres en minúsculas.
            settings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
            var data = JsonConvert.SerializeObject(messageChat, settings);

            try
            {
                // Verificar si la conexión WebSocket está abierta.
                if (ClientWebSocket.State == WebSocketState.Open)
                {
                    // Convertir la cadena JSON a bytes y enviarla al servidor.
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    await ClientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    MessageToSend = "";

                }
                else
                {
                    Debug.WriteLine("WebSocket connection is not open.");  // Registrar si la conexión WebSocket no está abierta.
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);  // Registrar cualquier excepción durante el envío del mensaje.
            }
        }

        private async Task ReceiveMessage(ClientWebSocket webSocket)
        {
            try
            {
                Debug.WriteLine("ReceiveMessages: Inicio");

                // Bucle continuo para recibir mensajes mientras la conexión esté abierta.
                while (webSocket.State == WebSocketState.Open)
                {
                    Debug.WriteLine("ReceiveMessages: Esperando mensaje...");

                    byte[] buffer = new byte[1024];

                    // Esperar y recibir un mensaje del servidor WebSocket.
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    // Verificar si el tipo de mensaje es de texto.
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Decodificar el mensaje y agregarlo a la propiedad MessagesReceived.
                        var messageChatModel = JsonConvert.DeserializeObject<MessageChatModel>(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        if (messageChatModel.Purpose.Equals("BroadCast"))
                        {
                            if (!ImMainChat)
                            {
                                ColorBotonSala = Colors.Red;
                            }
                            //Añado mensaje al chat general
                            string mensaje = (string)messageChatModel.Content;
                            if (mensaje.Contains("/images"))
                            {
                                var arrayMensajes = mensaje.Split(' ');
                                var sms = new MessageMediaModel();
                                sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                sms.Imagen = arrayMensajes[2];
                                ListMediaMessages.Add(sms);
                            }
                            else if (mensaje.Contains("/pdfs"))
                            {
                                var arrayMensajes = mensaje.Split(' ');
                                var sms = new MessageMediaModel();
                                sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                sms.Imagen = "pdf.png";
                                sms.Pdf = arrayMensajes[2];
                                ListMediaMessages.Add(sms);
                            }
                            else if (mensaje.Contains("/videos"))
                            {
                                var arrayMensajes = mensaje.Split(' ');
                                var sms = new MessageMediaModel();
                                sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                sms.Imagen = "video.png";
                                sms.Video = arrayMensajes[2];
                                ListMediaMessages.Add(sms);
                            }
                            else
                            {
                                var sms = new MessageMediaModel();
                                sms.Mensaje = mensaje;
                                ListMediaMessages.Add(sms);
                            }

                        }
                        else if (messageChatModel.Purpose.Equals("UserList"))
                        {
                            UserList = JsonConvert.
                                DeserializeObject<ObservableCollection<string>>(messageChatModel.Content.ToString());
                            Users = new ObservableCollection<UserModel>();
                            RegisterUserViewModel model = new RegisterUserViewModel();
                            ObservableCollection<UserModel> allUsers =await GetUsers();

                            foreach (string user in UserList){
                                foreach(var usuario in allUsers)
                                {
                                    if (user.Equals(usuario.UserName))
                                    {
                                        Users.Add(usuario);
                                    }
                                }

                          
                            }
                        }
                        else if (messageChatModel.Purpose.Equals("Private"))
                        {
                          
                            string sessionMessages = string.Empty;
                            MessagesDict.TryGetValue(messageChatModel.UserId, out sessionMessages);
                            if (sessionMessages != null)
                            {
                                sessionMessages += messageChatModel.Content + "\n";
                                MessagesDict.Remove(messageChatModel.UserId);
                                MessagesDict.Add(messageChatModel.UserId, sessionMessages);
                                if (null!=SelectedUser.UserName  ||messageChatModel.UserId.Equals(SelectedUser.UserName))
                                {
                                    MessagesPrivateReceived = sessionMessages;
                                }
                            }
                            else
                            {
                                MessagesDict.Add(messageChatModel.UserId, messageChatModel.Content + "\n");

                                MessagesPrivateReceived = messageChatModel.Content + "\n";
                            }
                            ShowNotification(messageChatModel.UserId);

                            ObservableCollection<UserModel> allUsersPrivate = new ObservableCollection<UserModel>(Users);
                            var usuario = new UserModel();
                            usuario = Users.FirstOrDefault(u => u.UserName == messageChatModel.UserId);
                            if (usuario != null)
                            {
                               foreach(var nombreUsuario in allUsersPrivate)
                                {
                                    if (nombreUsuario.UserName.Equals(usuario.UserName))
                                    {
                                        nombreUsuario.IsNotificationEnabled = true;
                                    }
                                }
                               Users= new ObservableCollection<UserModel>(allUsersPrivate);
                            }


                        }
                        else if (messageChatModel.Purpose.Equals("BroadcastMsg"))
                        {
                            JArray jArray = (JArray)messageChatModel.Content;
                            var msgList = jArray.ToObject<ObservableCollection<string>>();
                            foreach (var mensaje in msgList)
                            {
                                if (mensaje.Contains("/images"))
                                {   var arrayMensajes = mensaje.Split(' ');
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = arrayMensajes[0]+" " + arrayMensajes[1];
                                    sms.Imagen = arrayMensajes[2];
                                    ListMediaMessages.Add(sms);
                                }
                                else if (mensaje.Contains("/pdfs"))
                                {
                                    var arrayMensajes = mensaje.Split(' ');
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                    sms.Imagen = "pdf.png";
                                    sms.Pdf = arrayMensajes[2];
                                    ListMediaMessages.Add(sms);
                                }
                                else if (mensaje.Contains("/videos"))
                                {
                                    var arrayMensajes = mensaje.Split(' ');
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                    sms.Imagen = "video.png";
                                    sms.Video = arrayMensajes[2];
                                    ListMediaMessages.Add(sms);
                                }
                                else
                                {
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = mensaje;
                                    ListMediaMessages.Add(sms);
                                }
                            }
                        }
                        else if (messageChatModel.Purpose.Equals("NotificationMsg"))
                        {
                            JArray jArray = (JArray)messageChatModel.Content;
                            var msgList = jArray.ToObject<ObservableCollection<string>>();
                            foreach (var mensaje in msgList)
                            {
                                if (mensaje.Contains("/images"))
                                {
                                    var arrayMensajes = mensaje.Split(' ');
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                    sms.Imagen = arrayMensajes[2];
                                    ListNotificationMediaMessages.Add(sms);
                                }
                                else if (mensaje.Contains("/pdfs"))
                                {
                                    var arrayMensajes = mensaje.Split(' ');
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = arrayMensajes[0] + " " + arrayMensajes[1];
                                    sms.Imagen = "pdf.png";
                                    sms.Pdf = arrayMensajes[2];
                                    ListNotificationMediaMessages.Add(sms);
                                }
                                else
                                {
                                    var sms = new MessageMediaModel();
                                    sms.Mensaje = mensaje;
                                    ListNotificationMediaMessages.Add(sms);
                                }

                            }
                            
                        }
                        else if (messageChatModel.Purpose.Equals("Notification"))
                        {
                            if (!ImNotificationChat)
                            {
                                ColorBotonNotificacion = Colors.Red;
                            }
                            string mensaje = (string)messageChatModel.Content;
                            if (mensaje.Contains("/images"))
                            {
                                var sms = new MessageMediaModel();
                                sms.Imagen = mensaje;
                                ListNotificationMediaMessages.Add(sms);
                            }
                            else if (mensaje.Contains("/pdfs"))
                            {
                                var sms = new MessageMediaModel();
                                sms.Imagen = "pdf.png";
                                sms.Pdf = mensaje;
                                ListNotificationMediaMessages.Add(sms);
                            }
                            else
                            {
                                var sms = new MessageMediaModel();
                                sms.Mensaje = mensaje;
                                ListNotificationMediaMessages.Add(sms);
                            }
                            
                        }
                        Debug.WriteLine("Received message: " + MessagesReceived);
                    }
                }

                Debug.WriteLine("ReceiveMessages: Saliendo del bucle");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en ReceiveMessages: " + ex.Message);  // Registrar cualquier excepción durante la recepción de mensajes.
            }
            finally
            {
                // Cerrar la conexión si todavía está abierta al salir del bucle.
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando conexión", CancellationToken.None);
                }
            }
        }
        private Dictionary<string, bool> privateNotifications = new Dictionary<string, bool>();
        [RelayCommand]
        public async Task <ObservableCollection<UserModel>> GetUsers()
        {
            var request = new RequestModel(method: "GET",
                                            route: "/users/all",
                                            data: "",
                                            server: APIService.GestionServerUrl);
            ResponseModel response = await APIService.ExecuteRequest(request);
            if (response.Success == 0)
            {
              return JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(response.Data.ToString());
            }
            else
            {
                return null;
            }
        }


        public async Task ShowNotification(string user)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android ||
                DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
                {
                    await LocalNotificationCenter.Current.RequestNotificationPermission();
                }
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "MENSAJE PRIVADO",
                    Sound = "sound",
                    Description = "Tienes un mensaje de\n" + user
                };
                await LocalNotificationCenter.Current.Show(notification);

            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                await Toast.Make("Tienes un mensaje de\n"+user).Show();
            }
        }

    }
}
