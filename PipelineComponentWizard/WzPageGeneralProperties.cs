using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageGeneralProperties : Microsoft.BizTalk.Wizard.WizardInteriorPage, IWizardControl
	{
		public event AddWizardResultEvent AddWizardResultEvent;

		private const string ComponentVersionRegEx = @"[0-9]+\.[0-9]+$";
		private const string ComponentNameRegEx = @"(?i)^[a-z]+[0-9a-z]*$";

		public WzPageGeneralProperties()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		protected void AddWizardResult(string strName, object value)
		{
			PropertyPairEvent propertyPair = new PropertyPairEvent(strName, value);
			OnAddWizardResult(propertyPair);
		}

		// The protected OnRaiseProperty method raises the event by invoking 
		// the delegates. The sender is always this, the current instance 
		// of the class.
		protected virtual void OnAddWizardResult(PropertyPairEvent e)
		{
			if (e != null) 
			{
				// Invokes the delegates. 
				AddWizardResultEvent(this,e);
			}
		}

		public bool NextButtonEnabled
		{
			get {	return GetAllStates();	}
		}
		
		public bool NeedSummary
		{
			get {	return false;	}
		}

		private bool GetAllStates()
		{
			return (Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) &&
				Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx));
		}

		private void WzPageGeneralProperties_Leave(object sender, EventArgs e)
		{
			AddWizardResult(WizardValues.ComponentName, txtComponentName.Text);
			AddWizardResult(WizardValues.ComponentDescription, txtComponentDescription.Text);
			AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
			AddWizardResult(WizardValues.ComponentVersion, txtComponentVersion.Text);
		}
	
		private void ComponentIcon_DoubleClick(object sender, EventArgs e)
		{
			if(openFileDialog1.ShowDialog() == DialogResult.OK) 
			{
				ComponentIcon.Image = Image.FromFile(openFileDialog1.FileName);
				AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
			}
		}

		private void ComponentIcon_Click(object sender, EventArgs e)
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WzPageGeneralProperties));
			ComponentIcon.Image = ((Image)(resources.GetObject("ComponentIcon.Image")));
			AddWizardResult(WizardValues.ComponentIcon, ComponentIcon.Image);
		}

		private void txtComponentVersion_Validating(object sender, CancelEventArgs e)
		{
			if(!Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) && txtComponentVersion.Text.Length > 0)
			{
				ErrProv.SetError(txtComponentVersion,
					"ComponentVersion can only contain numbers and a single '.' character, e.g.: 1.0, 1.15 or 10.234");
			}
			else
			{
				EnableNext(GetAllStates());
				ErrProv.SetError(txtComponentVersion, "");
			}		
		}

		private void txtComponentName_Validating(object sender, CancelEventArgs e)
		{
			if(!Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx) && txtComponentName.Text.Length > 0)
			{
				ErrProv.SetError(txtComponentName,
					"txtComponentName can only contain alpha numeric characters and cannot start with a number");
			}
			else
			{
				EnableNext(GetAllStates());
				ErrProv.SetError(txtComponentName, "");
			}		
		}

		private void Element_Changed(object sender, EventArgs e)
		{
			EnableNext(GetAllStates());
		}
	}
}

