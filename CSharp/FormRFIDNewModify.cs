using Opc.Ua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductionLaunch
{
    public partial class FormRFIDNewModify : Form
    {
        public string size = string.Empty;
        public string foot = string.Empty;
        public string rfid = string.Empty;
        public int readingResult = -2;
        public FormRFIDNewModify()
        {
            InitializeComponent();
            
        }

        public void InitForm()
        {
            comboBoxFoot.Text = foot;
            numericUpDownSize.Text = size;
            if (readingResult == 0)
            {
                this.Text = "rfid " + rfid + " - NEW";
                //this.BackColor = Color.Orange;
            }
            else
            {
                this.Text = "rfid " + rfid + " - MODIFY";
                //this.BackColor = Color.LightGreen;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            size = numericUpDownSize.Value.ToString();
            foot = comboBoxFoot.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
