using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VSLangProj;
using System.Linq;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators.CodeDom;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    /// <summary>
    /// Class (com-object) called by VS2003.NET to start a new pipeline component
    /// project.
    /// </summary>
    [ProgId("VSWizard.BizTalkPipeLineComponentWizard")]
    [Guid("4A8C4088-9461-4890-8741-FE017B111AA0")]
    public class BizTalkPipeLineWizard : IDTWizard
    {

        enum ContextOptions
        {
            WizardType,
            ProjectName,
            LocalDirectory,
            InstallationDirectory,
            FExclusive,
            SolutionName,
            Silent
        }

        private string _bizTalkInstallPath;
        private string _projectDirectory;
        private string _projectName;

        private const string ProjectNamespace = "MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard";
        private DTE _application;

        bool _fExclusive;
        private Solution _pipelineComponentSolution;

        private IDictionary<string, object> _wizardResults;
        private IDictionary<string, string> _designerProperties;

        public BizTalkPipeLineWizard()
        {
            const string bizTalkKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
            using (RegistryKey bizTalkReg = Registry.LocalMachine.OpenSubKey(bizTalkKey))
            {
                _bizTalkInstallPath = bizTalkReg.GetValue("InstallPath").ToString();
            }
        }

        /// <summary>
        /// Main function for wizard project. Calls the wizard-
        /// form and then delegates control to the createsolution function
        /// </summary>
        /// <param name="application"></param>
        /// <param name="hwndOwner"></param>
        /// <param name="contextParams"></param>
        /// <param name="customParams"></param>
        /// <param name="retval"></param>
        public void Execute(object application, int hwndOwner, ref object[] contextParams, ref object[] customParams,
            ref wizardResult retval)
        {
            _application = (DTE) application;

            try
            {
                PipeLineComponentWizardForm wizardForm = new PipeLineComponentWizardForm();
                if (wizardForm.ShowDialog() == DialogResult.OK)
                {
                    //Retrieve the wizard data
                    _wizardResults = wizardForm.WizardResults;
                    //_TransmitHandlerProperties = WizardForm.TransmitHandlerProperties;
                    _designerProperties = wizardForm.DesignerProperties;

                    //Create the solution
                    CreateSolution(_application, contextParams);
                    retval = wizardResult.wizardResultSuccess;
                }
                else
                {
                    retval = wizardResult.wizardResultCancel;
                }

            }
            catch (Exception err)
            {
                Trace.WriteLine(err.ToString());
                MessageBox.Show(err.ToString());
                retval = wizardResult.wizardResultFailure;
            }
        }

        /// <summary>
        /// Creates the solution and calls the functions to create the projects
        /// </summary>
        /// <param name="ideObject"></param>
        /// <param name="contextParams"></param>
        private void CreateSolution(DTE ideObject, object[] contextParams)
        {
            TraceAllValues(contextParams);

            //Get the "official" wizard results
            _projectDirectory = contextParams[(int) ContextOptions.LocalDirectory].ToString();
            _projectName = contextParams[(int) ContextOptions.ProjectName].ToString();
            _fExclusive = bool.Parse(contextParams[(int) ContextOptions.FExclusive].ToString());

            if (!_fExclusive) //New solution or existing?
            {
                // Get a reference to the solution from the IDE Object
                _pipelineComponentSolution = ideObject.Solution;
            }
            else
            {
                // Use the solution class to create a new solution 
                _pipelineComponentSolution = ideObject.Solution;
                _pipelineComponentSolution.Create(_projectDirectory, _projectName);
            }

            SaveSolution();

            //Create the projects
            CreateProject(_pipelineComponentSolution);

            SaveSolution();
        }

        private void SaveSolution()
        {
            if (!Directory.Exists(_projectDirectory)) Directory.CreateDirectory(_projectDirectory);

            // Save the solution file
            _pipelineComponentSolution.SaveAs(_pipelineComponentSolution.FileName.Length == 0
                ? (_projectDirectory + @"\" + _projectName + ".sln")
                : _pipelineComponentSolution.FileName);
        }

        /// <summary>
        /// Creates the designtime project and adds the appropriate files to the
        /// project.
        /// </summary>
        private void CreateProject(_Solution mySolution)
        {
            // first, retrieve the visual studio installation folder

            // notice no try/catch, we want exceptions bubbling up as there's really nothing
            // we can do about them here.

            // retrieve the BizTalk Server installation folder
            string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
            using (var registryKey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey))
            {
                _bizTalkInstallPath = registryKey.GetValue("InstallPath").ToString();
            }

            string projectTemplate;
            string projectFileName;
            string classFileExtension;

            ImplementationLanguages language =
                (ImplementationLanguages) _wizardResults[WizardValues.ImplementationLanguage];

            switch (language)
            {
                case ImplementationLanguages.CSharp:
                    projectTemplate = ((Solution2) mySolution).GetProjectTemplate("ClassLibrary.zip", "CSharp");
                    projectFileName = _projectName + ".csproj";
                    classFileExtension = ".cs";

                    break;
                case ImplementationLanguages.VbNet:
                    projectTemplate = ((Solution2) mySolution).GetProjectTemplate("ClassLibrary.zip", "VisualBasic");
                    projectFileName = _projectName + ".vbproj";
                    classFileExtension = ".vb";

                    break;
                default:
                    MessageBox.Show($"Language \"{language}\" not supported");
                    return;
            }


            var startingProjects = _application.Solution.Projects.Cast<Project>();

            // add the specified project to the solution
            mySolution.AddFromTemplate(projectTemplate, _projectDirectory, _projectName, _fExclusive);

            // Query for te added project
            var pipelineComponentProject = _application.Solution.Projects
                .Cast<Project>().First(p => startingProjects.All(s => s.UniqueName != p.UniqueName));

            // delete the Class1.cs|vb|jsharp|... the template adds to the project
            pipelineComponentProject.ProjectItems.Item("Class1" + classFileExtension).Delete();

            // adjust project properties
            pipelineComponentProject.Properties.Item("RootNameSpace").Value =
                (string) _wizardResults[WizardValues.Namespace];
            pipelineComponentProject.Properties.Item("AssemblyName").Value =
                (string) _wizardResults[WizardValues.ClassName];

            // Get a reference to the Visual Studio Project and 
            // use it to add a reference to the framework assemblies
            VSProject pipelineComponentVsProject = (VSProject) pipelineComponentProject.Object;


            pipelineComponentVsProject.References.Add("System.dll");
            pipelineComponentVsProject.References.Add("System.Xml.dll");
            pipelineComponentVsProject.References.Add("System.Drawing.dll");
            pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath,
                @"Microsoft.BizTalk.Pipeline.dll"));
            pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath,
                @"Microsoft.Biztalk.Messaging.dll"));
            pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath,
                @"Microsoft.BizTalk.Streaming.dll"));

            // add our resource bundle
            string resourceBundle = Path.Combine(_projectDirectory,
                ((string) _wizardResults[WizardValues.ClassName]) + ".resx");
            using (ResXResourceWriter resx = new ResXResourceWriter(resourceBundle))
            {
                resx.AddResource("COMPONENTNAME", _wizardResults[WizardValues.ComponentName] as string);
                resx.AddResource("COMPONENTDESCRIPTION", _wizardResults[WizardValues.ComponentDescription] as string);
                resx.AddResource("COMPONENTVERSION", _wizardResults[WizardValues.ComponentVersion] as string);
                resx.AddResource("COMPONENTICON", _wizardResults[WizardValues.ComponentIcon]);
            }

            pipelineComponentProject.ProjectItems.AddFromFile(resourceBundle);

            // get the enum value of our choosen component type
            ComponentTypes componentType;
            Enum.TryParse(_wizardResults[WizardValues.ComponentStage] as string, out componentType);

            // create our actual class
            string pipelineComponentSourceFile = Path.Combine(_projectDirectory,
                ((string) _wizardResults[WizardValues.ClassName]) + classFileExtension);
            PipelineComponentCodeGenerator.GeneratePipelineComponent(
                pipelineComponentSourceFile,
                (string) _wizardResults[WizardValues.Namespace],
                (string) _wizardResults[WizardValues.ClassName],
                (bool) _wizardResults[WizardValues.ImplementIProbeMessage],
                _designerProperties,
                componentType,
                (ImplementationLanguages) _wizardResults[WizardValues.ImplementationLanguage]);

            pipelineComponentProject.ProjectItems.AddFromFile(pipelineComponentSourceFile);

            #region add the component utilities class, if needed

            if (DesignerVariableType.SchemaListUsed)
            {
                string bizTalkUtilitiesFileName = "Microsoft.BizTalk.Component.Utilities.dll";
                using (Stream stream = GetType().Assembly
                    .GetManifestResourceStream(ProjectNamespace + "." + bizTalkUtilitiesFileName))
                {
                    using (FileStream fs = new FileStream(Path.Combine(_projectDirectory, bizTalkUtilitiesFileName),
                        FileMode.Create))
                    {
                        stream.CopyTo(fs);
                    }
                }


                pipelineComponentVsProject.References.Add(Path.Combine(_projectDirectory, bizTalkUtilitiesFileName));
            }

            #endregion

            // save the project file
            pipelineComponentVsProject.Project.Save(Path.Combine(_projectDirectory, projectFileName));

            // only do this if we're creating a new solution, otherwise the user
            // might get distracted from his/her current work
