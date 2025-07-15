using Newtonsoft.Json.Linq;
//using Opc.Ua;
//using Opc.UaFx;
//using Opc.UaFx.Server;
using Robot;
using RSACommon;
using RSACommon.GraphicsForm;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Diagnostic;
using Diagnostic.State;
//using OpcCustom;
using RSACommon.Service;
using RSACommon.Event;
using System.IO;
using static System.Collections.Specialized.BitVector32;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using LBSoft.IndustrialCtrls.Leds;
using System.Security.Cryptography;
using Opc.UaFx;
using static Nevron.Interop.Win32.NWin32;
using System.ServiceModel;
namespace ProductionLaunch
{
    public partial class FormApp : Form
    {
        FormRFIDNewModify frm = new FormRFIDNewModify();       

        public async Task UpdateDiagnosticGUI(TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            while (true)
            {
                await CheckDiagnostic();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }   

        private void StartDiagnosticGUI()
        {
            //int xCoord = 10;
            //MatrixPanel diagnosticMatrix = new MatrixPanel(xCoord, 5, this.TabPageDiagnostic.Width - 5, this.TabPageDiagnostic.Height - 150, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormRow, myCore.DiagnosticConfigurator.Configuration.DiagnosticFormColumn, this.TabPageDiagnostic);

            //foreach (string value in myCore.DiagnosticConfigurator.VariableList)
            //{
            //    DiagnosticWindowsControl form = new DiagnosticWindowsControl(0, 0, value, null, null);
            //    DiagnosticVariableGroupbox[value] = form;

            //    if (!diagnosticMatrix.AddElements(form, 5, 5))
            //    {
            //        MessageBox.Show("Too much Diagnostic Variable for Panel Matrix");
            //        break;
            //    }

            //}
        }

        public async Task CheckDiagnostic()
        {
            return;
            //List<string> variableList = myCore.DiagnosticConfigurator.DiagnosticStatus.Keys.ToList();

            //foreach(string diagnosticVariableName in variableList)
            //{
            //    //int value = await myRobot.VirtualizedMemory.GetMemoryValueAsync<int>(diagnosticVariableName);
            //    string command = KawasakiMemoryVariable.MakeCommand(KawasakiCommand.ty, diagnosticVariableName);

            //    if (myRobot == null)
            //        return;


            //    int value = await myRobot?.ReadCommandAsync<int>(command);

            //    if (myCore.DiagnosticConfigurator.DiagnosticResult(diagnosticVariableName, value, out DiagnosticState state))
            //    {

            //        //controllo aggiunto per tracciare solo la variazione di Stato.
            //        if(!_lastState.ContainsKey(diagnosticVariableName) || _lastState[diagnosticVariableName] != value)
            //        {
            //            string stateOutput = state.DiagnosticMessage;

            //            if (DiagnosticVariableGroupbox.Count != 0)
            //            {
            //                DiagnosticVariableGroupbox[diagnosticVariableName].ThreadSafeWriteMessage($"| {value:000} | {stateOutput}");
            //            }

            //            _lastState[diagnosticVariableName] = value;
            //        }

            //    }
            //}
        }

        public async Task UpdateOPCUAStatus(TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            while (true)
            {
                await UpdateOPCUAStatus();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public async Task UpdateGUI(TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            while (true)
            {
                await UpdateGUI();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public async Task UpdateOPCUAReadOnly(TimeSpan interval, CancellationTokenSource cancellationToken)
        {
            while (true)
            {
                await UpdateOPCUAReadOnly();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public async Task UpdateGUI()
        {
            try
            {


                WriteAsyncOnTextbox(textBoxPallet1, MachineRuntime.pallet1Size);
                WriteAsyncOnTextbox(textBoxPallet2, MachineRuntime.pallet2Size);
                WriteAsyncOnTextbox(textBoxPallet3, MachineRuntime.pallet3Size);
                WriteAsyncOnTextbox(textBoxPallet4, MachineRuntime.pallet4Size);

                WriteAsyncOnTextbox(textBoxRFID_T0, MachineRuntime.rfidCodeG1A1);
                WriteAsyncOnTextbox(textBoxRFIDSize_T0, MachineRuntime.rfidSizeG1A1);
                WriteAsyncOnTextbox(textBoxRFIDFoot_T0, MachineRuntime.rfidFootG1A1);
                WriteAsyncOnCombobox(comboBoxRFIDVariant_T0, MachineRuntime.rfidVariantG1A1);
                WriteAsyncOnLed(lbLedRFIDResult_T0, MachineRuntime.rfidReadingResultG1A1);
                WriteAsyncOnLed(lbLedRFIDRequest_T0, MachineRuntime.rfidReadingRequestG1A1);
                WriteAsyncOnLedReady(lbLedRFIDR1Ready, MachineRuntime.r1.GetRobotReady());
                WriteAsyncOnLedReady(lbLedRFIDR2Ready, MachineRuntime.r2.GetRobotReady());
                
                //WriteAsyncOnLedReady(lbLedRFIDR3Ready, MachineRuntime.r3.GetRobotReady());
                //WriteAsyncOnLedReady(lbLedRFIDOV1Ready, MachineRuntime.gdevice1.GetDeviceReady());
                //WriteAsyncOnLedReady(lbLedRFIDOV2Ready, MachineRuntime.gdevice2.GetDeviceReady());                                
                WriteAsyncOnTextbox(textBoxRFIDG1A2, MachineRuntime.rfidCodeG1A2);
                WriteAsyncOnTextbox(textBoxSizeG1A2, MachineRuntime.rfidSizeG1A2);
                WriteAsyncOnTextbox(textBoxTypeG1A2, MachineRuntime.rfidTypeFromPlcG1A2);
                WriteAsyncOnTextbox(textBoxMNameG1A2, MachineRuntime.rfidModelNameG1A2);
                WriteAsyncOnLed(lbLedRFIDResultG1A2, MachineRuntime.rfidReadingResultG1A2);
                WriteAsyncOnLed(lbLedRFIDRequestG1A2, MachineRuntime.rfidReadingRequestG1A2);
                WriteAsyncOnTextbox(textBoxResultStringG1A2, MachineRuntime.rfidReadingResultStringG1A2);
                WriteAsyncOnLedReady(lbLedRFIDR1RSWareConnection, myRSA.GetPLServer().GetClientConnectionStatus(IPAddress.Parse("172.31.10.146"), 9999));
                WriteAsyncOnLedReady(lbLedRFIDR2RSWareConnection, myRSA.GetPLServer().GetClientConnectionStatus(IPAddress.Parse("172.31.10.166"), 9999));
                if (tcp != null) WriteAsyncOnLedReady(lbLedRFIDR3RSWareConnection, tcp.IsConnected());

                WriteAsyncOnLedMainSem(lbLedMainSemRed, PLCLineOutputDictionary[5], Color.Red);
                WriteAsyncOnLedMainSem(lbLedMainSemYellow, PLCLineOutputDictionary[6], Color.Yellow);
                WriteAsyncOnLedMainSem(lbLedMainSemGreen, PLCLineOutputDictionary[7], Color.LightGreen);

                //line status machine
                WriteAsyncOnButtonStatusMachine(buttonLineMachineStatus, MachineRuntime.lineMachineStatus);
                WriteAsyncOnLabelkStatusMachine(labelLineMachineStatus, MachineRuntime.lineMachineStatus);                

                if (MachineRuntime.lineMachineStatus == 0)
                    WriteAsyncOnLabelTextForeColor(labelResetEmergency, "reset emergency", Color.Red);
                else WriteAsyncOnLabelTextForeColor(labelResetEmergency, "emergency ok", Color.FromArgb(63, 124, 203));

                //start/stop status
                if (PLCLineOutputDictionary[8] == true)
                    WriteAsyncOnLabelTextForeColor(labelStartStop, "line started", Color.FromArgb(63, 124, 203));
                else WriteAsyncOnLabelTextForeColor(labelStartStop, "stop button pressed", Color.Red);

                //start/stop status
                if (PLCLineInputDictionary[4] == true)
                    WriteAsyncOnLabelTextForeColor(labelAirPressure, "air pressure ok", Color.FromArgb(63, 124, 203));
                else WriteAsyncOnLabelTextForeColor(labelAirPressure, "air pressure not ok", Color.Red);

                //selector status
                String sStatus = "";
                if (PLCLineInputDictionary[5] == true)
                    sStatus = "selector in automatic";
                else sStatus = "selector in manual";
                WriteAsyncOnLabelSelector(labelSelectorStatus, sStatus, PLCLineInputDictionary[5]);

                //gdevice runtime                
                WriteAsyncOnLabelTextForeColor(labelGD1RuntimeParam1, MachineRuntime.gdevice1.gdRuntimeParam1.ToString(), Color.Black);
                WriteAsyncOnLabelTextForeColor(labelGD1RuntimeParam2, MachineRuntime.gdevice1.gdRuntimeParam2.ToString(), Color.Black);

                //WriteAsyncOnLabelTextForeColor(labelGD2RuntimeParam1, MachineRuntime.gdevice2.gdRuntimeParam1.ToString(), Color.Black);
                //WriteAsyncOnLabelTextForeColor(labelGD2RuntimeParam2, MachineRuntime.gdevice2.gdRuntimeParam2.ToString(), Color.Black);
                //WriteAsyncOnLabelTextForeColor(labelGD2RuntimeParam3, MachineRuntime.gdevice2.gdRuntimeParam3.ToString(), Color.Black);
                //WriteAsyncOnLabelTextForeColor(labelGD2RuntimeParam4, MachineRuntime.gdevice2.gdRuntimeParam4.ToString(), Color.Black);

                //GD1 alarms
                GD1AlarmsDictionary[1] = AlarmsDictionary[32];
                GD1AlarmsDictionary[2] = AlarmsDictionary[33];
                GD1AlarmsDictionary[3] = AlarmsDictionary[34];
                WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R0, GD1AlarmsDictionary[1]);
                WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R1, GD1AlarmsDictionary[2]);
                WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R2, GD1AlarmsDictionary[3]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R3, GD1AlarmsDictionary[4]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R4, GD1AlarmsDictionary[5]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R5, GD1AlarmsDictionary[6]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD1R6, GD1AlarmsDictionary[7]);
                //MachineRuntime.gdevice1.gdIsInAlarm = (GD1AlarmsDictionary[1] == 1 || GD1AlarmsDictionary[2] == 1 || GD1AlarmsDictionary[3] == 1 ||
                //        GD1AlarmsDictionary[4] == 1 || GD1AlarmsDictionary[5] == 1 || GD1AlarmsDictionary[6] == 1 ||
                //        GD1AlarmsDictionary[7] == 1);
                //if (MachineRuntime.gdevice1.gdIsInAlarm) WriteAsyncOnButtonGeneral(buttonGD1RegisterAlarms, "show alarms", Color.Red, Color.WhiteSmoke);
                //else WriteAsyncOnButtonGeneral(buttonGD1RegisterAlarms, "no alarms", Color.FromArgb(5, 163, 161), Color.WhiteSmoke);

                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R0, GD2AlarmsDictionary[1]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R1, GD2AlarmsDictionary[2]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R2, GD2AlarmsDictionary[3]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R3, GD2AlarmsDictionary[4]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R4, GD2AlarmsDictionary[5]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R5, GD2AlarmsDictionary[6]);
                //WriteAsyncOnButtonGDRegisterAlarm(buttonGD2R6, GD2AlarmsDictionary[7]);
                //MachineRuntime.gdevice2.gdIsInAlarm = GD2AlarmsDictionary[1] == 1 || GD2AlarmsDictionary[2] == 1 || GD2AlarmsDictionary[3] == 1 ||
                //        GD2AlarmsDictionary[4] == 1 || GD2AlarmsDictionary[5] == 1 || GD2AlarmsDictionary[6] == 1 ||
                //        GD2AlarmsDictionary[7] == 1;

                //if (MachineRuntime.gdevice2.gdIsInAlarm) WriteAsyncOnButtonGeneral(buttonGD2RegisterAlarms, "show alarms", Color.Red, Color.WhiteSmoke);
                //else WriteAsyncOnButtonGeneral(buttonGD2RegisterAlarms, "no alarms", Color.FromArgb(5, 163, 161), Color.WhiteSmoke);

                WriteAsyncOnLedMainSem(lbLedR1SemRed, PLCR1OutputDictionary[14], Color.Red);
                WriteAsyncOnLedMainSem(lbLedR1SemOrange, PLCR1OutputDictionary[15], Color.Yellow);
                WriteAsyncOnLedMainSem(lbLedR1SemGreen, PLCR1OutputDictionary[16], Color.LightGreen);

                WriteAsyncOnLedMainSem(lbLedR2SemRed, PLCR2OutputDictionary[14], Color.Red);
                WriteAsyncOnLedMainSem(lbLedR2SemOrange, PLCR2OutputDictionary[15], Color.Yellow);
                WriteAsyncOnLedMainSem(lbLedR2SemGreen, PLCR2OutputDictionary[16], Color.LightGreen);

                //WriteAsyncOnLedMainSem(lbLedR3SemRed, WAGOR3OutputDictionary[3], Color.Red);
                //WriteAsyncOnLedMainSem(lbLedR3SemOrange, WAGOR3OutputDictionary[2], Color.Orange);
                //WriteAsyncOnLedMainSem(lbLedR3SemGreen, WAGOR3OutputDictionary[1], Color.LightGreen);

                //main alarms
                WriteAsyncOnButtonRobotRegisterAlarm(buttonMainWago, AlarmsDictionary[1]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonCamozziLine1, AlarmsDictionary[2]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonCamozziLine2, AlarmsDictionary[3]);
                //termic
                WriteAsyncOnButtonRobotRegisterAlarm(buttonTermic1, AlarmsDictionary[16]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonTermic2, AlarmsDictionary[17]);

                //R1 alarms
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm1, AlarmsDictionary[4]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm2, AlarmsDictionary[5]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm3, AlarmsDictionary[6]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm4, AlarmsDictionary[7]);

                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm5, AlarmsDictionary[12]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm6, AlarmsDictionary[14]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm7, AlarmsDictionary[19]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR1Alarm8, AlarmsDictionary[22]);

                //R2 alarm
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm1, AlarmsDictionary[8]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm2, AlarmsDictionary[9]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm3, AlarmsDictionary[13]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm4, AlarmsDictionary[15]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm5, AlarmsDictionary[20]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm6, AlarmsDictionary[23]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm7, AlarmsDictionary[58]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm8, AlarmsDictionary[51]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR2Alarm9, AlarmsDictionary[52]);


                //scanner alarms
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm1, AlarmsDictionary[24]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm2, AlarmsDictionary[26]);
                //hiwin disconnected
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm3, AlarmsDictionary[10]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm4, AlarmsDictionary[110]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm5, AlarmsDictionary[117]);
                WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm6, AlarmsDictionary[50]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm7, AlarmsDictionary[59]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm8, AlarmsDictionary[53]);
                //WriteAsyncOnButtonRobotRegisterAlarm(buttonR3Alarm9, AlarmsDictionary[54]);

