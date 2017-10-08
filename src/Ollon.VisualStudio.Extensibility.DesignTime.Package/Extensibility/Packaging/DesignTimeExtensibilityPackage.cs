using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Ollon.VisualStudio.Extensibility.Input;
using Task = System.Threading.Tasks.Task;

namespace Ollon.VisualStudio.Extensibility.Packaging
{
    
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuid)]
    [PackageRegistration(UseManagedResourcesOnly = true, RegisterUsing = RegistrationMethod.CodeBase, AllowsBackgroundLoading = true)]
    public sealed class DesignTimeExtensibilityPackage : AsyncPackage
    {
        public const string PackageGuid = "7aa84aca-6044-4e5a-8958-1c388379ba8f";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            IComponentModel componentModel = (IComponentModel) Package.GetGlobalService(typeof(SComponentModel));

            OleMenuCommandService service = (OleMenuCommandService)GetService(typeof(IMenuCommandService));

            IEnumerable<DynamicCommand> commands = componentModel.DefaultExportProvider.GetExportedValues<DynamicCommand>();

            foreach (DynamicCommand command in commands)
            {
                service.AddCommand(command);
            }
        }

    }
}
