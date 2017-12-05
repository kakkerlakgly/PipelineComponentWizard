namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    partial class WzPageDesignerProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageDesignerProperties));
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtDesignerProperty = new System.Windows.Forms.TextBox();
            this.cmdDesignerPropertyDel = new System.Windows.Forms.Button();
            this.cmdDesignerPropertyAdd = new System.Windows.Forms.Button();
            this.cmbDesignerPropertyDataType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstDesignerProperties = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHelpDesignerProperties = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            // 
            // labelSubTitle
            // 
            resources.ApplyResources(this.labelSubTitle, "labelSubTitle");
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // txtDesignerProperty
            // 
            resources.ApplyResources(this.txtDesignerProperty, "txtDesignerProperty");
            this.txtDesignerProperty.Name = "txtDesignerProperty";
            // 
            // cmdDesignerPropertyDel
            // 
            resources.ApplyResources(this.cmdDesignerPropertyDel, "cmdDesignerPropertyDel");
            this.cmdDesignerPropertyDel.Name = "cmdDesignerPropertyDel";
            this.cmdDesignerPropertyDel.Click += new System.EventHandler(this.cmdDesignerPropertyDel_Click);
            // 
            // cmdDesignerPropertyAdd
            // 
            resources.ApplyResources(this.cmdDesignerPropertyAdd, "cmdDesignerPropertyAdd");
            this.cmdDesignerPropertyAdd.Name = "cmdDesignerPropertyAdd";
            this.cmdDesignerPropertyAdd.Click += new System.EventHandler(this.cmdDesignerPropertyAdd_Click);
            // 
            // cmbDesignerPropertyDataType
            // 
            this.cmbDesignerPropertyDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbDesignerPropertyDataType, "cmbDesignerPropertyDataType");
            this.cmbDesignerPropertyDataType.Name = "cmbDesignerPropertyDataType";
            this.cmbDesignerPropertyDataType.SelectedIndexChanged += new System.EventHandler(this.cmbDesignerPropertyDataType_Changed);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lstDesignerProperties
            // 
            resources.ApplyResources(this.lstDesignerProperties, "lstDesignerProperties");
            this.lstDesignerProperties.Name = "lstDesignerProperties";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblHelpDesignerProperties
            // 
            resources.ApplyResources(this.lblHelpDesignerProperties, "lblHelpDesignerProperties");
            this.lblHelpDesignerProperties.Name = "lblHelpDesignerProperties";
            // 
            // WzPageDesignerProperties
            // 
            this.Controls.Add(this.lblHelpDesignerProperties);
            this.Controls.Add(this.txtDesignerProperty);
            this.Controls.Add(this.cmdDesignerPropertyDel);
            this.Controls.Add(this.cmdDesignerPropertyAdd);
            this.Controls.Add(this.cmbDesignerPropertyDataType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstDesignerProperties);
            this.Controls.Add(this.label1);
            this.Name = "WzPageDesignerProperties";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify design-time variables";
            this.Title = "Design-time variables";
            this.Load += new System.EventHandler(this.WzPageDesignerProperties_Load);
            this.Leave += new System.EventHandler(this.WzPageDesignerProperties_Leave);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lstDesignerProperties, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cmbDesignerPropertyDataType, 0);
            this.Controls.SetChildIndex(this.cmdDesignerPropertyAdd, 0);
            this.Controls.SetChildIndex(this.cmdDesignerPropertyDel, 0);
            this.Controls.SetChildIndex(this.txtDesignerProperty, 0);
            this.Controls.SetChildIndex(this.lblHelpDesignerProperties, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox txtDesignerProperty;
        private System.Windows.Forms.Button cmdDesignerPropertyDel;
        private System.Windows.Forms.Button cmdDesignerPropertyAdd;
        private System.Windows.Forms.ComboBox cmbDesignerPropertyDataType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstDesignerProperties;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblHelpDesignerProperties;
    }
}
