namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    partial class WzPageGeneralProperties
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageGeneralProperties));
            this.label1 = new System.Windows.Forms.Label();
            this.txtComponentName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComponentVersion = new System.Windows.Forms.TextBox();
            this.ErrProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtComponentDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ComponentIcon = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComponentIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            resources.ApplyResources(this.panelHeader, "panelHeader");
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            // 
            // labelSubTitle
            // 
            resources.ApplyResources(this.labelSubTitle, "labelSubTitle");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtComponentName
            // 
            resources.ApplyResources(this.txtComponentName, "txtComponentName");
            this.txtComponentName.Name = "txtComponentName";
            this.txtComponentName.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtComponentName.Validating += new System.ComponentModel.CancelEventHandler(this.txtComponentName_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtComponentVersion
            // 
            resources.ApplyResources(this.txtComponentVersion, "txtComponentVersion");
            this.txtComponentVersion.Name = "txtComponentVersion";
            this.txtComponentVersion.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtComponentVersion.Validating += new System.ComponentModel.CancelEventHandler(this.txtComponentVersion_Validating);
            // 
            // ErrProv
            // 
            this.ErrProv.ContainerControl = this;
            resources.ApplyResources(this.ErrProv, "ErrProv");
            // 
            // txtComponentDescription
            // 
            resources.ApplyResources(this.txtComponentDescription, "txtComponentDescription");
            this.txtComponentDescription.Name = "txtComponentDescription";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ComponentIcon
            // 
            resources.ApplyResources(this.ComponentIcon, "ComponentIcon");
            this.ComponentIcon.Name = "ComponentIcon";
            this.ComponentIcon.TabStop = false;
            this.ComponentIcon.Click += new System.EventHandler(this.ComponentIcon_Click);
            this.ComponentIcon.DoubleClick += new System.EventHandler(this.ComponentIcon_DoubleClick);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "bmp";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // WzPageGeneralProperties
            // 
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ComponentIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtComponentVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtComponentName);
            this.Controls.Add(this.txtComponentDescription);
            this.Controls.Add(this.label2);
            this.Name = "WzPageGeneralProperties";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify Component UI settings";
            this.Title = "UI settings";
            this.Click += new System.EventHandler(this.ComponentIcon_Click);
            this.DoubleClick += new System.EventHandler(this.ComponentIcon_DoubleClick);
            this.Leave += new System.EventHandler(this.WzPageGeneralProperties_Leave);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.txtComponentDescription, 0);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.txtComponentName, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtComponentVersion, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.ComponentIcon, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ComponentIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtComponentName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ErrorProvider ErrProv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox ComponentIcon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtComponentVersion;
        private System.Windows.Forms.TextBox txtComponentDescription;
    }
}
