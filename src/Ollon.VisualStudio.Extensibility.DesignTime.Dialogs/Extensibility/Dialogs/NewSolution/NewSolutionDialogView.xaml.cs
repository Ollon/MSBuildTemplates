using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.VisualStudio.PlatformUI;
using FBD = System.Windows.Forms.FolderBrowserDialog;

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    /// <summary>
    /// Interaction logic for NewSolutionDialogView.xaml
    /// </summary>
    /// 
    
    public partial class NewSolutionDialogView : DialogWindow
    {
        public NewSolutionDialogView()
        {
            InitializeComponent();
        }

        public NewSolutionDialogView(NewSolutionDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnDirectorySearchDialogClicked(object sender, RoutedEventArgs e)
        {
            using (FBD fbd = new FBD())
            {
                fbd.ShowNewFolderButton = true;
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                fbd.Description = "Select a repository directory for the solution.";

                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RepositoryDirectoryTextBox.SetValue(TextBox.TextProperty, fbd.SelectedPath);
                }
            }

            UpdateSolutionInfo();
        }

        private void UpdateSolutionInfo()
        {
            string repositoryDirectory = (string) RepositoryDirectoryTextBox.GetValue(TextBox.TextProperty);

            string solutionName = (string) SolutionNameTextBox.GetValue(TextBox.TextProperty);
            SolutionDirectoryTextBox.SetValue(TextBox.TextProperty, Path.Combine(repositoryDirectory, "src"));
            SolutionFilePathTextBox.SetValue(TextBox.TextProperty, Path.Combine(repositoryDirectory, "src", $"{solutionName}.sln"));
        }


    }
}
