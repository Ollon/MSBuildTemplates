

namespace Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs
{
    public static class SolutionItemFactory
    {
        public static SolutionItemInfrastructure Create()
        {
            SolutionItem model = new SolutionItem();
            SolutionItemViewModel viewModel = new SolutionItemViewModel(model);
            SolutionItemView view = new SolutionItemView(viewModel);
            return new SolutionItemInfrastructure(model, viewModel, view);
        }
    }
}
