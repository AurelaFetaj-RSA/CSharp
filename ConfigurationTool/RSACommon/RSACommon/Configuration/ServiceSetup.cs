using Diagnostic.Configuration;
using Diagnostic.Core;
using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using Robot;
using RSACommon.Alarm.Configuration;
using RSACommon.Configuration;
using RSAInterface;
using RSAInterface.Helper;
using RSAInterface.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApi;

namespace RSACommon.GraphicsForm
{
    public partial class ServiceSetup : Form
    {
        public string DEFAULT_PATH_STRING = @"PATH: ";
        public string DEFAULT_PATH = @"C:\";

        List<string> AlarmFilesInListbox = new List<string>();
        Core extCore = null;
        public ServiceSetup(Core core = null)
        {
            InitializeComponent();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //robotComboBox.SelectedIndex = 0;
            // Assembly.GetExecutingAssembly().Location;
            RobotCheckbox.Checked = true;
            OpcCheckbox.Checked = true;
            WebCheckbox.Checked = true;
            OpcClientCheckbox.Checked = true;
            readProgramsCheckbox.Checked = true;

            robotComboBox.Items.Add(typeof(Kawasaki));
            robotComboBox.Items.Add(typeof(Fanuc));

            extensionReadProgramsList.Items.Add("PROG");
            extensionReadProgramsList.Items.Add("LAV");
            extensionReadProgramsList.Items.Add("AU1");
            extensionReadProgramsList.Items.Add("AU2");

            readProgramHostTxtbox.Text = "localhost";

            selectedPathLabel.Text = $"{DEFAULT_PATH_STRING}";

            if (core != null)
                extCore = core;

            loggerMinLevel.Items.Add(log4net.Core.Level.Debug);
            loggerMinLevel.Items.Add(log4net.Core.Level.Info);
            loggerMinLevel.Items.Add(log4net.Core.Level.Warn);
            loggerMinLevel.Items.Add(log4net.Core.Level.Error);
            loggerMinLevel.SelectedIndex = 1;
        }

