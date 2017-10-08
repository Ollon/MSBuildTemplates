// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    public sealed partial class ProjectBlock
    {
        public ProjectBlock(Guid projectTypeGuid, string projectName, string projectPath, Guid projectGuid, IEnumerable<SectionBlock> projectSections)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(projectPath))
            {
                throw new ArgumentException();
            }

            ProjectTypeGuid = projectTypeGuid;
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectGuid = projectGuid;
            ProjectSections = projectSections.ToList().AsReadOnly();
        }

        public Guid ProjectTypeGuid { get; }

        public string ProjectName { get; }

        public string ProjectPath { get; }

        public Guid ProjectGuid { get; }

        public IEnumerable<SectionBlock> ProjectSections { get; }

        internal string GetText()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", ProjectTypeGuid.ToString("B").ToUpper(), ProjectName, ProjectPath, ProjectGuid.ToString("B").ToUpper());
            builder.AppendLine();

            foreach (SectionBlock block in ProjectSections)
            {
                builder.Append(block.GetText(indent: 1));
            }

            builder.AppendLine("EndProject");

            return builder.ToString();
        }

        internal static ProjectBlock Parse(TextReader reader)
        {
            string startLine = reader.ReadLine().TrimStart(null);
            LineScanner scanner = new LineScanner(startLine);

            if (scanner.ReadUpToAndEat("(\"") != "Project")
            {
                throw new Exception();
            }

            Guid projectTypeGuid = Guid.Parse(scanner.ReadUpToAndEat("\")"));

            // Read chars up to next quote, must contain "=" with optional leading/trailing whitespaces.
            if (scanner.ReadUpToAndEat("\"").Trim() != "=")
            {
                throw new Exception();
            }

            string projectName = scanner.ReadUpToAndEat("\"");

            // Read chars up to next quote, must contain "," with optional leading/trailing whitespaces.
            if (scanner.ReadUpToAndEat("\"").Trim() != ",")
            {
                throw new Exception();
            }

            string projectPath = scanner.ReadUpToAndEat("\"");

            // Read chars up to next quote, must contain "," with optional leading/trailing whitespaces.
            if (scanner.ReadUpToAndEat("\"").Trim() != ",")
            {
                throw new Exception();
            }

            Guid projectGuid = Guid.Parse(scanner.ReadUpToAndEat("\""));

            List<SectionBlock> projectSections = new List<SectionBlock>();

            while (char.IsWhiteSpace((char)reader.Peek()))
            {
                projectSections.Add(SectionBlock.Parse(reader));
            }

            // Expect to see "EndProject" but be tolerant with missing tags as in Dev12. 
            // Instead, we may see either P' for "Project" or 'G' for "Global", which will be handled next.
            if (reader.Peek() != 'P' && reader.Peek() != 'G')
            {
                if (reader.ReadLine() != "EndProject")
                {
                    throw new Exception();
                }
            }

            return new ProjectBlock(projectTypeGuid, projectName, projectPath, projectGuid, projectSections);
        }
    }
}
