using System;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.Wizard;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageSummary : WizardCompletionPage, IWizardControl
    {
        private string _summary;

        public WzPageSummary()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public bool NextButtonEnabled => true;

        public bool NeedSummary => true;

        public string Summary
        {
            private get { return _summary; }
            set
            {
                _summary = value;
                textBoxSubTitle.Text = Summary;
            }

        }

        private void WzPageSummary_Load(object sender, EventArgs e)
        {
        }

        private void WzPageSummary_VisibleChanged(object sender, EventArgs e)
        {
        }
    }
}
