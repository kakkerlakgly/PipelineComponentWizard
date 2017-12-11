namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
    partial class WzPageGeneralSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WzPageGeneralSetup));
            this.label1 = new System.Windows.Forms.Label();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.ErrProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboComponentStage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboPipelineType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkImplementIProbeMessage = new System.Windows.Forms.CheckBox();
            this.cboImplementationLanguage = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).BeginInit();
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtClassName
            // 
            resources.ApplyResources(this.txtClassName, "txtClassName");
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtClassName.Validating += new System.ComponentModel.CancelEventHandler(this.txtClassName_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtNameSpace
            // 
            resources.ApplyResources(this.txtNameSpace, "txtNameSpace");
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.TextChanged += new System.EventHandler(this.Element_Changed);
            this.txtNameSpace.Validating += new System.ComponentModel.CancelEventHandler(this.txtNamespace_Validating);
            // 
            // ErrProv
            // 
            this.ErrProv.ContainerControl = this;
            resources.ApplyResources(this.ErrProv, "ErrProv");
            // 
            // cboComponentStage
            // 
            resources.ApplyResources(this.cboComponentStage, "cboComponentStage");
            this.cboComponentStage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboComponentStage.Name = "cboComponentStage";
            this.cboComponentStage.SelectedIndexChanged += new System.EventHandler(this.cboComponentStage_Changed);
            this.cboComponentStage.SelectedValueChanged += new System.EventHandler(this.Element_Changed);
            this.cboComponentStage.Validating += new System.ComponentModel.CancelEventHandler(this.cboComponentStage_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cboPipelineType
            // 
            this.cboPipelineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboPipelineType, "cboPipelineType");
            this.cboPipelineType.Name = "cboPipelineType";
            this.cboPipelineType.Validating += new System.ComponentModel.CancelEventHandler(this.cboPipelineType_Validating);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // chkImplementIProbeMessage
            // 
            resources.ApplyResources(this.chkImplementIProbeMessage, "chkImplementIProbeMessage");
            this.chkImplementIProbeMessage.Name = "chkImplementIProbeMessage";
            // 
            // cboImplementationLanguage
            // 
            resources.ApplyResources(this.cboImplementationLanguage, "cboImplementationLanguage");
            this.cboImplementationLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImplementationLanguage.Name = "cboImplementationLanguage";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // WzPageGeneralSetup
            // 
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboImplementationLanguage);
            this.Controls.Add(this.chkImplementIProbeMessage);
            this.Controls.Add(this.cboPipelineType);
            this.Controls.Add(this.cboComponentStage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtClassName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Name = "WzPageGeneralSetup";
            resources.ApplyResources(this, "$this");
            this.SubTitle = "Specify generic component properties";
            this.Title = "General setup";
            this.Leave += new System.EventHandler(this.WzPageGeneralSetup_Leave);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.panelHeader, 0);
            this.Controls.SetChildIndex(this.txtClassName, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtNameSpace, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cboComponentStage, 0);
            this.Controls.SetChildIndex(this.cboPipelineType, 0);
            this.Controls.SetChildIndex(this.chkImplementIProbeMessage, 0);
            this.Controls.SetChildIndex(this.cboImplementationLanguage, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ErrProv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ErrorProvider ErrProv;
        private System.Windows.Forms.TextBox txtClassName;
        private System.Windows.Forms.TextBox txtNameSpace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboPipelineType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboComponentStage;
        private System.Windows.Forms.CheckBox chkImplementIProbeMessage;
        private System.Windows.Forms.ComboBox cboImplementationLanguage;
        private System.Windows.Forms.Label label5;
    }
}
