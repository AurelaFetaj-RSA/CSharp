namespace ProductionLaunch
{
    partial class FormRFIDNewModify
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.comboBoxFoot = new System.Windows.Forms.ComboBox();
            this.numericUpDownSize = new System.Windows.Forms.NumericUpDown();
            this.labelRFIDSize = new System.Windows.Forms.Label();
            this.labelRFIDFoot = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.buttonCancel.Location = new System.Drawing.Point(201, 120);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(148, 48);
            this.buttonCancel.TabIndex = 319;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.ForeColor = System.Drawing.SystemColors.WindowText;
            this.buttonOK.Location = new System.Drawing.Point(20, 120);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(148, 48);
            this.buttonOK.TabIndex = 318;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // comboBoxFoot
            // 
            this.comboBoxFoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(244)))));
            this.comboBoxFoot.Font = new System.Drawing.Font("Verdana", 21.75F);
            this.comboBoxFoot.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.comboBoxFoot.FormattingEnabled = true;
            this.comboBoxFoot.Items.AddRange(new object[] {
            "LF",
            "RG"});
            this.comboBoxFoot.Location = new System.Drawing.Point(70, 20);
            this.comboBoxFoot.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.comboBoxFoot.Name = "comboBoxFoot";
            this.comboBoxFoot.Size = new System.Drawing.Size(96, 43);
            this.comboBoxFoot.TabIndex = 410;
            this.comboBoxFoot.Text = "LF";
            // 
            // numericUpDownSize
            // 
            this.numericUpDownSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(244)))));
            this.numericUpDownSize.Font = new System.Drawing.Font("Verdana", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownSize.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.numericUpDownSize.Location = new System.Drawing.Point(248, 20);
            this.numericUpDownSize.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.numericUpDownSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownSize.Name = "numericUpDownSize";
            this.numericUpDownSize.Size = new System.Drawing.Size(97, 50);
            this.numericUpDownSize.TabIndex = 465;
            this.numericUpDownSize.Value = new decimal(new int[] {
            340,
            0,
            0,
            0});
            // 
            // labelRFIDSize
            // 
            this.labelRFIDSize.AutoSize = true;
            this.labelRFIDSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.labelRFIDSize.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRFIDSize.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelRFIDSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelRFIDSize.Location = new System.Drawing.Point(20, 20);
            this.labelRFIDSize.Name = "labelRFIDSize";
            this.labelRFIDSize.Size = new System.Drawing.Size(41, 18);
            this.labelRFIDSize.TabIndex = 466;
            this.labelRFIDSize.Text = "size";
            // 
            // labelRFIDFoot
            // 
            this.labelRFIDFoot.AutoSize = true;
            this.labelRFIDFoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.labelRFIDFoot.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRFIDFoot.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelRFIDFoot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelRFIDFoot.Location = new System.Drawing.Point(198, 20);
            this.labelRFIDFoot.Name = "labelRFIDFoot";
            this.labelRFIDFoot.Size = new System.Drawing.Size(44, 18);
            this.labelRFIDFoot.TabIndex = 467;
            this.labelRFIDFoot.Text = "type";
            // 
            // FormRFIDNewModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(233)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(376, 201);
            this.Controls.Add(this.labelRFIDFoot);
            this.Controls.Add(this.labelRFIDSize);
            this.Controls.Add(this.numericUpDownSize);
            this.Controls.Add(this.comboBoxFoot);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "FormRFIDNewModify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormRFIDNewModify";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ComboBox comboBoxFoot;
        private System.Windows.Forms.NumericUpDown numericUpDownSize;
        private System.Windows.Forms.Label labelRFIDSize;
        private System.Windows.Forms.Label labelRFIDFoot;
    }
}