using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility
{
    public static class SaveSolutionOptions
    {
        public static uint ForceSave = (uint) __VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave;
    }
}