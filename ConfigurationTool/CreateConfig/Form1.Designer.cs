namespace CreateConfig
{
    partial class Configurator
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.WebCheckbox = new System.Windows.Forms.CheckBox();
            this.OpcCheckbox = new System.Windows.Forms.CheckBox();
            this.RobotCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.selectedPathTextbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.MakeConfigBtn = new System.Windows.Forms.Button();
            this.webGB = new System.Windows.Forms.GroupBox();
            this.webApiActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.portWebApiTextbox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.webNameTextbox = new System.Windows.Forms.TextBox();
            this.schemeWebApiTextbox = new System.Windows.Forms.TextBox();
            this.ipWebApiTextbox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Opc = new System.Windows.Forms.GroupBox();
            this.opcActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.opcPortTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.opcNameTextbox = new System.Windows.Forms.TextBox();
            this.schemeOpcTextbox = new System.Windows.Forms.TextBox();
            this.ipOpcTextbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.RobotGB = new System.Windows.Forms.GroupBox();
            this.robotActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.robotPortTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.robootNameTextbox = new System.Windows.Forms.TextBox();
            this.robotSchemeTextbox = new System.Windows.Forms.TextBox();
            this.hostTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.robotComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.webGB.SuspendLayout();
            this.Opc.SuspendLayout();
            this.RobotGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(9, 10);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(860, 546);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.WebCheckbox);
            this.tabPage1.Controls.Add(this.OpcCheckbox);
            this.tabPage1.Controls.Add(this.RobotCheckbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(852, 520);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Config Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // WebCheckbox
            // 
            this.WebCheckbox.AutoSize = true;
            this.WebCheckbox.Location = new System.Drawing.Point(26, 77);
            this.WebCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.WebCheckbox.Name = "WebCheckbox";
            this.WebCheckbox.Size = new System.Drawing.Size(69, 17);
            this.WebCheckbox.TabIndex = 2;
            this.WebCheckbox.Text = "Web API";
            this.WebCheckbox.UseVisualStyleBackColor = true;
            // 
            // OpcCheckbox
            // 
            this.OpcCheckbox.AutoSize = true;
            this.OpcCheckbox.Location = new System.Drawing.Point(26, 56);
            this.OpcCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.OpcCheckbox.Name = "OpcCheckbox";
            this.OpcCheckbox.Size = new System.Drawing.Size(46, 17);
            this.OpcCheckbox.TabIndex = 1;
            this.OpcCheckbox.Text = "Opc";
            this.OpcCheckbox.UseVisualStyleBackColor = true;
            // 
            // RobotCheckbox
            // 
            this.RobotCheckbox.AutoSize = true;
            this.RobotCheckbox.Location = new System.Drawing.Point(26, 35);
            this.RobotCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.RobotCheckbox.Name = "RobotCheckbox";
            this.RobotCheckbox.Size = new System.Drawing.Size(55, 17);
            this.RobotCheckbox.TabIndex = 0;
            this.RobotCheckbox.Text = "Robot";
            this.RobotCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.selectedPathTextbox);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.MakeConfigBtn);
            this.tabPage2.Controls.Add(this.webGB);
            this.tabPage2.Controls.Add(this.Opc);
            this.tabPage2.Controls.Add(this.RobotGB);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(852, 520);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Config";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // selectedPathTextbox
            // 
            this.selectedPathTextbox.Location = new System.Drawing.Point(188, 493);
            this.selectedPathTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.selectedPathTextbox.Name = "selectedPathTextbox";
            this.selectedPathTextbox.Size = new System.Drawing.Size(522, 20);
            this.selectedPathTextbox.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 489);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 27);
            this.button1.TabIndex = 10;
            this.button1.Text = "Select Folder output";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // MakeConfigBtn
            // 
            this.MakeConfigBtn.Location = new System.Drawing.Point(721, 489);
            this.MakeConfigBtn.Margin = new System.Windows.Forms.Padding(2);
            this.MakeConfigBtn.Name = "MakeConfigBtn";
            this.MakeConfigBtn.Size = new System.Drawing.Size(64, 27);
            this.MakeConfigBtn.TabIndex = 9;
            this.MakeConfigBtn.Text = "Config";
            this.MakeConfigBtn.UseVisualStyleBackColor = true;
            this.MakeConfigBtn.Click += new System.EventHandler(this.MakeConfigBtn_Click);
            // 
            // webGB
            // 
            this.webGB.Controls.Add(this.webApiActiveCheckBox);
            this.webGB.Controls.Add(this.label10);
            this.webGB.Controls.Add(this.portWebApiTextbox);
            this.webGB.Controls.Add(this.label11);
            this.webGB.Controls.Add(this.label12);
            this.webGB.Controls.Add(this.webNameTextbox);
            this.webGB.Controls.Add(this.schemeWebApiTextbox);
            this.webGB.Controls.Add(this.ipWebApiTextbox);
            this.webGB.Controls.Add(this.label13);
            this.webGB.Location = new System.Drawing.Point(536, 31);
            this.webGB.Margin = new System.Windows.Forms.Padding(2);
            this.webGB.Name = "webGB";
            this.webGB.Padding = new System.Windows.Forms.Padding(2);
            this.webGB.Size = new System.Drawing.Size(249, 258);
            this.webGB.TabIndex = 8;
            this.webGB.TabStop = false;
            this.webGB.Text = "WebApi";
            // 
            // webApiActiveCheckBox
            // 
            this.webApiActiveCheckBox.AutoSize = true;
            this.webApiActiveCheckBox.Location = new System.Drawing.Point(8, 222);
            this.webApiActiveCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.webApiActiveCheckBox.Name = "webApiActiveCheckBox";
            this.webApiActiveCheckBox.Size = new System.Drawing.Size(56, 17);
            this.webApiActiveCheckBox.TabIndex = 26;
            this.webApiActiveCheckBox.Text = "Active";
            this.webApiActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 52);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Scheme";
            // 
            // portWebApiTextbox
            // 
            this.portWebApiTextbox.Location = new System.Drawing.Point(176, 81);
            this.portWebApiTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.portWebApiTextbox.Name = "portWebApiTextbox";
            this.portWebApiTextbox.Size = new System.Drawing.Size(44, 20);
            this.portWebApiTextbox.TabIndex = 24;
            this.portWebApiTextbox.Text = "5150";
            this.portWebApiTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 86);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Port";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(5, 114);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "ServiceName";
            // 
            // webNameTextbox
            // 
            this.webNameTextbox.Location = new System.Drawing.Point(107, 111);
            this.webNameTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.webNameTextbox.Name = "webNameTextbox";
            this.webNameTextbox.Size = new System.Drawing.Size(113, 20);
            this.webNameTextbox.TabIndex = 21;
            this.webNameTextbox.Text = "OPC UA SERVER";
            this.webNameTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // schemeWebApiTextbox
            // 
            this.schemeWebApiTextbox.Location = new System.Drawing.Point(168, 47);
            this.schemeWebApiTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.schemeWebApiTextbox.Name = "schemeWebApiTextbox";
            this.schemeWebApiTextbox.Size = new System.Drawing.Size(52, 20);
            this.schemeWebApiTextbox.TabIndex = 20;
            this.schemeWebApiTextbox.Text = "http";
            this.schemeWebApiTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ipWebApiTextbox
            // 
            this.ipWebApiTextbox.Location = new System.Drawing.Point(98, 17);
            this.ipWebApiTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.ipWebApiTextbox.Name = "ipWebApiTextbox";
            this.ipWebApiTextbox.Size = new System.Drawing.Size(122, 20);
            this.ipWebApiTextbox.TabIndex = 19;
            this.ipWebApiTextbox.Text = "localhost";
            this.ipWebApiTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(5, 22);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 18;
            this.label13.Text = "IP";
            // 
            // Opc
            // 
            this.Opc.Controls.Add(this.opcActiveCheckBox);
            this.Opc.Controls.Add(this.label6);
            this.Opc.Controls.Add(this.opcPortTextbox);
            this.Opc.Controls.Add(this.label7);
            this.Opc.Controls.Add(this.label8);
            this.Opc.Controls.Add(this.opcNameTextbox);
            this.Opc.Controls.Add(this.schemeOpcTextbox);
            this.Opc.Controls.Add(this.ipOpcTextbox);
            this.Opc.Controls.Add(this.label9);
            this.Opc.Location = new System.Drawing.Point(286, 31);
            this.Opc.Margin = new System.Windows.Forms.Padding(2);
            this.Opc.Name = "Opc";
            this.Opc.Padding = new System.Windows.Forms.Padding(2);
            this.Opc.Size = new System.Drawing.Size(245, 258);
            this.Opc.TabIndex = 7;
            this.Opc.TabStop = false;
            this.Opc.Text = "Opc";
            // 
            // opcActiveCheckBox
            // 
            this.opcActiveCheckBox.AutoSize = true;
            this.opcActiveCheckBox.Location = new System.Drawing.Point(7, 222);
            this.opcActiveCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.opcActiveCheckBox.Name = "opcActiveCheckBox";
            this.opcActiveCheckBox.Size = new System.Drawing.Size(56, 17);
            this.opcActiveCheckBox.TabIndex = 18;
            this.opcActiveCheckBox.Text = "Active";
            this.opcActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 52);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Scheme";
            // 
            // opcPortTextbox
            // 
            this.opcPortTextbox.Location = new System.Drawing.Point(175, 81);
            this.opcPortTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.opcPortTextbox.Name = "opcPortTextbox";
            this.opcPortTextbox.Size = new System.Drawing.Size(44, 20);
            this.opcPortTextbox.TabIndex = 16;
            this.opcPortTextbox.Text = "6000";
            this.opcPortTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 86);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Port";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 114);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "ServiceName";
            // 
            // opcNameTextbox
            // 
            this.opcNameTextbox.Location = new System.Drawing.Point(106, 111);
            this.opcNameTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.opcNameTextbox.Name = "opcNameTextbox";
            this.opcNameTextbox.Size = new System.Drawing.Size(113, 20);
            this.opcNameTextbox.TabIndex = 13;
            this.opcNameTextbox.Text = "OPC UA SERVER";
            this.opcNameTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // schemeOpcTextbox
            // 
            this.schemeOpcTextbox.Location = new System.Drawing.Point(167, 47);
            this.schemeOpcTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.schemeOpcTextbox.Name = "schemeOpcTextbox";
            this.schemeOpcTextbox.Size = new System.Drawing.Size(52, 20);
            this.schemeOpcTextbox.TabIndex = 12;
            this.schemeOpcTextbox.Text = "opc.tcp";
            this.schemeOpcTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ipOpcTextbox
            // 
            this.ipOpcTextbox.Location = new System.Drawing.Point(98, 17);
            this.ipOpcTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.ipOpcTextbox.Name = "ipOpcTextbox";
            this.ipOpcTextbox.Size = new System.Drawing.Size(122, 20);
            this.ipOpcTextbox.TabIndex = 11;
            this.ipOpcTextbox.Text = "localhost";
            this.ipOpcTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 22);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "IP";
            // 
            // RobotGB
            // 
            this.RobotGB.Controls.Add(this.robotActiveCheckBox);
            this.RobotGB.Controls.Add(this.label5);
            this.RobotGB.Controls.Add(this.robotPortTextbox);
            this.RobotGB.Controls.Add(this.label4);
            this.RobotGB.Controls.Add(this.label3);
            this.RobotGB.Controls.Add(this.robootNameTextbox);
            this.RobotGB.Controls.Add(this.robotSchemeTextbox);
            this.RobotGB.Controls.Add(this.hostTextbox);
            this.RobotGB.Controls.Add(this.label2);
            this.RobotGB.Controls.Add(this.label1);
            this.RobotGB.Controls.Add(this.robotComboBox);
            this.RobotGB.Location = new System.Drawing.Point(44, 31);
            this.RobotGB.Margin = new System.Windows.Forms.Padding(2);
            this.RobotGB.Name = "RobotGB";
            this.RobotGB.Padding = new System.Windows.Forms.Padding(2);
            this.RobotGB.Size = new System.Drawing.Size(238, 258);
            this.RobotGB.TabIndex = 6;
            this.RobotGB.TabStop = false;
            this.RobotGB.Text = "Robot";
            // 
            // robotActiveCheckBox
            // 
            this.robotActiveCheckBox.AutoSize = true;
            this.robotActiveCheckBox.Location = new System.Drawing.Point(8, 222);
            this.robotActiveCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.robotActiveCheckBox.Name = "robotActiveCheckBox";
            this.robotActiveCheckBox.Size = new System.Drawing.Size(56, 17);
            this.robotActiveCheckBox.TabIndex = 10;
            this.robotActiveCheckBox.Text = "Active";
            this.robotActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 52);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Scheme";
            // 
            // robotPortTextbox
            // 
            this.robotPortTextbox.Location = new System.Drawing.Point(144, 81);
            this.robotPortTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.robotPortTextbox.Name = "robotPortTextbox";
            this.robotPortTextbox.Size = new System.Drawing.Size(76, 20);
            this.robotPortTextbox.TabIndex = 8;
            this.robotPortTextbox.Text = "9105";
            this.robotPortTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 86);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 114);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "ServiceName";
            // 
            // robootNameTextbox
            // 
            this.robootNameTextbox.Location = new System.Drawing.Point(107, 111);
            this.robootNameTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.robootNameTextbox.Name = "robootNameTextbox";
            this.robootNameTextbox.Size = new System.Drawing.Size(113, 20);
            this.robootNameTextbox.TabIndex = 5;
            this.robootNameTextbox.Text = "Kawasaki";
            this.robootNameTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // robotSchemeTextbox
            // 
            this.robotSchemeTextbox.Location = new System.Drawing.Point(144, 47);
            this.robotSchemeTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.robotSchemeTextbox.Name = "robotSchemeTextbox";
            this.robotSchemeTextbox.Size = new System.Drawing.Size(76, 20);
            this.robotSchemeTextbox.TabIndex = 4;
            this.robotSchemeTextbox.Text = "http";
            this.robotSchemeTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // hostTextbox
            // 
            this.hostTextbox.Location = new System.Drawing.Point(98, 17);
            this.hostTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.hostTextbox.Name = "hostTextbox";
            this.hostTextbox.Size = new System.Drawing.Size(122, 20);
            this.hostTextbox.TabIndex = 3;
            this.hostTextbox.Text = "localhost";
            this.hostTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 145);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Robot";
            // 
            // robotComboBox
            // 
            this.robotComboBox.FormattingEnabled = true;
            this.robotComboBox.Location = new System.Drawing.Point(88, 139);
            this.robotComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.robotComboBox.Name = "robotComboBox";
            this.robotComboBox.Size = new System.Drawing.Size(132, 21);
            this.robotComboBox.TabIndex = 0;
            // 
            // Configurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 712);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Configurator";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.webGB.ResumeLayout(false);
            this.webGB.PerformLayout();
            this.Opc.ResumeLayout(false);
            this.Opc.PerformLayout();
            this.RobotGB.ResumeLayout(false);
            this.RobotGB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox selectedPathTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button MakeConfigBtn;
        private System.Windows.Forms.GroupBox webGB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox portWebApiTextbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox webNameTextbox;
        private System.Windows.Forms.TextBox schemeWebApiTextbox;
        private System.Windows.Forms.TextBox ipWebApiTextbox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox Opc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox opcPortTextbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox opcNameTextbox;
        private System.Windows.Forms.TextBox schemeOpcTextbox;
        private System.Windows.Forms.TextBox ipOpcTextbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox RobotGB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox robotPortTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox robootNameTextbox;
        private System.Windows.Forms.TextBox robotSchemeTextbox;
        private System.Windows.Forms.TextBox hostTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox robotComboBox;
        private System.Windows.Forms.CheckBox WebCheckbox;
        private System.Windows.Forms.CheckBox OpcCheckbox;
        private System.Windows.Forms.CheckBox RobotCheckbox;
        private System.Windows.Forms.CheckBox webApiActiveCheckBox;
        private System.Windows.Forms.CheckBox opcActiveCheckBox;
        private System.Windows.Forms.CheckBox robotActiveCheckBox;
    }
}

