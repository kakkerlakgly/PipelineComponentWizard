using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
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

        private void AddDesignerProperty(string strName, string strValue)
        {
            PropertyPairEvent propertyPair = new PropertyPairEvent(strName, strValue);
            OnAddDesignerProperty(propertyPair);
        }

        // The protected OnAddReceiveHandlerProperty method raises the event by invoking 
        // the delegates. The sender is always this, the current instance 
        // of the class.
        private void OnAddDesignerProperty(PropertyPairEvent e)
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
                foreach (object objItem in lstDesignerProperties.Items)
                {
                    string strVal = objItem.ToString();

                    string strPropName = strVal.Substring(0, strVal.IndexOf("(", StringComparison.Ordinal) - 1);

                    string strPropType = strVal.Replace(strPropName + " (", string.Empty);
                    strPropType = strPropType.Replace(")", string.Empty);

                    AddDesignerProperty(strPropName, strPropType);
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

                cmbDesignerPropertyDataType.Items.AddRange(DesignerVariableType.ToArray());
                //cmbDesignerPropertyDataType.Text = strDataTypes[0];
                cmbDesignerPropertyDataType.SelectedIndex = 0;

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
            foreach (object o in lstDesignerProperties.Items)
            {
                string strObjVal = o.ToString();
                strObjVal = strObjVal.Remove(strObjVal.IndexOf(" (", StringComparison.Ordinal),
                    strObjVal.Length - strObjVal.IndexOf(" (", StringComparison.Ordinal));
                if (strObjVal == strValue)
                    return true;
            }
            return false;
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
                lstDesignerProperties.Items.Add(
                    txtDesignerProperty.Text + " (" + cmbDesignerPropertyDataType.Text + ")");
                txtDesignerProperty.Clear();
                cmbDesignerPropertyDataType.Text = "string";

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }

        }

        private void cmbDesignerPropertyDataType_Changed(object sender, EventArgs e)
        {
            string currentSelection =
                cmbDesignerPropertyDataType.Items[cmbDesignerPropertyDataType.SelectedIndex].ToString();
            if (currentSelection == "SchemaList")
            {
                lblHelpDesignerProperties.Text =
                    "SchemaList allows for a dialog to pick any number of referenced schemas";
                lblHelpDesignerProperties.Visible = true;
            }
            else if (currentSelection == "SchemaWithNone")
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
