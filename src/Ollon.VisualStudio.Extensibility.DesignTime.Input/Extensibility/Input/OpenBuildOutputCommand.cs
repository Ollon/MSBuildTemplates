using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Ollon.VisualStudio.Extensibility.Input;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility.Input
{
    [Export(typeof(DynamicCommand))]
    public class OpenBuildOutputCommand : DynamicCommand
    {
        [ImportingConstructor]
        public OpenBuildOutputCommand([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider) 
            : base(serviceProvider, MenuCommands.OpenBuildOutputComand)
        {

        }

        [Import]
        internal IMSBuildOutputService Service { get; private set; }

        protected override void Execute()
        {
            Service.OpenOutput("OutputPath");
        }
    }
}
