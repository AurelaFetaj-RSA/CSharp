namespace CreateRobotMemory
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.SaveDictBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.OutputCmdTextbox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.keyTextbox = new System.Windows.Forms.TextBox();
            this.CmdTypeComboBox = new System.Windows.Forms.ComboBox();
            this.variableNameTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MemoryTypeComboBox = new System.Windows.Forms.ComboBox();
            this.genCmdBtn = new System.Windows.Forms.Button();
            this.cmdListbox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MemoryCleanBtn = new System.Windows.Forms.Button();
            this.fileNameToLoadTextBox = new System.Windows.Forms.TextBox();
            this.LoadMemoryBtn = new System.Windows.Forms.Button();
            this.ModifyCmdBtn = new System.Windows.Forms.Button();
            this.resultTextbox = new System.Windows.Forms.TextBox();
            this.deleteSelectedBtn = new System.Windows.Forms.Button();
            this.filenameToSaveTextbox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveDictBtn
            // 
            this.SaveDictBtn.Location = new System.Drawing.Point(384, 137);
            this.SaveDictBtn.Name = "SaveDictBtn";
            this.SaveDictBtn.Size = new System.Drawing.Size(103, 48);
            this.SaveDictBtn.TabIndex = 11;
            this.SaveDictBtn.Text = "SaveDictionary";
            this.SaveDictBtn.UseVisualStyleBackColor = true;
            this.SaveDictBtn.Click += new System.EventHandler(this.SaveDictBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 48);
            this.button1.TabIndex = 9;
            this.button1.Text = "Add Cmd";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // OutputCmdTextbox
            // 
            this.OutputCmdTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputCmdTextbox.Location = new System.Drawing.Point(89, 92);
            this.OutputCmdTextbox.Name = "OutputCmdTextbox";
            this.OutputCmdTextbox.Size = new System.Drawing.Size(136, 26);
            this.OutputCmdTextbox.TabIndex = 12;
            this.OutputCmdTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.filenameToSaveTextbox);
            this.groupBox1.Controls.Add(this.deleteSelectedBtn);
            this.groupBox1.Controls.Add(this.ModifyCmdBtn);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.keyTextbox);
            this.groupBox1.Controls.Add(this.CmdTypeComboBox);
            this.groupBox1.Controls.Add(this.variableNameTextbox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.MemoryTypeComboBox);
            this.groupBox1.Controls.Add(this.genCmdBtn);
            this.groupBox1.Controls.Add(this.OutputCmdTextbox);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.SaveDictBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(493, 223);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Saving";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(338, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Key";
            // 
            // keyTextbox
            // 
            this.keyTextbox.Location = new System.Drawing.Point(341, 42);
            this.keyTextbox.Name = "keyTextbox";
            this.keyTextbox.Size = new System.Drawing.Size(100, 20);
            this.keyTextbox.TabIndex = 20;
            this.keyTextbox.Text = "1001";
            // 
            // CmdTypeComboBox
            // 
            this.CmdTypeComboBox.FormattingEnabled = true;
            this.CmdTypeComboBox.Location = new System.Drawing.Point(142, 42);
            this.CmdTypeComboBox.Name = "CmdTypeComboBox";
            this.CmdTypeComboBox.Size = new System.Drawing.Size(64, 21);
            this.CmdTypeComboBox.TabIndex = 19;
            // 
            // variableNameTextbox
            // 
            this.variableNameTextbox.Location = new System.Drawing.Point(232, 42);
            this.variableNameTextbox.Name = "variableNameTextbox";
            this.variableNameTextbox.Size = new System.Drawing.Size(100, 20);
            this.variableNameTextbox.TabIndex = 18;
            this.variableNameTextbox.Text = "test";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Variable Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Cmd";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Variable Type";
            // 
            // MemoryTypeComboBox
            // 
            this.MemoryTypeComboBox.FormattingEnabled = true;
            this.MemoryTypeComboBox.Location = new System.Drawing.Point(12, 43);
            this.MemoryTypeComboBox.Name = "MemoryTypeComboBox";
            this.MemoryTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.MemoryTypeComboBox.TabIndex = 14;
            // 
            // genCmdBtn
            // 
            this.genCmdBtn.Location = new System.Drawing.Point(12, 70);
            this.genCmdBtn.Name = "genCmdBtn";
            this.genCmdBtn.Size = new System.Drawing.Size(71, 48);
            this.genCmdBtn.TabIndex = 13;
            this.genCmdBtn.Text = "Generate Command";
            this.genCmdBtn.UseVisualStyleBackColor = true;
            this.genCmdBtn.Click += new System.EventHandler(this.genCmdBtn_Click);
            // 
            // cmdListbox
            // 
            this.cmdListbox.FormattingEnabled = true;
            this.cmdListbox.Location = new System.Drawing.Point(543, 28);
            this.cmdListbox.MultiColumn = true;
            this.cmdListbox.Name = "cmdListbox";
            this.cmdListbox.Size = new System.Drawing.Size(190, 407);
            this.cmdListbox.TabIndex = 23;
            this.cmdListbox.Click += new System.EventHandler(this.cmdListbox_Click);
            this.cmdListbox.SelectedIndexChanged += new System.EventHandler(this.cmdListbox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(540, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Memory Variable Dictionary";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.MemoryCleanBtn);
            this.groupBox2.Controls.Add(this.fileNameToLoadTextBox);
            this.groupBox2.Controls.Add(this.LoadMemoryBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 241);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(493, 199);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Load section";
            // 
            // MemoryCleanBtn
            // 
            this.MemoryCleanBtn.Location = new System.Drawing.Point(219, 65);
            this.MemoryCleanBtn.Name = "MemoryCleanBtn";
            this.MemoryCleanBtn.Size = new System.Drawing.Size(180, 23);
            this.MemoryCleanBtn.TabIndex = 2;
            this.MemoryCleanBtn.Text = "Clean Dictionary";
            this.MemoryCleanBtn.UseVisualStyleBackColor = true;
            this.MemoryCleanBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // fileNameToLoadTextBox
            // 
            this.fileNameToLoadTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileNameToLoadTextBox.Location = new System.Drawing.Point(6, 33);
            this.fileNameToLoadTextBox.Name = "fileNameToLoadTextBox";
            this.fileNameToLoadTextBox.Size = new System.Drawing.Size(481, 26);
            this.fileNameToLoadTextBox.TabIndex = 1;
            this.fileNameToLoadTextBox.Text = "MemoryConfigurator.json";
            // 
            // LoadMemoryBtn
            // 
            this.LoadMemoryBtn.Location = new System.Drawing.Point(412, 65);
            this.LoadMemoryBtn.Name = "LoadMemoryBtn";
            this.LoadMemoryBtn.Size = new System.Drawing.Size(75, 23);
            this.LoadMemoryBtn.TabIndex = 0;
            this.LoadMemoryBtn.Text = "Load Memory";
            this.LoadMemoryBtn.UseVisualStyleBackColor = true;
            this.LoadMemoryBtn.Click += new System.EventHandler(this.LoadMemoryBtn_Click);
            // 
            // ModifyCmdBtn
            // 
            this.ModifyCmdBtn.Location = new System.Drawing.Point(93, 124);
            this.ModifyCmdBtn.Name = "ModifyCmdBtn";
            this.ModifyCmdBtn.Size = new System.Drawing.Size(67, 48);
            this.ModifyCmdBtn.TabIndex = 22;
            this.ModifyCmdBtn.Text = "Modify";
            this.ModifyCmdBtn.UseVisualStyleBackColor = true;
            this.ModifyCmdBtn.Click += new System.EventHandler(this.ModifyCmdBtn_Click);
            // 
            // resultTextbox
            // 
            this.resultTextbox.BackColor = System.Drawing.SystemColors.Menu;
            this.resultTextbox.Enabled = false;
            this.resultTextbox.Location = new System.Drawing.Point(0, 470);
            this.resultTextbox.Name = "resultTextbox";
            this.resultTextbox.Size = new System.Drawing.Size(747, 20);
            this.resultTextbox.TabIndex = 26;
            // 
            // deleteSelectedBtn
            // 
            this.deleteSelectedBtn.Location = new System.Drawing.Point(166, 124);
            this.deleteSelectedBtn.Name = "deleteSelectedBtn";
            this.deleteSelectedBtn.Size = new System.Drawing.Size(89, 48);
            this.deleteSelectedBtn.TabIndex = 23;
            this.deleteSelectedBtn.Text = "Delete Selected";
            this.deleteSelectedBtn.UseVisualStyleBackColor = true;
            this.deleteSelectedBtn.Click += new System.EventHandler(this.deleteSelectedBtn_Click);
            // 
            // filenameToSaveTextbox
            // 
            this.filenameToSaveTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filenameToSaveTextbox.Location = new System.Drawing.Point(12, 191);
            this.filenameToSaveTextbox.Name = "filenameToSaveTextbox";
            this.filenameToSaveTextbox.Size = new System.Drawing.Size(481, 26);
            this.filenameToSaveTextbox.TabIndex = 24;
            this.filenameToSaveTextbox.Text = "MemoryConfigurator.json";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 446);
            this.Controls.Add(this.resultTextbox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdListbox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Memory Configurator (Kawasaki)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveDictBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox OutputCmdTextbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CmdTypeComboBox;
        private System.Windows.Forms.TextBox variableNameTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MemoryTypeComboBox;
        private System.Windows.Forms.Button genCmdBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox keyTextbox;
        private System.Windows.Forms.ListBox cmdListbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox fileNameToLoadTextBox;
        private System.Windows.Forms.Button LoadMemoryBtn;
        private System.Windows.Forms.Button MemoryCleanBtn;
        private System.Windows.Forms.Button ModifyCmdBtn;
        private System.Windows.Forms.TextBox resultTextbox;
        private System.Windows.Forms.Button deleteSelectedBtn;
        private System.Windows.Forms.TextBox filenameToSaveTextbox;
    }
}