        public void LoadSetup(List<IServiceConfiguration> listOfServiceConfig)
        {
            opcClientActive.Checked = false;
            opcActiveCheckBox.Checked = false;
            robotActiveCheckBox.Checked = false;
            webApiActiveCheckBox.Checked = false;

            extensionReadProgramsList.Items.Clear();

            OpcClientCheckbox.Checked = false;
            OpcCheckbox.Checked = false;
            RobotCheckbox.Checked = false;
            WebCheckbox.Checked = false;
            readProgramsCheckbox.Checked = false;
            mySqlCheckbox.Checked = false;


            foreach (IServiceConfiguration config in listOfServiceConfig)
            {
                if (config is OpcClientConfiguration clientConfig)
                {
                    opcClientHost.Text = clientConfig.Host;
                    opcClientPort.Text = clientConfig.Port.ToString();
                    opcClientKAExpectedTextbox.Text = clientConfig.DefaultKeepAliveFutureValueExpected.ToString();
                    opcClientKASetTextbox.Text = clientConfig.DefaultKeepAliveFutureValueToSet.ToString();
                    opcClienttimeoutDC.Text = clientConfig.DisconnectionTimeoutMilliseconds.ToString();
                    opcClienttimeOutKAtextbox.Text = clientConfig.TimeoutMilliseconds.ToString();

                    opcClientActive.Checked = clientConfig.Active;

                    OpcClientCheckbox.Checked = true;


                }
                else if (config is OpcServerConfiguration serverConfig)
                {
                    ipOpcTextbox.Text = serverConfig.Host;
                    schemeOpcTextbox.Text = serverConfig.Scheme;
                    opcNameTextbox.Text = serverConfig.ServiceName;
                    portOpcServerTextbox.Text = serverConfig.Port.ToString();

                    opcServerKAExpectedTextbox.Text = serverConfig.DefaultKeepAliveFutureValueExpected.ToString();
                    opcServerKASetTextbox.Text = serverConfig.DefaultKeepAliveFutureValueToSet.ToString();
                    opcActiveCheckBox.Checked = serverConfig.Active;
                    opcServerRobotActivatedCheckbox.Checked = serverConfig.RobotIsActive;

                    OpcCheckbox.Checked = true;
                }
                else if (config is Diagnostic.Configuration.DiagnosticConfiguration diagnosticConfig)
                {

                }
                else if (config is Alarm.Configuration.AlarmConfiguration alarmConfig)
                {
                    alarmActiveCheckbox.Checked = alarmConfig.Active;
                    alarmServiceName.Text = alarmConfig.ServiceName;

                    foreach (var filename in alarmConfig.Files)
                    {
                        alarmFileListBox.Items.Add(filename);
                    }

                }
                else if (config is RobotConfiguration robotConfig)
                {

                    ipRobotTextbox.Text = robotConfig.Host;
                    portRobotTextbox.Text = robotConfig.Port.ToString();
                    robootNameTextbox.Text = robotConfig.ServiceName;

                    robotActiveCheckBox.Checked = robotConfig.Active;


                    RobotCheckbox.Checked = true;

                }
                else if (config is WebApiCoreConfiguration webApiConfig)
                {
                    ipWebApiTextbox.Text = webApiConfig.Host;
                    schemeWebApiTextbox.Text = webApiConfig.Scheme;
                    portWebApiTextbox.Text = webApiConfig.Port.ToString();
                    webNameTextbox.Text = webApiConfig.ServiceName;
                    webApiActiveCheckBox.Checked = webApiConfig.Active;

                    WebCheckbox.Checked = true;
                }
                else if (config is ReadProgramsConfiguration readConfig)
                {
                    readProgramsServiceNameTextbox.Text = readConfig.ServiceName;
                    readProgramServiceActiveCheckbox.Checked = readConfig.Active;
                    readProgramsSchemeTxtbox.Text = readConfig.Scheme;

                    foreach (string path in readConfig.ProgramsPath)
                    {
                        readProgramsPathListbox.Items.Add(path);
                    }

                    foreach (string extension in readConfig.Extensions)
                    {
                        extensionReadProgramsList.Items.Add(extension);
                    }

                    readProgramsCheckbox.Checked = true;
                }
                else if (config is MySqlConfiguration mySqlConfig)
                {
                    mySqlHostTextbox.Text = mySqlConfig.Host;
                    mySqlPortTextbox.Text = mySqlConfig.Port.ToString();
                    mySqlSchemeTextbox.Text = mySqlConfig.Scheme;

                    string decryptedString = RSACommon.PasswordSecurity.DecryptString(mySqlConfig.User.Password);

                    if (mySqlConfig.User != null)
                    {
                        mySqlUserTextobox.Text = mySqlConfig.User.UserName;
                        mySqlPwdTextbox.Text = decryptedString;
                    }

                    mySqlCheckbox.Checked = true;
                }
            }
        }


        private void CreateConfigurationFile(string path)
        {
            CoreConfigurationsBuilder coreBuilder = new CoreConfigurationsBuilder(path);

            CoreConfigurations newConfiguration = CreateCoreConfigurations();

            if (newConfiguration != null)
                coreBuilder.Save(newConfiguration);

        }

