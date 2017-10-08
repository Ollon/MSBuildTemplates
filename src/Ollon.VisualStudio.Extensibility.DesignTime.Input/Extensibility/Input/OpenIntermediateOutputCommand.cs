using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Ollon.VisualStudio.Extensibility.Input;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility.Input
{
    [Export(typeof(DynamicCommand))]
    public class OpenIntermediateOutputCommand : DynamicCommand
    {
        [ImportingConstructor]
        public OpenIntermediateOutputCommand([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider, MenuCommands.OpenIntermediateOutput)
        {
        }

        [Import]
        internal IMSBuildOutputService Service { get; private set; }

        protected override void Execute()
        {
            Service.OpenOutput("IntermediateOutputPath");
        }
    }
}