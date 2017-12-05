namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    partial class WzPageWelcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageWelcome));
            this.labelNavigation = new System.Windows.Forms.LinkLabel();
            this.checkBoxSkipWelcome = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelSubTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelNavigation
            // 
            resources.ApplyResources(this.labelNavigation, "labelNavigation");
            this.labelNavigation.Name = "labelNavigation";
            this.labelNavigation.TabStop = true;
            this.labelNavigation.UseCompatibleTextRendering = true;
            this.labelNavigation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelNavigation_LinkClicked);
            // 
            // checkBoxSkipWelcome
            // 
            resources.ApplyResources(this.checkBoxSkipWelcome, "checkBoxSkipWelcome");
            this.checkBoxSkipWelcome.Name = "checkBoxSkipWelcome";
            this.checkBoxSkipWelcome.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard.Properties.Resources.watermark;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.Name = "labelTitle";
            // 
            // labelSubTitle
            // 
            resources.ApplyResources(this.labelSubTitle, "labelSubTitle");
            this.labelSubTitle.Name = "labelSubTitle";
            // 
            // WzPageWelcome
            // 
            this.Controls.Add(this.labelSubTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBoxSkipWelcome);
            this.Controls.Add(this.labelNavigation);
            this.Name = "WzPageWelcome";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel labelNavigation;
        private System.Windows.Forms.CheckBox checkBoxSkipWelcome;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelSubTitle;
    }
}
