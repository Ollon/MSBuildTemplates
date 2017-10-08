

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Ollon.VisualStudio.Extensibility.Dialogs.SolutionExplorer;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    public class NewSolutionDialog : AbstractModel, ISolutionOptions
    {
        public string CompanyName { get; set; }

        public string SolutionName { get; set; }

        public string RepositoryDirectory { get; set; }

        public ObservableCollection<SolutionNode> Nodes { get; set; }

        public string BuildDirectory
        {
            get
            {
                return Path.Combine(RepositoryDirectory, "build");
            }
            set
            {
                
            }
        }

        public string DocsDirectory
        {
            get
            {
                return Path.Combine(RepositoryDirectory, "docs");
            }
            set
            {

            }
        }

        public string StrongNameKeysDirectory
        {
            get
            {
                return Path.Combine(BuildDirectory, "strong name keys");
            }
            set
            {

            }
        }

        public string RuleSetDirectory
        {
            get
            {
                return Path.Combine(BuildDirectory, "rulesets");
            }
            set
            {

            }
        }

        public string DirectoryBuildPropsPath
        {
            get
            {
                return Path.Combine(SolutionDirectory, "Directory.Build.props");
            }
            set
            {

            }
        }

        public string SolutionDirectory { get; set; }

        public int SolutionVersionMajor { get; set; }
        public int SolutionVersionMinor { get; set; }

        public string SolutionFilePath { get; set; }


        public NewSolutionDialog()
        {
            Nodes = new ObservableCollection<SolutionNode>();
        }

    }

}
