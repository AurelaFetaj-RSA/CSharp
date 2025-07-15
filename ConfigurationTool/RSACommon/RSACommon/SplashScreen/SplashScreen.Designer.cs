namespace RSACommon.GraphicsForm
{
    partial class SplashScreen
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
            this.loadingTextbox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // loadingTextbox
            // 
            this.loadingTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(181)))), ((int)(((byte)(181)))));
            this.loadingTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.loadingTextbox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.loadingTextbox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(53)))), ((int)(((byte)(61)))));
            this.loadingTextbox.Location = new System.Drawing.Point(12, 260);
            this.loadingTextbox.Multiline = true;
            this.loadingTextbox.Name = "loadingTextbox";
            this.loadingTextbox.Size = new System.Drawing.Size(496, 136);
            this.loadingTextbox.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 240);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 410);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.loadingTextbox);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(520, 410);
            this.MinimumSize = new System.Drawing.Size(520, 410);
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox loadingTextbox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}