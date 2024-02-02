using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EFDocenteMAUI.Models;
using GestorChat.Views.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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


        //Popups
        [ObservableProperty]
        private PrivateMessagePopup privateMessagePopup;

        public MainViewModel()
        {
            MessagesReceived = new ObservableCollection<string>();
            MessagesDict = new Dictionary<string, string>();
            Emojis = new ObservableCollection<string>() { "🍻", "🙋‍", "♂️", "💁‍", "♀️", "😍" };
            UserList = new ObservableCollection<string>();
            UserList.Add("Rafa");
            UserList.Add("Victor");
            UserList.Add("Adrian");
            ImageNodeInfo = new ObservableCollection<FileManager>();
            FileManager fileManager = new FileManager()
            {
                ItemName = "Carpeta",
                ImageIcon = "ricardo.jpg"

            };
            ImageNodeInfo.Add(fileManager);
            GenerateSource();
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



        [RelayCommand]
        public async Task ShowPrivateMessagePopup()
        {
            //string sessionMessages = null;
            //MessagesDict.TryGetValue(SelectedUser, out sessionMessages);
            //if (sessionMessages != null)
            //{
            //    PrivateMessagesReceived += sessionMessages;
            //}
            //else
            //{
            //    PrivateMessagesReceived = string.Empty;
            //}

            PrivateMessagePopup = new PrivateMessagePopup();
            await App.Current.MainPage.ShowPopupAsync(PrivateMessagePopup);
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






    }
}
