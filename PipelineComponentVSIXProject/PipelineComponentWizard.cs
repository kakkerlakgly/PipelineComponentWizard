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
        private WizardValues _wizardResults;
        private IDictionary<string, Type> _designerProperties;
        private string _destinationdirectory;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            try
            {
                if (_wizardResults != null)
                {
                    // add our resource bundle
                    string resourceBundle = Path.Combine(_destinationdirectory, _wizardResults.ClassName + ".resx");
                    using (ResXResourceWriter resx = new ResXResourceWriter(resourceBundle))
                    {
                        resx.AddResource("COMPONENTNAME", _wizardResults.ComponentName);
                        resx.AddResource("COMPONENTDESCRIPTION", _wizardResults.ComponentDescription);
                        resx.AddResource("COMPONENTVERSION", _wizardResults.ComponentVersion);
                        resx.AddResource("COMPONENTICON", _wizardResults.ComponentIcon);
                    }

                    _wizardResults.ImplementationLanguage =
                        project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageCSharp
                            ? ImplementationLanguages.CSharp
                            : ImplementationLanguages.VbNet;

                    // create our actual class
                    string pipelineComponentSourceFile = Path.Combine(_destinationdirectory, _wizardResults.ClassName);
                    var codeGenerator = new PipelineComponentCodeGenerator();
                    codeGenerator.GeneratePipelineComponent(
                        _wizardResults,
                        pipelineComponentSourceFile,
                        _designerProperties);
                }
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
                string btsinstallpath = Environment.GetEnvironmentVariable("BTSINSTALLPATH");
                if (!string.IsNullOrEmpty(btsinstallpath))
                {
                    replacementsDictionary.Add("$BTSINSTALLPATH$", btsinstallpath);
                }
                else
                {
                    string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
                    using (var registryKey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey))
                    {
                        if (registryKey != null)
                            replacementsDictionary.Add("$BTSINSTALLPATH$",
                                registryKey.GetValue("InstallPath").ToString());
                        else throw new InvalidOperationException("BizTalk is not installed on this computer");
                    }
                }

                var wizardForm = new PipeLineComponentWizardForm();
                if (wizardForm.ShowDialog() == DialogResult.OK)
                {
                    //Retrieve the wizard data
                    _wizardResults = wizardForm.WizardResults;
                    //_TransmitHandlerProperties = WizardForm.TransmitHandlerProperties;
                    _designerProperties = wizardForm.DesignerProperties;

                    replacementsDictionary.Add("$pipelineComponentFileName$", _wizardResults.ClassName);

                    replacementsDictionary.Add("$SchemaListUsed$",
                        _designerProperties.Values.Any(DesignerVariableType.IsSchemaList).ToString());
                }
                else
                {
                    replacementsDictionary.Add("$pipelineComponentFileName$", "Class1");
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
