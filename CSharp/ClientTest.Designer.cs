namespace GUI
{
    partial class ClientTest
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
            this.connectBtn = new System.Windows.Forms.Button();
            this.hostTxtbox = new System.Windows.Forms.TextBox();
            this.portTxtbox = new System.Windows.Forms.TextBox();
            this.userTxtbox = new System.Windows.Forms.TextBox();
            this.pwdTxtbox = new System.Windows.Forms.TextBox();
            this.schemeTxtbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLogTxtbox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.jobAltoBtn = new System.Windows.Forms.Button();
            this.ReadJogAltoBtn = new System.Windows.Forms.Button();
            this.jogAltoCheckbox = new System.Windows.Forms.CheckBox();
            this.writeQuotaLong = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.readQuotaLong = new System.Windows.Forms.Button();
            this.quota4 = new System.Windows.Forms.TextBox();
            this.quota3 = new System.Windows.Forms.TextBox();
            this.quota2 = new System.Windows.Forms.TextBox();
            this.quota1 = new System.Windows.Forms.TextBox();
            this.pcPercVeloBtn = new System.Windows.Forms.Button();
            this.velocitTxtbox = new System.Windows.Forms.TextBox();
            this.readPercVelocitaManuelBtn = new System.Windows.Forms.Button();
            this.readMultiploBtn = new System.Windows.Forms.Button();
            this.cleanLogBtn = new System.Windows.Forms.Button();
            this.writeMultiploBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(193, 44);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 31);
            this.connectBtn.TabIndex = 0;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // hostTxtbox
            // 
            this.hostTxtbox.Location = new System.Drawing.Point(82, 16);
            this.hostTxtbox.Name = "hostTxtbox";
            this.hostTxtbox.Size = new System.Drawing.Size(100, 20);
            this.hostTxtbox.TabIndex = 1;
            // 
            // portTxtbox
            // 
            this.portTxtbox.Location = new System.Drawing.Point(82, 69);
            this.portTxtbox.Name = "portTxtbox";
            this.portTxtbox.Size = new System.Drawing.Size(100, 20);
            this.portTxtbox.TabIndex = 2;
            // 
            // userTxtbox
            // 
            this.userTxtbox.Location = new System.Drawing.Point(82, 135);
            this.userTxtbox.Name = "userTxtbox";
            this.userTxtbox.Size = new System.Drawing.Size(100, 20);
            this.userTxtbox.TabIndex = 3;
            // 
            // pwdTxtbox
            // 
            this.pwdTxtbox.Location = new System.Drawing.Point(82, 161);
            this.pwdTxtbox.Name = "pwdTxtbox";
            this.pwdTxtbox.Size = new System.Drawing.Size(100, 20);
            this.pwdTxtbox.TabIndex = 4;
            // 
            // schemeTxtbox
            // 
            this.schemeTxtbox.Location = new System.Drawing.Point(82, 42);
            this.schemeTxtbox.Name = "schemeTxtbox";
            this.schemeTxtbox.Size = new System.Drawing.Size(100, 20);
            this.schemeTxtbox.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(82, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Make URI";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(13, 196);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(38, 13);
            this.AddressLabel.TabIndex = 7;
            this.AddressLabel.Text = "http://";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "scheme";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "user";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "password";
            // 
            // textBoxLogTxtbox
            // 
            this.textBoxLogTxtbox.Location = new System.Drawing.Point(493, 12);
            this.textBoxLogTxtbox.Multiline = true;
            this.textBoxLogTxtbox.Name = "textBoxLogTxtbox";
            this.textBoxLogTxtbox.Size = new System.Drawing.Size(629, 593);
            this.textBoxLogTxtbox.TabIndex = 13;
            this.textBoxLogTxtbox.TextChanged += new System.EventHandler(this.textBoxLogTxtbox_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 226);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 34);
            this.button2.TabIndex = 14;
            this.button2.Text = "Read Folder Name";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // jobAltoBtn
            // 
            this.jobAltoBtn.Location = new System.Drawing.Point(13, 275);
            this.jobAltoBtn.Name = "jobAltoBtn";
            this.jobAltoBtn.Size = new System.Drawing.Size(74, 40);
            this.jobAltoBtn.TabIndex = 15;
            this.jobAltoBtn.Text = "Write Jog Basso";
            this.jobAltoBtn.UseVisualStyleBackColor = true;
            this.jobAltoBtn.Click += new System.EventHandler(this.jobAltoBtn_Click);
            // 
            // ReadJogAltoBtn
            // 
            this.ReadJogAltoBtn.Location = new System.Drawing.Point(15, 321);
            this.ReadJogAltoBtn.Name = "ReadJogAltoBtn";
            this.ReadJogAltoBtn.Size = new System.Drawing.Size(74, 40);
            this.ReadJogAltoBtn.TabIndex = 16;
            this.ReadJogAltoBtn.Text = "Read Jog Basso";
            this.ReadJogAltoBtn.UseVisualStyleBackColor = true;
            this.ReadJogAltoBtn.Click += new System.EventHandler(this.ReadJogAltoBtn_Click);
            // 
            // jogAltoCheckbox
            // 
            this.jogAltoCheckbox.AutoSize = true;
            this.jogAltoCheckbox.Location = new System.Drawing.Point(102, 288);
            this.jogAltoCheckbox.Name = "jogAltoCheckbox";
            this.jogAltoCheckbox.Size = new System.Drawing.Size(97, 17);
            this.jogAltoCheckbox.TabIndex = 17;
            this.jogAltoCheckbox.Text = "Jog Alto Status";
            this.jogAltoCheckbox.UseVisualStyleBackColor = true;
            // 
            // writeQuotaLong
            // 
            this.writeQuotaLong.Location = new System.Drawing.Point(6, 19);
            this.writeQuotaLong.Name = "writeQuotaLong";
            this.writeQuotaLong.Size = new System.Drawing.Size(102, 31);
            this.writeQuotaLong.TabIndex = 18;
            this.writeQuotaLong.Text = "Write Quota Long";
            this.writeQuotaLong.UseVisualStyleBackColor = true;
            this.writeQuotaLong.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.readQuotaLong);
            this.groupBox1.Controls.Add(this.quota4);
            this.groupBox1.Controls.Add(this.writeQuotaLong);
            this.groupBox1.Controls.Add(this.quota3);
            this.groupBox1.Controls.Add(this.quota2);
            this.groupBox1.Controls.Add(this.quota1);
            this.groupBox1.Location = new System.Drawing.Point(16, 367);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 135);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // readQuotaLong
            // 
            this.readQuotaLong.Location = new System.Drawing.Point(6, 56);
            this.readQuotaLong.Name = "readQuotaLong";
            this.readQuotaLong.Size = new System.Drawing.Size(102, 31);
            this.readQuotaLong.TabIndex = 27;
            this.readQuotaLong.Text = "Read Quota Long";
            this.readQuotaLong.UseVisualStyleBackColor = true;
            this.readQuotaLong.Click += new System.EventHandler(this.readQuotaLong_Click);
            // 
            // quota4
            // 
            this.quota4.Location = new System.Drawing.Point(114, 99);
            this.quota4.Name = "quota4";
            this.quota4.Size = new System.Drawing.Size(100, 20);
            this.quota4.TabIndex = 26;
            this.quota4.Text = "155";
            // 
            // quota3
            // 
            this.quota3.Location = new System.Drawing.Point(114, 73);
            this.quota3.Name = "quota3";
            this.quota3.Size = new System.Drawing.Size(100, 20);
            this.quota3.TabIndex = 25;
            this.quota3.Text = "132";
            // 
            // quota2
            // 
            this.quota2.Location = new System.Drawing.Point(114, 47);
            this.quota2.Name = "quota2";
            this.quota2.Size = new System.Drawing.Size(100, 20);
            this.quota2.TabIndex = 24;
            this.quota2.Text = "150";
            // 
            // quota1
            // 
            this.quota1.Location = new System.Drawing.Point(114, 21);
            this.quota1.Name = "quota1";
            this.quota1.Size = new System.Drawing.Size(100, 20);
            this.quota1.TabIndex = 23;
            this.quota1.Text = "100";
            // 
            // pcPercVeloBtn
            // 
            this.pcPercVeloBtn.Location = new System.Drawing.Point(275, 275);
            this.pcPercVeloBtn.Name = "pcPercVeloBtn";
            this.pcPercVeloBtn.Size = new System.Drawing.Size(93, 40);
            this.pcPercVeloBtn.TabIndex = 24;
            this.pcPercVeloBtn.Text = "Write % velocita manuale";
            this.pcPercVeloBtn.UseVisualStyleBackColor = true;
            this.pcPercVeloBtn.Click += new System.EventHandler(this.pcPercVeloBtn_Click);
            // 
            // velocitTxtbox
            // 
            this.velocitTxtbox.Location = new System.Drawing.Point(387, 275);
            this.velocitTxtbox.Name = "velocitTxtbox";
            this.velocitTxtbox.Size = new System.Drawing.Size(100, 20);
            this.velocitTxtbox.TabIndex = 25;
            this.velocitTxtbox.Text = "100";
            // 
            // readPercVelocitaManuelBtn
            // 
            this.readPercVelocitaManuelBtn.Location = new System.Drawing.Point(275, 321);
            this.readPercVelocitaManuelBtn.Name = "readPercVelocitaManuelBtn";
            this.readPercVelocitaManuelBtn.Size = new System.Drawing.Size(93, 40);
            this.readPercVelocitaManuelBtn.TabIndex = 26;
            this.readPercVelocitaManuelBtn.Text = "Read % velocita manuale";
            this.readPercVelocitaManuelBtn.UseVisualStyleBackColor = true;
            this.readPercVelocitaManuelBtn.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // readMultiploBtn
            // 
            this.readMultiploBtn.Location = new System.Drawing.Point(297, 20);
            this.readMultiploBtn.Name = "readMultiploBtn";
            this.readMultiploBtn.Size = new System.Drawing.Size(93, 34);
            this.readMultiploBtn.TabIndex = 27;
            this.readMultiploBtn.Text = "Read Multiplo";
            this.readMultiploBtn.UseVisualStyleBackColor = true;
            this.readMultiploBtn.Click += new System.EventHandler(this.readMultiploBtn_Click);
            // 
            // cleanLogBtn
            // 
            this.cleanLogBtn.Location = new System.Drawing.Point(1064, 558);
            this.cleanLogBtn.Name = "cleanLogBtn";
            this.cleanLogBtn.Size = new System.Drawing.Size(49, 37);
            this.cleanLogBtn.TabIndex = 28;
            this.cleanLogBtn.Text = "Clean";
            this.cleanLogBtn.UseVisualStyleBackColor = true;
            this.cleanLogBtn.Click += new System.EventHandler(this.cleanLogBtn_Click);
            // 
            // writeMultiploBtn
            // 
            this.writeMultiploBtn.Location = new System.Drawing.Point(297, 61);
            this.writeMultiploBtn.Name = "writeMultiploBtn";
            this.writeMultiploBtn.Size = new System.Drawing.Size(93, 34);
            this.writeMultiploBtn.TabIndex = 29;
            this.writeMultiploBtn.Text = "Write Multiplo";
            this.writeMultiploBtn.UseVisualStyleBackColor = true;
            this.writeMultiploBtn.Click += new System.EventHandler(this.writeMultiploBtn_Click);
            // 
            // ClientTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1125, 617);
            this.Controls.Add(this.writeMultiploBtn);
            this.Controls.Add(this.cleanLogBtn);
            this.Controls.Add(this.readMultiploBtn);
            this.Controls.Add(this.readPercVelocitaManuelBtn);
            this.Controls.Add(this.velocitTxtbox);
            this.Controls.Add(this.pcPercVeloBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.jogAltoCheckbox);
            this.Controls.Add(this.ReadJogAltoBtn);
            this.Controls.Add(this.jobAltoBtn);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxLogTxtbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.schemeTxtbox);
            this.Controls.Add(this.pwdTxtbox);
            this.Controls.Add(this.userTxtbox);
            this.Controls.Add(this.portTxtbox);
            this.Controls.Add(this.hostTxtbox);
            this.Controls.Add(this.connectBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ClientTest";
            this.Text = " ";
            this.Load += new System.EventHandler(this.ClientTest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox hostTxtbox;
        private System.Windows.Forms.TextBox portTxtbox;
        private System.Windows.Forms.TextBox userTxtbox;
        private System.Windows.Forms.TextBox pwdTxtbox;
        private System.Windows.Forms.TextBox schemeTxtbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLogTxtbox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button jobAltoBtn;
        private System.Windows.Forms.Button ReadJogAltoBtn;
        private System.Windows.Forms.CheckBox jogAltoCheckbox;
        private System.Windows.Forms.Button writeQuotaLong;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox quota4;
        private System.Windows.Forms.TextBox quota3;
        private System.Windows.Forms.TextBox quota2;
        private System.Windows.Forms.TextBox quota1;
        private System.Windows.Forms.Button readQuotaLong;
        private System.Windows.Forms.Button pcPercVeloBtn;
        private System.Windows.Forms.TextBox velocitTxtbox;
        private System.Windows.Forms.Button readPercVelocitaManuelBtn;
        private System.Windows.Forms.Button readMultiploBtn;
        private System.Windows.Forms.Button cleanLogBtn;
        private System.Windows.Forms.Button writeMultiploBtn;
    }
}