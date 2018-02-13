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
        private bool _shouldAddProjectItem = true;
        private WizardValues _wizardResults;
        private IDictionary<string, Type> _designerProperties;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            string projectPath = Path.GetDirectoryName(project.FileName);
            var language = project.CodeModel.Language;
            try
            {
                if (_wizardResults != null)
                {
                    // add our resource bundle
                    string resourceBundle = Path.Combine(projectPath, _wizardResults.ClassName + ".resx");
                    CreateResourceFile(resourceBundle);

                    string pipelineComponentSourceFile = Path.Combine(projectPath, _wizardResults.ClassName);
                    CreateSourceFile(language, pipelineComponentSourceFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _shouldAddProjectItem = false;
            }
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            var filename = projectItem.FileNames[0];
            if (Path.GetExtension(filename) == ".resx")
            {
                CreateResourceFile(filename);
            }
            else
            {
                var language = projectItem.FileCodeModel.Language;
                CreateSourceFile(language, filename);
            }
        }

        private void CreateSourceFile(string language, string filename)
        {
            _wizardResults.ImplementationLanguage =
                language == CodeModelLanguageConstants.vsCMLanguageVB
                    ? ImplementationLanguages.VbNet
                    : ImplementationLanguages.CSharp;

            // create our actual class
            string pipelineComponentSourceFile = filename;
            var codeGenerator = new PipelineComponentCodeGenerator();
            codeGenerator.GeneratePipelineComponent(
                _wizardResults,
                pipelineComponentSourceFile,
                _designerProperties);
        }

        private void CreateResourceFile(string filename)
        {
            using (ResXResourceWriter resx = new ResXResourceWriter(filename))
            {
                resx.AddResource("COMPONENTNAME", _wizardResults.ComponentName);
                resx.AddResource("COMPONENTDESCRIPTION", _wizardResults.ComponentDescription);
                resx.AddResource("COMPONENTVERSION", _wizardResults.ComponentVersion);
                resx.AddResource("COMPONENTICON", _wizardResults.ComponentIcon);
            }
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
                replacementsDictionary.Add("$pipelineComponentFileName$", "PipelineComponent");
                replacementsDictionary.Add("$namespace$",
                    replacementsDictionary.TryGetValue("$safeprojectname$", out var safeProjectName)
                        ? safeProjectName
                        : replacementsDictionary["$rootnamespace$"]);

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

                using (var wizardForm = new PipeLineComponentWizardForm(replacementsDictionary))
                {
                    if (wizardForm.ShowDialog() == DialogResult.OK)
                    {
                        //Retrieve the wizard data
                        _wizardResults = wizardForm.WizardResult;
                        //_TransmitHandlerProperties = WizardForm.TransmitHandlerProperties;
                        _designerProperties = wizardForm.DesignerProperties;

                        replacementsDictionary["$pipelineComponentFileName$"] = _wizardResults.ClassName;
                        replacementsDictionary["$namespace$"] = _wizardResults.Namespace;

                        replacementsDictionary.Add("$SchemaListUsed$",
                            _designerProperties.Values.Any(DesignerVariableType.IsSchemaList).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _shouldAddProjectItem = false;
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return _shouldAddProjectItem;
        }
    }
}
