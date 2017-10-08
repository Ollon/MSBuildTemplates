using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs
{
    public class SolutionItemViewModel : INotifyPropertyChanged
    {
        private readonly SolutionItem _model;
        private int _solutionVersionMajor;
        private int _solutionVersionMinor;
        private int _solutionVersionRevision;
        private int _solutionVersionBuildNumber;

        public event PropertyChangedEventHandler PropertyChanged;


        [DesignOnly(true)]
        public SolutionItemViewModel()
        {
            _model = new SolutionItem();
        }

        public SolutionItemViewModel(SolutionItem model)
        {
            _model = model;
        }

        public string CompanyName
        {
            get
            {
                return _model.CompanyName;
            }
            set
            {
                if (_model.CompanyName != value)
                {
                    _model.CompanyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IncludeStrongNameKey
        {
            get
            {
                return _model.IncludeStrongNameKey;
            }
            set
            {
                if (_model.IncludeStrongNameKey != value)
                {
                    _model.IncludeStrongNameKey = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IncludeRuleSet
        {
            get
            {
                return _model.IncludeRuleSet;
            }
            set
            {
                if (_model.IncludeRuleSet != value)
                {
                    _model.IncludeRuleSet = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SolutionName
        {
            get
            {
                return _model.SolutionName;
            }
            set
            {
                if (_model.SolutionName != value)
                {
                    _model.SolutionName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SolutionDirectory
        {
            get
            {
                return _model.SolutionDirectory;
            }
            set
            {
                if (_model.SolutionDirectory != value)
                {
                    _model.SolutionDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SolutionVersionMajor
        {
            get
            {
                return _solutionVersionMajor;
            }
            set
            {
                _solutionVersionMajor = value;
            }
        }

        public int SolutionVersionMinor
        {
            get
            {
                return _solutionVersionMinor;
            }
            set
            {
                _solutionVersionMinor = value;
            }
        }

        public int SolutionVersionBuild
        {
            get
            {
                return _solutionVersionBuildNumber;
            }
            set
            {
                _solutionVersionBuildNumber = value;
            }
        }

        public int SolutionVersionRevision
        {
            get
            {
                return _solutionVersionRevision;
            }
            set
            {
                _solutionVersionRevision = value;
            }
        }

        public Version SolutionVersion
        {
            get
            {
                return new Version(
                    _solutionVersionMajor,
                    _solutionVersionMinor,
                    _solutionVersionRevision,
                    _solutionVersionBuildNumber
                    );
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
