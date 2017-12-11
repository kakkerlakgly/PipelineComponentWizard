using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators;
using Microsoft.BizTalk.Wizard;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageDesignerProperties : WizardInteriorPage, IWizardControl
    {
        public event AddDesignerPropertyEvent AddDesignerPropertyEvent;
        private bool _isLoaded;



        public WzPageDesignerProperties()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }



        public bool NextButtonEnabled => true;

        public bool NeedSummary => false;

        private void AddDesignerProperty(DesignerVariable variable)
        {
            DesignerVariableEvent variablePair = new DesignerVariableEvent(variable);
            OnAddDesignerProperty(variablePair);
        }

        // The protected OnAddReceiveHandlerProperty method raises the event by invoking 
        // the delegates. The sender is always this, the current instance 
        // of the class.
        private void OnAddDesignerProperty(DesignerVariableEvent e)
        {
            if (e != null)
            {
                // Invokes the delegates. 
                AddDesignerPropertyEvent(this, e);
            }
        }

        private void WzPageDesignerProperties_Leave(object sender, EventArgs e)
        {
            try
            {
                foreach (DesignerVariable objItem in lstDesignerProperties.Items)
                {
                    AddDesignerProperty(objItem);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }
        }

        private void WzPageDesignerProperties_Load(object sender, EventArgs e)
        {
            try
            {
                if (_isLoaded)
                    return;

                cmbDesignerPropertyDataType.DataSource = DesignerVariableType.ToArray;
                cmbDesignerPropertyDataType.DisplayMember = nameof(Type.Name);
                cmbDesignerPropertyDataType.SelectedItem = typeof(string);

                _isLoaded = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }
            cmbDesignerPropertyDataType.Focus();
        }

        private bool VarNameAlreadyExists(string strValue)
        {
            return lstDesignerProperties.Items.Cast<DesignerVariable>()
                .Any(strObjVal => strObjVal.Name == strValue);
        }

        /// <summary>
        /// Resets all of the errorproviders when anything succeeds
        /// </summary>
        private void ResetAllErrProviders()
        {
            foreach (Control ctl in Controls)
            {
                errorProvider.SetError(ctl, "");
            }
        }

        private void cmdDesignerPropertyAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ResetAllErrProviders();
                if (!Regex.IsMatch(txtDesignerProperty.Text, @"^[_a-zA-Z][_a-zA-Z0-9]*$"))
                {
                    errorProvider.SetError(txtDesignerProperty,
                        "Please enter a valid name for the new property");
                    return;
                }
                if (VarNameAlreadyExists(txtDesignerProperty.Text))
                {
                    errorProvider.SetError(txtDesignerProperty,
                        "Please enter a unique name. No two properties can have the same name");
                    return;
                }
                lstDesignerProperties.Items.Add(new DesignerVariable(txtDesignerProperty.Text,
                    (Type) cmbDesignerPropertyDataType.SelectedItem));
                txtDesignerProperty.Clear();
                cmbDesignerPropertyDataType.SelectedItem = typeof(string);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }

        }

        private void cmbDesignerPropertyDataType_Changed(object sender, EventArgs e)
        {
            Type currentSelection = (Type)cmbDesignerPropertyDataType.SelectedItem;
            if (currentSelection.Name == "SchemaList")
            {
                lblHelpDesignerProperties.Text =
                    "SchemaList allows for a dialog to pick any number of referenced schemas";
                lblHelpDesignerProperties.Visible = true;
            }
            else if (currentSelection.Name == "SchemaWithNone")
            {
                lblHelpDesignerProperties.Text =
                    "SchemaWithNone allows for a dropdown listbox with referenced schemas, selecting one only";
                lblHelpDesignerProperties.Visible = true;
            }
            else
            {
                lblHelpDesignerProperties.Visible = false;
            }
        }

        private void cmdDesignerPropertyDel_Click(object sender, EventArgs e)
        {
            try
            {
                ResetAllErrProviders();

                if (lstDesignerProperties.SelectedIndex == -1) return;

                lstDesignerProperties.Items.RemoveAt(lstDesignerProperties.SelectedIndex);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }

        }
    }
}
