using System;

namespace Ollon.VisualStudio.Extensibility.Dialogs.Input
{
    public interface IActiveAware
    {
        bool IsActive { get; set; }

        event EventHandler IsActiveChanged;
    }
}
