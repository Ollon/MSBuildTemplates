

namespace Ollon.VisualStudio.Extensibility.Dialogs
{
    public class NewSolutionDialogFactory
    {
        public NewSolutionDialogInfrastructure Create()
        {
            NewSolutionDialog model = new NewSolutionDialog();
            NewSolutionDialogViewModel viewModel = new NewSolutionDialogViewModel(model);
            NewSolutionDialogView view = new NewSolutionDialogView(viewModel);
            return new NewSolutionDialogInfrastructure(model, viewModel, view);
        }
    }
}
