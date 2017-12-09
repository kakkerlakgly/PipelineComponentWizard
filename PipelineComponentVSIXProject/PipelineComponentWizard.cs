using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using EnvDTE;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators.CodeDom;
using MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.Win32;

namespace PipelineComponentVSIXProject
{
    public class PipelineComponentWizard : IWizard
    {
        private IDictionary<string, object> _wizardResults;
        private IDictionary<string, Type> _designerProperties;
        private string _pipelineComponentSourceFile;
        private string _destinationdirectory;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            try
            {
                // add our resource bundle
                string resourceBundle = Path.Combine(_destinationdirectory,
                    (string) _wizardResults[WizardValues.ClassName] + ".resx");
                using (ResXResourceWriter resx = new ResXResourceWriter(resourceBundle))
                {
                    resx.AddResource("COMPONENTNAME", _wizardResults[WizardValues.ComponentName] as string);
                    resx.AddResource("COMPONENTDESCRIPTION",
                        _wizardResults[WizardValues.ComponentDescription] as string);
                    resx.AddResource("COMPONENTVERSION", _wizardResults[WizardValues.ComponentVersion] as string);
                    resx.AddResource("COMPONENTICON", _wizardResults[WizardValues.ComponentIcon]);
                }

                // get the enum value of our choosen component type
                Enum.TryParse(_wizardResults[WizardValues.ComponentStage] as string, out ComponentTypes componentType);

                // create our actual class
                _pipelineComponentSourceFile = Path.Combine(_destinationdirectory,
                    (string) _wizardResults[WizardValues.ClassName] + ".cs");
                PipelineComponentCodeGenerator.GeneratePipelineComponent(
                    _pipelineComponentSourceFile,
                    (string) _wizardResults[WizardValues.Namespace],
                    (string) _wizardResults[WizardValues.ClassName],
                    (bool) _wizardResults[WizardValues.ImplementIProbeMessage],
                    _designerProperties,
                    componentType,
                    ImplementationLanguages.CSharp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
                _destinationdirectory = replacementsDictionary["$destinationdirectory$"];

                string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
                using (var registryKey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey))
                {
                    if (registryKey != null)
                        replacementsDictionary.Add("$BTSINSTALLPATH$", registryKey.GetValue("InstallPath").ToString());
                    else throw new InvalidOperationException("BizTalk is not installed on this computer");
                }

                var wizardForm = new PipeLineComponentWizardForm();
                if (wizardForm.ShowDialog() == DialogResult.OK)
                {
                    //Retrieve the wizard data
                    _wizardResults = wizardForm.WizardResults;
                    //_TransmitHandlerProperties = WizardForm.TransmitHandlerProperties;
                    _designerProperties = wizardForm.DesignerProperties;

                    replacementsDictionary.Add("$pipelineComponentFileName$",
                        (string) _wizardResults[WizardValues.ClassName]);

                    replacementsDictionary.Add("$SchemaListUsed$",
                        _designerProperties.Values.Any(DesignerVariableType.IsSchemaList).ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
