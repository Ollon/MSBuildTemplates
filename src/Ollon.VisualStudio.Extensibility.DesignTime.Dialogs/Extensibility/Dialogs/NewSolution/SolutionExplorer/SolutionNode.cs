// -----------------------------------------------------------------------
// <copyright file="SolutionNode.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ollon.VisualStudio.Extensibility.Dialogs.SolutionExplorer
{

    public sealed class SolutionExplorer : TreeView
    {
        private readonly List<SolutionNode> _nodes;

        public SolutionExplorer()
        {
            _nodes = new List<SolutionNode>();
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                return _nodes.GetEnumerator();
            }
        }

        public void AddChild(SolutionNode node)
        {
            AddChild((object)node);
        }

        protected override void AddChild(object value)
        {
            _nodes.Add(Cast<SolutionNode>(value));
        }

        public SolutionNode this[string name]
        {
            get
            {
                SolutionNode node = null;
                foreach(SolutionNode solutionNode in _nodes)
                {
                    if (solutionNode.ProjectName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        node = solutionNode;
                        break;
                    }
                }
                return node;
            }
        }

        private T Cast<T>(object o) => (T)o;
    }

    public abstract class SolutionNode
    {

        protected const string _packUri = "pack://application:,,,/{0};component/{1}";


        public SolutionNode(SolutionExplorer parent)
        {
            Parent = parent;
            ProjectGuid = Guid.NewGuid();
        }

        protected Image CreateImage(string relativePath)
        {
            Image image = new Image();
            image.BeginInit();
            image.Source = FromUri(relativePath);
            image.EndInit();
            return image;
        }
        private ImageSource FromUri(string relativePath) 
            => new ImageSourceConverter()
            .ConvertFromString(
                string.Format(
                    _packUri, 
                    this.GetType().Assembly.FullName, relativePath)) as ImageSource;

        public Guid ProjectGuid { get; } 

        public Guid ProjectTypeGuid
        {
            get
            {
                switch (Kind)
                {
                    case ProjectKind.Vsix:
                    case ProjectKind.Console:
                    case ProjectKind.ClassLibrary:
                        return  IsLegacy ? ProjectTypeGuids.LegacyCSharpProjectTypeGuid : ProjectTypeGuids.CpsCSharpProjectTypeGuid;
                    case ProjectKind.SolutionFolder:
                        return ProjectTypeGuids.SolutionFolderGuid;
    
                }

                return Guid.Empty;
            }
        }

        public SolutionExplorer Parent { get; }

        public abstract string ProjectName { get; set; }

        public abstract string RootNamespace { get; set; }

        public abstract bool IsLegacy { get; set; }

        public abstract ProjectKind Kind { get; }

        public abstract Image CollapsedIcon { get; }

        public abstract Image ExpandedIcon { get; }

        public virtual ObservableCollection<SolutionNode> Children { get; set; }
    }

    public class ClassLibraryProjectNode : SolutionNode
    {
        public ClassLibraryProjectNode(SolutionExplorer parent) : base(parent)
        {
        }

        public override string ProjectName { get; set; }
        public override string RootNamespace { get; set; }
        public override bool IsLegacy { get; set; }


        public override ProjectKind Kind => ProjectKind.ClassLibrary;
        
        public override Image CollapsedIcon 
            => CreateImage("Themes/Icons/CS_ProjectSENode_16x.png");
        public override Image ExpandedIcon => CollapsedIcon;

        
    }

    public class SolutionFolderNode : SolutionNode
    {
        public override string ProjectName { get; set; }
        public override string RootNamespace { get; set; }
        public override bool IsLegacy { get; set; }

        public override ProjectKind Kind => ProjectKind.SolutionFolder;
        public override Image CollapsedIcon => CreateImage("Themes/Icons/VSO_Folder_16x.png");
        public override Image ExpandedIcon => CreateImage("Themes/Icons/VSO_FolderOpen_16x.png");

        public SolutionFolderNode(SolutionExplorer solutionExplorer) : base(solutionExplorer)
        {
            IsLegacy = false;
            Children = new ObservableCollection<SolutionNode>();
        }
    }
}