        public CoreConfigurations CreateCoreConfigurations()
        {
            CoreConfigurations newConfiguration = new CoreConfigurations();

            WebApiCoreConfiguration webApiCoreConf = new WebApiCoreConfiguration()
            {
                Host = ipWebApiTextbox.Text,
                Scheme = schemeWebApiTextbox.Text,
                Port = Int32.Parse(portWebApiTextbox.Text),
                ServiceName = webNameTextbox.Text,
                Active = webApiActiveCheckBox.Checked
            };

            CoreConfiguration coreCOnfig = new CoreConfiguration();

            if (extCore != null)
                coreCOnfig.ServiceName = extCore.Name;

            OpcServerConfiguration opcConfig = new OpcServerConfiguration()
            {
                Host = ipOpcTextbox.Text,
                Scheme = schemeOpcTextbox.Text,
                ServiceName = opcNameTextbox.Text,
                Port = Int32.Parse(portOpcServerTextbox.Text),

                DefaultKeepAliveFutureValueExpected = Int32.Parse(opcServerKAExpectedTextbox.Text),
                DefaultKeepAliveFutureValueToSet = Int32.Parse(opcServerKASetTextbox.Text),

                Active = opcActiveCheckBox.Checked,
                UserList = new List<OpcUser>
                {
                    new OpcUser() { UserP = UserPrivilegiesType.Admin, UserName = "Admin", Password = CryptoPassword("robots") },
                    new OpcUser() { UserP = UserPrivilegiesType.User, UserName = "MES", Password = CryptoPassword("MesUser!0") },
                },
                RobotIsActive = opcServerRobotActivatedCheckbox.Checked,
            };

            KawasakiConfiguration robotConfigurator = new KawasakiConfiguration()
            {
                Host = ipRobotTextbox.Text,
                Scheme = schemeRobotTextbox.Text,
                Port = Int32.Parse(portRobotTextbox.Text),
                RobotType = (Type)robotComboBox.SelectedItem,
                ServiceName = robootNameTextbox.Text,
                Active = robotActiveCheckBox.Checked
            };

            OpcClientConfiguration opcClientConfig = new OpcClientConfiguration()
            {
                Host = opcClientHost.Text,
                Scheme = opcClientScheme.Text,
                Port = Int32.Parse(opcClientPort.Text),
                ServiceName = opcClientServiceName.Text,
                Active = opcClientActive.Checked,
                DefaultKeepAliveFutureValueExpected = Int32.Parse(opcClientKAExpectedTextbox.Text),
                DefaultKeepAliveFutureValueToSet = Int32.Parse(opcClientKASetTextbox.Text),
                TimeoutMilliseconds = Int32.Parse(opcClienttimeOutKAtextbox.Text),
                DisconnectionTimeoutMilliseconds = Int32.Parse(opcClienttimeoutDC.Text),
            };

            ReadProgramsConfiguration readProgramsConfiguration = new ReadProgramsConfiguration()
            {
                //Host = readProgramHostTxtbox.Text,
                Scheme = readProgramsSchemeTxtbox.Text,
                ServiceName = readProgramsServiceNameTextbox.Text,
                Active = readProgramServiceActiveCheckbox.Checked,
                Extensions = extensionReadProgramsList.Items.Cast<string>().ToArray(),
                ProgramsPath = readProgramsPathListbox.Items.Cast<string>().ToArray()
            };

            AlarmConfiguration alarmConfiguration = new AlarmConfiguration()
            {
                ServiceName = alarmServiceName.Text,
                Active = alarmActiveCheckbox.Checked,
                Files = AlarmFilesInListbox
            };

            string password = mySqlPwdTextbox.Text;
            string salt = DateTime.Now.ToString();

            byte[] bytePassword = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] byteSalt = System.Text.Encoding.UTF8.GetBytes(salt);

            string encryptedString = CryptoPassword(password);
            mySqlPWDOutputTextbox.Text = encryptedString;

            MySqlConfiguration mySqlConfig = new MySqlConfiguration()
            {
                Host = mySqlHostTextbox.Text,
                Port = Int32.Parse(mySqlPortTextbox.Text),
                Scheme = mySqlSchemeTextbox.Text,

                User = new SqlUser()
                {
                    UserName = mySqlUserTextobox.Text,
                    Password = password,
                }
            };

            if (diagnosticEnabled.Checked)
                newConfiguration.ServiceConfigurations.Add(DiagnosticConfig);

            if (OpcCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(opcConfig);

            if (WebCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(webApiCoreConf);

            if (RobotCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(robotConfigurator);

            if (OpcClientCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(opcClientConfig);

            if (readProgramsCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(readProgramsConfiguration);

            if (mySqlCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(mySqlConfig);

            if (alarmDiagnosticCheckbox.Checked)
                newConfiguration.ServiceConfigurations.Add(alarmConfiguration);

            newConfiguration.ServiceConfigurations.Add(coreCOnfig);

            return newConfiguration;

        }


        private void MakeConfigBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog newFileDialog = new SaveFileDialog();
            newFileDialog.DefaultExt = ".json";
            newFileDialog.AddExtension = true;
            newFileDialog.FileName = "config.json";
            newFileDialog.Title = "Save configuration";
            newFileDialog.Filter = "Configuration (*.json)|*.json";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DialogResult result = newFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK)
            {
                CreateConfigurationFile(newFileDialog.FileName);
            }
        }

        public string CryptoPassword(string password)
        {
            string encryptedString = RSACommon.PasswordSecurity.EncryptString(password);
            string decryptedString = RSACommon.PasswordSecurity.DecryptString(encryptedString);

            if (password == decryptedString)
            {
                return encryptedString;
            }

            return "Error";

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OpcCheckbox.Checked)
                OpcServerGroupbox.Visible = true;
            else
                OpcServerGroupbox.Visible = false;

            if (WebCheckbox.Checked)
                webGB.Visible = true;
            else
                webGB.Visible = false;

            if (RobotCheckbox.Checked)
                RobotGB.Visible = true;
            else
                RobotGB.Visible = false;

            if (OpcClientCheckbox.Checked)
                opcClientConfigGroupbox.Visible = true;
            else
                opcClientConfigGroupbox.Visible = false;

        }

        private void loadConfigBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Filter = "Configuration (*.json)|*.json";
            newFileDialog.Multiselect = false;
            newFileDialog.Title = "Select configuration to load";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DialogResult result = newFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                CoreConfigurationsBuilder builder = new CoreConfigurationsBuilder(Path.GetFileName(newFileDialog.FileName), Path.GetDirectoryName(newFileDialog.FileName));
                CoreConfigurations config = builder.LoadConfiguration(newFileDialog.FileName);
                LoadSetup(config.ServiceConfigurations);
            }
        }
        private void deletePathBtn_Click(object sender, EventArgs e)
        {
            readProgramsPathListbox.Items.RemoveAt(readProgramsPathListbox.SelectedIndex);
        }

