using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ollon.VisualStudio.Extensibility.Input;
using Ollon.VisualStudio.Extensibility.Progress;
using Ollon.VisualStudio.Extensibility.Services;
using Ollon.VisualStudio.Extensibility.Utilities;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility.Input
{
    [Export(typeof(DynamicCommand))]
    public class InitializeDirectoryBuildPropsCommand : DynamicCommand
    {

        [ImportingConstructor]
        protected InitializeDirectoryBuildPropsCommand([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider) 
            : base(serviceProvider, MenuCommands.InitializeDirectoryBuildPropsCommand)
        {
        }

        [Import]
        internal IVsSolution Solution { get; private set; }

        [Import]
        internal IVsSolution4 Solution4 { get; private set; }

        [Import]
        internal IStrongNameKeyGenerator KeyGen { get; private set; }

        [Import]
        internal IMSBuildProjectLoader ProjectLoader { get; private set; }

        [Import]
        internal IProjectXmlService ProjectXmlService { get; private set; }

        [Import]
        internal IDirectoryBuildPropsWriter DirectoryBuildPropsWriter { get; private set; }

        protected override void Execute()
        {
            SolutionOptions options = new SolutionOptions();

            Solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionDirectory, out object solutionDirectoryRaw);
            Solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionBaseName, out object solutionNameRaw);
            string solutionName = solutionNameRaw.ToString();
            string solutionDirectory = solutionDirectoryRaw.ToString();

            options.SolutionName = solutionName;
            options.SolutionDirectory = solutionDirectory;
            options.RepositoryDirectory = new DirectoryInfo(solutionDirectory).Parent.FullName;

            IEnumerable<Project> projects = DTE.Solution.GetAllProjects();

            using (DialogScope progress2 = new DialogScope())
            {
                progress2.Update("Synchronizing project file properties with solution. Please wait...");


                foreach (Project project in projects)
                {
                    progress2.Update(
                        "Synchronizing project file properties with solution. Please wait...",
                        $"Processing Project '{project.Name}'"
                        );

                    // ===========================
                    //            Flow
                    // ===========================
                    // 
                    // 1) Snapshot project file full path.
                    // 2) Open Project File with filePath and snapshot its ProjectRootElement.
                    // 3) Retrieve Project GUID of the project snapshotted in step 1
                    // 4) Unload project as if user manually unloaded it, and then close it
                    // 
                    // 5) Walk property elements, stop at chosen property Name
                    // 5.1) Get the parent of the property element (should be a ProjectPropertyGroupElement)
                    // 5.2) Remove the property element, which will be an immediate child of the parent you just got in 5.1
                    // 6) Save the ProjectRootElement
                    // 7) Reload the project. This is where the GUID from step 3 comes in handy.
                    // 8) For good measure, save the project via the hierarchy.
                    // 9) You are done, Son!
                    // 
                    //

                    string projectFilePath = project.FullName;



                    Solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy ppHierarchy);
                    Solution.GetGuidOfProject(ppHierarchy, out Guid projectGuid);
                    Solution4.UnloadProject(ref projectGuid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
                    Solution.CloseSolutionElement((uint)__VSSLNCLOSEOPTIONS.SLNCLOSEOPT_UnloadProject, ppHierarchy, VSConstants.VSITEMID_NIL);



                    ProjectRootElement root = ProjectLoader.LoadProject(projectFilePath).Xml;

                    foreach (ProjectPropertyElement propertyElement in root.Properties)
                    {
                        // Check for the most common property names for 
                        // output paths. The names checked below are 
                        // non-exhaustive. Technically, OutputPath should 
                        // be sufficient; however, OutDir is used as 
                        // well, but not very often.
                        if (propertyElement.Name.Equals("OutputPath")
                            || propertyElement.Name.Equals("OutDir"))
                        {
                            propertyElement.Parent.RemoveChild(propertyElement);
                            root.Save(projectFilePath);
                        }
                    }

                    // This has to come after the properties are removed. Otherwise you get some error 
                    // saing The SDK '' could not be found. 
                    ProjectXmlService.SetExplicitSdkImportsIfNecessaryAsync(projectFilePath);

                    Solution4.ReloadProject(ref projectGuid);
                    Solution4.EnsureProjectIsLoaded(ref projectGuid, 0);
                    Solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, ppHierarchy, VSConstants.VSITEMID_NIL);
                }

            }

            DirectoryBuildPropsWriter.WriteDirectoryBuildProps(options);

            Project solutionItems = DTE.Solution.GetOrCreateSolutionFolder("Solution Items");

            solutionItems.ProjectItems.AddFromFile(options.DirectoryBuildPropsPath);

            DTE.Collapse();

        }

        private class SolutionOptions : ISolutionOptions
        {
            public string CompanyName { get; set; }

            public string RepositoryDirectory { get; set; }

            public string BuildDirectory { get; set; }

            public string DocsDirectory { get; set; }

            public string StrongNameKeysDirectory { get; set; }

            public string RuleSetDirectory { get; set; }

            public string SolutionDirectory { get; set; }

            public string SolutionName { get; set; }

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

            public int SolutionVersionMajor { get; set; }

            public int SolutionVersionMinor { get; set; }
            public string SolutionFilePath { get; set; }
        }
    }
}
