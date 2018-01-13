using System;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.Wizard;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    /// <summary>
    /// defines the <see cref="C:Microsoft.BizTalk.Wizard.WizardPage"/> that welcomes
    /// the user to this wizard
    /// </summary>
    [ComVisible(false)]
    public partial class WzPageWelcome : WizardPage, IWizardControl
    {
        /// <summary>
        /// defines the Registry hive our settings are located
        /// </summary>
        const string OurSettingKey = @"Software\MartijnHoogendoorn\BizTalkPipelineComponentWizard";

        /// <summary>
        /// defines the Name of the Registry key which determines whether this page needs to be skipped
        /// </summary>
        const string SkipWelcome = "SkipWelcome";

        /// <summary>
        /// constructor, sets general settings for this instance
        /// </summary>
        public WzPageWelcome()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            labelNavigation.Links.Clear();
            labelNavigation.Links.Add(0, 53, "http://blogs.msdn.com/martijnh/");
            labelNavigation.Links.Add(95, 15, "https://twitter.com/mrashwinprabhu");
            labelNavigation.Links.Add(117, 19, "https://gallery.technet.microsoft.com/BizTalk-Server-2016Visual-33c23537"); 
            labelNavigation.Links.Add(181, 16, "https://twitter.com/kristianrastrup");
            labelNavigation.Links.Add(203, 12, "https://github.com/kakkerlakgly/PipelineComponentWizard");
        }

        /// <summary>
        /// whether the Next button should be enabled
        /// </summary>
        public bool NextButtonEnabled => true;

        /// <summary>
        /// whether this page needs a summary
        /// </summary>
        public bool NeedSummary => false;

        public override void OnEnterPage(object sender, PageEventArgs e)
        {
            // retrieve the WizardForm which hosts our page
            WizardForm form1 = WizardForm;

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

            // open our private 'configuration' key, enable writing
            using (var wizardKey = Registry.CurrentUser.OpenSubKey(OurSettingKey))
            {
                var currentWelcomeValue = wizardKey?.GetValue(SkipWelcome).ToString();

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

        public override void OnLeavePage(object sender, PageEventArgs e)
        {
            // if the user pressed the "Next" button
            if (e.Button == PageEventButton.Next)
            {
                // the RegistryKey to query / update

                // open our private 'configuration' key, enable writing
                using (var wizardKey = Registry.CurrentUser.OpenSubKey(OurSettingKey, true) ??
                                       Registry.CurrentUser.CreateSubKey(OurSettingKey))
                {
                    wizardKey.SetValue(SkipWelcome, checkBoxSkipWelcome.Checked);
                }
            }

            base.OnLeavePage(sender, e);
        }

        void labelNavigation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // set the visited state for the clicked link
            labelNavigation.Links[labelNavigation.Links.IndexOf(e.Link)].Visited = true;

            // spawn a *new* browser process to view the link
            Process.Start(e.Link.LinkData.ToString());
        }

        /// <summary>
        /// 'borrowed from http://ryanfarley.com/blog/archive/2004/05/16/649.aspx
        /// </summary>
        /// <returns>the default registered browser, without arguments</returns>
        private string GetDefaultBrowser()
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false))
            {
                var browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

                //get rid of everything after the ".exe"
                browser = browser.Substring(0, browser.IndexOf(".exe", StringComparison.Ordinal) + 4);
                return browser;
            }
        }
    }
}
