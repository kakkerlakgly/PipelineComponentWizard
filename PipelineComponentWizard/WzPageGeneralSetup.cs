using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MartijnHoogendoorn.BizTalk.Wizards.CodeGenerators;
using Microsoft.BizTalk.Wizard;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageGeneralSetup : WizardInteriorPage, IWizardControl
    {
        public WizardValues WizardValues;
        private readonly PipelineType[] _pipelineTypes = {
            PipelineType.Receive,
            PipelineType.Send,
            PipelineType.Any
        };

        private readonly ImplementationLanguages[] _implementationLanguageses = {
            ImplementationLanguages.CSharp,
            ImplementationLanguages.VbNet
        };
        private const string TransportRegEx = @"^[_a-zA-Z][_a-zA-Z0-9]*$";
        private const string NamespaceRegEx = @"(?i)^([a-z].?)*$";

        public WzPageGeneralSetup()
        {
           
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            cboImplementationLanguage.DataSource = _implementationLanguageses;
            cboImplementationLanguage.Visible = false;
            label5.Visible = false;

            cboPipelineType.DataSource = _pipelineTypes;
            cboPipelineType.SelectedIndex = -1;
            cboPipelineType.SelectedIndexChanged += PipelineType_Changed;
            cboPipelineType.SelectedValueChanged += Element_Changed;
        }

        /// <summary>
        /// Returns true if all fields are correctly entered
        /// </summary>
        /// <returns></returns>
        private bool GetAllStates()
        {
            return txtClassName.Text.Length > 0 && Regex.IsMatch(txtClassName.Text, TransportRegEx) &&
                   txtNameSpace.Text.Length > 0 && Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) &&
                   cboComponentStage.SelectedIndex > -1 && cboPipelineType.SelectedIndex > -1;
        }

        private void WzPageGeneralSetup_Leave(object sender, EventArgs e)
        {
            try
            {
                WizardValues.ClassName = txtClassName.Text;

                WizardValues.Namespace = txtNameSpace.Text;
                WizardValues.PipelineType = (PipelineType) cboPipelineType.SelectedItem;
                WizardValues.ComponentStage = (ComponentType) cboComponentStage.SelectedItem;
                WizardValues.ImplementationLanguage = (ImplementationLanguages) cboImplementationLanguage.SelectedItem;
                WizardValues.ImplementIProbeMessage = chkImplementIProbeMessage.Checked;
            }
            catch (Exception err)
            {
#if DEBUG
                MessageBox.Show(err.Message);
#endif
                Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
            }
        }

        private void txtClassName_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(txtClassName.Text, TransportRegEx) && txtClassName.Text.Length > 0)
            {
                ErrProv.SetError(txtClassName,
                    "TransportType must start with a non-alphanumeric character and may only include special character '_'");
            }
            else
            {
                ErrProv.SetError(txtClassName, "");
            }
            EnableNext(GetAllStates());
        }

        private void txtNamespace_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(txtNameSpace.Text, NamespaceRegEx) && txtNameSpace.Text.Length > 0)
            {
                ErrProv.SetError(txtNameSpace,
                    "Namespace must be a valid identifier");
            }
            else
            {
                ErrProv.SetError(txtNameSpace, "");
            }
            EnableNext(GetAllStates());
        }

        public bool NextButtonEnabled => GetAllStates();

        public bool NeedSummary => false;

        private void PipelineType_Changed(object sender, EventArgs e)
        {
            cboComponentStage.Enabled = true;

            switch ((PipelineType)cboPipelineType.SelectedItem)
            {
                // do we have a receive pipeline component selected?
                case PipelineType.Receive:
                    cboComponentStage.DataSource = new[]
                    {
                        ComponentType.Decoder,
                        ComponentType.DisassemblingParser,
                        ComponentType.Validate,
                        ComponentType.PartyResolver,
                        ComponentType.Any
                    };
                    cboComponentStage.SelectedItem = ComponentType.Decoder;
                    break;
                case PipelineType.Send:
                    cboComponentStage.DataSource = new[]
                    {
                        ComponentType.Encoder,
                        ComponentType.AssemblingSerializer,
                        ComponentType.Any
                    };
                    cboComponentStage.SelectedItem = ComponentType.Encoder;
                    break;
                case PipelineType.Any:
                    cboComponentStage.Items.Add(ComponentType.Any);
                    cboComponentStage.Enabled = false;
                    cboComponentStage.SelectedItem = ComponentType.Any;
                    break;
                default:
                    throw new ArgumentException("Unsupported pipeline type selected");
            }
        }

        private void cboPipelineType_Validating(object sender, CancelEventArgs e)
        {
            EnableNext(GetAllStates());
        }

        private void cboComponentStage_Validating(object sender, CancelEventArgs e)
        {
            EnableNext(GetAllStates());
        }

        private void Element_Changed(object sender, EventArgs e)
        {
            EnableNext(GetAllStates());
        }

        private void cboComponentStage_Changed(object sender, EventArgs e)
        {
            // do we have a disassembler selected?
            // only disassemblers can implement IProbeMessage
            if ((ComponentType)cboComponentStage.SelectedItem ==
                ComponentType.DisassemblingParser)
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
