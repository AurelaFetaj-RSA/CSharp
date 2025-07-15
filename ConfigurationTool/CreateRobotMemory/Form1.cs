using Robot;
using RSACommon;
using RSACommon.Configuration;
using RSAInterface.Helper;
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


namespace CreateRobotMemory
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FillMemoryTypeComboBox();
            FillRobotTypeComboBox();
        }

        string loadedVariableName = string.Empty;

        public Dictionary<string, Type> RobotType = new Dictionary<string, Type>() { };

        IRobotMemory<IRobotVariable> virtualizedMemory = new KawasakiMemory((Kawasaki)null);

        public void FillMemoryTypeComboBox()
        {
            MemoryTypeComboBox.Items.Add(typeof(int));
            MemoryTypeComboBox.Items.Add(typeof(string));
            MemoryTypeComboBox.Items.Add(typeof(double));

            MemoryTypeComboBox.SelectedIndex = 0;
        }

        public void FillRobotTypeComboBox()
        {
            RobotType["Kawasaki"] = typeof(Kawasaki);
            RobotType["Fanuc"] = typeof(BaseRobot<IRobotVariable>);
            RobotType["KUKA"] = typeof(BaseRobot<IRobotVariable>);

            CmdTypeComboBox.Items.Add(KawasakiCommand.ty);
            CmdTypeComboBox.Items.Add(KawasakiCommand.po);
            CmdTypeComboBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int parseKey = Int32.Parse(keyTextbox.Text);

            KawasakiMemoryVariable newVar = new KawasakiMemoryVariable()
            {
                Name = variableNameTextbox.Text,
                Type = (Type)MemoryTypeComboBox.SelectedItem,
                CommandType = (KawasakiCommand)CmdTypeComboBox.SelectedItem,
                Key = parseKey
            };

            if (virtualizedMemory.AddMemory(newVar.Name, newVar))
                cmdListbox.Items.Add(newVar.Name);

        }

        private void CmdType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SaveDictBtn_Click(object sender, EventArgs e)
        {
            virtualizedMemory.FixVirtualizedMemory();

            string filename = Path.Combine("", filenameToSaveTextbox.Text);
            Helper.Save(filename, virtualizedMemory);

            resultTextbox.Text = $"Saved {filenameToSaveTextbox.Text}";
        }

        private void genCmdBtn_Click(object sender, EventArgs e)
        {
            string cmd = CmdTypeComboBox.SelectedItem + " " + variableNameTextbox.Text;
            OutputCmdTextbox.Text = cmd;
        }

        private void cmdListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadedVariableName == null || loadedVariableName == "")
                return;

            if (virtualizedMemory is KawasakiMemory kwMem)
            {
                var memoryVar = (KawasakiMemoryVariable)kwMem.VirtualMemory[loadedVariableName];

                for (int i = 0; i < CmdTypeComboBox.Items.Count; i++)
                {
                    if ((KawasakiCommand)CmdTypeComboBox.Items[i] == memoryVar.CommandType)
                    {
                        CmdTypeComboBox.SelectedIndex = i;
                    }
                }

                for (int i = 0; i < MemoryTypeComboBox.Items.Count; i++)
                {
                    if ((Type)MemoryTypeComboBox.Items[i] == memoryVar.Type)
                    {
                        MemoryTypeComboBox.SelectedIndex = i;
                    }
                }

                OutputCmdTextbox.Text = memoryVar.CommandString;
                variableNameTextbox.Text = memoryVar.Name;
                keyTextbox.Text = memoryVar.Key.ToString();

                resultTextbox.Text = $"Selected {memoryVar.Name} element";
            }

        }

        private void LoadMemoryBtn_Click(object sender, EventArgs e)
        {
            Clean();

            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Filter = "Configuration (*.json)|*.json";
            newFileDialog.Multiselect = false;
            newFileDialog.Title = "Select configuration to load";
            newFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            DialogResult result = newFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                virtualizedMemory = Helper.Load<IRobotMemory<IRobotVariable>>(newFileDialog.FileName).FixVirtualizedMemory();

                if (virtualizedMemory is KawasakiMemory kwMem)
                {
                    foreach (var memoryVar in kwMem.VirtualMemory)
                    {
                        if (memoryVar.Value is KawasakiMemoryVariable kwVar)
                            cmdListbox.Items.Add(kwVar.Name);
                    }

                    resultTextbox.Text = "Loading oK";
                    return;
                }
            }

            resultTextbox.Text = "Loading fault";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clean();
        }

        private void Clean()
        {
            cmdListbox.Items.Clear();
            virtualizedMemory.FreeVirtualMemory();
        }

        private void ModifyCmdBtn_Click(object sender, EventArgs e)
        {
            if (loadedVariableName == null || loadedVariableName == "")
                return;
                    
            if (virtualizedMemory is KawasakiMemory kwMem)
            {
                var memoryVar = (KawasakiMemoryVariable)kwMem.VirtualMemory[loadedVariableName];

                memoryVar.CommandType = (KawasakiCommand)CmdTypeComboBox.SelectedItem;
                memoryVar.Key = Int32.Parse(keyTextbox.Text);
                memoryVar.Name = variableNameTextbox.Text;

                resultTextbox.Text = $"{loadedVariableName}: Modified the {memoryVar.Name} object";


                kwMem.VirtualMemory.Remove(loadedVariableName);
                loadedVariableName = null;

                kwMem.VirtualMemory[memoryVar.Name] = memoryVar;


                DeleteFromComboList(cmdListbox.SelectedIndex);
                cmdListbox.Items.Add(memoryVar.Name);
            }
        }

        private void DeleteFromComboList(int index)
        {
            if (index < 0)
                return;

            cmdListbox.Items.RemoveAt(index);
        }

        private void cmdListbox_Click(object sender, EventArgs e)
        {
            loadedVariableName = (string)cmdListbox.SelectedItem;
        }

        private void deleteSelectedBtn_Click(object sender, EventArgs e)
        {
            if (loadedVariableName == null || loadedVariableName == "" )
            {
                resultTextbox.Text = $"Memory is empty";
                return;
            }


            if (virtualizedMemory is KawasakiMemory kwMem)
            {
                resultTextbox.Text = $"Deleted {loadedVariableName}";
                kwMem.VirtualMemory.Remove(loadedVariableName);
                loadedVariableName = null;
                DeleteFromComboList(cmdListbox.SelectedIndex);

                if(cmdListbox.Items.Count == 0)
                {
                    resultTextbox.Text = $"Memory is empty";
                    return;
                }

                cmdListbox.SelectedIndex = 0;
                loadedVariableName = (string)cmdListbox.SelectedItem;
            }


        }
    }
}
