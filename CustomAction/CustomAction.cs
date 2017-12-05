using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.Installation
{
	/// <summary>
	/// basically installs (registers) and removes (unregisters and cleans up) 
	/// the BizTalk Pipeline Component Wizard
	/// </summary>
	[RunInstaller(true)]
	[ComVisible(false)]
    public class CustomActions : Installer
	{
		/// <summary>
		/// contains the BizTalk Server [version] installation folder
		/// </summary>
		private string _BizTalkInstallPath = null;
        /// <summary>
        /// contains the retrieved BizTalk Server target Visual Studio version from registry.
        /// </summary>
        private string _TargetVSVersion = null;
        /// <summary>
		/// contains the Visual Studio Wizard definition file location
		/// </summary>
		private string _BizTalkVszFileLocation = null;
		/// <summary>
		/// contains the path to the running .NET framework version for use of RegAsm.exe
		/// </summary>
		private string _DotNetFrameworkPath = null;
		/// <summary>
		/// stores any exception that might occur for review
		/// </summary>
		private Exception _Exception = null;
		/// <summary>
		/// defines whether the occured exception is a 'general' exception
		/// </summary>
		private bool _GeneralError = false;
		/// <summary>
		/// contains the Visual Studio installation folder
		/// </summary>
		private string _VisualStudioInstallPath = null;
		/// <summary>
		///  contains the path to the base folder where the Wizard definition file resides
		/// </summary>
		private string _VsDirPath = null;
		/// <summary>
		/// defines the Wizard definition file
		/// </summary>
		private const string vszFile = "BizTalkPipeLineComponentWizard.vsz";

		/// <summary>
		/// plain constructor, determines the locations of various of the used components
		/// (BizTalk Server, .NET framework, Visual Studio)
		/// </summary>
		public CustomActions()
		{
			// regkey will contain the opened registry key for retrieving data
			RegistryKey regkey;

            
			try
			{
				// retrieve the BizTalk Server installation folder
				string bizTalkInstallRegistryKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
				regkey = Registry.LocalMachine.OpenSubKey(bizTalkInstallRegistryKey);

				try
				{
					_BizTalkInstallPath = regkey.GetValue("InstallPath").ToString();
                    _TargetVSVersion = "14.0"; //regkey.GetValue("TargetVSVersion").ToString();
                    _BizTalkVszFileLocation = Path.Combine(_BizTalkInstallPath, string.Format(@"Developer Tools\BizTalkProjects\{0}", vszFile));

					regkey.Close();
				}
				catch
				{
                    Context.LogMessage(string.Format(@"Unable to locate BizTalk installation folder from registry. Tried InstallPath (2006/2009/2010/2013/2013 R2/2016) in HKLM\{0}", bizTalkInstallRegistryKey));
				}

				// Visual studio installation folder
				_VsDirPath = Path.Combine(_BizTalkInstallPath, @"Developer Tools\BizTalkProjects\BTSProjects.vsdir");
				string vsInstallFolderRegistryKey = string.Format(@"SOFTWARE\Microsoft\VisualStudio\{0}_Config", _TargetVSVersion);

				try
				{
					regkey = Registry.CurrentUser.OpenSubKey(vsInstallFolderRegistryKey);

					// set the actual Visual Studio installation folder for later use
					_VisualStudioInstallPath = regkey.GetValue("InstallDir").ToString();

					regkey.Close();
				}
				catch
				{
					Context.LogMessage(string.Format("Unable to find Visual Studio installation path for version {0}", _TargetVSVersion));
				}

				// .NET framework installation folder
				regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
				_DotNetFrameworkPath = regkey.GetValue("InstallRoot").ToString();
				string frameworkVersion = string.Format("v{0}.{1}.{2}", Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build);

				// the path to the .NET framework folder is in the form vx.y.z, where x.y.z is Major, Minor and Build
				// version of the framework. within the folder defined in HKLM\SOFTWARE\Microsoft\.NETFramework
				_DotNetFrameworkPath = Path.Combine(_DotNetFrameworkPath, frameworkVersion);

				regkey.Close();

			}
			catch(Exception e)
			{
				Context.LogMessage(e.Message);

				_Exception = e;
				_GeneralError = true;
			}
		}

        /// <summary>
		/// actually registers our wizard within the Visual Studio environment by adding a line to BTSProjects.vsdir
		/// </summary>
		/// <returns>whether the action actually succeeded</returns>
		private bool AddVsDirLine(bool removeLine = false)
        {
            string vsDirLine;
            string definitionBuffer;

            try
            {

                vsDirLine = vszFile + "| |Pipeline Component Project|300|Creates a BizTalk Server Pipeline Component|{7a51b143-7eea-450d-baef-827253c52e43}|400| |PipelineComponent";
                // reset file attributes
                if ((File.GetAttributes(_VsDirPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(_VsDirPath, FileAttributes.Normal);
                }
                using (StreamReader reader = new StreamReader(_VsDirPath))
                {
                    definitionBuffer = reader.ReadToEnd().TrimEnd();
                }

                if (removeLine)
                {
                    definitionBuffer = definitionBuffer.Replace(vsDirLine, string.Empty).TrimEnd() + "\r\n";
                }
                else
                {
                    // only append the wizard line of not present
                    if (definitionBuffer.IndexOf(vszFile) == -1)
                    {
                        definitionBuffer += "\r\n" + vsDirLine + "\r\n";

                    }
                }

                var tempFile = string.Format("{0}.tmp", _VsDirPath);

                File.WriteAllText(tempFile, definitionBuffer);

                File.Copy(tempFile, _VsDirPath, true);

                File.Delete(tempFile);

                // set the RO flag to the file
                File.SetAttributes(_VsDirPath, FileAttributes.ReadOnly);
            }
            catch (Exception e)
            {
                return HandleError("AddVsDirLine", e);
            }

            return true;
        }


        /// <summary>
        /// creates the .vsz file. existing file is removed if need be
        /// </summary>
        /// <returns>whether the operation succeeded</returns>
        private bool AddVszFile()
		{
			try
			{
				RemoveVszFile();

				using (TextWriter writer = new StreamWriter(_BizTalkVszFileLocation, false))
				{
					writer.WriteLine("VSWIZARD 7.0");
					writer.WriteLine("Wizard=VSWizard.BizTalkPipeLineComponentWizard");
					writer.WriteLine("Param=\"WIZARD_NAME = BizTalkPipeLineComponentWizard\"");
					writer.WriteLine("Param=\"WIZARD_UI = FALSE\"");
					writer.WriteLine("Param=\"PROJECT_TYPE = CSPROJ\"");
				}
			}
			catch (Exception e)
			{
				return HandleError("AddVszFile", e);
			}
			return true;
		}

		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);

			try
			{
				if (!_GeneralError)
				{
					return;
				}
				throw _Exception;
			}
			catch (Exception e)
			{
				Context.LogMessage(e.Message);

				throw;
			}
		}

		private bool HandleError(string functionName, Exception e)
		{
			Context.LogMessage(e.Message);

			DialogResult result2 = MessageBox.Show(e.ToString(), "PipelineComponentWizard Installer", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

			switch (result2)
			{
				case DialogResult.Abort:
				{
					throw e;
				}
				case DialogResult.Retry:
				{
					return false;
				}
				case DialogResult.Ignore:
				{
					if (MessageBox.Show("If you choose to ignore you will have to perform some actions manually as described in the readme. " + Environment.NewLine + "Continue?", "PipelineComponentWizard Installer - " + functionName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						throw e;
					}
					break;
				}
			}

			return true;
		}

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			
			try
			{
				if (_GeneralError)
				{
					throw _Exception;
				}
				while (!AddVszFile())
				{
				}
				while (!AddVsDirLine())
				{
				}
				while (!RegisterPipelineComponentWizard(false))
				{
				}
			}
			catch (Exception e)
			{
				Context.LogMessage(e.Message);

				throw;
			}
		}

		/// <summary>
		/// performs the necessery steps to either register or remove the wizard
		/// </summary>
		/// <param name="unregister">whether the installer is registering or removing the wizards</param>
		/// <returns>whether the operation succeeded</returns>
		private bool RegisterPipelineComponentWizard(bool unregister)
		{
			string regAsmLocation;
			string regAsmArguments;
			ProcessStartInfo piInfo;
			Process process;

			try
			{
				// we use RegAsm.exe by spawning it just like the command-line would
				regAsmLocation = Path.Combine(_DotNetFrameworkPath, "RegAsm.exe");

				// append /u if we're removing
				if (unregister)
				{
					regAsmLocation = regAsmLocation + " /u";
				}

				// format the RegAsm arguments
				regAsmArguments = string.Format("\"{0}\"", Path.Combine(Context.Parameters["ApplicationPath"], "PipelineComponentWizard.dll"));
				regAsmArguments += " /codebase /s";

				// create and run the command-line in the background
			    piInfo = new ProcessStartInfo(regAsmLocation, regAsmArguments)
			    {
			        CreateNoWindow = true,
			        WindowStyle = ProcessWindowStyle.Hidden
			    };
			    process = Process.Start(piInfo);
				process.WaitForExit();
			}
			catch (Exception e)
			{
				if (!unregister)
				{
					return HandleError("RegisterPipelineComponentWizard", e);
				}

				return true;
			}

			return true;
		}

		/// <summary>
		/// removes the wizard definition file
		/// </summary>
		private void RemoveVszFile()
		{
			FileInfo fi;
			try
			{
				fi = new FileInfo(_BizTalkVszFileLocation);
				if (!fi.Exists)
				{
					return;
				}
				fi.Delete();
			}
			catch (Exception e)
			{
				Context.LogMessage(e.Message);
			}
		}

		public override void Rollback(IDictionary savedState)
		{
			try
			{
				if (_GeneralError)
				{
					throw _Exception;
				}

				RemoveVszFile();
			}
			catch (Exception e)
			{
				Context.LogMessage(e.Message);
			}
			base.Rollback(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			try
			{
				if (_GeneralError)
				{
					throw _Exception;
				}

				RemoveVszFile();
				RegisterPipelineComponentWizard(true);
			}
			catch (Exception e)
			{
				Context.LogMessage(e.Message);
			}
			base.Uninstall(savedState);
		}
	}
}