        private void addPathBtn_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string uri = Helper.BuildUri(readProgramsSchemeTxtbox.Text, readProgramHostTxtbox.Text).AbsoluteUri;
                string path = Path.Combine(uri, dialog.FileName);

                readProgramsPathListbox.Items.Add(path);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            extensionReadProgramsList.Items.Add(extensionTxt.Text);
        }

        private void removeExtBtn_Click(object sender, EventArgs e)
        {
            if (extensionReadProgramsList.SelectedIndex != -1)
                extensionReadProgramsList.Items.RemoveAt(extensionReadProgramsList.SelectedIndex);

            if (extensionReadProgramsList.Items.Count > 0)
            {
                extensionReadProgramsList.SelectedIndex = 0;
            }

        }

        private void createLogServiceBtn_Click(object sender, EventArgs e)
        {
            CoreConfigurations newConfiguration = CreateCoreConfigurations();

            SaveFileDialog newFileDialog = new SaveFileDialog();
            newFileDialog.DefaultExt = ".json";
            newFileDialog.AddExtension = true;
            newFileDialog.FileName = "LoggerConfiguration.json";
            newFileDialog.Title = "Save configuration";
            newFileDialog.Filter = "Configuration (*.json)|*.json";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DialogResult result = newFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK)
            {
                LoggerConfigurator newLogger = new LoggerConfigurator(newFileDialog.FileName);
                newLogger.CreateDummyLogConfigurations(newConfiguration.ServiceConfigurations, (log4net.Core.Level)loggerMinLevel.SelectedItem);
            }

        }
        public Diagnostic.Core.Diagnostic DiagnosticInstance { get; set; } = null;
        public string DiagnosticFile { get; set; } = string.Empty;
        public DiagnosticConfiguration DiagnosticConfig { get; set; } = null;
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Multiselect = false;
            newFileDialog.Title = "Select diagnostic file to load";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (newFileDialog.ShowDialog() == DialogResult.OK)
            {

                DiagnosticConfig = new DiagnosticConfiguration();
                DiagnosticConfig.DiagnosticFile = newFileDialog.FileName;
                DiagnosticConfig.ServiceName = "Diagnostic for Kawasaki";
                DiagnosticConfig.DiagnosticFormRows = 2;
                DiagnosticConfig.DiagnosticFormColumns = 2;
                DiagnosticConfig.Active = diagnosticIsActive.Checked;
                //DiagnosticInstance = new Diagnostic.Core.Diagnostic(DiagnosticConfig);
            }
        }

        private void ServiceSetup_Load(object sender, EventArgs e)
        {

        }

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Filter = "RSAConfiguration (*.$$$)|*.$$$";
            newFileDialog.Multiselect = false;
            newFileDialog.Title = "Select alarm file to load";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DialogResult result = newFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                alarmFileListBox.Items.Add(newFileDialog.FileName);
            }

            AlarmFilesInListbox.Clear();

            foreach (string filename in alarmFileListBox.Items)
            {
                AlarmFilesInListbox.Add(filename);
            }
        }

        private void deleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = alarmFileListBox.SelectedIndex;

            if (index != -1)
            {
                alarmFileListBox.Items.RemoveAt(index);
                AlarmFilesInListbox.RemoveAt(index);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
