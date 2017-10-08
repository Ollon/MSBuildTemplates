using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ollon.VisualStudio
{
    internal static class ProjectTypeGuids
    {
        public const string ExtensibilityFlavorProjectGuid = "{82b43b9b-a64c-4715-b499-d71e9ca2bd60};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";


        public const string SolutionFolderGuidString = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
        public static readonly Guid SolutionFolderGuid = new Guid(SolutionFolderGuidString);

        public const string LegacyCSharpProjectTypeGuidString = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public static readonly Guid LegacyCSharpProjectTypeGuid = new Guid(LegacyCSharpProjectTypeGuidString);

        public const string CpsCSharpProjectTypeGuidString = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
        public static readonly Guid CpsCSharpProjectTypeGuid = new Guid(CpsCSharpProjectTypeGuidString);
    }
}
