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
		private readonly IDictionary<string, object> _wizardResults = new Dictionary<string, object>();

		private readonly IList<IWizardControl> _pageCollection = new List<IWizardControl>();
		private int _pageCount;

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

			_pageCollection.Add(wzPageWelcome1);
			_pageCollection.Add(wzPageGeneralSetup1);
			_pageCollection.Add(WzPageGeneralProperties1);
			_pageCollection.Add(WzPageDesignerProperties1);
			_pageCollection.Add(wzPageSummary1);

			wzPageGeneralSetup1.AddWizardResultEvent += AddWizardResult;
			WzPageDesignerProperties1.AddDesignerPropertyEvent +=AddDesignerProperty;
			WzPageGeneralProperties1.AddWizardResultEvent += AddWizardResult;

			ButtonHelp.Enabled = false;
		}

        private void buttonNext_Click(object sender, EventArgs e)
		{
			try
			{
				PageEventArgs e2 = new PageEventArgs((WizardPage) _pageCollection[_pageCount], PageEventButton.Next);
				_pageCount = AdjustPageCount(_pageCount, true);
				if(_pageCollection[_pageCount].NeedSummary)
				{
					((WzPageSummary) _pageCollection[_pageCount]).Summary = CreateSummary();
				}
				SetCurrentPage((WizardPage) _pageCollection[_pageCount], e2);
				ButtonNext.Enabled = _pageCollection[_pageCount].NextButtonEnabled;
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
				PageEventArgs e2 = new PageEventArgs((WizardPage)_pageCollection[_pageCount],PageEventButton.Back);
				_pageCount = AdjustPageCount(_pageCount,false);
				SetCurrentPage((WizardPage)_pageCollection[_pageCount],e2);
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
			string summary = 
				"The pipeline component wizard will create the following project:" + Environment.NewLine + Environment.NewLine;
			
			summary += "- A project for the pipeline component" + Environment.NewLine;
			return summary;
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
				if(_wizardResults.ContainsKey(e.Name))
				{
					_wizardResults.Remove(e.Name);
				}
				_wizardResults.Add(e.Name, e.Value);
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

		public IDictionary<string, object> DesignerProperties { get; } = new Dictionary<string, object>();

	    public IDictionary<string, object> WizardResults => _wizardResults;
	}
}

