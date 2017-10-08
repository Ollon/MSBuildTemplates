// -----------------------------------------------------------------------
// <copyright file="VsEnvDTE.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Implementation.Interop
{
    [Export(typeof(DTE))]
    internal sealed class VsEnvDTE : DTE
    {
        private readonly DTE _dte;

        public Document ActiveDocument
        {
            get
            {
                return _dte.ActiveDocument;
            }
        }

        public object ActiveSolutionProjects
        {
            get
            {
                return _dte.ActiveSolutionProjects;
            }
        }

        public Window ActiveWindow
        {
            get
            {
                return _dte.ActiveWindow;
            }
        }

        public AddIns AddIns
        {
            get
            {
                return _dte.AddIns;
            }
        }

        public DTE Application
        {
            get
            {
                return _dte.Application;
            }
        }

        public object CommandBars
        {
            get
            {
                return _dte.CommandBars;
            }
        }

        public string CommandLineArguments
        {
            get
            {
                return _dte.CommandLineArguments;
            }
        }

        public EnvDTE.Commands Commands
        {
            get
            {
                return _dte.Commands;
            }
        }

        public ContextAttributes ContextAttributes
        {
            get
            {
                return _dte.ContextAttributes;
            }
        }

        public Debugger Debugger
        {
            get
            {
                return _dte.Debugger;
            }
        }

        public vsDisplay DisplayMode
        {
            get
            {
                return _dte.DisplayMode;
            }
            set
            {
                _dte.DisplayMode = value;
            }
        }

        public Documents Documents
        {
            get
            {
                return _dte.Documents;
            }
        }

        public DTE DTE
        {
            get
            {
                return _dte.DTE;
            }
        }

        public string Edition
        {
            get
            {
                return _dte.Edition;
            }
        }

        public Events Events
        {
            get
            {
                return _dte.Events;
            }
        }

        public string FileName
        {
            get
            {
                return _dte.FileName;
            }
        }

        public Find Find
        {
            get
            {
                return _dte.Find;
            }
        }

        public string FullName
        {
            get
            {
                return _dte.FullName;
            }
        }

        public Globals Globals
        {
            get
            {
                return _dte.Globals;
            }
        }

        public ItemOperations ItemOperations
        {
            get
            {
                return _dte.ItemOperations;
            }
        }

        public int LocaleID
        {
            get
            {
                return _dte.LocaleID;
            }
        }

        public Macros Macros
        {
            get
            {
                return _dte.Macros;
            }
        }

        public DTE MacrosIDE
        {
            get
            {
                return _dte.MacrosIDE;
            }
        }

        public Window MainWindow
        {
            get
            {
                return _dte.MainWindow;
            }
        }

        public vsIDEMode Mode
        {
            get
            {
                return _dte.Mode;
            }
        }

        public string Name
        {
            get
            {
                return _dte.Name;
            }
        }

        public ObjectExtenders ObjectExtenders
        {
            get
            {
                return _dte.ObjectExtenders;
            }
        }

        public string RegistryRoot
        {
            get
            {
                return _dte.RegistryRoot;
            }
        }

        public SelectedItems SelectedItems
        {
            get
            {
                return _dte.SelectedItems;
            }
        }

        public Solution Solution
        {
            get
            {
                return _dte.Solution;
            }
        }

        public SourceControl SourceControl
        {
            get
            {
                return _dte.SourceControl;
            }
        }

        public StatusBar StatusBar
        {
            get
            {
                return _dte.StatusBar;
            }
        }

        public bool SuppressUI
        {
            get
            {
                return _dte.SuppressUI;
            }
            set
            {
                _dte.SuppressUI = value;
            }
        }

        public UndoContext UndoContext
        {
            get
            {
                return _dte.UndoContext;
            }
        }

        public bool UserControl
        {
            get
            {
                return _dte.UserControl;
            }
            set
            {
                _dte.UserControl = value;
            }
        }

        public string Version
        {
            get
            {
                return _dte.Version;
            }
        }

        public WindowConfigurations WindowConfigurations
        {
            get
            {
                return _dte.WindowConfigurations;
            }
        }

        public Windows Windows
        {
            get
            {
                return _dte.Windows;
            }
        }

        public void ExecuteCommand(string CommandName, string CommandArgs = "")
        {
            _dte.ExecuteCommand(CommandName, CommandArgs);
        }

        public bool get_IsOpenFile(string ViewKind, string FileName)
        {
            return _dte.IsOpenFile[ViewKind, FileName];
        }

        public Properties get_Properties(string Category, string Page)
        {
            return _dte.Properties[Category, Page];
        }

        public object GetObject(string Name)
        {
            return _dte.GetObject(Name);
        }

        public wizardResult LaunchWizard(string VSZFile, ref object[] ContextParams)
        {
            return _dte.LaunchWizard(VSZFile, ref ContextParams);
        }

        public Window OpenFile(string ViewKind, string FileName)
        {
            return _dte.OpenFile(ViewKind, FileName);
        }

        public void Quit()
        {
            _dte.Quit();
        }

        public string SatelliteDllPath(string Path, string Name)
        {
            return _dte.SatelliteDllPath(Path, Name);
        }

        public VsEnvDTE(DTE dte)
        {
            _dte = dte;
        }

        [ImportingConstructor]
        private VsEnvDTE([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this(serviceProvider.GetService(typeof(SDTE)) as DTE)
        {
        }
    }
}
