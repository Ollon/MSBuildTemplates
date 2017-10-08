// -----------------------------------------------------------------------
// <copyright file="MSBuildProjectExtensions.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Utilities
{

    public static class MSBuildProjectExtensions
    {

        public static ProjectRootElement ShallowCloneWithoutSdkOrXmlnsAttribute(this ProjectRootElement originalRoot)
        {

            ProjectRootElement newRoot = ProjectRootElement.Create(NewProjectFileOptions.None);
            foreach (ProjectElement projectElement in originalRoot.Children)
            {
                if (projectElement is ProjectImportGroupElement importGroup)
                {
                    ProjectImportGroupElement newImportGroup = newRoot.CreateImportGroupElement();
                    foreach (ProjectImportElement import in importGroup.Imports)
                    {

                    }
                }
            }
            if (!string.IsNullOrEmpty(originalRoot.DefaultTargets))
            {
                newRoot.DefaultTargets = originalRoot.DefaultTargets;
            }
            if (!string.IsNullOrEmpty(originalRoot.TreatAsLocalProperty))
            {
                newRoot.TreatAsLocalProperty = originalRoot.TreatAsLocalProperty;
            }
            if (!string.IsNullOrEmpty(originalRoot.ToolsVersion))
            {
                newRoot.ToolsVersion = originalRoot.ToolsVersion;
            }
            return newRoot;
        }

        public static bool TryGetDirectoryBuildProps(this Project project, out ProjectRootElement directoryBuildProps)
        {
            if (project.ImportsDirectoryBuildProps())
            {
                foreach (ResolvedImport resolvedImport in project.Imports)
                {
                    if (resolvedImport.ImportingElement.Project.Equals("$(DirectoryBuildPropsPath)", StringComparison.OrdinalIgnoreCase))
                    {
                        directoryBuildProps = resolvedImport.ImportedProject;
                        return true;
                    }
                }
            }

            directoryBuildProps = null;
            return false;
        }

        public static bool ImportsDirectoryBuildProps(this Project project)
        {
            bool retVal = false;
            foreach (ResolvedImport resolvedImport in project.Imports)
            {
                if (resolvedImport.ImportingElement.Project.Equals("$(DirectoryBuildPropsPath)", StringComparison.OrdinalIgnoreCase))
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }



        public static ProjectImportElement Clone(this ProjectImportElement oldImport, ProjectRootElement newRoot, ElementLocationOption option = ElementLocationOption.None)
        {
            ProjectImportElement newImport = newRoot.CreateImportElement(oldImport.Project);
            if (!string.IsNullOrEmpty(oldImport.Condition))
            {
                newImport.Condition = oldImport.Condition;
            }
            if (!string.IsNullOrEmpty(oldImport.Sdk))
            {
                newImport.Sdk = oldImport.Sdk;
            }
            ProcessElementLocationOption(newRoot, option, newImport);
            return newImport;
        }

        private static void ProcessElementLocationOption(ProjectElementContainer container, ElementLocationOption option, ProjectElement element)
        {
            switch (option)
            {
                case ElementLocationOption.BeforeFirstChild:
                {
                    ProjectElement firstChild = container.FirstChild;
                    container.InsertBeforeChild(element, firstChild);
                    break;
                }
                case ElementLocationOption.AfterLastChild:
                {
                    ProjectElement lastChild = container.FirstChild;
                    container.InsertAfterChild(element, lastChild);
                    break;
                }
                default:
                {
                    return;
                }
            }
        }
    }


    public enum ElementLocationOption
    {
        None,
        BeforeFirstChild,
        AfterLastChild
    }
}
