// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    /// <summary>
    /// Represents a SectionBlock in a .sln file. Section blocks are of the form:
    /// 
    /// Type(ParenthesizedName) = Value
    ///     Key = Value
    ///     [more keys/values]
    /// EndType
    /// </summary>
    public sealed partial class SectionBlock
    {
        private readonly string _type;
        private readonly string _parenthesizedName;
        private readonly string _value;

        public SectionBlock(string type, string parenthesizedName, string value, IEnumerable<SolutionProperty> keyValuePairs)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(parenthesizedName))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            _type = type;
            _parenthesizedName = parenthesizedName;
            _value = value;
            Properties = keyValuePairs.ToList().AsReadOnly();
        }

        public string Type
        {
            get { return _type; }
        }

        public string ParenthesizedName
        {
            get { return _parenthesizedName; }
        }

        public string Value
        {
            get { return _value; }
        }

        public IEnumerable<SolutionProperty> Properties { get; }

        internal string GetText(int indent)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('\t', indent);
            builder.AppendFormat("{0}({1}) = ", Type, ParenthesizedName);
            builder.AppendLine(Value);

            foreach (SolutionProperty pair in Properties)
            {
                builder.Append('\t', indent + 1);
                builder.Append(pair.Name);
                builder.Append(" = ");
                builder.AppendLine(pair.Value);
            }

            builder.Append('\t', indent);
            builder.AppendFormat("End{0}", Type);
            builder.AppendLine();

            return builder.ToString();
        }

        internal static SectionBlock Parse(TextReader reader)
        {
            string startLine;
            while ((startLine = reader.ReadLine()) != null)
            {
                startLine = startLine.TrimStart(null);
                if (startLine != string.Empty)
                {
                    break;
                }
            }

            LineScanner scanner = new LineScanner(startLine);

            string type = scanner.ReadUpToAndEat("(");
            string parenthesizedName = scanner.ReadUpToAndEat(") = ");
            string sectionValue = scanner.ReadRest();

            List<SolutionProperty> properties = new List<SolutionProperty>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.TrimStart(null);

                // ignore empty lines
                if (line == string.Empty)
                {
                    continue;
                }

                if (line == "End" + type)
                {
                    break;
                }

                scanner = new LineScanner(line);
                string key = scanner.ReadUpToAndEat(" = ");
                string value = scanner.ReadRest();

                properties.Add(new SolutionProperty(key, value));
            }

            return new SectionBlock(type, parenthesizedName, sectionValue, properties);
        }
    }
}
