

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    public class NewSolutionDialogInfrastructure
    {
        public NewSolutionDialog Model { get; }

        public NewSolutionDialogViewModel ViewModel { get; }

        public NewSolutionDialogView View { get; }

        public NewSolutionDialogInfrastructure(
            NewSolutionDialog model,
            NewSolutionDialogViewModel viewModel,
            NewSolutionDialogView view
        )
        {
            Model = model;
            ViewModel = viewModel;
            View = view;
        }
    }
}
