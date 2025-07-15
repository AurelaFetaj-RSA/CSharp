using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RSACommon;
using Robot;
using WebApi;
using RSACommon.Configuration;
using RSAInterface;

namespace CreateConfig
{
    public partial class Configurator : Form
    {
        private string selectedPath = null;
        public Configurator()
        {
            InitializeComponent();

            robotComboBox.Items.Add(typeof(Kawasaki));
            robotComboBox.Items.Add(typeof(Fanuc));

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            selectedPathTextbox.Text = path;
            selectedPath = path;
            folderBrowserDialog.SelectedPath = path;

            robotComboBox.SelectedIndex = 0;
            // Assembly.GetExecutingAssembly().Location;
            RobotCheckbox.Checked = true;
            OpcCheckbox.Checked = true;
            WebCheckbox.Checked = true;

            robotActiveCheckBox.Checked = true;
            webApiActiveCheckBox.Checked = true;
            opcActiveCheckBox.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            selectedPath = folderBrowserDialog.SelectedPath;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OpcCheckbox.Checked)
                Opc.Visible = true;
            else
                Opc.Visible = false;

            if (WebCheckbox.Checked)
                webGB.Visible = true;
            else
                webGB.Visible = false;

            if (RobotCheckbox.Checked)
                RobotGB.Visible = true;
            else
                RobotGB.Visible = false;
        }

        public string CryptoPassword(string password)
        {
            string salt = DateTime.Now.ToString();

            string encryptedString = RSACommon.PasswordSecurity.EncryptString(password);
            string decryptedString = RSACommon.PasswordSecurity.DecryptString(encryptedString);

            if (password == decryptedString)
            {
                return encryptedString;
            }

            return "Error";

        }
        private void MakeConfigBtn_Click(object sender, EventArgs e)
        {
            RSACommon.Configuration.CoreConfigurationsBuilder coreBuilder = new RSACommon.Configuration.CoreConfigurationsBuilder("config.json", ".\\");
            CoreConfigurations newConfiguration = new CoreConfigurations();

            WebApiCoreConfiguration webApiCoreConf = new WebApiCoreConfiguration()
            {
                Host = ipWebApiTextbox.Text,
                Scheme = schemeWebApiTextbox.Text,
                Port = Int32.Parse(portWebApiTextbox.Text),
                ServiceName = webNameTextbox.Text,
                Active = webApiActiveCheckBox.Checked
            };

            OpcServerConfiguration opcConfig = new OpcServerConfiguration()
            {
                Host = ipOpcTextbox.Text,
                Scheme = schemeOpcTextbox.Text,
                ServiceName = opcNameTextbox.Text,
                Port = Int32.Parse(opcPortTextbox.Text),

                DefaultKeepAliveFutureValueExpected = 0,
                DefaultKeepAliveFutureValueToSet = 1,

                Active = opcActiveCheckBox.Checked,
                UserList = new List<OpcUser>
                {
                    new OpcUser() {UserP = UserPrivilegiesType.Admin, UserName = "Admin", Password = CryptoPassword("robots")},
                    new OpcUser() {UserP = UserPrivilegiesType.User, UserName = "MES", Password = CryptoPassword("MesUser!0")},
                }
            };

            RobotConfiguration robotConfigurator = new RobotConfiguration()
            {
                Host = hostTextbox.Text,
                Scheme = robotSchemeTextbox.Text,
                Port = Int32.Parse(robotPortTextbox.Text),
                RobotType = (Type)robotComboBox.SelectedItem,
                ServiceName = robootNameTextbox.Text,
                Active = robotActiveCheckBox.Checked
        };

            if (OpcCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(opcConfig);

            if (WebCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(webApiCoreConf);

            if (RobotCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(robotConfigurator);

            coreBuilder.Save(newConfiguration);
        }
    }
}