                //pl external emergency
                WriteAsyncOnButtonRobotRegisterAlarm(buttonLEmergency, AlarmsDictionary[11]);


                
                if (MachineRuntime.rfidReadingResultG1A1 == 2 || MachineRuntime.rfidReadingResultG1A1 == -1)
                WriteAsyncOnLabelTextForeColor(labelRFIDResultString, MachineRuntime.rfidReadingResultStringG1A1, Color.Red);
                else WriteAsyncOnLabelTextForeColor(labelRFIDResultString, MachineRuntime.rfidReadingResultStringG1A1, Color.FromArgb(63, 124, 203));
                //emergency buttons
                WriteAsyncOnButtonAlarms(buttonAllarm30, AlarmsDictionary[30]);
                WriteAsyncOnButtonAlarms(buttonAllarm31, AlarmsDictionary[31]);
                WriteAsyncOnButtonAlarms(buttonAllarm32, AlarmsDictionary[32]);
                WriteAsyncOnButtonAlarms(buttonAllarm33, AlarmsDictionary[33]);
                WriteAsyncOnButtonAlarms(buttonAllarm34, AlarmsDictionary[34]);
                WriteAsyncOnButtonAlarms(buttonAllarm35, AlarmsDictionary[35]);
                WriteAsyncOnButtonAlarms(buttonAllarm36, AlarmsDictionary[36]);
                WriteAsyncOnButtonAlarms(buttonAllarm37, AlarmsDictionary[37]);
                WriteAsyncOnButtonAlarms(buttonAllarm38, AlarmsDictionary[38]);
                WriteAsyncOnButtonAlarms(buttonAllarm39, AlarmsDictionary[39]);
                WriteAsyncOnButtonAlarms(buttonAllarm40, AlarmsDictionary[40]);
                WriteAsyncOnButtonAlarms(buttonAllarm41, AlarmsDictionary[41]);
                WriteAsyncOnButtonAlarms(buttonAllarm42, AlarmsDictionary[42]);
                WriteAsyncOnButtonAlarms(buttonAllarm43, AlarmsDictionary[43]);
                WriteAsyncOnButtonAlarms(buttonAllarm44, AlarmsDictionary[44]);

