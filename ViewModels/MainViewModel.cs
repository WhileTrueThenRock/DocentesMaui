using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ObservableCollection<string> userList;

        [ObservableProperty]
        private Dictionary<string, string> messagesDict;

        [ObservableProperty]
        private ObservableCollection<string> emojis;

        [ObservableProperty]
        private string selectedUser;

        [ObservableProperty]
        private string privateMessagesReceived;

        [ObservableProperty]
        private string privateMessageToSend;

        [ObservableProperty]
        private string messageToSend;     

        [ObservableProperty]
        private ObservableCollection<string> messagesReceived;  // Almacena los mensajes recibidos.



        //Popups
        [ObservableProperty]
        private PrivateMessagePopup privateMessagePopup;

        public MainViewModel() 
        {
            MessagesReceived = new ObservableCollection<string>();
            MessagesDict = new Dictionary<string, string>();
            Emojis = new ObservableCollection<string>() { "🍻", "🙋‍", "♂️", "💁‍", "♀️", "😍" };
            UserList = new ObservableCollection<string>();
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

    }
}
