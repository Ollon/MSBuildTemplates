using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    /// <summary>
    /// Interaction logic for FolderBrowserDialog.xaml
    /// </summary>
    public partial class FolderBrowserDialog : DialogWindow
    {
        private BrowserViewModel _viewModel;

        public BrowserViewModel ViewModel
        {
            get
            { 
                return _viewModel = _viewModel ?? new BrowserViewModel();
            }
        }

        public string SelectedFolder
        {
            get
            {
                return ViewModel.SelectedFolder;
            }
        }

        public FolderBrowserDialog()
        {
            InitializeComponent();

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
