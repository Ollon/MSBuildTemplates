using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    internal class SolutionFileWriter : TextWriter, IDisposable
    {
        private readonly Stream _stream;
        private readonly StreamWriter _writer;

        public SolutionFileWriter(string filePath)
        {
            _stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            _writer = new StreamWriter(_stream);

        }



        public SolutionFileWriter()
        {
            _stream = new MemoryStream();
            _writer = new StreamWriter(_stream);
        }

        public void WriteSolutionDeclaration()
        {
            _writer.WriteLine(new SolutionDeclaration());
        }

        public override void Write(string value)
        {
            _writer.Write(value);
        }
        public override void WriteLine(string value)
        {
            _writer.WriteLine(value);
        }

        public void WriteSectionStart(string sectionType, string parenthesizedName, string value)
        {
            WriteLine($"\t{sectionType}({parenthesizedName}) = {value}");
        }

        /// <summary>
        /// 
        ///     GlobalSection(SolutionProperties) = preSolution
        ///             HideSolutionNode = FALSE
        ///     EndGlobalSection
        /// 
        /// </summary>
        public void WriteSolutionPropertiesSection()
        {
            WriteSectionStart("GlobalSection", "SolutionProperties", "preSolution");
            WriteProperty("HideSolutionNode", "FALSE");
            WriteSectionEnd("GlobalSection");
        }

        /// <summary>
        /// 
        ///     GlobalSection(ExtensibilityGlobals) = postSolution
        ///             SolutionGuid = {...}
        ///     EndGlobalSection
        /// 
        /// </summary>
        /// <param name="solutionGuid"></param>
        public void WriteExtensibilityGlobalsSection(Guid solutionGuid)
        {
            WriteSectionStart("GlobalSection", "ExtensibilityGlobals", "postSolution");
            WriteProperty("SolutionGuid", solutionGuid.ToString("B").ToUpper());
            WriteSectionEnd("GlobalSection");
        }

        /// <summary>
        /// 
        ///     GlobalSection(NestedProjects) = preSolution
        ///             {...} = {...}
        ///             ...
        ///     EndGlobalSection
        /// 
        /// </summary>
        /// <param name="pairs"></param>
        public void WriteNestedProjectsSection(params (Guid nestedProject, Guid solutionFolder)[] pairs)
        {
            WriteSectionStart("GlobalSection", "NestedProjects", "preSolution");
            if (pairs != null && pairs.Length > 0)
            {
                foreach (var tuple in pairs)
                {
                    WriteProperty(tuple.nestedProject.ToString("B").ToUpper(), tuple.solutionFolder.ToString("B").ToUpper());
                }
            }

            WriteSectionEnd("GlobalSection");
        }

        public void WriteSolutionConfigurationPlatforms(string[] platformConfigurations, params Guid[] notSolutionFolderGuids)
        {
            WritePreSolutionConfigurationPlatforms(platformConfigurations);
            WritePostSolutionConfigurationPlatforms(platformConfigurations, notSolutionFolderGuids);
        }

        public void WritePreSolutionConfigurationPlatforms(params string[] platformConfigurations)
        {
            WriteSectionStart("GlobalSection", "SolutionConfigurationPlatforms", "preSolution");
            foreach (string platformConfiguration in platformConfigurations)
            {
                WriteProperty(platformConfiguration);
            }
            WriteSectionEnd("GlobalSection");

        }

        public void WritePostSolutionConfigurationPlatforms(string[] platformConfigurations, params Guid[] notSolutionFolderGuids)
        {
            WriteSectionStart("GlobalSection", "SolutionConfigurationPlatforms", "postSolution");
            foreach (Guid guid in notSolutionFolderGuids)
            {
                foreach (string platformConfiguration in platformConfigurations)
                {
                    WriteProperty($"{guid.ToString("B").ToUpper()}.{platformConfiguration}.ActiveCfg", platformConfiguration);
                    WriteProperty($"{guid.ToString("B").ToUpper()}.{platformConfiguration}.Build.0", platformConfiguration);
                }

            }
            WriteSectionEnd("GlobalSection");

        }

        public void WriteProperty(string name)
        {
            WriteLine($"\t\t{name} = {name}");
        }
        public void WriteProperty(string name, string value)
        {
            WriteLine($"\t\t{name} = {value}");
        }

        public void WriteSectionEnd(string sectionType)
        {
            WriteLine($"\tEnd{sectionType}");
        }










        public void WriteSolutionFolderStart(string folderName, string folderPath, Guid folderGuid)
        {
            WriteProjectBlockStart(ProjectTypeGuids.SolutionFolderGuid, folderName, folderPath, folderGuid);
        }
        public void WriteLegacyCSharpProjectStart(string folderName, string folderPath, Guid folderGuid)
        {
            WriteProjectBlockStart(ProjectTypeGuids.LegacyCSharpProjectGuid, folderName, folderPath, folderGuid);
        }

        public void WriteCPSCSharpProjectStart(string folderName, string folderPath, Guid folderGuid)
        {
            WriteProjectBlockStart(ProjectTypeGuids.CSharpProjectGuid, folderName, folderPath, folderGuid);
        }
        public void WriteProjectBlockStart(string projectTypeGuid, string projectName, string projectPath, string projectGuid)
        {
            WriteProjectBlockStart(new Guid(projectTypeGuid), projectName, projectPath, new Guid(projectGuid));
        }
        public void WriteProjectBlockStart(Guid projectTypeGuid, string projectName, string projectPath, Guid projectGuid)
        {
            WriteLine($"Project(\"{projectTypeGuid.ToString("B").ToUpper()}\") = \"{projectName}\", \"{projectPath}\", \"{projectGuid.ToString("B").ToUpper()}\"");
        }

        public void WriteSolutionFolderEnd() => WriteProjectBlockEnd();
        public void WriteProjectBlockEnd()
        {
            WriteLine("EndProject");
        }

        public void WriteGlobalStart()
        {
            WriteLine("Global");
        }

        public void WriteSolutionConfigurationPlatformsSection(params string[] items)
        {
            SolutionConfigurationPlatformsSection section = new SolutionConfigurationPlatformsSection();
            foreach (var item in items)
            {
                section.Properties.Add(new NameValuePair(item));
            }
            WriteLine(section);
        }

        public void WriteGlobalEnd()
        {
            WriteLine("EndGlobal");
        }

        public void WriteSolutionItemsProjectSection(params string[] items)
        {

            SolutionItemsProjectSection section = new SolutionItemsProjectSection();
            foreach (var item in items)
            {
                section.Properties.Add(new NameValuePair(item));
            }
            WriteLine(section);
        }



        public override string ToString()
        {
            using (StreamReader reader = new StreamReader(_stream))
            {
                _writer.Flush();
                _stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        private abstract class SolutionNode
        {
            public abstract int IndentLevel { get; }

            public string GetIndentation()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < IndentLevel; i++)
                {
                    sb.Append('\t');
                }
                return sb.ToString();
            }
        }
        private sealed class SolutionCommentLine
        {
            public SolutionCommentLine(string commentText = "Visual Studio 15")
            {
                CommentText = commentText;
            }

            public string CommentStart => "# ";

            public string CommentText { get; }

            public static implicit operator string(SolutionCommentLine other)
                => other.CommentStart + other.CommentText;
        }
        private abstract class HeaderLine : SolutionNode
        {
            public abstract string LeftSideText { get; }
            public abstract string SeparatorText { get; }
            public abstract string RightSideText { get; }
        }
        private sealed class SolutionHeaderLine : HeaderLine
        {
            public override int IndentLevel => 0;

            public override string LeftSideText
            {
                get
                {
                    return "Microsoft Visual Studio Solution File";
                }
            }

            public override string SeparatorText
            {
                get
                {
                    return ", Format Version ";
                }
            }

            public override string RightSideText
            {
                get
                {
                    return "12.00";
                }
            }

            public Version FormatVersion
            {
                get
                {
                    return Version.Parse(RightSideText);
                }
            }

            public static implicit operator string(SolutionHeaderLine other) =>
                other.LeftSideText + other.SeparatorText + other.RightSideText;
        }
        private abstract class SolutionVersionLine : HeaderLine
        {
            public sealed override string SeparatorText => " = ";
        }
        private class VisualStudioVersionLine : SolutionVersionLine
        {
            public override int IndentLevel => 0;
            public override string LeftSideText
            {
                get
                {
                    return "VisualStudioVersion";
                }
            }

            public override string RightSideText
            {
                get
                {
                    return "15.0.26430.12";
                }
            }


            public static implicit operator string(VisualStudioVersionLine other) =>
                other.LeftSideText + other.SeparatorText + other.RightSideText;
        }
        private class MinimumVisualStudioVersionLine : SolutionVersionLine
        {
            public override int IndentLevel => 0;
            public override string LeftSideText
            {
                get
                {
                    return "MinimumVisualStudioVersion";
                }
            }

            public override string RightSideText
            {
                get
                {
                    return "10.0.40219.1";
                }
            }


            public static implicit operator string(MinimumVisualStudioVersionLine other) =>
                other.LeftSideText + other.SeparatorText + other.RightSideText;
        }
        private struct SolutionDeclaration
        {
            public SolutionHeaderLine Header => new SolutionHeaderLine();
            public SolutionCommentLine Comment => new SolutionCommentLine();
            public VisualStudioVersionLine VisualStudioVersion => new VisualStudioVersionLine();
            public MinimumVisualStudioVersionLine MinimumVisualStudioVersion => new MinimumVisualStudioVersionLine();


            public static implicit operator string(SolutionDeclaration other)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine(other.Header);
                sb.AppendLine(other.Comment);
                sb.AppendLine(other.VisualStudioVersion);
                sb.Append(other.MinimumVisualStudioVersion);
                return sb.ToString();
            }
        }
        private struct ParenthesizedName
        {
            public ParenthesizedName(string name)
            {
                Name = name;
            }

            public string OpenParenthesis
            {
                get
                {
                    return "(";
                }
            }
            public string Name { get; }

            public string CloseParenthesis
            {
                get
                {
                    return ")";
                }
            }

            public static implicit operator string(ParenthesizedName other) =>
                other.OpenParenthesis + other.Name + other.CloseParenthesis;

            public static implicit operator ParenthesizedName(string other)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < other.Length; i++)
                {
                    char c = other[i];
                    switch (c)
                    {
                        case '(':
                        case ')':
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
                return new ParenthesizedName(sb.ToString());
            }



        }
        private class NameValuePair : HeaderLine
        {
            public NameValuePair(string name) : this(name, name)
            {
            }

            public NameValuePair(string name, string value)
            {
                LeftSideText = name;
                RightSideText = value;
            }

            public override int IndentLevel => 2;
            public string Name => LeftSideText;
            public string Value => RightSideText;
            public override string LeftSideText { get; }
            public override string RightSideText { get; }
            public override string SeparatorText => " = ";

            public static implicit operator (string name, string value) (NameValuePair other)
            {
                return (other.Name, other.Value);
            }

            public static implicit operator string(NameValuePair other)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(other.GetIndentation());
                sb.Append(other.Name);
                sb.Append(other.SeparatorText);
                sb.Append(other.Value);
                return sb.ToString();
            }
        }
        private class NameValuePairCollection : ICollection<NameValuePair>
        {
            private readonly List<NameValuePair> _list;

            public NameValuePairCollection(params NameValuePair[] items)
            {
                _list = items?.ToList();
            }

            public NameValuePairCollection(IEnumerable<NameValuePair> list)
            {
                _list = list.ToList();
            }

            public Enumerator GetEnumerator() => new Enumerator(this);

            IEnumerator<NameValuePair> IEnumerable<NameValuePair>.GetEnumerator() => new EnumeratorImpl(this);

            private class EnumeratorImpl : IEnumerator<NameValuePair>
            {
                private readonly Enumerator _e;

                internal EnumeratorImpl(IEnumerable<NameValuePair> items)
                {
                    _e = new Enumerator(items.ToList());
                }

                public bool MoveNext()
                {
                    return _e.MoveNext();
                }

                public NameValuePair Current
                {
                    get
                    {
                        return _e.Current;
                    }
                }

                void IDisposable.Dispose()
                {
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return _e.Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    _e.Reset();
                }
            }

            public struct Enumerator
            {
                private readonly List<NameValuePair> _list;
                private int _index;

                internal Enumerator(IEnumerable<NameValuePair> list)
                {
                    _list = list.ToList();
                    _index = -1;
                }

                public bool MoveNext()
                {
                    int newIndex = _index + 1;
                    if (newIndex < _list.Count)
                    {
                        _index = newIndex;
                        return true;
                    }

                    return false;
                }

                public NameValuePair Current
                {
                    get
                    {
                        return _list[_index];
                    }
                }

                public void Reset()
                {
                    _index = -1;
                }

                public override bool Equals(object obj)
                {
                    throw new NotSupportedException();
                }

                public override int GetHashCode()
                {
                    throw new NotSupportedException();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_list).GetEnumerator();
            }

            public void Add(NameValuePair item)
            {
                _list.Add(item);
            }

            public void Clear()
            {
                _list.Clear();
            }

            public bool Contains(NameValuePair item)
            {
                return _list.Contains(item);
            }

            public void CopyTo(NameValuePair[] array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public bool Remove(NameValuePair item)
            {
                return _list.Remove(item);
            }

            public int Count
            {
                get
                {
                    return _list.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }
        }

        private abstract class SectionDeclaration : SolutionNode
        {
            public abstract string Type { get; }

            public abstract ParenthesizedName Name { get; }

            public abstract string Value { get; }

            public abstract NameValuePairCollection Properties { get; }
        }


        private class GlobalSectionDeclaration : SectionDeclaration
        {
            public GlobalSectionDeclaration(ParenthesizedName name, string value, NameValuePairCollection properties = null)
            {
                Name = name;
                Value = value;
                Properties = properties ?? new NameValuePairCollection();
            }
            public override string Type => "GlobalSection";
            public override ParenthesizedName Name { get; }

            public override string Value { get; }

            public override NameValuePairCollection Properties { get; }
            public sealed override int IndentLevel => 1;
        }

        private class SolutionConfigurationPlatformsSection : GlobalSectionDeclaration
        {
            public SolutionConfigurationPlatformsSection(params NameValuePair[] properties)
                : base("SolutionConfigurationPlatforms", "preSolution", new NameValuePairCollection(properties))
            {
            }

            public static implicit operator string(SolutionConfigurationPlatformsSection other)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(other.GetIndentation());
                sb.Append(other.Type);
                sb.Append(other.Name);
                sb.Append(" = ");
                sb.AppendLine(other.Value);
                foreach (NameValuePair property in other.Properties)
                {
                    sb.AppendLine(property);
                }
                sb.Append(other.GetIndentation());
                sb.Append("End");
                sb.Append(other.Type);
                return sb.ToString();
            }
        }



        private class ProjectSectionDeclaration : SectionDeclaration
        {
            public ProjectSectionDeclaration(ParenthesizedName name, NameValuePairCollection properties = null)
            {
                Name = name;
                Properties = properties ?? new NameValuePairCollection();
            }

            public override string Type => "ProjectSection";

            public override ParenthesizedName Name { get; }

            public override string Value => "preProject";

            public override NameValuePairCollection Properties { get; }

            public sealed override int IndentLevel => 1;
        }

        private class SolutionItemsProjectSection : ProjectSectionDeclaration
        {
            public SolutionItemsProjectSection(params NameValuePair[] properties)
                : base("SolutionItems", new NameValuePairCollection(properties))
            {
            }

            public static implicit operator string(SolutionItemsProjectSection other)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(other.GetIndentation());
                sb.Append(other.Type);
                sb.Append(other.Name);
                sb.Append(" = ");
                sb.AppendLine(other.Value);
                foreach (NameValuePair property in other.Properties)
                {
                    sb.AppendLine(property);
                }
                sb.Append(other.GetIndentation());
                sb.Append("End");
                sb.Append(other.Type);
                return sb.ToString();
            }
        }

        private class ProjectDeclaration
        {
            public ProjectDeclaration(
                Guid projectTypeGuid,
                string projectName,
                string projectPath,
                Guid projectGuid,
                List<SectionDeclaration> sections)
            {
                ProjectTypeGuid = projectTypeGuid;
                ProjectName = projectName;
                ProjectPath = projectPath;
                ProjectGuid = projectGuid;
                Sections = sections;
            }

            public int IndentLevel => 0;
            public Guid ProjectTypeGuid { get; }
            public string ProjectName { get; }
            public string ProjectPath { get; }
            public Guid ProjectGuid { get; }
            public List<SectionDeclaration> Sections { get; }
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public new void Dispose()
        {
            base.Dispose(true);
            _writer?.Dispose();
            _stream?.Dispose();
            
        }
    }


    public enum SectionType
    {
        GlobalSection,
        ProjectGlobalSection,
        SolutionGlobalSection,
    }

    public enum BuildStage
    {
        preSolution,

    }

    internal static class ProjectTypeGuids
    {
        public static readonly Guid SolutionFolderGuid = new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
        public static readonly Guid NETStandardProjectGuid = new Guid("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}");
        public static readonly Guid CSharpProjectGuid = new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
        public static readonly Guid LegacyCSharpProjectGuid = new Guid("{FF286327-C783-4F7A-AB73-9BCBAD0D4460}");
    }
}
