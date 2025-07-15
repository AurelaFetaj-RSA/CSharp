namespace ProductionLaunch
{
    partial class FormUpdateArticleName
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdateArticleName));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxUpdateVariantName = new System.Windows.Forms.ComboBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxUpdateModelName = new System.Windows.Forms.ComboBox();
            this.textBoxUpdateArticleName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.comboBoxUpdateVariantName);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxUpdateModelName);
            this.groupBox1.Controls.Add(this.textBoxUpdateArticleName);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // comboBoxUpdateVariantName
            // 
            resources.ApplyResources(this.comboBoxUpdateVariantName, "comboBoxUpdateVariantName");
            this.comboBoxUpdateVariantName.FormattingEnabled = true;
            this.comboBoxUpdateVariantName.Items.AddRange(new object[] {
            resources.GetString("comboBoxUpdateVariantName.Items"),
            resources.GetString("comboBoxUpdateVariantName.Items1"),
            resources.GetString("comboBoxUpdateVariantName.Items2"),
            resources.GetString("comboBoxUpdateVariantName.Items3"),
            resources.GetString("comboBoxUpdateVariantName.Items4"),
            resources.GetString("comboBoxUpdateVariantName.Items5"),
            resources.GetString("comboBoxUpdateVariantName.Items6"),
            resources.GetString("comboBoxUpdateVariantName.Items7"),
            resources.GetString("comboBoxUpdateVariantName.Items8"),
            resources.GetString("comboBoxUpdateVariantName.Items9"),
            resources.GetString("comboBoxUpdateVariantName.Items10"),
            resources.GetString("comboBoxUpdateVariantName.Items11"),
            resources.GetString("comboBoxUpdateVariantName.Items12"),
            resources.GetString("comboBoxUpdateVariantName.Items13")});
            this.comboBoxUpdateVariantName.Name = "comboBoxUpdateVariantName";
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxUpdateModelName
            // 
            resources.ApplyResources(this.comboBoxUpdateModelName, "comboBoxUpdateModelName");
            this.comboBoxUpdateModelName.FormattingEnabled = true;
            this.comboBoxUpdateModelName.Items.AddRange(new object[] {
            resources.GetString("comboBoxUpdateModelName.Items"),
            resources.GetString("comboBoxUpdateModelName.Items1"),
            resources.GetString("comboBoxUpdateModelName.Items2"),
            resources.GetString("comboBoxUpdateModelName.Items3"),
            resources.GetString("comboBoxUpdateModelName.Items4"),
            resources.GetString("comboBoxUpdateModelName.Items5"),
            resources.GetString("comboBoxUpdateModelName.Items6"),
            resources.GetString("comboBoxUpdateModelName.Items7"),
            resources.GetString("comboBoxUpdateModelName.Items8"),
            resources.GetString("comboBoxUpdateModelName.Items9"),
            resources.GetString("comboBoxUpdateModelName.Items10"),
            resources.GetString("comboBoxUpdateModelName.Items11"),
            resources.GetString("comboBoxUpdateModelName.Items12"),
            resources.GetString("comboBoxUpdateModelName.Items13")});
            this.comboBoxUpdateModelName.Name = "comboBoxUpdateModelName";
            // 
            // textBoxUpdateArticleName
            // 
            resources.ApplyResources(this.textBoxUpdateArticleName, "textBoxUpdateArticleName");
            this.textBoxUpdateArticleName.Name = "textBoxUpdateArticleName";
            // 
            // FormUpdateArticleName
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FormUpdateArticleName";
            this.Load += new System.EventHandler(this.FormUpdateArticleName_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxUpdateModelName;
        private System.Windows.Forms.TextBox textBoxUpdateArticleName;
        private System.Windows.Forms.ComboBox comboBoxUpdateVariantName;
    }
}