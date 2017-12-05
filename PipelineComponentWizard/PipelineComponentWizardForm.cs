using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.BizTalk.Wizard;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    [ComVisible(false)]
    public partial class PipeLineComponentWizardForm : WizardForm
	{
		//private NameValueCollection _WizardResults = new NameValueCollection();
		private IDictionary<string, object> _WizardResults = new Dictionary<string, object>();

	    private HelpProvider _HelpProvider = new HelpProvider();

		private IList<IWizardControl> _PageCollection = new List<IWizardControl>();
		private int _PageCount = 0;

        /// <summary>
		/// Constructor. Sets eventhandlers for inherited buttons and 
		/// custom events. Creates the page-collection. 
		/// </summary>
		public PipeLineComponentWizardForm()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			ButtonNext.Click += buttonNext_Click;
			ButtonBack.Click += buttonBack_Click;

			AddPage(wzPageWelcome1,false);
			AddPage(wzPageGeneralSetup1,false);
			AddPage(WzPageGeneralProperties1,false);
			AddPage(WzPageDesignerProperties1,false);
			AddPage(wzPageSummary1,false);

			_PageCollection.Add(wzPageWelcome1);
			_PageCollection.Add(wzPageGeneralSetup1);
			_PageCollection.Add(WzPageGeneralProperties1);
			_PageCollection.Add(WzPageDesignerProperties1);
			_PageCollection.Add(wzPageSummary1);

			wzPageGeneralSetup1._AddWizardResultEvent += AddWizardResult;
			WzPageDesignerProperties1._AddDesignerPropertyEvent +=AddDesignerProperty;
			WzPageGeneralProperties1._AddWizardResultEvent += AddWizardResult;

			ButtonHelp.Enabled = false;
		}

        private void buttonNext_Click(object sender, EventArgs e)
		{
			try
			{
				PageEventArgs e2 = new PageEventArgs((WizardPage) _PageCollection[_PageCount], PageEventButton.Next);
				_PageCount = AdjustPageCount(_PageCount, true);
				if(_PageCollection[_PageCount].NeedSummary)
				{
					((WzPageSummary) _PageCollection[_PageCount]).Summary = CreateSummary();
				}
				SetCurrentPage((WizardPage) _PageCollection[_PageCount], e2);
				ButtonNext.Enabled = _PageCollection[_PageCount].NextButtonEnabled;
			}
			catch(Exception exc)
			{
#if DEBUG
				MessageBox.Show(this, exc.ToString(), this.Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
				MessageBox.Show(this, exc.Message, Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
			}
		}

		private void buttonBack_Click(object sender, EventArgs e)
		{
			try
			{
				PageEventArgs e2 = new PageEventArgs((WizardPage)_PageCollection[_PageCount],PageEventButton.Back);
				_PageCount = AdjustPageCount(_PageCount,false);
				SetCurrentPage((WizardPage)_PageCollection[_PageCount],e2);
			}
			catch(Exception exc)
			{
#if DEBUG
				MessageBox.Show(this, exc.ToString(), this.Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
				MessageBox.Show(this,exc.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
#endif
			}
		}

		protected override void OnHelp(EventArgs e)
		{
			try
			{
				//_VsHelp.DisplayTopicFromKeyword(_HelpPages[_PageCount]);
			}
			catch(Exception exc)
			{
#if DEBUG
				MessageBox.Show(this, exc.ToString(), this.Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
				MessageBox.Show(this,exc.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
#endif
			}
		}

		/// <summary>
		/// Creates a summary based on the properties assembled by the wizard. To
		/// be shown on the endpage of the wizard.
		/// </summary>
		/// <returns></returns>
		private string CreateSummary()
		{
			string Summary = 
				"The pipeline component wizard will create the following project:" + Environment.NewLine + Environment.NewLine;
			
			Summary += "- A project for the pipeline component" + Environment.NewLine;
			return Summary;
		}
		
		private int AdjustPageCount(int pageCount,bool countingUp)
		{
			return (countingUp ? ++pageCount : --pageCount);
		}
        
		private void AddWizardResult(object sender, PropertyPairEvent e)
		{
			try
			{
				//Replace the value if it already exists
				if(_WizardResults.ContainsKey(e.Name))
				{
					_WizardResults.Remove(e.Name);
				}
				_WizardResults.Add(e.Name, e.Value);
			}
			catch(Exception err)
			{
#if DEBUG
				MessageBox.Show(this, err.ToString());
#else
				MessageBox.Show(this,err.Message);
#endif
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
		}

		private void AddProperty(IDictionary<string, object> ht, PropertyPairEvent e)
		{
			if (e.Remove)
			{
				if (ht.ContainsKey(e.Name))
					ht.Remove(e.Name);
				return;
			}
            //Replace the value if it already exists
		    if (ht.ContainsKey(e.Name))
                ht.Remove(e.Name);
			ht.Add(e.Name, e.Value);
		}

		private void AddDesignerProperty(object sender, PropertyPairEvent e)
		{
			try
			{
				AddProperty(DesignerProperties, e);
			}
			catch(Exception err)
			{
#if DEBUG
				MessageBox.Show(this, err.ToString());
#else
				MessageBox.Show(this,err.Message);
#endif
				Trace.WriteLine(err.Message + Environment.NewLine + err.StackTrace);
			}
		}

		private void wzPageSummary1_Load(object sender, EventArgs e)
		{
		
		}

		public IDictionary<string, object> DesignerProperties { get; } = new Dictionary<string, object>();

	    public IDictionary<string, object> WizardResults => _WizardResults;

	    public IDictionary<string, object> TransmitEndpointProperties { get; } = new Dictionary<string, object>();

	    public IDictionary<string, object> ReceiveEndpointProperties { get; } = new Dictionary<string, object>();

	    public IDictionary<string, object> TransmitHandlerProperties { get; } = new Dictionary<string, object>();
	}
}

