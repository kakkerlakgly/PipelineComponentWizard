using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageGeneralSetup : Microsoft.BizTalk.Wizard.WizardInteriorPage, IWizardControl
	{
		private const string TransportRegEx = @"^[_a-zA-Z][_a-zA-Z0-9]*$";
		private const string NamespaceRegEx = @"(?i)^([a-z].?)*$";
		public event AddWizardResultEvent _AddWizardResultEvent;

        public WzPageGeneralSetup()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// re-clear all items from the stage dropdown
			cboComponentStage.Items.Clear();
		}

        protected void AddWizardResult(string strName, object Value)
		{
			PropertyPairEvent PropertyPair = new PropertyPairEvent(strName, Value);
			OnAddWizardResult(PropertyPair);
		}

		/// <summary>
		/// The protected OnRaiseProperty method raises the event by invoking 
		/// the delegates. The sender is always this, the current instance 
		/// of the class.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnAddWizardResult(PropertyPairEvent e)
		{
			if (e != null) 
			{
				// Invokes the delegates. 
				_AddWizardResultEvent(this,e);
			}
		}

		/// <summary>
		/// Returns true if all fields are correctly entered
		/// </summary>
		/// <returns></returns>
		private bool GetAllStates()
		{
			return (txtClassName.Text.Length > 0 && Regex.IsMatch(txtClassName.Text,TransportRegEx) &&
				txtNameSpace.Text.Length > 0 && Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) && 
				cboComponentStage.SelectedIndex > -1 && cboPipelineType.SelectedIndex > -1);
		}

		private void WzPageGeneralSetup_Leave(object sender, System.EventArgs e)
		{
			try
			{
				AddWizardResult(WizardValues.ClassName, txtClassName.Text);
				AddWizardResult(WizardValues.Namespace, txtNameSpace.Text);
				AddWizardResult(WizardValues.PipelineType, cboPipelineType.Items[cboPipelineType.SelectedIndex].ToString());
				AddWizardResult(WizardValues.ComponentStage, cboComponentStage.Items[cboComponentStage.SelectedIndex].ToString());
				AddWizardResult(WizardValues.ImplementationLanguage, (implementationLanguages) cboImplementationLanguage.SelectedIndex);
				AddWizardResult(WizardValues.ImplementIProbeMessage, chkImplementIProbeMessage.Checked);
			}
			catch(Exception err)
			{
#if DEBUG
				MessageBox.Show(err.Message);
#endif
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
		}

		private void txtClassName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Regex.IsMatch(txtClassName.Text,TransportRegEx) && txtClassName.Text.Length > 0)
			{
				ErrProv.SetError(txtClassName,
					"TransportType must start with a non-alphanumeric character and may only include special character '_'");
			}
			else
			{
				ErrProv.SetError(txtClassName,"");
			}		
			EnableNext(GetAllStates());	
		}

		private void txtNamespace_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) && txtNameSpace.Text.Length > 0)
			{
				ErrProv.SetError(txtNameSpace,
					"Namespace must be a valid identifier");
			}
			else
			{
				ErrProv.SetError(txtNameSpace,"");
			}
			EnableNext(GetAllStates());	
		}

		public bool NextButtonEnabled
		{
			get {	return GetAllStates();	}
		}

		public bool NeedSummary
		{
			get {	return false;	}
		}
        
		private void PipelineType_Changed(object sender, System.EventArgs e)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WzPageGeneralSetup));

			this.cboComponentStage.Items.Clear();
			this.cboComponentStage.Enabled = true;

			switch(cboPipelineType.SelectedIndex)
			{
				// do we have a receive pipeline component selected?
				case 0:
					this.cboComponentStage.Items.AddRange(new object[] 
					{
						componentTypes.Decoder.ToString(),
						componentTypes.DisassemblingParser.ToString(),
						componentTypes.Validate.ToString(),
						componentTypes.PartyResolver.ToString(),
						componentTypes.Any.ToString()
					});
					this.cboComponentStage.SelectedIndex = 0;
					break;
				case 1:
					this.cboComponentStage.Items.AddRange(new object[] 
					{
						componentTypes.Encoder.ToString(),
						componentTypes.AssemblingSerializer.ToString(),
						componentTypes.Any.ToString()
					});
					this.cboComponentStage.SelectedIndex = 0;
					break;
				case 2:
					this.cboComponentStage.Items.Add(componentTypes.Any.ToString());
					this.cboComponentStage.Enabled = false;
					this.cboComponentStage.SelectedIndex = 0;
					break;
				default:
					throw new ApplicationException("Unsupported pipeline type selected");
			}
		}

		private void cboPipelineType_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void cboComponentStage_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void Element_Changed(object sender, System.EventArgs e)
		{
			EnableNext(GetAllStates());
		}

		private void cboComponentStage_Changed(object sender, System.EventArgs e)
		{
			// do we have a disassembler selected?
			// only disassemblers can implement IProbeMessage
			if(cboComponentStage.Items[cboComponentStage.SelectedIndex].ToString() == componentTypes.DisassemblingParser.ToString())
			{
				chkImplementIProbeMessage.Visible = true;
			}
			else 
			{
				chkImplementIProbeMessage.Visible = false;
				chkImplementIProbeMessage.Checked = false;
			}
		}
	}
}