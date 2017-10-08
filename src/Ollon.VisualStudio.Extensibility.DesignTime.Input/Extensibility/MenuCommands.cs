// -----------------------------------------------------------------------
// <copyright file="MenuCommands.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Design;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility
{
    
    internal static class MenuCommands
    {
        public const int cmdidInitializeDirectoryBuildProps = 0x0100;
        public const int cmdidOpenBuildOutput = 0x0101;
        public const int cmdidOpenIntermediateOutput = 0x0102;
        public const int cmdidFixSdkImports = 0x0103;
        public const int cmdidNewSolution = 0x0104;

        public const string guidPackageCmdsString = "5444433f-3a1d-4e50-91ab-88aaac852258";

        public static Guid guidPackageCmds = new Guid(guidPackageCmdsString);

        public static readonly CommandID InitializeDirectoryBuildPropsCommand =  new CommandID(guidPackageCmds, cmdidInitializeDirectoryBuildProps);
        public static readonly CommandID FixSdkAttributeCommand = new CommandID(guidPackageCmds, cmdidFixSdkImports);
        public static readonly CommandID NewSolutionCommand = new CommandID(guidPackageCmds, cmdidNewSolution);
        public static readonly CommandID OpenBuildOutputComand = new CommandID(guidPackageCmds, cmdidOpenBuildOutput);
        public static readonly CommandID OpenIntermediateOutput = new CommandID(guidPackageCmds, cmdidOpenIntermediateOutput);
    }
}
