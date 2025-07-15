using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace ProductionLaunch
{
    public partial class FormUpdateArticleName : Form
    {
        public string articleName, variantName, modelName;
        
        public FormUpdateArticleName()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //check if model name is empty
            if (comboBoxUpdateModelName.Text == "")
            {
                return;
            }

            articleName = textBoxUpdateArticleName.Text.ToString();
            modelName = comboBoxUpdateModelName.Text.ToString();
            variantName = comboBoxUpdateVariantName.Text.ToString();

            string SQLUpdateString;

            SQLUpdateString = "UPDATE articles SET variant_name ='" + comboBoxUpdateVariantName.Text + "', model_name=" + "'" + comboBoxUpdateModelName.Text + "' WHERE article_name=" + "'" +  textBoxUpdateArticleName.Text + "'";


            OleDbCommand SQLCommand = new OleDbCommand();
            OleDbConnection connL = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source=" +  Properties.SettingsFilesPath.Default.dbLocalPath);

            SQLCommand.CommandText = SQLUpdateString;
            SQLCommand.Connection = connL;
            connL.Open();
            int response = SQLCommand.ExecuteNonQuery();
                MessageBox.Show("Update successful!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();

        }

        private void FormUpdateArticleName_Load(object sender, EventArgs e)
        {
            textBoxUpdateArticleName.Text = articleName;
            comboBoxUpdateVariantName.Text = variantName;
            comboBoxUpdateModelName.Text = modelName;

            InitRefreshUpdateModelNameList();
            InitRefreshUpdateVariantNameList();
        }

        private void InitRefreshUpdateVariantNameList()
        {
            List<string> mylist = new List<string>();
            int i = 0;
            string[] vList = Properties.Settings.Default.variantList.Split(',');
            comboBoxUpdateVariantName.Items.Clear();

            for (i = 0; i <= vList.Length - 1; i++)
            {                
                comboBoxUpdateVariantName.Items.Add(vList[i]);
            }
        }


        private void InitRefreshUpdateModelNameList()
        {
            List<string> mylist = new List<string>();

            if (Properties.SettingsFilesPath.Default.r1FilenamePath != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(Properties.SettingsFilesPath.Default.r1FilenamePath, "*.*", SearchOption.AllDirectories);
                    var namesOnly = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

                    int i = 0;
                    for (i = 0; i <= namesOnly.Count() - 1; i++)
                    {
                        string tmp = namesOnly[i];
                        tmp = tmp.Substring(2, 4);
                        mylist.Add(tmp);
                    }
                }
                catch (Exception Ex)
                {

                }
            }

            if (Properties.SettingsFilesPath.Default.r2FilenamePath != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(Properties.SettingsFilesPath.Default.r2FilenamePath, "*.*", SearchOption.AllDirectories);
                    var namesOnly = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
                    int i = 0;
                    for (i = 0; i <= namesOnly.Count() - 1; i++)
                    {
                        string tmp = namesOnly[i];
                        tmp = tmp.Substring(2, 4);
                        mylist.Add(tmp);
                    }
                }
                catch (Exception Ex)
                {

                }
            }

            if (Properties.SettingsFilesPath.Default.r3FilenamePath != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(Properties.SettingsFilesPath.Default.r3FilenamePath, "*.*", SearchOption.AllDirectories);
                    var namesOnly = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
                    int i = 0;
                    for (i = 0; i <= namesOnly.Count() - 1; i++)
                    {
                        string tmp = namesOnly[i];
                        tmp = tmp.Substring(2, 4);
                        mylist.Add(tmp);
                    }
                }
                catch (Exception Ex)
                {

                }
            }

            mylist = mylist.Distinct().ToList();

            comboBoxUpdateModelName.Items.Clear();
            comboBoxUpdateModelName.Items.AddRange(mylist.ToArray());
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