                //stop pallet timeout
                WriteAsyncOnButtonAlarms(buttonAlarm65, AlarmsDictionary[65]);
                WriteAsyncOnButtonAlarms(buttonAlarm66, AlarmsDictionary[66]);
                WriteAsyncOnButtonAlarms(buttonAlarm67, AlarmsDictionary[67]);
                WriteAsyncOnButtonAlarms(buttonAlarm68, AlarmsDictionary[68]);
                WriteAsyncOnButtonAlarms(buttonAlarm69, AlarmsDictionary[69]);
                WriteAsyncOnButtonAlarms(buttonAlarm70, AlarmsDictionary[70]);
                WriteAsyncOnButtonAlarms(buttonAlarm71, AlarmsDictionary[71]);
                WriteAsyncOnButtonAlarms(buttonAlarm72, AlarmsDictionary[72]);
                WriteAsyncOnButtonAlarms(buttonAlarm73, AlarmsDictionary[73]);
                WriteAsyncOnButtonAlarms(buttonAlarm74, AlarmsDictionary[74]);
                WriteAsyncOnButtonAlarms(buttonAlarm75, AlarmsDictionary[75]);
                WriteAsyncOnButtonAlarms(buttonAlarm76, AlarmsDictionary[76]);
                WriteAsyncOnButtonAlarms(buttonAlarm77, AlarmsDictionary[77]);
                WriteAsyncOnButtonAlarms(buttonAlarm78, AlarmsDictionary[78]);
                WriteAsyncOnButtonAlarms(buttonAlarm79, AlarmsDictionary[79]);


                WriteAsyncOnButtonAlarms(buttonAlarm80, AlarmsDictionary[80]);
                WriteAsyncOnButtonAlarms(buttonAlarm81, AlarmsDictionary[81]);
                WriteAsyncOnButtonAlarms(buttonAlarm82, AlarmsDictionary[82]);
                WriteAsyncOnButtonAlarms(buttonAlarm83, AlarmsDictionary[83]);
                WriteAsyncOnButtonAlarms(buttonAlarm84, AlarmsDictionary[84]);
                WriteAsyncOnButtonAlarms(buttonAlarm85, AlarmsDictionary[85]);
                WriteAsyncOnButtonAlarms(buttonAlarm86, AlarmsDictionary[86]);
                WriteAsyncOnButtonAlarms(buttonAlarm87, AlarmsDictionary[87]);
                WriteAsyncOnButtonAlarms(buttonAlarm88, AlarmsDictionary[88]);
                WriteAsyncOnButtonAlarms(buttonAlarm89, AlarmsDictionary[89]);
                WriteAsyncOnButtonAlarms(buttonAlarm90, AlarmsDictionary[90]);
                WriteAsyncOnButtonAlarms(buttonAlarm91, AlarmsDictionary[91]);


                WriteAsyncOnButtonAlarms(buttonAlarm92, AlarmsDictionary[92]);
                WriteAsyncOnButtonAlarms(buttonAlarm93, AlarmsDictionary[93]);
                WriteAsyncOnButtonAlarms(buttonAlarm94, AlarmsDictionary[94]);
                WriteAsyncOnButtonAlarms(buttonAlarm95, AlarmsDictionary[95]);
                WriteAsyncOnButtonAlarms(buttonAlarm96, AlarmsDictionary[96]);
                WriteAsyncOnButtonAlarms(buttonAlarm97, AlarmsDictionary[97]);
                WriteAsyncOnButtonAlarms(buttonAlarm98, AlarmsDictionary[98]);
                WriteAsyncOnButtonAlarms(buttonAlarm99, AlarmsDictionary[99]);
                WriteAsyncOnButtonAlarms(buttonAlarm100, AlarmsDictionary[100]);
                WriteAsyncOnButtonAlarms(buttonAlarm101, AlarmsDictionary[101]);
                WriteAsyncOnButtonAlarms(buttonAlarm102, AlarmsDictionary[102]);


                WriteAsyncOnButtonAlarms(buttonAlarm103, AlarmsDictionary[103]);
                WriteAsyncOnButtonAlarms(buttonAlarm104, AlarmsDictionary[104]);
                WriteAsyncOnButtonAlarms(buttonAlarm105, AlarmsDictionary[105]);
                WriteAsyncOnButtonAlarms(buttonAlarm106, AlarmsDictionary[106]);
                WriteAsyncOnButtonAlarms(buttonAlarm107, AlarmsDictionary[107]);
                WriteAsyncOnButtonAlarms(buttonAlarm108, AlarmsDictionary[108]);
                WriteAsyncOnButtonAlarms(buttonAlarm109, AlarmsDictionary[109]);
                WriteAsyncOnButtonAlarms(buttonAlarm110, AlarmsDictionary[110]);
                WriteAsyncOnButtonAlarms(buttonAlarm111, AlarmsDictionary[111]);
                WriteAsyncOnButtonAlarms(buttonAlarm112, AlarmsDictionary[112]);
                WriteAsyncOnButtonAlarms(buttonAlarm113, AlarmsDictionary[113]);
                WriteAsyncOnButtonAlarms(buttonAlarm114, AlarmsDictionary[114]);

                IsLineInTimeoutAlarm();
                if (MachineRuntime.lineInTimeout) WriteAsyncOnButtonGeneral(buttonLineTimeoutAlarms, "show timeout alarms", Color.Red, Color.WhiteSmoke);
                else WriteAsyncOnButtonGeneral(buttonLineTimeoutAlarms, "no timeout alarms", Color.FromArgb(5, 163, 161), Color.WhiteSmoke);

                //gdevice1 disconnected status
                MachineRuntime.gdevice1.gdIsDisconnected = Convert.ToBoolean(AlarmsDictionary[17]);
                MachineRuntime.gdevice2.gdIsDisconnected = Convert.ToBoolean(AlarmsDictionary[18]);
                MachineRuntime.r1.SetRobotIsDisconnected(Convert.ToBoolean(AlarmsDictionary[7]));
                MachineRuntime.r2.SetRobotIsDisconnected(Convert.ToBoolean(AlarmsDictionary[9]));

                if (MachineRuntime.scannerSolePresence) WriteAsyncOnButtonGeneral(buttonSolePresence, Color.DarkGreen);
                else WriteAsyncOnButtonGeneral(buttonSolePresence, Color.LightGreen);

                if (MachineRuntime.scannerAxisHomingDone) WriteAsyncOnButtonGeneral(buttonHomingDone, Color.FromArgb(63, 124, 203));
                else WriteAsyncOnButtonGeneral(buttonHomingDone, Color.Red);

                if (MachineRuntime.scannerAxisInAlarm) WriteAsyncOnButtonGeneral(buttonAxisInAlarm, Color.Red);
                else WriteAsyncOnButtonGeneral(buttonAxisInAlarm, Color.FromArgb(63, 124, 203));


