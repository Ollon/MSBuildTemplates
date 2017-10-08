using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    public class NewSolutionDialogViewModel : INotifyPropertyChanged
    {
        private readonly NewSolutionDialog _model;

        public event PropertyChangedEventHandler PropertyChanged;

        public NewSolutionDialogViewModel(NewSolutionDialog model)
        {
            _model = model;
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

        public string RepositoryDirectory
        {
            get
            {
                return _model.RepositoryDirectory;
            }
            set
            {
                if (_model.RepositoryDirectory != value)
                {
                    _model.RepositoryDirectory = value;
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

        public string SolutionFilePath
        {
            get
            {
                return _model.SolutionFilePath;
            }
            set
            {
                if (_model.SolutionFilePath != value)
                {
                    _model.SolutionFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SolutionVersionMajor
        {
            get
            {
                return _model.SolutionVersionMajor;
            }
            set
            {
                if (_model.SolutionVersionMajor != value)
                {
                    _model.SolutionVersionMajor = value;
                    OnPropertyChanged();
                }
            }
        }
        public int SolutionVersionMinor
        {
            get
            {
                return _model.SolutionVersionMinor;
            }
            set
            {
                if (_model.SolutionVersionMinor != value)
                {
                    _model.SolutionVersionMinor = value;
                    OnPropertyChanged();
                }
            }
        }

        //public int SolutionVersionBuild
        //{
        //    get
        //    {
        //        return _model.SolutionVersionBuild;
        //    }
        //    set
        //    {
        //        if (_model.SolutionVersionBuild != value)
        //        {
        //            _model.SolutionVersionBuild = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public int SolutionVersionRevision
        //{
        //    get
        //    {
        //        return _model.SolutionVersionRevision;
        //    }
        //    set
        //    {
        //        if (_model.SolutionVersionRevision != value)
        //        {
        //            _model.SolutionVersionRevision = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}



        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
