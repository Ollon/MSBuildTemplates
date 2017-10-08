// -----------------------------------------------------------------------
// <copyright file="PathUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public static class PathUtilities
    {
        public static bool PathNeedsNormalization(string path)
        {
            if (path.Length == 0)
            {
                return false;
            }
            if (path.IndexOf("..", StringComparison.Ordinal) < 0 && path.IndexOf(".\\", StringComparison.Ordinal) < 0)
            {
                return path.IndexOf("\\\\", 1, StringComparison.Ordinal) >= 0;
            }
            return true;
        }

        public static string MakeRelative(string basePath, string path)
        {
            bool flag1 = PathNeedsNormalization(path);
            string uriString = EnsureTrailingSlash(basePath);
            bool flag2;
            if (!flag1 && path.StartsWith(uriString, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(uriString.Length);
                flag2 = false;
            }
            else
            {
                flag2 = Path.IsPathRooted(path);
            }
            if (flag2 && !flag1 && uriString.Length == path.Length + 1 && uriString.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            {
                path = string.Empty;
                flag2 = false;
            }
            string str;
            if (flag2 | flag1)
            {
                Uri baseUri;
                try
                {
                    baseUri = new Uri(uriString, UriKind.Absolute);
                }
                catch (UriFormatException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                Uri uriFromPath = CreateUriFromPath(CleanupLocalFilePath(path));
                Uri uri1;
                if (uriFromPath.IsAbsoluteUri)
                {
                    uri1 = baseUri.MakeRelativeUri(uriFromPath);
                }
                else if (uriFromPath.ToString().StartsWith("\\", StringComparison.Ordinal))
                {
                    uri1 = uriFromPath;
                }
                else
                {
                    Uri uri2 = new Uri(baseUri, uriFromPath);
                    uri1 = baseUri.MakeRelativeUri(uri2);
                }
                str = Uri.UnescapeDataString(uri1.IsAbsoluteUri ? uri1.LocalPath : uri1.ToString());
            }
            else
            {
                str = path;
            }
            return str.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static string CleanupLocalFilePath(string path)
        {
            path = path.Trim();
            if (path.Length > 1 && path.IndexOf("\\\\", 1, StringComparison.Ordinal) >= 0)
            {
                path = path.Substring(0, 1) + Regex.Replace(path.Substring(1), "\\\\+", "\\");
            }
            return path;
        }

        public static Uri CreateUriFromPath(string path)
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri result))
            {
                result = new Uri(path, UriKind.Relative);
            }
            return result;
        }

        public static string EnsureTrailingSlash(string path)
        {
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                path += Path.DirectorySeparatorChar.ToString();
            }
            return path;
        }
    }
}