                MachineRuntime.triggerToScanner = PLCLineOutputDictionary[11];
                if (MachineRuntime.triggerToScanner) WriteAsyncOnButtonGeneral(buttonScannerTrigger, Color.LightGreen);
                else WriteAsyncOnButtonGeneral(buttonScannerTrigger, Color.DarkGreen);

            }
            catch (Exception ex)
            {
                Common.WritelogFile(file_log_line, "UpdateGUI " + ex.Message);
            }

            //check client connection
            if (tcp == null) ;
            else
            {
                if (tcp.IsConnected())
                {

                }
                else
                {
                    //try to reconnect to server
                    //OnDisconnect();
                    ClientConnect("172.31.10.126", "49155");
                }
            }
        }

        public void IsLineInTimeoutAlarm()
        {
            int i = 0;
            bool ret = false;
            for (i = 65; i <= 128; i++)
            {
                if (AlarmsDictionary[i] == 1)
                {
                    ret = true;
                    break;
                }
            }

            MachineRuntime.lineInTimeout = ret;
        }

        public void WriteAsyncOnCombobox(ComboBox combo, string value)
        {
            if (combo.InvokeRequired)
            {
                combo.Invoke((MethodInvoker)delegate
                {
                    combo.Text = value;
                });
            }
        }

        public void WriteAsyncOnTextbox(TextBox txtbox, string value)
        {
            if (txtbox.InvokeRequired)
            {
                txtbox.Invoke((MethodInvoker)delegate
                {
                    txtbox.Text = value;
                });
            }
        }

        public void WriteAsyncOnLed(LBLed led, int value)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {
                    if (value == -2) led.LedColor = Color.DarkGreen;
                    //-1 = rfid not read
                    if (value == -1) led.LedColor = Color.Red;
                    //0 = rfid_new
                    if (value == 0) led.LedColor = Color.LightGreen;
                    //1 = rfid_old
                    if (value == 1) led.LedColor = Color.LightGreen;
                    if (value == 2) led.LedColor = Color.Red;
                });
            }
        }

        public void WriteAsyncOnLed(LBLed led, bool value)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {
                    if (!value) led.LedColor = Color.DarkGreen;
                    if (value) led.LedColor = Color.LightGreen;
                });
            }
        }

        public void WriteAsyncOnLabelSelector(Label lbl, string value, bool selValue)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    if (!selValue) lbl.ForeColor = Color.Red;
                    if (selValue) lbl.ForeColor = Color.FromArgb(63, 124, 203);
                    lbl.Text = value;
                });
            }
        }

        public void WriteAsyncOnLedReady(LBLed led, bool value)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {
                    if (!value) led.LedColor = Color.Red;
                    if (value) led.LedColor = Color.LightGreen;
                });
            }
        }
        public void WriteAsyncOnLedMainSem(LBLed led, bool value, Color semColor)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {
                    if (!value) led.LedColor = Color.DarkGreen;
                    if (value) led.LedColor = semColor;
                });
            }
        }

        public void WriteAsyncOnLedGeneral(LBLed led, Color semColor)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {                   
                    led.LedColor = semColor;
                });
            }
        }

        public void WriteAsyncOnButtonStatusMachine(Button btn, short value)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    string statusStr = "";
                    if (value == -1)
                    {
                        btn.BackColor = Color.Black;
                        statusStr = "offline";                        
                    }
                    if (value == 0)
                    {
                        btn.BackColor = Color.Red;
                        statusStr = "emergency";
                    }
                    if (value == 1)
                    {
                        btn.BackColor = Color.Blue;                        
                        statusStr = "automatic";
                    }
                    if (value == 2)
                    {
                        btn.BackColor = Color.Orange;
                        statusStr = "manual";
                    }
                    if (value == 3)
                    {
                        btn.BackColor = Color.LightGreen;
                        statusStr = "in cycle";
                    }
                    if (value == 4)
                    {
                        btn.BackColor = Color.DarkOrange;
                        statusStr = "in alarm";
                    }
                });
            }
        }

        public void WriteAsyncOnButtonGDeviceStatusMachine(Button btn, short value)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        btn.BackColor = Color.Black;
                    }
                    if (value == 0)
                    {
                        //stand by
                        btn.BackColor = Color.DarkGreen;                        
                    }
                    if (value == 1)
                    {
                        btn.BackColor = Color.Orange;
                    }
                    if (value == 2)
                    {
                        btn.BackColor = Color.DarkOrange;                        
                    }
                    if (value == 3)
                    {
                        btn.BackColor = Color.LightGreen;
                    }
                    if (value == 4)
                    {
                        btn.BackColor = Color.Red;
                    }
                });
            }
        }

        public void WriteAsyncOnButtonGDRegisterAlarm(Button btn, short value)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        btn.BackColor = Color.Black;
                    }
                    if (value == 0)
                    {
                        //stand by
                        btn.BackColor = Color.FromArgb(63, 124, 203);
                    }
                    if (value == 1)
                    {
                        btn.BackColor = Color.Red;
                    }                    
                });
            }
        }

        public void WriteAsyncOnButtonAlarms(Button btn, short value)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        btn.BackColor = Color.DarkGreen;
                    }
                    if (value == 0)
                    {
                        //stand by
                        btn.BackColor = Color.FromArgb(63, 124, 203);
                    }
                    if (value == 1)
                    {
                        btn.BackColor = Color.Red;
                    }
                });
            }
        }

        public void WriteAsyncOnButtonGeneral(Button btn, Color btnColor)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    btn.BackColor = btnColor;
                });
            }
        }

        public void WriteAsyncOnButtonRobotRegisterAlarm(Button btn, short value)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        btn.BackColor = Color.Black;
                    }
                    if (value == 0)
                    {
                        //stand by
                        btn.BackColor = Color.FromArgb(63,124,203);
                    }
                    if (value == 1)
                    {
                        btn.BackColor = Color.Red;
                    }
                });
            }
        }

        public void WriteAsyncOnLabelTextForeColor(Label lbl, string value, Color labelColor)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    lbl.Text = value;
                    lbl.ForeColor = labelColor;
                    
                });
            }
        }

        public void WriteAsyncOnButtonGeneral(Button btn, string value, Color btnForeColor, Color btnBackColor)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    btn.Text = value;
                    btn.ForeColor = btnForeColor;
                    btn.BackColor = btnBackColor;
                });
            }
        }

        public void WriteAsyncOnLabelkStatusMachine(Label lbl, short value)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        //lbl.BackColor = Color.Black;
                        lbl.Text = "offline";
                    }
                    if (value == 0)
                    {
                        //btn.BackColor = Color.Red;
                        lbl.Text = "emergency";
                    }
                    if (value == 1)
                    {
                        //btn.BackColor = Color.LightBlue;
                        lbl.Text = "automatic";                        
                    }
                    if (value == 2)
                    {
                        //btn.BackColor = Color.Orange;
                        lbl.Text = "manual";
                    }
                    if (value == 3)
                    {
                        lbl.ForeColor = Color.LightGreen;
                        lbl.Text = "in cycle";
                    }
                    if (value == 4)
                    {
                        //btn.BackColor = Color.DarkOrange;
                        lbl.Text = "in alarm";
                    }
                });
            }
        }

        public void WriteAsyncOnLabelGDeviceStatusMachine(Label lbl, short value)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    if (value == -1)
                    {
                        //lbl.BackColor = Color.Black;
                        lbl.Text = "offline";
                    }
                    if (value == 0)
                    {
                        //btn.BackColor = Color.Red;
                        lbl.Text = "stand by";
                    }
                    if (value == 1)
                    {
                        //btn.BackColor = Color.LightBlue;
                        lbl.Text = "heating";
                    }
                    if (value == 2)
                    {
                        //btn.BackColor = Color.Orange;
                        lbl.Text = "maintenance";
                    }
                    if (value == 3)
                    {
                        //lbl.ForeColor = Color.LightGreen;
                        lbl.Text = "working";
                    }
                    if (value == 4)
                    {
                        //btn.BackColor = Color.DarkOrange;
                        lbl.Text = "in alarm";
                    }
                });
            }
        }


        public void WriteAsyncOnLabelReady(LBLed led, bool value)
        {
            if (led.InvokeRequired)
            {
                led.Invoke((MethodInvoker)delegate
                {
                    if (!value) led.LedColor = Color.Red;
                    if (value) led.LedColor = Color.LightGreen;
                });
            }
        }


        public async Task UpdateOPCUAStatus()
        {
            if (ccService == null) return;

            try
            {
                if (ccService.ClientIsConnected)
                {
                    try
                    {
                        #region(* keep alive *)
                        var readKeepAliveResult = await ccService.Read("pcClientKeepAlive");

                        //keep alive
                        UpdateOPCUAKeepAlive(readKeepAliveResult, lbLedKeepAliveW);
                        #endregion

                        #region (* restart from plc *)
                        string key = "pcPLCRestart";
                        var resultRestart = await ccService.Read(key);
                        if (resultRestart.Value != null)
                        {
                            if (bool.Parse(resultRestart.Value.ToString()))
                            {
                                SendOPCUATopicOnRestart();
                            }
                        }
                        #endregion

                        
                   
                    }
                    catch (Exception ex)
                    {
                        int i = 0;
                    }
                }
                else
                {
                    ccService.Disconnect();
                    await ccService.Connect();
                    Common.WritelogFile(file_log_line, "UpdateOPCUAStatus: server disconnected");
                }
            }
            catch (Exception Ex)
            {
                int j = 0;
                Common.WritelogFile(file_log_line, "UpdateOPCUAStatus " + Ex.Message);
            }
        }

        public async void UpdateOPCUAKeepAlive(ClientResult cr, LBSoft.IndustrialCtrls.Leds.LBLed lblLed)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo manage error
                lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Blink;
            }
            else
            {
                if ((bool)cr.Value)
                {
                    var sendResult = await ccService.Send(cr.Key, false);
                    lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.Off;
                }
                else
                {
                    var sendResult = await ccService.Send(cr.Key, true);
                    lblLed.State = LBSoft.IndustrialCtrls.Leds.LBLed.LedState.On;
                }
            }
        }
        public async void SendOPCUATopicOnRestart()
        {
            if (ccService == null) return;
            if (ccService.ClientIsConnected == false) return;
            string keyToSend = null;

            keyToSend = "pcR1Inclusion";
            var sendResult = await ccService.Send(keyToSend, MachineRuntime.r1.GetRobotInclusion());            

            keyToSend = "pcR1CleaningInclusion";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.r1.GetRobotInclusionCleaning());

            keyToSend = "pcR2Inclusion";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.r2.GetRobotInclusion());
           
            keyToSend = "pcGD1Inclusion";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice1.GetDeviceInclusion());

            //keyToSend = "pcGD2Inclusion";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice2.GetDeviceInclusion());


            keyToSend = "pcGD1Param1";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice1.GetDeviceParam1());


            keyToSend = "pcGD1Param2";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice1.GetDeviceParam2());


            //keyToSend = "pcGD1Param3";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice1.GetDeviceParam3());


            //keyToSend = "pcGD1Param4";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice1.GetDeviceParam4());


            //keyToSend = "pcGD2Param1";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice2.GetDeviceParam1());


            //keyToSend = "pcGD2Param2";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice2.GetDeviceParam2());


            //keyToSend = "pcGD2Param3";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice2.GetDeviceParam3());


            //keyToSend = "pcGD2Param4";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.gdevice2.GetDeviceParam4());

            //keyToSend = "pcAutoManMode";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.lineReadingAuto);

            //keyToSend = "pcUnloadAutoManMode";
            //sendResult = await ccService.Send(keyToSend, MachineRuntime.lineUnloadReadingAuto);

            keyToSend = "pcTimerAutoReading";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.lineTimerReadingAuto);

            keyToSend = "pcTimerOnEmptyPallet";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.lineTimerEmpty);

            keyToSend = "pcTimerReadingTimeout";
            sendResult = await ccService.Send(keyToSend, MachineRuntime.lineTimerTimeout);
        }

        public async Task UpdateOPCUAReadOnly()
        {
            if (ccService == null) return;

            try
            {
                if (ccService.ClientIsConnected)
                {
                    try
                    {
                        List<string> keys = new List<string>()
                        {
                        "pcR1Ready",
                        "pcR2Ready",
                        //"pcR3Ready",
                        //"pcGD1Ready",
                        //"pcGD2Ready",
                        };
                        var readResults = await ccService.Read(keys);

                        if (readResults == null)
                        {

                        }
                        else
                        {
                            MachineRuntime.r1.SetRobotReady(bool.Parse(readResults["pcR1Ready"].Value.ToString()));
                            MachineRuntime.r2.SetRobotReady(bool.Parse(readResults["pcR2Ready"].Value.ToString()));
                            //MachineRuntime.r3.SetRobotReady(bool.Parse(readResults["pcR3Ready"].Value.ToString()));
                            //MachineRuntime.gdevice1.SetDeviceReady(bool.Parse(readResults["pcGD1Ready"].Value.ToString()));
                            //MachineRuntime.gdevice2.SetDeviceReady(bool.Parse(readResults["pcGD2Ready"].Value.ToString()));
                        }

                        #region(* I/O PLC LINEA *)
                        keys = new List<string>()
                        {
                            "pcPLCLINEDI",
                            "pcPLCLINEDO"
                        };

                        var readLineIOResult = await ccService.Read(keys);
                        if (readLineIOResult == null)
                        {
                        }
                        else
                        {
                            UpdateOPCUADIO(readLineIOResult["pcPLCLINEDI"], PLCLineInputDictionary, Properties.Settings.Default.LinePLCInputNumber);
                            UpdateOPCUADIO(readLineIOResult["pcPLCLINEDO"], PLCLineOutputDictionary, Properties.Settings.Default.LinePLCOutputNumber);

                        }
                        #endregion

                        string keySize = "pcTrackingSize";
                        var readSizes = await ccService.Read(keySize);

                        if (readSizes.OpcResult)
                        {
                            List<string> mySizes = UpdateSizes(readSizes, 4);
                            //MachineRuntime.
                        }

                        #region(* I/O PLC R1 *)
                        keys = new List<string>()
                        {
                            "pcPLCR1DI",
                            "pcPLCR1DO"
                        };

                        var readR1IOResult = await ccService.Read(keys);
                        if (readR1IOResult == null)
                        {
                        }
                        else
                        {
                            UpdateOPCUADIO(readR1IOResult["pcPLCR1DI"], PLCR1InputDictionary, Properties.R1.Default.InputNumber);
                            UpdateOPCUADIO(readR1IOResult["pcPLCR1DO"], PLCR1OutputDictionary, Properties.R1.Default.OutputNumber);

                        }
                        #endregion

                        #region(* I/O PLC R2 *)
                        keys = new List<string>()
                        {
                            "pcPLCR2DI",
                            "pcPLCR2DO"
                        };

                        var readR2IOResult = await ccService.Read(keys);
                        if (readR2IOResult == null)
                        {
                        }
                        else
                        {
                            UpdateOPCUADIO(readR2IOResult["pcPLCR2DI"], PLCR2InputDictionary, Properties.R2.Default.InputNumber);
                            UpdateOPCUADIO(readR2IOResult["pcPLCR2DO"], PLCR2OutputDictionary, Properties.R2.Default.OutputNumber);

                        }
                        #endregion

                        //#region(* I/O WAGO R3 *)
                        //keys = new List<string>()
                        //{
                        //    "pcWAGOR3DI",
                        //    "pcWAGOR3DO"
                        //};

                        //var readR3IOResult = await ccService.Read(keys);
                        //if (readR3IOResult == null)
                        //{
                        //}
                        //else
                        //{
                        //    UpdateOPCUADIO(readR3IOResult["pcWAGOR3DI"], WAGOR3InputDictionary, Properties.R3.Default.InputNumber);
                        //    UpdateOPCUADIO(readR3IOResult["pcWAGOR3DO"], WAGOR3OutputDictionary, Properties.R3.Default.OutputNumber);

                        //}
                        //#endregion

                        #region (* line timeout/general alarms *)
                        keys = new List<string>()
                    {
                        "pcLineTimeoutAlarms",
                        "pcLineGeneralAlarms"
                    };
                        var readAlarmsResult = await ccService.Read(keys);
                        UpdateOPCUALineGeneralAlarms(readAlarmsResult["pcLineGeneralAlarms"]);
                        UpdateOPCUALineTimeoutAlarms(readAlarmsResult["pcLineTimeoutAlarms"]);


                        #endregion

                        #region(* all device machine status *)
                        keys = new List<string>()
                        {
                            "pcLineMachinestatus",
                            //"pcGD1Machinestatus",
                            //"pcGD2Machinestatus"

                        };
                        var readStatusResult = await ccService.Read(keys);
                        if (readStatusResult != null)
                        {
                            MachineRuntime.lineMachineStatus = short.Parse(readStatusResult["pcLineMachinestatus"].Value.ToString());
                            //MachineRuntime.gdevice1.machineStatus = short.Parse(readStatusResult["pcGD1Machinestatus"].Value.ToString());
                            //MachineRuntime.gdevice2.machineStatus = short.Parse(readStatusResult["pcGD2Machinestatus"].Value.ToString());
                        }
                        #endregion

                        keys = new List<string>()
                        {
                        "pcGD1RuntimeParam1",
                        "pcGD1RuntimeParam2",
                        //"pcGD1RuntimeParam3",
                        //"pcGD1RuntimeParam4",
                        //"pcGD2RuntimeParam1",
                        //"pcGD2RuntimeParam2",
                        //"pcGD2RuntimeParam3",
                        //"pcGD2RuntimeParam4"
                        };
                        var readGDRuntimeResult = await ccService.Read(keys);

                        if (readGDRuntimeResult == null)
                        {

                        }
                        else
                        {
                            MachineRuntime.gdevice1.gdRuntimeParam1 = (float.Parse(readGDRuntimeResult["pcGD1RuntimeParam1"].Value.ToString()))*10.0f;
                            MachineRuntime.gdevice1.gdRuntimeParam2 = (float.Parse(readGDRuntimeResult["pcGD1RuntimeParam2"].Value.ToString()))*10.0f;
                            //MachineRuntime.gdevice1.gdRuntimeParam3 = (float.Parse(readGDRuntimeResult["pcGD1RuntimeParam3"].Value.ToString()));
                            //MachineRuntime.gdevice1.gdRuntimeParam4 = (float.Parse(readGDRuntimeResult["pcGD1RuntimeParam4"].Value.ToString()));

                            //MachineRuntime.gdevice2.gdRuntimeParam1 = (float.Parse(readGDRuntimeResult["pcGD2RuntimeParam1"].Value.ToString()));
                            //MachineRuntime.gdevice2.gdRuntimeParam2 = (float.Parse(readGDRuntimeResult["pcGD2RuntimeParam2"].Value.ToString()));
                            //MachineRuntime.gdevice2.gdRuntimeParam3 = (float.Parse(readGDRuntimeResult["pcGD2RuntimeParam3"].Value.ToString()));
                            //MachineRuntime.gdevice2.gdRuntimeParam4 = (float.Parse(readGDRuntimeResult["pcGD2RuntimeParam4"].Value.ToString()));
                        }

                        keys = new List<string>()
                        {
                        "pcScannerSolePresence",
                        "pcScannerAxisInHome",
                        "pcScannerAxisAlarm",
                        };

                        var readScannerStatus = await ccService.Read(keys);

                        if (readScannerStatus != null)
                        {
                            MachineRuntime.scannerSolePresence = bool.Parse(readScannerStatus["pcScannerSolePresence"].Value.ToString());
                            MachineRuntime.scannerAxisHomingDone = bool.Parse(readScannerStatus["pcScannerAxisInHome"].Value.ToString());
                            MachineRuntime.scannerAxisInAlarm = bool.Parse(readScannerStatus["pcScannerAxisAlarm"].Value.ToString());
                        }
                        else
                        {

                        }

                        keys = new List<string>()
                        {
                        "pcReadingRequestOnG1_A1",
                        "pcReadingRequestOnG1_A4",
                        };
                        readResults = await ccService.Read(keys);

                        if (readResults == null)
                        {

                        }
                        else
                        {
                            #region(* G1_1 reading request *)
                            if (bool.Parse(readResults["pcReadingRequestOnG1_A1"].Value.ToString()))
                            {
                                MachineRuntime.rfidReadingRequestG1A1 = true;
                                string keyToRead = "pcAckOnG1ReadingRequest_A1";

                                var readResult2 = await ccService.Read(keyToRead);

                                if ((readResult2.OpcResult) && (bool.Parse(readResult2.Value.ToString()) == false))
                                {
                                    //go on read
                                    int antenna = 0;
                                    if (Properties.Settings.Default.readerType == "BALLUFF") antenna = myRSA.GetG1Balluff().IPort1;
                                    else antenna = 1;
                                    string tmpG1A1 = "";
                                    bool retG1A1 = false;
                                    try
                                    {
                                        retG1A1 = ReadRequestG1(MachineRuntime.RFIDReaderType, antenna, ref tmpG1A1);
                                    }
                                    catch (Exception ex)
                                    {
                                        retG1A1 = false;
                                    }
                                    if (retG1A1)
                                    {
                                        //rfid code
                                        MachineRuntime.rfidCodeG1A1 = tmpG1A1;
                                        //find rfid association
                                        DBRetCode ret = ManageRequestG1A1(MachineRuntime.rfidCodeG1A1);
                                        if (ret == DBRetCode.RFID_NOT_READ)
                                        {
                                            MachineRuntime.rfidReadingResultG1A1 = -1;
                                            MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
                                        }
                                        else if (ret == DBRetCode.RFID_OLD)
                                        {
                                            MachineRuntime.rfidReadingResultG1A1 = 1;
                                            //update RFID database table
                                            myRSA.GetDBL().GetRFIDTable().UpdateRFIDAssociationInfoMN(myRSA.GetDBL().GetConnection(),
                                                MachineRuntime.rfidCodeG1A1, MachineRuntime.rfidModelNameG1A1, MachineRuntime.rfidSizeG1A1);
                                            MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
                                        }
                                        else if (ret == DBRetCode.RFID_NEW)
                                        {
                                            myRSA.GetDBL().GetRFIDTable().AddNewRFIDAssociationInfo(myRSA.GetDBL().GetConnection(), MachineRuntime.rfidCodeG1A1,
                                                MachineRuntime.rfidModelNameG1A1, MachineRuntime.rfidSizeG1A1, "XX");
                                            MachineRuntime.rfidReadingResultStringG1A1 = "rfid succesfully read and associated";
                                            MachineRuntime.rfidReadingResultG1A1 = 0;
                                            ret = DBRetCode.RFID_OLD;
                                        }
                                        else if (ret == DBRetCode.RFID_ERROR)
                                        {
                                            MachineRuntime.rfidReadingResultG1A1 = 2;
                                            MachineRuntime.rfidReadingResultStringG1A1 = "rfid reading error";
                                        }

                                        if (ret == DBRetCode.RFID_OLD)
                                        {
                                            //reset ack end package
                                            string keyToSend = "pcAckOnG1ReadingRequest_A1";
                                            var sendResult = await ccService.Send(keyToSend, true);
                                            if (sendResult.OpcResult)
                                            {

                                            }
                                        }
                                    }
                                    else
                                    {
                                        //no tag
                                        MachineRuntime.rfidReadingResultG1A1 = 2;
                                        MachineRuntime.rfidReadingResultStringG1A1 = "rfid not present";
                                    }
                                }
                                else
                                {
                                    //reset ack end package
                                    string keyToSend = "pcAckOnG1ReadingRequest_A1";
                                    var sendResult = await ccService.Send(keyToSend, false);
                                    if (sendResult.OpcResult)
                                    {

                                    }
                                }
                            }
                            else
                            {
                                MachineRuntime.rfidReadingRequestG1A1 = false;
                                //reset ack end package
                                string keyToSend = "pcAckOnG1ReadingRequest_A1";
                                var sendResult = await ccService.Send(keyToSend, false);
                                if (sendResult.OpcResult)
                                {

                                }
                                MachineRuntime.rfidReadingResultG1A1 = -2;
                                MachineRuntime.rfidReadingResultStringG1A1 = "waiting for rfid tag";
                            }
                            #endregion

                            #region(* G1_4 reading request *)

                            if (bool.Parse(readResults["pcReadingRequestOnG1_A4"].Value.ToString()))
                            {
                                MachineRuntime.rfidReadingRequestG1A4 = true;
                                string keyToRead = "pcAckOnG1ReadingRequest_A4";

                                var readResult2 = await ccService.Read(keyToRead);

                                if ((readResult2.OpcResult) && (bool.Parse(readResult2.Value.ToString()) == false))
                                {
                                    //reset ack end package
                                    string keyToSend1 = "pcSizeOnUnload";
                                    var sendSResult = await ccService.Send(keyToSend1, MachineRuntime.rfidSizeG1A4);
                                    if (sendSResult.OpcResult)
                                    {
                                       
                                    }


                                    //go on read
                                    int antenna = 0;
                                    if (Properties.Settings.Default.readerType == "BALLUFF") antenna = myRSA.GetG1Balluff().IPort3;
                                    else antenna = 1;
                                    string tmpG1A4 = "";

                                    //reset ack end package
                                    string keyToSend = "pcAckOnG1ReadingRequest_A4";
                                    var sendResult = await ccService.Send(keyToSend, true);
                                    if (sendResult.OpcResult)
                                    {
                                        //MachineRuntime.lineLastCounter = MachineRuntime.lineLastCounter + 1;
                                    }


                                    //bool retG1A4 = ReadRequestG1(MachineRuntime.RFIDReaderType, antenna, ref tmpG1A4);
                                    //if (retG1A4)
                                    //{
                                    //    //rfid code
                                    //    MachineRuntime.rfidCodeG1A4 = tmpG1A4;
                                    //    //find rfid association
                                    //    DBRetCode ret = ManageRequestG1A4(MachineRuntime.rfidCodeG1A4);
                                    //    if (ret == DBRetCode.RFID_NOT_READ)
                                    //    {
                                    //        MachineRuntime.rfidReadingResultG1A4 = -1;
                                    //    }
                                    //    else if (ret == DBRetCode.RFID_OLD)
                                    //    {
                                    //        MachineRuntime.rfidReadingResultG1A4 = 1;
                                    //    }
                                    //    else if (ret == DBRetCode.RFID_NEW)
                                    //    {
                                    //        //TODO ERROR
                                    //    }
                                    //    else if (ret == DBRetCode.RFID_ERROR)
                                    //    {
                                    //        MachineRuntime.rfidReadingResultG1A4 = 2;
                                    //    }

                                    //    if (ret == DBRetCode.RFID_OLD || ret == DBRetCode.RFID_NEW)
                                    //    {
                                    //        //reset ack end package
                                    //        string keyToSend3 = "pcAckOnG1ReadingRequest_A4";
                                    //        var sendResult1 = await ccService.Send(keyToSend3, true);
                                    //        if (sendResult1.OpcResult)
                                    //        {
                                    //            MachineRuntime.lineLastCounter = MachineRuntime.lineLastCounter + 1;
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    //reset ack end package
                                    //    string keyToSend4 = "pcAckOnG1ReadingRequest_A4";
                                    //    var sendResult4 = await ccService.Send(keyToSend4, false);
                                    //    if (sendResult4.OpcResult)
                                    //    {
                                    //        MachineRuntime.lineLastCounter = MachineRuntime.lineLastCounter + 1;
                                    //    }
                                    //}
                                }
                            }
                            else
                            {
                                MachineRuntime.rfidReadingRequestG1A4 = false;
                                //reset ack end package
                                string keyToSend = "pcAckOnG1ReadingRequest_A4";
                                var sendResult = await ccService.Send(keyToSend, false);
                                if (sendResult.OpcResult)
                                {

                                }
                            }
                            #endregion
                        }




                        #region(* G1_2 first reading request *)
                        keys = new List<string>()
                        {
                        "pcReading1RequestOnG1_A2",
                        "pcReading2RequestOnG1_A2",
                        //"pcGD1RuntimeParam3",
                        //"pcGD1RuntimeParam4",
                        //"pcGD2RuntimeParam1",
                        //"pcGD2RuntimeParam2",
                        //"pcGD2RuntimeParam3",
                        //"pcGD2RuntimeParam4"
                        };
                        readResults = await ccService.Read(keys);

                        if (bool.Parse(readResults["pcReading1RequestOnG1_A2"].Value.ToString()))
                        {
                            MachineRuntime.rfidReadingRequestG1A2 = true;
                            MachineRuntime.rfidReadingResultStringG1A2 = "first plc reading request..." + "\r\n";
                            string keyToRead = "pcAck1OnG1ReadingRequest_A2";

                            var readResult2 = await ccService.Read(keyToRead);

                            if ((readResult2.OpcResult) && (bool.Parse(readResult2.Value.ToString()) == false))
                            {
                                //send automatic mode to scanner
                                tcp.Write("MODE," + 1 + ",3,0,0,0,0,0,0,0,0,");
                                MachineRuntime.ackFromScanner = false;
                                //go on read
                                int antenna = 0;
                                if (Properties.Settings.Default.readerType == "BALLUFF") antenna = myRSA.GetG1Balluff().IPort2;
                                else antenna = 1;
                                string tmpG1A2 = "";
                                bool retG1A2 = false;
                                try
                                {
                                    retG1A2 = ReadRequestG1(MachineRuntime.RFIDReaderType, antenna, ref tmpG1A2);
                                }
                                catch (Exception ex)
                                {
                                    retG1A2 = false;
                                }
                                if (retG1A2)
                                {
                                    //rfid code
                                    MachineRuntime.rfidCodeG1A2 = tmpG1A2;
                                    //find rfid association
                                    DBRetCode ret = ManageRequestG1A2(MachineRuntime.rfidCodeG1A2);
                                    if (ret == DBRetCode.RFID_NOT_READ)
                                    {
                                        MachineRuntime.rfidReadingResultG1A2 = -1;
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "rfid reading error" + "\r\n";
                                    }
                                    else if (ret == DBRetCode.RFID_OLD)
                                    {
                                        MachineRuntime.rfidReadingResultG1A2 = 1;
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "rfid succesfully read and associated" + "\r\n";
                                    }
                                    else if (ret == DBRetCode.RFID_NEW)
                                    {
                                        MachineRuntime.rfidReadingResultG1A2 = 0;
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "rfid is NEW, association needed" + "\r\n";
                                        MachineRuntime.rfidTypeFromPlcG1A2 = "XX";
                                        string keyToSend = "pcAck1OnG1ReadingRequest_A2";
                                        var sendResult = await ccService.Send(keyToSend, true);
                                        if (sendResult.OpcResult)
                                        {

                                        }
                                    }
                                    else if (ret == DBRetCode.RFID_ERROR)
                                    {
                                        MachineRuntime.rfidReadingResultG1A2 = 2;
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "rfid reading error" + "\r\n";
                                    }

                                    if (ret == DBRetCode.RFID_OLD)
                                    {
                                        //read pallet type 
                                        var readResultType = await ccService.Read("pcSoleType");
                                        short plcType = -1;
                                        string strMes1 = "";
                                        string strMes2 = "";
                                        string strMes3 = "";
                                        if (readResultType.OpcResult)
                                        {
                                            plcType = short.Parse(readResultType.Value.ToString());
                                            if (plcType == 1 || plcType == 2)
                                            {
                                                MachineRuntime.rfidTypeFromPlcG1A2 = (plcType == 2) ? "LF" : "RG";
                                                strMes1 = "model name read: " + MachineRuntime.rfidModelNameG1A2 + "\r\n";
                                                strMes2 = "pallet type: " + MachineRuntime.rfidTypeFromPlcG1A2 + "\r\n";

                                                var readResultSP = await ccService.Read("pcScannerSolePresence");
                                                if (readResultSP.OpcResult)
                                                {
                                                    bool plcPh = bool.Parse(readResultSP.Value.ToString());
                                                    if (plcPh)
                                                    {
                                                        myRSA.GetDBL().GetRFIDTable().UpdateRFIDParam2ByCode(MachineRuntime.rfidCodeG1A2, "0", myRSA.GetDBL().GetConnection());
                                                    }
                                                    else
                                                    {
                                                        myRSA.GetDBL().GetRFIDTable().UpdateRFIDParam2ByCode(MachineRuntime.rfidCodeG1A2, "1", myRSA.GetDBL().GetConnection());
                                                    }
                                                }

                                                //reset ack end package
                                                string keyToSend = "pcAck1OnG1ReadingRequest_A2";
                                                var sendResult = await ccService.Send(keyToSend, true);
                                                if (sendResult.OpcResult)
                                                {
                                                    strMes3 = "first reading completed" + "\r\n";
                                                    MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + strMes1 + strMes2 + strMes3;
                                                }
                                                else
                                                {
                                                    strMes3 = "first reading NOT completed" + "\r\n";
                                                    MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + strMes1 + strMes2 + strMes3;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //no tag
                                    MachineRuntime.rfidReadingResultG1A2 = 2;
                                    MachineRuntime.rfidReadingResultStringG1A2 = "rfid not present";
                                    //reset ack end package
                                    string keyToSend = "pcAck1OnG1ReadingRequest_A2";
                                    var sendResult = await ccService.Send(keyToSend, false);
                                    if (sendResult.OpcResult)
                                    {

                                    }
                                }
                            }
                        }
                        else
                        {
                            MachineRuntime.rfidReadingRequestG1A2 = false;
                            //reset ack end package
                            string keyToSend = "pcAck1OnG1ReadingRequest_A2";
                            var sendResult = await ccService.Send(keyToSend, false);
                            if (sendResult.OpcResult)
                            {

                            }
                            MachineRuntime.rfidReadingResultG1A2 = -2;
                            //MachineRuntime.rfidReadingResultStringG1A2 = "waiting for rfid tag";
                        }
                        #endregion

                        #region(* G1_2 second reading request *)
                        if (bool.Parse(readResults["pcReading2RequestOnG1_A2"].Value.ToString()))
                        {
                            MachineRuntime.rfidReadingRequestG1A2 = true;
                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "second plc reading request..." + "\r\n";
                            string keyToRead = "pcAck2OnG1ReadingRequest_A2";

                            var readResult2 = await ccService.Read(keyToRead);

                            if ((readResult2.OpcResult) && (bool.Parse(readResult2.Value.ToString()) == false))
                            {
                                if (MachineRuntime.ackFromScanner)
                                {                                    
                                    MachineRuntime.ackFromScanner = false;
                                    if (MachineRuntime.rfidTypeFromPlcG1A2 == MachineRuntime.rfidTypeFromScannerG1A2)
                                    {
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "sole left/right match the pallet type" + "\r\n";
                                        //reset ack end package
                                        string keyToSend = "pcAck2OnG1ReadingRequest_A2";
                                        var sendResult = await ccService.Send(keyToSend, true);
                                        if (sendResult.OpcResult)
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "second reading completed" + "\r\n";
                                        }
                                        else
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "second reading NOT completed" + "\r\n";
                                        }
                                    }
                                    else
                                    {
                                        MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "sole left/right doesn't match the pallet" + "\r\n";
                                        MachineRuntime.rfidReadingRequestG1A2 = false;
                                        //reset ack end package
                                        string keyToSend = "pcAck2OnG1ReadingRequest_A2";
                                        //todo controllare macth pc/plc
                                        var sendResult = await ccService.Send(keyToSend, true);
                                        if (sendResult.OpcResult)
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "second reading completed" + "\r\n";
                                        }
                                        else
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "second reading NOT completed" + "\r\n";
                                        }
                                    }
                                    MachineRuntime.scannerWaitingCycle = 0;
                                }
                                else
                                {
                                    MachineRuntime.scannerWaitingCycle = MachineRuntime.scannerWaitingCycle + 1;
                                    MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "scanner response not received" + "\r\n";
                                    if (MachineRuntime.scannerWaitingCycle >= 5)
                                    {
                                        myRSA.GetDBL().GetRFIDTable().UpdateRFIDParam2ByCode(MachineRuntime.rfidCodeG1A2, "1", myRSA.GetDBL().GetConnection());

                                        //reset ack end package
                                        string keyToSend = "pcAck2OnG1ReadingRequest_A2";
                                        var sendResult = await ccService.Send(keyToSend, true);
                                        if (sendResult.OpcResult)
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "pallet released/discarged" + "\r\n";
                                        }
                                        else
                                        {
                                            MachineRuntime.rfidReadingResultStringG1A2 = MachineRuntime.rfidReadingResultStringG1A2 + "pallet released/discarged" + "\r\n";
                                        }
                                        MachineRuntime.scannerWaitingCycle = 0;
                                    }
                                }
                            }

                        }
                        else
                        {
                            
                            
                            //MachineRuntime.rfidReadingResultStringG1A2 = "waiting for rfid tag";
                            MachineRuntime.rfidReadingRequestG1A2 = false;
                            //reset ack end package
                            string keyToSend = "pcAck2OnG1ReadingRequest_A2";
                            var sendResult = await ccService.Send(keyToSend, false);
                            if (sendResult.OpcResult)
                            {

                            }
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                }
            }
            catch (Exception Ex)
            {

            }
        }            

        public void UpdateOPCUALineGeneralAlarms(ClientResult cr)
        {
            int k = 1;

            if ((cr == null) || (cr.OpcResult == false))
            {
                //offline, error
                for (k = 1; k <= Properties.Settings.Default.AlarmsNumber; k++)
                {
                    AlarmsDictionary[k] = -1;
                }
            }
            else
            {
                ushort[] arrayShort = (ushort[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 1;
                k = 1;
                for (k = 1; k <= 64; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //second WORD
                alarms = ToBinary(arrayShort[2]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 17;
                for (k = 17; k <= 32; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //third WORD
                alarms = ToBinary(arrayShort[3]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 33;
                for (k = 33; k <= 48; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //fourth WORD
                alarms = ToBinary(arrayShort[4]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 49;
                for (k = 49; k <= 64; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }
            }
        }

        public void UpdateOPCUALineTimeoutAlarms(ClientResult cr)
        {
            int k = 1;

            if ((cr == null) || (cr.OpcResult == false))
            {
                //offline, error
                for (k = 1; k <= Properties.Settings.Default.AlarmsNumber; k++)
                {
                    AlarmsDictionary[k] = -1;
                }
            }
            else
            {
                ushort[] arrayShort = (ushort[])cr.Value;

                string alarms = ToBinary(arrayShort[1]);
                alarms = alarms.PadLeft(16, '0');
                int i = 0;
                int j = 65;
                k = 1;
                for (k = 65; k <= 81; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //second WORD
                alarms = ToBinary(arrayShort[2]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 82;
                for (k = 82; k <= 98; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //third WORD
                alarms = ToBinary(arrayShort[3]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 99;
                for (k = 99; k <= 115; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }

                //fourth WORD
                alarms = ToBinary(arrayShort[4]);
                alarms = alarms.PadLeft(16, '0');
                i = 0;
                j = 116;
                for (k = 116; k <= 132; k++)
                {
                    AlarmsDictionary[k] = 0;
                }
                for (i = alarms.Length - 1; i >= 0; i--)
                {
                    string exe = alarms.Substring(i, 1);

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 1))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 2))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 3))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 4))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 5))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 6))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 7))
                    {
                        AlarmsDictionary[j] = 1;
                    }


                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 8))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 9))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 10))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 11))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 12))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 13))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 14))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 15))
                    {
                        AlarmsDictionary[j] = 1;
                    }

                    if ((Int32.Parse(exe) == 1) & (i == alarms.Length - 16))
                    {
                        AlarmsDictionary[j] = 1;
                    }
                    j = j + 1;
                }                
            }
        }

        public string ToBinary(short n)
        {
            if (n < 2) return n.ToString();

            var divisor = n / 2;
            var remainder = n % 2;

            return ToBinary(divisor) + remainder;
        }

        public string GetGD1RegisterAlarmsDescription(int register, short value)
        {
            string strDescription = "";

            if (register == 1 & value == 1)
            {
                strDescription = "- scale in error";
            }

            if (register == 2 & value == 1)
            {
                strDescription = "- tank almost empty";
            }

            if (register == 3 & value == 1)
            {
                strDescription = "- tank empty";
            }

            if (register == 4 & value == 1)
            {
                strDescription = "- todo";
            }

            if (register == 5 & value == 1)
            {
                strDescription = "- todo";
            }

            if (register == 6 & value == 1)
            {
                strDescription = "- todo";
            }

            if (register == 7 & value == 1)
            {
                strDescription = "- todo";
            }

            return strDescription;
        }

        public string GetGD2RegisterAlarmsDescription(int register, short value)
        {
            string strDescription = "";

            if (register == 1 & value == 1)
            {
                strDescription = "- anomalia sonda risc. camera";
            }

            if (register == 2 & value == 1)
            {
                strDescription = "- anomalia sonda raffr. camera";
            }

            if (register == 3 & value == 1)
            {
                strDescription = "- anomalia sonda umidita";
            }

            if (register == 4 & value == 1)
            {
                strDescription = "- termico compressore";
            }

            if (register == 5 & value == 1)
            {
                strDescription = "- termico vent. ricircolo";
            }

            if (register == 6 & value == 1)
            {
                strDescription = "- max pressione gas compressore";
            }

            if (register == 7 & value == 1)
            {
                strDescription = "- emergenza";
            }

            return strDescription;
        }

        public void UpdateOPCUADIO(ClientResult cr, Dictionary<int, bool> myDictionary, int counter)
        {
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                bool[] arrayBool = (bool[])cr.Value;
                foreach (bool item in arrayBool)
                {
                    if ((i != 0) & (i <= counter))
                    {
                        myDictionary[i] = item;
                    }
                    i = i + 1;
                }
            }
        }

        public List<string> UpdateSizes(ClientResult cr, int counter)
        {
            List<string> myList = new List<string>();
            if ((cr == null) || (cr.OpcResult == false))
            {
                //todo da gestire
            }
            else
            {
                int i = 0;
                short[] arrayShort = (short[])cr.Value;
                foreach (short item in arrayShort)
                {
                    if ((i != 0) & (i <= counter))
                    {
                        myList.Add(item.ToString());
                    }
                    i = i + 1;
                }
            }

            return myList;
        }

        private bool IsFormOpened()
        { 
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                //iterate through
                if (frm.Name == "FormRFIDNewModify")
                {
                    return true;
                }
            }

            return false;
        }

public string ToBinary(int n)
        {
            if (n < 2) return n.ToString();

            var divisor = n / 2;
            var remainder = n % 2;

            return ToBinary(divisor) + remainder;
        }
    }
}
