using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class WzPageSummary : Microsoft.BizTalk.Wizard.WizardCompletionPage, IWizardControl
	{
		private string _Summary = null;

		public WzPageSummary()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public bool NextButtonEnabled
		{
			get {	return true;	}
		}

		public bool NeedSummary
		{
			get {	return true;	}
		}

		public string Summary
		{
			get {	return _Summary;	}
			set 
			{
				_Summary = value;
				textBoxSubTitle.Text = Summary;
			}

		}

		private void WzPageSummary_Load(object sender, System.EventArgs e)
		{
		}

		private void WzPageSummary_VisibleChanged(object sender, System.EventArgs e)
		{
		}
	}
}

