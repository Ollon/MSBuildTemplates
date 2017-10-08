using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ollon.VisualStudio.Extensibility.Dialogs.Input;

namespace Ollon.VisualStudio.Extensibility.Dialogs
{

    public class BrowserViewModel : ViewModelBase
    {
        private string _selectedFolder;

        public string SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged("SelectedFolder");
            }
        }

        public ObservableCollection<FolderViewModel> Folders
        {
            get;
            set;
        }

        public DelegateCommand<object> FolderSelectedCommand
        {
            get
            {
                return new DelegateCommand<object>(it => SelectedFolder = Environment.GetFolderPath((Environment.SpecialFolder)it));
            }
        }
     
        
        public BrowserViewModel()
        {
            Folders = new ObservableCollection<FolderViewModel>();
            Environment.GetLogicalDrives().ToList().ForEach(it => Folders.Add(new FolderViewModel { Root = this, FolderPath = it.TrimEnd('\\'), FolderName = it.TrimEnd('\\'), FolderIcon = "Images\\HardDisk.ico" }));
        }
    }
}
