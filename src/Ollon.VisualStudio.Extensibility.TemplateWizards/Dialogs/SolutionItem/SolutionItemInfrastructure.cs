

namespace Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs
{
    public class SolutionItemInfrastructure
    {
        public SolutionItem Model { get; }

        public SolutionItemViewModel ViewModel { get; }

        public SolutionItemView View { get; }

        public SolutionItemInfrastructure(
            SolutionItem model,
            SolutionItemViewModel viewModel,
            SolutionItemView view
        )
        {
            Model = model;
            ViewModel = viewModel;
            View = view;
        }
    }
}
