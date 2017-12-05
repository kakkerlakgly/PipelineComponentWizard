using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VSLangProj;
using System.Linq;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	public delegate void AddWizardResultEvent(object sender,PropertyPairEvent e);
	public delegate void AddDesignerPropertyEvent(object sender,PropertyPairEvent e);
	
	/// <summary>
	/// List of constants to find values in the namevaluecollection
	/// </summary>
	internal class WizardValues
	{
		/// <summary>
		/// defines the version of the component, as entered by the user
		/// </summary>
		public const string ComponentVersion = "ComponentVersion";
		/// <summary>
		/// defines the classname, as entered by the user
		/// </summary>
		public const string ClassName = "ClassName";
		/// <summary>
		/// defines the description (single-line) of the component, as entered by the user
		/// </summary>
		public const string ComponentDescription = "ComponentDescription";
		/// <summary>
		/// defines the namespace in which the component should reside, as entered by the user
		/// </summary>
		public const string Namespace = "Namespace";
		/// <summary>
		/// defines the component name, as entered by the user
		/// </summary>
		public const string ComponentName = "ComponentName";
		/// <summary>
		/// defines the default namespace for the newly created project, as entered by the user
		/// </summary>
		public const string NewProjectNamespace = "NewProjectNamespace";
		/// <summary>
		/// defines the icon this component will display within the toolbox of Visual Studio
		/// </summary>
		public const string ComponentIcon = "ComponentIcon";
		/// <summary>
		/// defines the type of pipeline component the user wishes to have generated
		/// </summary>
		public const string PipelineType = "PipelineType";
		/// <summary>
		/// defines the stage in which the user would like it's generated
		/// pipeline component to reside
		/// </summary>
		public const string ComponentStage = "ComponentStage";
		/// <summary>
		/// defines whether the user wants to let the wizard implement the IProbeMessage
		/// interface, which allows the pipeline component to determine for itself whether
		/// it's interested in processing an inbound message
		/// </summary>
		public const string ImplementIProbeMessage = "ImplementIProbeMessage";
		/// <summary>
		/// defines the programming languages in which the pipeline component should
		/// be implemented, as choosen by the user
		/// </summary>
		public const string ImplementationLanguage = "ImplementationLanguage";
	}

	/// <summary>
	/// defines the types of pipeline components we support
	/// see SDK\Include\Pipeline_Int.idl
	/// </summary>
	internal enum ComponentTypes
	{
		/// <summary>
		/// links to CATID_Decoder
		/// </summary>
		Decoder = 0,
		/// <summary>
		/// links to CATID_DisassemblingParser
		/// </summary>
		DisassemblingParser,
		/// <summary>
		/// links to CATID_Validate
		/// </summary>
		Validate,
		/// <summary>
		/// links to CATID_PartyResolver
		/// </summary>
		PartyResolver,
		/// <summary>
		/// links to CATID_Any
		/// </summary>
		Any,

		/// <summary>
		/// links to CATID_Encoder
		/// </summary>
		Encoder,
		//PreAssembler,	// BUG: Pre-Assembler has no specific CATID associated
		/// <summary>
		/// links to CATID_AssemblingSerializer
		/// </summary>
		AssemblingSerializer,
	}

	/// <summary>
	/// defines the supported languages we generate sourcecode for
	/// </summary>
	internal enum ImplementationLanguages
	{
		CSharp = 0,
		VbNet = 1
	}

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
        private string _targetVsVersion;
		private string _projectDirectory;
		private string _projectName;

		private const string ProjectNamespace = "MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard";
		private DTE2 _application;

		bool _fExclusive;
		private Solution2 _pipelineComponentSolution;

		private IDictionary<string, object> _wizardResults;
		private IDictionary<string, object> _designerProperties;
		
		public BizTalkPipeLineWizard()
		{
			const string bizTalkKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
			RegistryKey bizTalkReg = Registry.LocalMachine.OpenSubKey(bizTalkKey);
			_bizTalkInstallPath = bizTalkReg.GetValue("InstallPath").ToString();
			bizTalkReg.Close();
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
		public void Execute(object application, int hwndOwner, ref object[] contextParams, ref object[] customParams, ref wizardResult retval)
		{
			_DTE ideObject = (_DTE)application;
			_application = (DTE2)application;

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
					CreateSolution(ideObject,contextParams);
					retval = wizardResult.wizardResultSuccess;
				}
				else
				{
					retval = wizardResult.wizardResultCancel;
				}

			}
			catch(Exception err)
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
		private void CreateSolution(_DTE ideObject,object[] contextParams)
		{
			TraceAllValues(contextParams);

			//Get the "official" wizard results
			_projectDirectory = contextParams[(int)ContextOptions.LocalDirectory].ToString();
			_projectName = contextParams[(int)ContextOptions.ProjectName].ToString();
			_fExclusive = bool.Parse(contextParams[(int)ContextOptions.FExclusive].ToString());	

			if (!_fExclusive)//New solution or existing?
			{
				// Get a reference to the solution from the IDE Object
                _pipelineComponentSolution = (Solution2)ideObject.Solution;
			}
			else
			{
				// Use the solution class to create a new solution 
                _pipelineComponentSolution = (Solution2)ideObject.Solution;
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
            _pipelineComponentSolution.SaveAs(_pipelineComponentSolution.FileName.Length == 0 ? (_projectDirectory + @"\" + _projectName + ".sln") : _pipelineComponentSolution.FileName);
        }

		/// <summary>
		/// Creates the designtime project and adds the appropriate files to the
		/// project.
		/// </summary>
		private void CreateProject(Solution2 mySolution)
		{  
			// first, retrieve the visual studio installation folder

		    // notice no try/catch, we want exceptions bubbling up as there's really nothing
            // we can do about them here.

			// retrieve the BizTalk Server installation folder
			string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
		    using (var regkey1 = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey))
		    {
		        _bizTalkInstallPath = regkey1.GetValue("InstallPath").ToString();
		    }
		    // This is no longer present in the registry under the BizTalk Server key
            _targetVsVersion = "14.0"; // regkey.GetValue("TargetVSVersion").ToString();

			string projectTemplate;
			string projectFileName;
			string classFileExtension;
			
			ImplementationLanguages language = (ImplementationLanguages) _wizardResults[WizardValues.ImplementationLanguage];

			switch(language)
			{
				case ImplementationLanguages.CSharp:
					projectTemplate = mySolution.GetProjectTemplate("ClassLibrary.zip", "CSharp");
                    projectFileName = _projectName + ".csproj";
					classFileExtension = ".cs";
					
                    break;
				case ImplementationLanguages.VbNet:
                    projectTemplate = mySolution.GetProjectTemplate("ClassLibrary.zip", "VisualBasic");
                    projectFileName = _projectName + ".vbproj";
					classFileExtension = ".vb";
					
                    break;
				default:
					MessageBox.Show(String.Format("Language \"{0}\" not supported", language));
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
            pipelineComponentProject.Properties.Item("RootNameSpace").Value = (string)_wizardResults[WizardValues.Namespace];
            pipelineComponentProject.Properties.Item("AssemblyName").Value = (string)_wizardResults[WizardValues.ClassName];

			// Get a reference to the Visual Studio Project and 
			// use it to add a reference to the framework assemblies
			VSProject pipelineComponentVsProject = (VSProject)pipelineComponentProject.Object;

      
			pipelineComponentVsProject.References.Add("System.dll");
			pipelineComponentVsProject.References.Add("System.Xml.dll");
			pipelineComponentVsProject.References.Add("System.Drawing.dll");
			pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath, @"Microsoft.BizTalk.Pipeline.dll"));
			pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath, @"Microsoft.Biztalk.Messaging.dll"));
            pipelineComponentVsProject.References.Add(Path.Combine(_bizTalkInstallPath, @"Microsoft.BizTalk.Streaming.dll"));

			// add our resource bundle
			string resourceBundle = Path.Combine(_projectDirectory, ((string) _wizardResults[WizardValues.ClassName]) + ".resx");
			ResXResourceWriter resx = new ResXResourceWriter(resourceBundle);
			resx.AddResource("COMPONENTNAME", _wizardResults[WizardValues.ComponentName] as string);
			resx.AddResource("COMPONENTDESCRIPTION", _wizardResults[WizardValues.ComponentDescription] as string);
			resx.AddResource("COMPONENTVERSION", _wizardResults[WizardValues.ComponentVersion] as string);
			resx.AddResource("COMPONENTICON", _wizardResults[WizardValues.ComponentIcon]);
			resx.Close();

			pipelineComponentProject.ProjectItems.AddFromFile(resourceBundle);

			// get the enum value of our choosen component type
		    ComponentTypes componentType;
            Enum.TryParse( _wizardResults[WizardValues.ComponentStage] as string, out componentType);

			// create our actual class
			string pipelineComponentSourceFile = Path.Combine(_projectDirectory, ((string) _wizardResults[WizardValues.ClassName]) + classFileExtension);
			PipelineComponentCodeGenerator.GeneratePipelineComponent(
				pipelineComponentSourceFile,
				_wizardResults[WizardValues.Namespace] as string,
				_wizardResults[WizardValues.ClassName] as string,
				(bool) _wizardResults[WizardValues.ImplementIProbeMessage],
				_designerProperties,
				componentType,
				(ImplementationLanguages) _wizardResults[WizardValues.ImplementationLanguage]);

			pipelineComponentProject.ProjectItems.AddFromFile(pipelineComponentSourceFile);

			#region add the component utilities class, if needed
			if(DesignerVariableType.SchemaListUsed)
			{
				string bizTalkUtilitiesFileName = "Microsoft.BizTalk.Component.Utilities.dll";
				Stream stream = GetType().Assembly.GetManifestResourceStream(ProjectNamespace  + "." + bizTalkUtilitiesFileName);
				using(BinaryReader br = new BinaryReader(stream))
				{
					using(FileStream fs =  new FileStream(Path.Combine(_projectDirectory, bizTalkUtilitiesFileName), FileMode.Create))
					{
						// temporary storage
						byte[] b;

						// read the stream in blocks of ushort.MaxValue
						while((b = br.ReadBytes(ushort.MaxValue)).Length > 0)
						{
							// write what we've read to the output file
							fs.Write(b, 0, b.Length);
						}
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
                ProjectItem item = pipelineComponentVsProject.Project.ProjectItems.Item(Path.GetFileName(pipelineComponentSourceFile));

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
			foreach(object o in contextParams)
			{
				Trace.WriteLine(o.ToString());
			}
			Trace.WriteLine("-- End ContextParams");

			Trace.WriteLine("++ Start _WizardResults");
			foreach(var o in _wizardResults)
			{
                // only trace strings
			    Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
			}
            Trace.WriteLine("-- End _WizardResults");

			Trace.WriteLine("++ Start _DesignerProperties");
			foreach(var o in _designerProperties)
			{
			    Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
			}
            Trace.WriteLine("-- End _DesignerProperties");

			Trace.WriteLine("++ Start _DesignerProperties");
			foreach(var o in _designerProperties)
			{
        		Trace.WriteLine("Name:" + o.Key + " - Value = " + o.Value);
			}
			Trace.WriteLine("-- End _DesignerProperties");
		}
	}

    public class DteHandle
    {
        //EnvDTE.Project proj;
        //EnvDTE.Configuration config;
        //EnvDTE.Properties configProps;
        //EnvDTE.Property prop;
        readonly DTE _dte = Marshal.GetActiveObject("VisualStudio.DTE.14.0") as DTE;
        public Project GetProject(String name)
        {
            foreach (Project item in _dte.Solution.Projects)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
