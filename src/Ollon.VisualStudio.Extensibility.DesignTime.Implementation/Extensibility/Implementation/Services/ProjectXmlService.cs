using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.Shell;
using Ollon.VisualStudio.Extensibility.Services;
using Task = System.Threading.Tasks.Task;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IProjectXmlService))]
    internal class ProjectXmlService : IProjectXmlService
    {

        [Import]
        internal MSBuildProjectCollection ProjectCollection { get; private set; }


        public async Task SetExplicitSdkImportsIfNecessaryAsync(string filePath)
        {
            XDocument xdoc = XDocument.Load(filePath);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (xdoc.Root.HasAttributes && xdoc.Root.Attribute("Sdk") != null)
            {
                xdoc.Root.Attribute("Sdk").Remove();
                xdoc.Save(filePath);

                ProjectRootElement root = ProjectRootElement.Open(filePath, ProjectCollection, true);
                ProjectElement firstChild = root.FirstChild;
                ProjectElement lastChild = root.LastChild;
                ProjectImportElement sdkPropsImportElement = root.CreateImportElement("Sdk.props");
                sdkPropsImportElement.Sdk = "Microsoft.NET.Sdk";
                ProjectImportElement sdkTargetsImportElement = root.CreateImportElement("Sdk.targets");
                sdkTargetsImportElement.Sdk = "Microsoft.NET.Sdk";
                root.InsertBeforeChild(sdkPropsImportElement, firstChild);
                root.InsertAfterChild(sdkTargetsImportElement, lastChild);
                root.Save(filePath);
            }
        }
    }
}