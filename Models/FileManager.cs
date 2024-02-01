using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDocenteMAUI.Models
{
     internal partial class FileManager : ObservableObject
    {
        [ObservableProperty]
        private string _itemName;
        [ObservableProperty]
        private ImageSource _imageIcon;
        [ObservableProperty]
        private ObservableCollection<FileManager> _subFiles;

        public FileManager() 
        {
            
        }
    }
}
