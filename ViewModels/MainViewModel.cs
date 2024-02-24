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
        private string _resourceToShow;
        [ObservableProperty]
        private MessageMediaModel _messageMedia;
        [ObservableProperty]
        private VisorArchivosPopup _visorPopup;
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
            GenerateSource();
            Conectar();
            ImMainChat = true;
            ImNotificationChat = false;
            PrivateNotification = false;
            ImageMainChat = "botonmain.png";
            ImageNotificationChat = "botonnotification.png";
            Users = new ObservableCollection<UserModel>();
            ListMediaMessages = new ObservableCollection<MessageMediaModel>();
            ListNotificationMediaMessages = new ObservableCollection<MessageMediaModel>();
            ShowMainMsg();
        }
        [RelayCommand]
        public void AddEmoji(string emoji)
        {
            MessageToSend += emoji;
        }
        
        private void GenerateSource()
        {
            var nodeImageInfo = new ObservableCollection<FileManager>();

            var calendar = new FileManager() { ItemName = "Calendario", ImageIcon = "calendar.png" };
            var alumnos = new FileManager() { ItemName = "Alumnos", ImageIcon = "student.png" };
            var mp3 = new FileManager() { ItemName = "Music", ImageIcon = "ricardo.jpg" };
            var pictures = new FileManager() { ItemName = "Pictures", ImageIcon = "ricardo.jpg" };
            var video = new FileManager() { ItemName = "Videos", ImageIcon = "ricardo.jpg" };

            var pollution = new FileManager() { ItemName = "Calendar", ImageIcon = "calendar.png" };
            var globalWarming = new FileManager() { ItemName = "Global Warming.ppt", ImageIcon = "ricardo.jpg" };
            var sanitation = new FileManager() { ItemName = "Sanitation.docx", ImageIcon = "ricardo.jpg" };
            var socialNetwork = new FileManager() { ItemName = "Social Network.pdf", ImageIcon = "ricardo.jpg" };
            var youthEmpower = new FileManager() { ItemName = "Youth Empowerment.pdf", ImageIcon = "ricardo.jpg" };


            var tutorials = new FileManager() { ItemName = "Tutorials.zip", ImageIcon = "ricardo.jpg" };
            var typeScript = new FileManager() { ItemName = "TypeScript.7z", ImageIcon = "ricardo.jpg" };
            var uiGuide = new FileManager() { ItemName = "UI-Guide.pdf", ImageIcon = "ricardo.jpg" };

            var song = new FileManager() { ItemName = "Gouttes", ImageIcon = "ricardo.jpg" };

            var camera = new FileManager() { ItemName = "Camera Roll", ImageIcon = "ricardo.jpg" };
            var stone = new FileManager() { ItemName = "Stone.jpg", ImageIcon = "ricardo.jpg" };
            var wind = new FileManager() { ItemName = "Wind.jpg", ImageIcon = "ricardo.jpg" };

            var img0 = new FileManager() { ItemName = "WIN_20160726_094117.JPG", ImageIcon = "ricardo.jpg" };
            var img1 = new FileManager() { ItemName = "WIN_20160726_094118.JPG", ImageIcon = "ricardo.jpg" };

            var video1 = new FileManager() { ItemName = "Naturals.mp4", ImageIcon = "ricardo.jpg" };
            var video2 = new FileManager() { ItemName = "Wild.mpeg", ImageIcon = "ricardo.jpg" };


            mp3.SubFiles = new ObservableCollection<FileManager>
            {
                song
            };

            pictures.SubFiles = new ObservableCollection<FileManager>
            {
                camera,
                stone,
                wind
            };
            camera.SubFiles = new ObservableCollection<FileManager>
            {
                img0,
                img1
            };

            video.SubFiles = new ObservableCollection<FileManager>
            {
                video1,
                video2
            };

            nodeImageInfo.Add(mp3);
            nodeImageInfo.Add(pictures);
            nodeImageInfo.Add(video);
            ImageNodeInfo = nodeImageInfo;
        }
        public async Task Conectar()
        {
            ClientWebSocket = new ClientWebSocket();  // Crear una nueva instancia de ClientWebSocket.

            // Construir la URI para la conexión WebSocket con el identificador del usuario. 192.168.20.12
            Uri uri = new Uri($"ws://127.0.0.1:5000/chat-websocket?userId={UserName}");
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
            ImageMainChat = "botonmain.png";
            Profesor = true;
        }
        [RelayCommand]
        public void ShowNotificationMsg()
        {
            ChatSelection = "NOTIFICACIONES";
            LoadMessagesList(ListNotificationMediaMessages);
            NotificationMessage = true;
            ImMainChat = false;
            ImNotificationChat = true;
            ImageNotificationChat = "botonnotification.png";
            Profesor = User.RolProfesor;
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
                                ImageMainChat = "botonmainnotifications.png";
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
                            foreach (string user in UserList){
                               
                                UserModel user1 = new UserModel();
                                user1.UserName = user;
                                Users.Add(user1);
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
                                if (SelectedUser.Equals(messageChatModel.UserId))
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


                            User = Users.FirstOrDefault(u => u.UserName == messageChatModel.UserId);
                            if (User != null)
                            {
                                User.IsNotificationEnabled = true;
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
                                ImageNotificationChat = "botonnotificationnotifications.png";
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
