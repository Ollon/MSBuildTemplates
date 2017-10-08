// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    internal class LineScanner
    {
        private readonly string _line;
        private int _currentPosition;

        public LineScanner(string line)
        {
            _line = line;
        }

        public string ReadUpToAndEat(string delimiter)
        {
            int index = _line.IndexOf(delimiter, _currentPosition, StringComparison.Ordinal);

            if (index == -1)
            {
                return ReadRest();
            }
            else
            {
                string upToDelimiter = _line.Substring(_currentPosition, index - _currentPosition);
                _currentPosition = index + delimiter.Length;
                return upToDelimiter;
            }
        }

        public string ReadRest()
        {
            string rest = _line.Substring(_currentPosition);
            _currentPosition = _line.Length;
            return rest;
        }
    }
}