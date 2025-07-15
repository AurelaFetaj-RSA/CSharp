namespace ProductionLaunch
{
    partial class DialogPassword
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
            this.txtNewPassword = new System.Windows.Forms.MaskedTextBox();
            this.txtRepeatPassword = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLoginPassword = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Location = new System.Drawing.Point(131, 15);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(158, 20);
            this.txtNewPassword.TabIndex = 1;
            // 
            // txtRepeatPassword
            // 
            this.txtRepeatPassword.Location = new System.Drawing.Point(131, 41);
            this.txtRepeatPassword.Name = "txtRepeatPassword";
            this.txtRepeatPassword.PasswordChar = '*';
            this.txtRepeatPassword.Size = new System.Drawing.Size(158, 20);
            this.txtRepeatPassword.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "New password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Repeat password";
            // 
            // txtLoginPassword
            // 
            this.txtLoginPassword.Location = new System.Drawing.Point(131, 80);
            this.txtLoginPassword.Name = "txtLoginPassword";
            this.txtLoginPassword.PasswordChar = '*';
            this.txtLoginPassword.Size = new System.Drawing.Size(158, 20);
            this.txtLoginPassword.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Login password";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 131);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 25);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(131, 131);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(73, 25);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // DialogPassword
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(330, 172);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtLoginPassword);
            this.Controls.Add(this.txtRepeatPassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "DialogPassword";
            this.Text = "PLC Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        public System.Windows.Forms.MaskedTextBox txtNewPassword;
        public System.Windows.Forms.MaskedTextBox txtRepeatPassword;
        public System.Windows.Forms.MaskedTextBox txtLoginPassword;
    }
}