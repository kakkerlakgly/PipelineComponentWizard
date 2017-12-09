using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.Wizard;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageGeneralProperties : WizardInteriorPage, IWizardControl
    {
        private readonly WizardValues _wizardValues;

        private const string ComponentVersionRegEx = @"[0-9]+\.[0-9]+$";
        private const string ComponentNameRegEx = @"(?i)^[a-z]+[0-9a-z]*$";

        public WzPageGeneralProperties(WizardValues wizardValues)
        {
            _wizardValues = wizardValues;
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }
        public bool NextButtonEnabled => GetAllStates();

        public bool NeedSummary => false;

        private bool GetAllStates()
        {
            return (Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) &&
                    Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx));
        }

        private void WzPageGeneralProperties_Leave(object sender, EventArgs e)
        {
            _wizardValues.ComponentName = txtComponentName.Text;
            _wizardValues.ComponentDescription = txtComponentDescription.Text;
            _wizardValues.ComponentIcon = ComponentIcon.Image;
            _wizardValues.ComponentVersion = txtComponentVersion.Text;
        }

        private void ComponentIcon_DoubleClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ComponentIcon.Image = Image.FromFile(openFileDialog1.FileName);
                _wizardValues.ComponentIcon = ComponentIcon.Image;
            }
        }

        private void ComponentIcon_Click(object sender, EventArgs e)
        {
            System.Resources.ResourceManager resources =
                new System.Resources.ResourceManager(typeof(WzPageGeneralProperties));
            ComponentIcon.Image = (Image) resources.GetObject("ComponentIcon.Image");
            _wizardValues.ComponentIcon= ComponentIcon.Image;
        }

        private void txtComponentVersion_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(txtComponentVersion.Text, ComponentVersionRegEx) && txtComponentVersion.Text.Length > 0)
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
            if (!Regex.IsMatch(txtComponentName.Text, ComponentNameRegEx) && txtComponentName.Text.Length > 0)
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
