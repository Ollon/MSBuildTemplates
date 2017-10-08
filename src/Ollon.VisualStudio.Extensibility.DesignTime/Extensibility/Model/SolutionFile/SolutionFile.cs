// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    internal sealed partial class SolutionFile : IDisposable
    {
        internal static readonly Guid SolutionFolderGuid = new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
        internal static readonly Guid NETStandardProjectGuid = new Guid("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}");
        internal static readonly Guid CSharpProjectGuid = new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
        internal static readonly Guid LegacyCSharpProjectGuid = new Guid("{FF286327-C783-4F7A-AB73-9BCBAD0D4460}");
        internal static readonly Guid CommonSolutionFolderGuid = new Guid(Guid.NewGuid().ToString("B").ToUpper());



        private StreamReader _reader;


        public SolutionFile()
        {
            VisualStudioVersionLineOpt = "VisualStudioVersion = 15.0.26430.12";
            MinimumVisualStudioVersionLineOpt = "MinimumVisualStudioVersion = 10.0.40219.1";
            HeaderLines = new List<string>()
            {
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                "# Visual Studio 15",
                VisualStudioVersionLineOpt,
                MinimumVisualStudioVersionLineOpt
            };

            ProjectBlocks = new List<ProjectBlock>
            {
            };
            GlobalSectionBlocks = new List<SectionBlock>()
            {
                new SectionBlock("GlobalSection", "SolutionConfigurationPlatforms" , "preSolution",
                    new[]
                    {
                        new SolutionProperty("Debug|Any CPU","Debug|Any CPU"),
                        new SolutionProperty("Release|Any CPU","Release|Any CPU")
                    }),
                new SectionBlock("GlobalSection", "SolutionProperties", "preSolution",
                    new[]
                    {
                        new SolutionProperty("HideSolutionNode", "FALSE"),
                    }
                )
            };
        }
        public SolutionFile(string filePath)
        {
            FilePath = filePath;
            VisualStudioVersionLineOpt = "VisualStudioVersion = 15.0.26430.12";
            MinimumVisualStudioVersionLineOpt = "MinimumVisualStudioVersion = 10.0.40219.1";
            HeaderLines = new List<string>()
            {
                "Microsoft Visual Studio Solution File, Format Version 12.00",
                "# Visual Studio 15",
                VisualStudioVersionLineOpt,
                MinimumVisualStudioVersionLineOpt
            };


            ProjectBlocks = new List<ProjectBlock>
            {
                //new ProjectBlock(SolutionFolderGuid, "Common", "Common", CommonSolutionFolderGuid, new List<SectionBlock>())
            };
            GlobalSectionBlocks = new List<SectionBlock>()
            {
                new SectionBlock("GlobalSection", "SolutionConfigurationPlatforms" , "preSolution",
                    new[]
                    {
                        new SolutionProperty("Debug|Any CPU","Debug|Any CPU"),
                        new SolutionProperty("Release|Any CPU","Release|Any CPU")
                    }),
                new SectionBlock("GlobalSection", "SolutionProperties", "preSolution",
                new[]
                {
                    new SolutionProperty("HideSolutionNode", "FALSE"),
                }
                )
            };


            File.WriteAllText(filePath, GetText());
        }
        public SolutionFile(
            IEnumerable<string> headerLines,
            string visualStudioVersionLineOpt,
            string minimumVisualStudioVersionLineOpt,
            IEnumerable<ProjectBlock> projectBlocks,
            IEnumerable<SectionBlock> globalSectionBlocks)
        {
            HeaderLines = headerLines?.ToList() ?? throw new ArgumentNullException(nameof(headerLines));
            VisualStudioVersionLineOpt = visualStudioVersionLineOpt;
            MinimumVisualStudioVersionLineOpt = minimumVisualStudioVersionLineOpt;
            ProjectBlocks = projectBlocks?.ToList() ?? throw new ArgumentNullException(nameof(projectBlocks));
            GlobalSectionBlocks = globalSectionBlocks?.ToList() ?? throw new ArgumentNullException(nameof(globalSectionBlocks));
        }

        public string FilePath { get; internal set; }

        public string SolutionDir
        {
            get { return Path.GetDirectoryName(FilePath); }
        }

        public List<string> HeaderLines { get; }

        public string VisualStudioVersionLineOpt { get; }

        public string MinimumVisualStudioVersionLineOpt { get; }

        public List<ProjectBlock> ProjectBlocks { get; }

        public List<SectionBlock> GlobalSectionBlocks { get; }

        public string GetText()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine();

            foreach (string headerLine in HeaderLines)
            {
                builder.AppendLine(headerLine);
            }

            foreach (ProjectBlock block in ProjectBlocks)
            {
                builder.Append(block.GetText());
            }

            builder.AppendLine("Global");

            foreach (SectionBlock block in GlobalSectionBlocks)
            {
                builder.Append(block.GetText(indent: 1));
            }

            builder.AppendLine("EndGlobal");

            return builder.ToString();
        }

        public static SolutionFile Parse(StreamReader reader)
        {
            List<string> headerLines = new List<string>();

            string headerLine1 = GetNextNonEmptyLine(reader);
            if (headerLine1 == null || !headerLine1.StartsWith("Microsoft Visual Studio Solution File", StringComparison.Ordinal))
            {
                throw new Exception("Expected Header: Microsoft Visual Studio Solution File");
            }

            headerLines.Add(headerLine1);

            // skip comment lines and empty lines
            while (reader.Peek() != -1 && "#\r\n".Contains((char)reader.Peek()))
            {
                headerLines.Add(reader.ReadLine());
            }

            string visualStudioVersionLineOpt = null;
            if (reader.Peek() == 'V')
            {
                visualStudioVersionLineOpt = GetNextNonEmptyLine(reader);
                if (!visualStudioVersionLineOpt.StartsWith("VisualStudioVersion", StringComparison.Ordinal))
                {
                    throw new Exception(string.Format("Expected Header: VisualStudioVersion"));
                }
            }

            string minimumVisualStudioVersionLineOpt = null;
            if (reader.Peek() == 'M')
            {
                minimumVisualStudioVersionLineOpt = GetNextNonEmptyLine(reader);
                if (!minimumVisualStudioVersionLineOpt.StartsWith("MinimumVisualStudioVersion", StringComparison.Ordinal))
                {
                    throw new Exception(string.Format("Expected Header: MinimumVisualStudioVersion"));
                }
            }

            List<ProjectBlock> projectBlocks = new List<ProjectBlock>();

            // Parse project blocks while we have them
            while (reader.Peek() == 'P')
            {
                projectBlocks.Add(ProjectBlock.Parse(reader));
                while (reader.Peek() != -1 && "#\r\n".Contains((char)reader.Peek()))
                {
                    // Comments and Empty Lines between the Project Blocks are skipped
                    reader.ReadLine();
                }
            }

            // We now have a global block
            IEnumerable<SectionBlock> globalSectionBlocks = ParseGlobal(reader);

            // We should now be at the end of the file
            if (reader.Peek() != -1)
            {
                throw new Exception("Expected end of file");
            }

            SolutionFile file = new SolutionFile(headerLines, visualStudioVersionLineOpt, minimumVisualStudioVersionLineOpt, projectBlocks, globalSectionBlocks);
            file._reader = reader;
            return file;
        }
        private static IEnumerable<SectionBlock> ParseGlobal(TextReader reader)
        {
            if (reader.Peek() == -1)
            {
                return Enumerable.Empty<SectionBlock>();
            }

            if (GetNextNonEmptyLine(reader) != "Global")
            {
                throw new Exception();
            }

            List<SectionBlock> globalSectionBlocks = new List<SectionBlock>();

            // The blocks inside here are indented
            while (reader.Peek() != -1 && char.IsWhiteSpace((char)reader.Peek()))
            {
                globalSectionBlocks.Add(SectionBlock.Parse(reader));
            }

            if (GetNextNonEmptyLine(reader) != "EndGlobal")
            {
                throw new Exception();
            }

            // Consume potential empty lines at the end of the global block
            while (reader.Peek() != -1 && "\r\n".Contains((char)reader.Peek()))
            {
                reader.ReadLine();
            }

            return globalSectionBlocks;
        }

        private static string GetNextNonEmptyLine(TextReader reader)
        {
            string line;

            do
            {
                line = reader.ReadLine();
            }
            while (line != null && line.Trim() == string.Empty);

            return line;
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                using (StreamWriter writer = new StreamWriter(_reader.BaseStream))
                {
                    writer.Write(GetText());
                    _reader.Close();
                }
            }
            else
            {
                File.WriteAllText(FilePath, GetText());
            }

        }
    }
}
