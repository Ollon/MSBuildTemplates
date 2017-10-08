using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Utilities
{
    public static class EnvDTEProjectExtensions
    {
        public static void Collapse(this DTE2 dte)
        {
            UIHierarchy solutionExplorer = dte.ToolWindows.SolutionExplorer;
            if (solutionExplorer.UIHierarchyItems.Count <= 0)
            {
                return;
            }

            UIHierarchyItem rootNode = solutionExplorer.UIHierarchyItems.Item(1);
            Collapse(rootNode, ref solutionExplorer);
            rootNode.Select(vsUISelectionType.vsUISelectionTypeSelect);
            rootNode.DTE.SuppressUI = false;
        }

        private static void Collapse(UIHierarchyItem item, ref UIHierarchy solutionExplorer)
        {
            foreach (UIHierarchyItem innerItem in item.UIHierarchyItems)
            {
                if (innerItem.UIHierarchyItems.Count > 0)
                {
                    Collapse(innerItem, ref solutionExplorer);
                    if (innerItem.UIHierarchyItems.Expanded)
                    {
                        innerItem.UIHierarchyItems.Expanded = false;
                        if (innerItem.UIHierarchyItems.Expanded)
                        {
                            innerItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                            solutionExplorer.DoDefaultAction();
                        }
                    }
                }
            }
        }

        public static bool IsCpsProject(this Project project)
        {
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
            return hierarchy.IsCpsProject();
        }

        private static ProjectItems GetSelectedProjectItems(this DTE dte)
        {
            SelectedItem item = dte.SelectedItems.Item(1);
            if (item.ProjectItem != null)
            {
                return item.ProjectItem.ProjectItems;
            }
            return item.Project.ProjectItems;
        }

        public static Project GetOrCreateSolutionFolder(this Solution solution, string name)
        {
            if (solution.TryFindSolutionFolder(name, out Project solutionFolder))
            {
                return solutionFolder;
            }
            Solution2 solution2 = solution as Solution2;
            return solution2.AddSolutionFolder(name);
        }

        public static bool TryFindSolutionFolder(this Solution solution, string name, out Project solutionFolder)
        {
            Solution2 solution2 = solution as Solution2;
            IEnumerable<Project> allProjects = solution.GetAllProjects();
            foreach (object projectObject in solution2.Projects)
            {
                if (projectObject is Project project)
                {
                    if (project.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        solutionFolder = project;
                        return true;
                    }
                }
            }

            solutionFolder = null;
            return false;
        }

        public static bool TryFindProjectItemByName(this Project project, string name, out ProjectItem foundItem)
        {
            foundItem = FindProjectItemByNameCore(project, name);
            if (foundItem == null)
            {
                return false;
            }
            return true;
        }

        public static void AddProjectItemFromFileIfNeccessary(this Project project, string file)
        {
            if (!project.TryGetProjectItemFromFile(file, out ProjectItem projectItem))
            {
                project.ProjectItems.AddFromFile(file);
            }
        }

        public static bool TryGetProjectItemFromFile(this Project project, string file, out ProjectItem projectItem)
        {
            projectItem = project.DTE.Solution.FindProjectItem(file);

            return projectItem == null;
        }

        private static ProjectItem FindProjectItemByNameCore(Project project, string projectItemName)
        {
            ProjectItem result = null;
            foreach (ProjectItem item in project.GetAllProjectItems())
            {
                if (item.Name.Equals(projectItemName, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = item;
                }
            }
            return result;
        }

        public static IEnumerable<ProjectItem> GetAllProjectItems(this Project project)
        {
            return GetAllProjectItems(project.ProjectItems, true);
        }

        private static IEnumerable<ProjectItem> GetAllProjectItems(ProjectItems collection, bool recursive = true)
        {
            foreach (ProjectItem item in collection)
            {
                yield return item;
                if (recursive)
                {
                    IEnumerable<ProjectItem> children = GetAllProjectItems(item.ProjectItems, recursive);
                    if (children != null)
                    {
                        foreach (ProjectItem childItem in children)
                        {
                            yield return childItem;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Project> GetAllProjects(this Solution sln)
        {
            return (sln as Solution2).Projects
                .Cast<Project>()
                .SelectMany(GetProjects)
                .Where(x => x.Kind != MiscellaneousFilesProjectGuid);
        }

        private static IEnumerable<Project> GetProjects(Project project)
        {
            if (project.Kind == SolutionFolder)
            {
                return project.ProjectItems
                    .Cast<ProjectItem>()
                    .Select(x => x.SubProject)
                    .Where(x => x != null)
                    .SelectMany(GetProjects);
            }

            return new Project[]
            {
                project
            };
        }

        public const string SolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string MiscellaneousFilesProjectGuid = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";
    }
}