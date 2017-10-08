

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Ollon.VisualStudio.Extensibility.MSBuild;

namespace Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs
{
    public class SolutionItem : IMSBuildOptions
    {
        public string CompanyName { get; set; }

        public bool IncludeStrongNameKey { get; set; }

        public bool IncludeRuleSet { get; set; }

        public string SolutionName { get; set; }

        public string SolutionDirectory { get; set; }



        public Version SolutionVersion { get; }
    }


}
