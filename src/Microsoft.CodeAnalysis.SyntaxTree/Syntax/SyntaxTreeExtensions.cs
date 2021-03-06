﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    internal static class SyntaxTreeExtensions
    {
        /// <summary>
        /// Verify nodes match source.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void VerifySource(this SyntaxTree tree, IEnumerable<TextChangeRange> changes = null)
        {
            SyntaxNode root = tree.GetRoot();
            SourceText text = tree.GetText();
            TextSpan fullSpan = new TextSpan(0, text.Length);
            SyntaxNode node = null;

            // If only a subset of the document has changed,
            // just check that subset to reduce verification cost.
            if (changes != null)
            {
                TextSpan change = TextChangeRange.Collapse(changes).Span;
                if (change != fullSpan)
                {
                    // Find the lowest node in the tree that contains the changed region.
                    node = root.DescendantNodes(n => n.FullSpan.Contains(change)).LastOrDefault();
                }
            }

            if (node == null)
            {
                node = root;
            }

            TextSpan span = node.FullSpan;
            TextSpan? textSpanOpt = span.Intersection(fullSpan);
            int index;

            if (textSpanOpt == null)
            {
                index = 0;
            }
            else
            {
                string fromText = text.ToString(textSpanOpt.Value);
                string fromNode = node.ToFullString();
                index = FindFirstDifference(fromText, fromNode);
            }

            if (index >= 0)
            {
                index += span.Start;
                string message;
                if (index < text.Length)
                {
                    LinePosition position = text.Lines.GetLinePosition(index);
                    TextLine line = text.Lines[position.Line];
                    string allText = text.ToString(); // Entire document as string to allow inspecting the text in the debugger.
                    message = string.Format("Unexpected difference at offset {0}: Line {1}, Column {2} \"{3}\"",
                        index,
                        position.Line + 1,
                        position.Character + 1,
                        line.ToString());
                }
                else
                {
                    message = "Unexpected difference past end of the file";
                }
                Debug.Assert(false, message);
            }
        }

        /// <summary>
        /// Return the index of the first difference between
        /// the two strings, or -1 if the strings are the same.
        /// </summary>
        private static int FindFirstDifference(string s1, string s2)
        {
            int n1 = s1.Length;
            int n2 = s2.Length;
            int n = Math.Min(n1, n2);
            for (int i = 0; i < n; i++)
            {
                if (s1[i] != s2[i])
                {
                    return i;
                }
            }
            return (n1 == n2) ? -1 : n + 1;
        }

        /// <summary>
        /// Returns <c>true</c> if the provided position is in a hidden region inaccessible to the user.
        /// </summary>
        public static bool IsHiddenPosition(this SyntaxTree tree, int position, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!tree.HasHiddenRegions())
            {
                return false;
            }

            LineVisibility lineVisibility = tree.GetLineVisibility(position, cancellationToken);
            return lineVisibility == LineVisibility.Hidden || lineVisibility == LineVisibility.BeforeFirstLineDirective;
        }
    }
}
