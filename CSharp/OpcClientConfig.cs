using OpcCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLaunch
{
    public class OpcClientConfig: IOpcClientConfigurator
    {
        string stringToFillWrite = $"ns=3;s=" + "\"" + "DB202_PC" + "\"" + "." + "\"" + "Input" + "\"" + ".";
        string stringToFillRead = $"ns=3;s=" + "\"" + "DB202_PC" + "\"" + "." + "\"" + "Output" + "\"" + ".";
        public OpcClientObject ClientDataConfig { get; private set; } = new OpcClientObject();
        public IOpcClientConfigurator Config()
        {            
            #region(* OPCUA variables WRITE *)
            ClientDataConfig.Add(new OpcObjectData("pcR1Inclusion", stringToFillWrite + "inclusione_esclusione_robot_cardatura", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcR2Inclusion", stringToFillWrite + "inclusione_esclusione_robot_colla", typeof(bool)));
            
            ClientDataConfig.Add(new OpcObjectData("pcR1CleaningInclusion", stringToFillWrite + "inclusione_esclusione_soffio_post_cardatura", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcGD1Inclusion", stringToFillWrite + "inclusione_esclusione_bilancia", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1Param1", stringToFillWrite + "livello_riserva_bilancia", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1Param2", stringToFillWrite + "livello_vuoto_bilancia", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1CommandTare", stringToFillWrite + "start_tara_bilancia", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1CommandZero", stringToFillWrite + "zero_semiautomatico_bilancia", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1CommandLockHMI", stringToFillWrite + "blocco_tastiera_bilancia", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1CommandUnlockHMI", stringToFillWrite + "sblocco_tastiera_bilancia", typeof(bool)));


            ClientDataConfig.Add(new OpcObjectData("pcTimerAutoReading", stringToFillWrite + "timer_lancio_lettura_automatica", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcTimerOnEmptyPallet", stringToFillWrite + "timer_lancio_attesa_suola", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcTimerReadingTimeout", stringToFillWrite + "timer_lancio_lettura_timeout", typeof(short)));

            //ClientDataConfig.Add(new OpcObjectData("pcAutoManMode", $"ns=2;s=Tags.Eren/pc_modo_lavoro_lancio", typeof(short)));
            //ClientDataConfig.Add(new OpcObjectData("pcUnloadAutoManMode", $"ns=2;s=Tags.Eren/pc_modo_lavoro_scarico_bolla", typeof(short)));
                        
            ClientDataConfig.Add(new OpcObjectData("pcAckOnG1ReadingRequest_A1", stringToFillWrite + "lettura_terminata_lancio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcAck1OnG1ReadingRequest_A2", stringToFillWrite + "prima_lettura_terminata_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcAck2OnG1ReadingRequest_A2", stringToFillWrite + "seconda_lettura_terminata_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcAckOnG1ReadingRequest_A4", stringToFillWrite + "lettura_terminata_scarico_bolla", typeof(bool)));            
            #endregion

            #region(* READONLY *)
            ClientDataConfig.Add(new OpcObjectData("pcReadingRequestOnG1_A1", stringToFillRead + "richiesta_lettura_lancio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcReading1RequestOnG1_A2", stringToFillRead + "richiesta_prima_lettura_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcReading2RequestOnG1_A2", stringToFillRead + "richiesta_seconda_lettura_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcReadingRequestOnG1_A4", stringToFillRead + "richiesta_lettura_scarico_bolla", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcR1Ready", stringToFillRead + "robot_cardatura_ready", typeof(bool)));            
            ClientDataConfig.Add(new OpcObjectData("pcR2Ready", stringToFillRead + "robot_colla_ready", typeof(bool)));            
            #endregion

            ClientDataConfig.Add(new OpcObjectData("pcPLCRestart", stringToFillRead + "restart_plc", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcSoleType", stringToFillRead + "piede", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1RuntimeParam1", stringToFillRead + "peso_lordo_bilancia", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pcGD1RuntimeParam2", stringToFillRead + "peso_netto_bilancia", typeof(int)));



            ClientDataConfig.Add(new OpcObjectData("pcPLCLINEDI", stringToFillRead + "input_plc_linea", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcPLCLINEDO", stringToFillRead + "output_plc_linea", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcLineMachinestatus", stringToFillRead + "stato_macchina", typeof(short)));
            
            ClientDataConfig.Add(new OpcObjectData("pcPLCR1DI", stringToFillRead + "input_espansione_plc_cardatura", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcPLCR1DO", stringToFillRead + "output_espansione_plc_cardatura", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcPLCR2DI", stringToFillRead + "input_espansione_plc_colla", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcPLCR2DO", stringToFillRead + "output_espansione_plc_colla", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcWAGOR3DI", $"ns=2;s=Tags.Eren/pc_input_wago_colla", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcWAGOR3DO", $"ns=2;s=Tags.Eren/pc_output_wago_colla", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcWAGOR3DI", $"ns=2;s=Tags.Eren/pc_input_wago_colla", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcWAGOR3DO", $"ns=2;s=Tags.Eren/pc_output_wago_colla", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcLineTimeoutAlarms", stringToFillRead + "allarmi_timeout", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcLineGeneralAlarms", stringToFillRead + "allarmi_generali", typeof(short[])));

            ClientDataConfig.Add(new OpcObjectData("pcClientKeepAlive", stringToFillWrite + "comunicazione_da_opc_a_plc", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcServerKeepAlive", stringToFillRead + "comunicazione_da_plc_a_opc", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcScannerStartTraining", stringToFillWrite + "start_training_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcScannerStartCalib", stringToFillWrite + "start_calibrazione_scanner", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcScannerSolePresence", stringToFillRead + "fotocellula_presenza", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcScannerAxisAlarm", stringToFillRead + "motore_scanner_in_errore", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcScannerAxisInHome", stringToFillRead + "homing_done_motore_scanner", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcScannerAxisHome", stringToFillWrite + "homing_motore_scanner", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcScannerAxisReset", stringToFillWrite + "reset_motore_scanner", typeof(bool)));

            //ClientDataConfig.Add(new OpcObjectData("pcPallet1Size", stringToFillRead + "homing_done_motore_scanner", typeof(int)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet1Model", stringToFillRead + "homing_done_motore_scanner", typeof(string)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet2Size", stringToFillRead + "homing_done_motore_scanner", typeof(int)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet2Model", stringToFillRead + "homing_done_motore_scanner", typeof(string)));

            //ClientDataConfig.Add(new OpcObjectData("pcPallet3Size", stringToFillRead + "homing_done_motore_scanner", typeof(int)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet3Model", stringToFillRead + "homing_done_motore_scanner", typeof(string)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet4Size", stringToFillRead + "homing_done_motore_scanner", typeof(int)));
            //ClientDataConfig.Add(new OpcObjectData("pcPallet4Model", stringToFillRead + "homing_done_motore_scanner", typeof(string)));

            ClientDataConfig.Add(new OpcObjectData("pcLockUnlockPallets", stringToFillWrite + "apertura_riposizionamento_lancio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcSizeOnUnload", stringToFillWrite + "taglia_scarico_bolla", typeof(bool)));



            ClientDataConfig.Add(new OpcObjectData("pcTrackingSize", stringToFillRead + "taglia_lancio_produzione", typeof(short[])));


            return this;
        }
    }
}
