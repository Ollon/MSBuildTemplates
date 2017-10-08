using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;

namespace Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs
{
    /// <summary>
    /// Interaction logic for SolutionItemView.xaml
    /// </summary>
    public partial class SolutionItemView : DialogWindow
    {
        public SolutionItemView()
        {
            InitializeComponent();
        }

        public SolutionItemView(SolutionItemViewModel viewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = viewModel;
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