#if RELEASE
            if (this._FExclusive)
            {
#endif
            // retrieve the main code file from the added project
            ProjectItem item =
                pipelineComponentVsProject.Project.ProjectItems.Item(Path.GetFileName(pipelineComponentSourceFile));

            // let's open up the main code file and show it so the user can start editing
            Window mainSourceFile = item.Open();

            // set the editor to the newly created sourcecode
            mainSourceFile.Activate();
#if RELEASE
            }
#endif
        }

        /// <summary>
        /// This will dump all of the values in the namedvaluecollections
        /// and in the contextparams coll. to the debug window 
        /// (or better: debugview).
        /// </summary>
        /// <param name="contextParams"></param>
        private void TraceAllValues(object[] contextParams)
        {
            Trace.WriteLine("++ Start ContextParams");
            foreach (object o in contextParams)
            {
                Trace.WriteLine(o.ToString());
            }
            Trace.WriteLine("-- End ContextParams");

            Trace.WriteLine("++ Start _WizardResults");
            foreach (var o in _wizardResults)
            {
                // only trace strings
                Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
            }
            Trace.WriteLine("-- End _WizardResults");

            Trace.WriteLine("++ Start _DesignerProperties");
            foreach (var o in _designerProperties)
            {
                Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
            }
            Trace.WriteLine("-- End _DesignerProperties");

            Trace.WriteLine("++ Start _DesignerProperties");
            foreach (var o in _designerProperties)
            {
                Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
            }
            Trace.WriteLine("-- End _DesignerProperties");
        }
    }
}
