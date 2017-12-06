namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    partial class PipeLineComponentWizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipeLineComponentWizardForm));
            this.wzPageGeneralSetup1 = new MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageGeneralSetup();
            this.WzPageGeneralProperties1 = new MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageGeneralProperties();
            this.WzPageDesignerProperties1 = new MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageDesignerProperties();
            this.wzPageWelcome1 = new MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageWelcome();
            this.wzPageSummary1 = new MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageSummary();
            this.SuspendLayout();
            // 
            // wzPageGeneralSetup1
            // 
            this.wzPageGeneralSetup1.Back = null;
            this.wzPageGeneralSetup1.Glyph = ((System.Drawing.Image)(resources.GetObject("wzPageGeneralSetup1.Glyph")));
            resources.ApplyResources(this.wzPageGeneralSetup1, "wzPageGeneralSetup1");
            this.wzPageGeneralSetup1.Name = "wzPageGeneralSetup1";
            this.wzPageGeneralSetup1.Next = null;
            this.wzPageGeneralSetup1.SubTitle = "Specify generic pipeline component properties";
            this.wzPageGeneralSetup1.Title = "General setup";
            this.wzPageGeneralSetup1.WizardForm = null;
            // 
            // WzPageGeneralProperties1
            // 
            this.WzPageGeneralProperties1.Back = null;
            this.WzPageGeneralProperties1.Glyph = ((System.Drawing.Image)(resources.GetObject("WzPageGeneralProperties1.Glyph")));
            resources.ApplyResources(this.WzPageGeneralProperties1, "WzPageGeneralProperties1");
            this.WzPageGeneralProperties1.Name = "WzPageGeneralProperties1";
            this.WzPageGeneralProperties1.Next = null;
            this.WzPageGeneralProperties1.SubTitle = "Specify Component UI settings";
            this.WzPageGeneralProperties1.Title = "UI Settings";
            this.WzPageGeneralProperties1.WizardForm = null;
            // 
            // WzPageDesignerProperties1
            // 
            this.WzPageDesignerProperties1.Back = null;
            this.WzPageDesignerProperties1.Glyph = ((System.Drawing.Image)(resources.GetObject("WzPageDesignerProperties1.Glyph")));
            resources.ApplyResources(this.WzPageDesignerProperties1, "WzPageDesignerProperties1");
            this.WzPageDesignerProperties1.Name = "WzPageDesignerProperties1";
            this.WzPageDesignerProperties1.Next = null;
            this.WzPageDesignerProperties1.SubTitle = "Specify design-time variables";
            this.WzPageDesignerProperties1.Title = "Design-time variables";
            this.WzPageDesignerProperties1.WizardForm = null;
            // 
            // wzPageWelcome1
            // 
            this.wzPageWelcome1.Back = null;
            this.wzPageWelcome1.BackColor = System.Drawing.SystemColors.Window;
            this.wzPageWelcome1.Glyph = null;
            resources.ApplyResources(this.wzPageWelcome1, "wzPageWelcome1");
            this.wzPageWelcome1.Name = "wzPageWelcome1";
            this.wzPageWelcome1.Next = null;
            this.wzPageWelcome1.SubTitle = null;
            this.wzPageWelcome1.Title = null;
            this.wzPageWelcome1.WizardForm = null;
            // 
            // wzPageSummary1
            // 
            this.wzPageSummary1.Back = null;
            this.wzPageSummary1.BackColor = System.Drawing.SystemColors.Window;
            this.wzPageSummary1.Glyph = ((System.Drawing.Image)(resources.GetObject("wzPageSummary1.Glyph")));
            resources.ApplyResources(this.wzPageSummary1, "wzPageSummary1");
            this.wzPageSummary1.Name = "wzPageSummary1";
            this.wzPageSummary1.Next = null;
            this.wzPageSummary1.SubTitle = "";
            this.wzPageSummary1.Title = "BizTalk Server Pipeline Component Wizard";
            this.wzPageSummary1.WizardForm = null;
            // 
            // PipeLineComponentWizardForm
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "PipeLineComponentWizardForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageDesignerProperties WzPageDesignerProperties1;
        private MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageGeneralSetup wzPageGeneralSetup1;
        private MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageGeneralProperties WzPageGeneralProperties1;
        private MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageWelcome wzPageWelcome1;
        private MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.WzPageSummary wzPageSummary1;
    }
}
