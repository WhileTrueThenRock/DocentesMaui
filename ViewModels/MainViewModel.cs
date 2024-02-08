﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using GestorChat.Views.Popups;
using Mopups.PreBaked.PopupPages.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> _userList;

        [ObservableProperty]
        private Dictionary<string, string> _messagesDict;

        [ObservableProperty]
        private ObservableCollection<string> _emojis;

        [ObservableProperty]
        private string _selectedUser;

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
        private ObservableCollection<string> _showMessagesList;

        [ObservableProperty]
        private bool _notificationMessage;

        //Popups
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
            ShowMessagesList = new ObservableCollection<string>();
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
            ShowMainMsg();
            ImMainChat = true;
            ImNotificationChat = false;
            ImageMainChat = "botonmain.png";
            ImageNotificationChat = "botonnotification.png";
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
            Uri uri = new Uri($"ws://192.168.20.10:5000/chat-websocket?userId={UserName}");
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


        }


        [RelayCommand]
        public async Task ShowPrivateMessagePopup()
        {

            PrivateMessagePopup = new PrivateMessagePopup();
            MessagesPrivateReceived = string.Empty;
            string sessionMessage = null;
            MessagesDict.TryGetValue(SelectedUser, out sessionMessage);
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

        public void LoadMessagesList(ObservableCollection<string> lista)
        {
            ShowMessagesList = lista;
        }
        [RelayCommand]
        public void ShowMainMsg()
        {
            ChatSelection = "SALA PRINCIPAL";
            LoadMessagesList(MessagesReceived);
            NotificationMessage = false;
            ImMainChat = true;
            ImNotificationChat = false;
            ImageMainChat = "botonmain.png";
        }
        [RelayCommand]
        public void ShowNotificationMsg()
        {
            ChatSelection = "NOTIFICACIONES";
            LoadMessagesList(NotificationMessagesReceived);
            NotificationMessage = true;
            ImMainChat = false;
            ImNotificationChat = true;
            ImageNotificationChat = "botonnotification.png";
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
            await Shell.Current.GoToAsync("//CalendarPage");
        }

        [RelayCommand]
        public async Task LoadRegisterPage()
        {
            await Shell.Current.GoToAsync("//RegisterUserPage");
        }

        [RelayCommand]
        public async Task SendMessage(string purpose)
        {
            // Crear una nueva instancia del modelo de mensaje de chat.
            var messageChat = new MessageChatModel();

            // Asignar el identificador de usuario y el contenido del mensaje.
            messageChat.UserId = UserName;
            messageChat.Content = MessageToSend;
            messageChat.Purpose = purpose;
            if (NotificationMessage)
            {
                messageChat.Purpose = "Notification";
            }
            if (purpose.Equals("Private"))
            {
                string sessionMessages = string.Empty;
                MessagesDict.TryGetValue(SelectedUser, out sessionMessages);
                if (sessionMessages != null)
                {
                    sessionMessages += messageChat.Content + "\n";
                    MessagesDict.Remove(SelectedUser);
                    MessagesDict.Add(SelectedUser, sessionMessages);
                }
                else
                {
                    MessagesDict.Add(SelectedUser, PrivateMessageToSend + "\n");
                }
                messageChat.Content = PrivateMessageToSend;
                messageChat.TargetUserID = SelectedUser;
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
                            MessagesReceived.Add((string)messageChatModel.Content);

                        }
                        else if (messageChatModel.Purpose.Equals("UserList"))
                        {
                            UserList = JsonConvert.
                                DeserializeObject<ObservableCollection<string>>(messageChatModel.Content.ToString());
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
                                if (messageChatModel.UserId.Equals(SelectedUser))
                                {
                                    MessagesPrivateReceived = sessionMessages;
                                }
                            }
                            else
                            {
                                MessagesDict.Add(messageChatModel.UserId, messageChatModel.Content + "\n");
                                if (messageChatModel.UserId == SelectedUser)
                                {
                                    MessagesPrivateReceived = messageChatModel.Content + "\n";
                                }
                            }
                        }
                        else if (messageChatModel.Purpose.Equals("BroadcastMsg"))
                        {
                            JArray jArray = (JArray)messageChatModel.Content;
                            var msgList = jArray.ToObject<ObservableCollection<string>>();
                            foreach (var msg in msgList)
                            {
                                MessagesReceived.Add(msg);
                            }
                        }
                        else if (messageChatModel.Purpose.Equals("NotificationMsg"))
                        {
                            JArray jArray = (JArray)messageChatModel.Content;
                            var msgList = jArray.ToObject<ObservableCollection<string>>();
                            foreach (var msg in msgList)
                            {
                                NotificationMessagesReceived.Add(msg);
                               
                            }
                            
                        }
                        else if (messageChatModel.Purpose.Equals("Notification"))
                        {
                            if (!ImNotificationChat)
                            {
                                ImageNotificationChat = "botonnotificationnotifications.png";
                            }
                            NotificationMessagesReceived.Add((string)messageChatModel.Content);
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


    }
}
