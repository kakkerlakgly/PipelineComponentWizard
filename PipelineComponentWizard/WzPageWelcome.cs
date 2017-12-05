using System;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    /// <summary>
    /// defines the <see cref="C:Microsoft.BizTalk.Wizard.WizardPage"/> that welcomes
    /// the user to this wizard
    /// </summary>
    [ComVisible(false)]
	public partial class WzPageWelcome : Microsoft.BizTalk.Wizard.WizardPage, IWizardControl
	{
        /// <summary>
        /// defines the Registry hive our settings are located
        /// </summary>
        const string ourSettingKey = @"Software\MartijnHoogendoorn\BizTalkPipelineComponentWizard";
        /// <summary>
        /// defines the Name of the Registry key which determines whether this page needs to be skipped
        /// </summary>
        const string skipWelcome = "SkipWelcome";

        /// <summary>
        /// constructor, sets general settings for this instance
        /// </summary>
		public WzPageWelcome()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

            this.labelNavigation.Links.Clear();
            this.labelNavigation.Links.Add(0, this.labelNavigation.Text.Length - 1, "http://blogs.msdn.com/martijnh/");
        }

        /// <summary>
        /// whether the Next button should be enabled
        /// </summary>
		public bool NextButtonEnabled
		{
			get {	return true;	}
		}

        /// <summary>
        /// whether this page needs a summary
        /// </summary>
		public bool NeedSummary
		{
			get {	return false;	}
		}

        public override void OnEnterPage(object sender, Microsoft.BizTalk.Wizard.PageEventArgs e)
        {
            // retrieve the WizardForm which hosts our page
            Microsoft.BizTalk.Wizard.WizardForm form1 = base.WizardForm;

            // enable the buttons as we see fit
            form1.ButtonBack.Enabled = false;
            form1.ButtonBack.Visible = true;
            form1.ButtonNext.Enabled = true;
            form1.ButtonNext.Visible = true;
            form1.ButtonFinish.Enabled = false;
            form1.ButtonFinish.Visible = false;

            // select the Next button
            form1.ButtonNext.Select();

            // and set focus to it
            form1.ButtonNext.Focus();

            RegistryKey wizardKey = null;

            try
            {
                // open our private 'configuration' key, enable writing
                wizardKey = Registry.CurrentUser.OpenSubKey(ourSettingKey);

                if (wizardKey != null)
                {
                    // try and retrieve the setting whether to skip this page
                    string currentWelcomeValue = wizardKey.GetValue(skipWelcome) as string;

                    // if we should skip this page,
                    if (currentWelcomeValue != null && bool.Parse(currentWelcomeValue))
                    {
                        // set the checkbox
                        checkBoxSkipWelcome.Checked = true;

                        // programmatically click the Next button
                        form1.ButtonNext.PerformClick();
                    }
                }
            }
            finally
            {
                // make sure the RegistryKey instance is closed
                if (wizardKey != null)
                {
                    wizardKey.Close();
                }
            }
        }

        public override void OnLeavePage(object sender, Microsoft.BizTalk.Wizard.PageEventArgs e)
        {
            // if the user pressed the "Next" button
            if (e.Button == Microsoft.BizTalk.Wizard.PageEventButton.Next)
            {
                // the RegistryKey to query / update
                RegistryKey wizardKey = null;

                try
                {
                    // open our private 'configuration' key, enable writing
                    wizardKey = Registry.CurrentUser.OpenSubKey(ourSettingKey, true);

                    // if our key doesn't exist,
                    if (wizardKey == null)
                    {
                        // create it
                        wizardKey = Registry.CurrentUser.CreateSubKey(ourSettingKey);
                    }

                    // set the value the user selected regarding skipping this dialog
                    wizardKey.SetValue(skipWelcome, checkBoxSkipWelcome.Checked);
                }
                finally
                {
                    // make sure the RegistryKey instance is closed
                    if(wizardKey != null)
                    {
                        wizardKey.Close();
                    }
                }
            }

            base.OnLeavePage(sender, e);
        }

	    void labelNavigation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	    {
	        // set the visited state for the clicked link
	        this.labelNavigation.Links[this.labelNavigation.Links.IndexOf(e.Link)].Visited = true;

	        // get the target of the link
	        string target = e.Link.LinkData as string;

	        // spawn a *new* browser process to view the link
	        System.Diagnostics.Process.Start(new ProcessStartInfo(getDefaultBrowser(), target));
	    }

	    /// <summary>
	    /// 'borrowed from http://ryanfarley.com/blog/archive/2004/05/16/649.aspx
	    /// </summary>
	    /// <returns>the default registered browser, without arguments</returns>
	    private string getDefaultBrowser()
	    {
	        string browser = string.Empty;

	        RegistryKey key = null;
	        try
	        {
	            key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

	            //trim off quotes
	            browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

	            //get rid of everything after the ".exe"
	            browser = browser.Substring(0, browser.IndexOf(".exe") + 4);
	        }
	        finally
	        {
	            if (key != null) key.Close();
	        }
	        return browser;
	    }
    }
}

