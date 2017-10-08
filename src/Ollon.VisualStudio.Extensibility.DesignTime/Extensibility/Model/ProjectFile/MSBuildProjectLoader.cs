// -----------------------------------------------------------------------
// <copyright file="MSBuildProjectLoader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectFile
{
    public class MSBuildProjectLoader
    {
        private readonly ProjectCollection _projectCollection
            = new ProjectCollection(null, null, null, ToolsetDefinitionLocations.Default, 8, false);


        public MSBuildProjectLoader()
        {
        }

        public Project LoadProject(string filePath)
        {

            ICollection<Project> loadedProjects = _projectCollection.GetLoadedProjects(filePath);

            return HasProjects(loadedProjects) ? GetFirstProject(loadedProjects) : OpenProject(filePath);
        }

        private Project OpenProject(string filePath)
        {
            ProjectRootElement root = ProjectRootElement.Open(filePath, _projectCollection, true);
            return new Project(root);
        }

        private bool HasProjects(ICollection<Project> projects)
        {
            if (projects != null)
            {
                foreach (Project p in projects)
                {


                    return true;
                }
            }



            return false;
        }

        private Project GetFirstProject(ICollection<Project> projects)
        {
            if (HasProjects(projects))
            {
                foreach (Project p in projects)
                {
                    return p;
                }
            }

            throw new ArgumentNullException(nameof(projects));
        }
    }
}
