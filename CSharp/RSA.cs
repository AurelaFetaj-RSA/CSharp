using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.IO.Ports;
using RSABB;
using BIS_V_TCPIP_DLL;
using BIS_V_TCPIP_DLL.Helpers;
using KRcc;
using Opc.Ua;
using ProductionLaunch.Properties;
using Org.BouncyCastle.Asn1.X500;


namespace ProductionLaunch
{
    enum DBRetCode { RFID_NOT_READ = -1, RFID_NEW = 0, RFID_OLD = 1, RFID_ERROR = 2, ARCHIVE_NOT_PRESENT = 2, ARCHIVE_OLD = 3, ARCHIVE_NEW = 4, ARCHIVE_ERROR, RECIPE_OLD = 5, RECIPE_NEW, RECIPE_ERROR = 6, PARAMETER_NEW = 7, PARAMETER_OLD = 8, PARAMETER_ERROR = 9 };

    public enum StationID { NONE = -1, ROBOT_1 = 0, ROBOT_2 = 1, ROBOT_3 = 2, LP = 3, SCANNER = 4, FL1 = 5, FL2 = 6, OV1 = 7, OV2 = 8, OV3 = 9 };  
    //severity level message
    public enum EventTrackerID { Info, Warning, Error, Production };

    class RSA
    {
        #region(* kawasaki connection *)
        public class Kawasaki
        {
            Commu robot;
            ArrayList answer;

            public bool Connect(string ipRobot, bool log)
            {
                try
                {
                    // "as@localhost 9105"  pc-as (emulatore)
                    // "192.168.0.2"        real robot
                    robot = new Commu("TCP " + ipRobot);

                    if (log)
                        robot.startLog("log-file.log");     // specifys log file name and starts logging

                    return robot.IsConnected;
                }
                catch (Exception ex)
                {
                    
                    return false;
                }
            }

            public bool IsConnected
            {
                get
                {
                    if (robot == null) return false;
                    return robot.IsConnected;
                }
            }

            public bool Disconnect()
            {
                if (robot == null) return true;
                robot.disconnect();
                bool ret = robot.IsConnected;
                robot.Dispose();
                robot = null;
                return ret;
            }

            public string RobotRelease()
            {
                return robot.getVersion();
            }

            public bool ReadVariable(string monitorCommand, ref string value)
            {
                Console.WriteLine(DateTime.Now + "init read");
                monitorCommand = "ty " + monitorCommand;
                if (!IsConnected)
                    return false;

                if (monitorCommand == "")
                    return false;
                
                robot.cmdInquiry = null;
                answer = robot.command(monitorCommand);
                value = answer[1].ToString();                
                value = value.TrimEnd(new char[] { '\r', '\n' });
                Console.WriteLine(DateTime.Now + "end read");
                return ((int)answer[0] == 0);
            }

            public bool WriteVariable(string monitorCommand, string value)
            {
                monitorCommand = monitorCommand + " = " + value;
                if (!IsConnected)
                    return false;

                if (monitorCommand == "")
                    return false;

                robot.cmdInquiry = null;
                answer = robot.command(monitorCommand);
                value = answer[1].ToString();
                return ((int)answer[0] == 0);
            }
        }
        #endregion

        #region (*--- DB class ---*)
        public class DB
        {
            #region (*--- rfid table class ---*)
            public class RFIDTable
            {
                #region (* RFIDTable attributes *)
                protected string rfidCode; 
                protected string rfidRobotCode;
                protected string rfidSize;
                protected string rfidType;                
                protected string rfidNote;
                protected string rfidVariant;
                protected int rfidStatus;
                protected string rfidParam1;
                protected string rfidParam2;
                protected string rfidParam3;

                protected string dbRFIDTableName;
                protected string dbRFIDCodeField;
                protected string dbRFIDRobotCodeField;
                protected string dbRFIDNoteField;
                protected string dbRFIDSizeField;
                protected string dbRFIDTypeField;
                protected string dbRFIDVariantField;
                protected string dbRFIDStatusField;
                protected string dbRFIDParam1Field;
                protected string dbRFIDParam2Field;
                protected string dbRFIDParam3Field;
                #endregion

                #region (* RFIDTable constructor *)
                public RFIDTable()
                {
                    rfidCode = "0000000000";
                    rfidRobotCode = "TEST";
                    rfidNote = "--------------------";
                    rfidSize = "035";
                    rfidType = "LF";
                    rfidVariant = "--";
                    rfidStatus = 0;
                    rfidParam1 = "";
                    rfidParam2 = "";
                    dbRFIDTableName = "";
                    dbRFIDCodeField = "";
                    dbRFIDRobotCodeField = "";
                    dbRFIDNoteField = "";
                    dbRFIDSizeField = "";
                    dbRFIDTypeField = "";
                    dbRFIDVariantField = "";
                    dbRFIDStatusField = "";
                    dbRFIDParam1Field = "";
                    dbRFIDParam2Field = "";
                    dbRFIDParam3Field = "";
                }
                #endregion

                #region(* RFIDTable copy constructor *)
                public RFIDTable(string tabR, string tabF1, string tabF2, string tabF3, string tabF4, string tabF5, string tabF6, string tabF7, string tabF8, string tabF9, string tabF10)
                {
                    dbRFIDTableName = tabR;
                    dbRFIDCodeField = tabF1;
                    dbRFIDRobotCodeField = tabF2;
                    dbRFIDNoteField = tabF3;
                    dbRFIDSizeField = tabF4;
                    dbRFIDTypeField = tabF5;
                    dbRFIDVariantField = tabF6;
                    dbRFIDStatusField = tabF7;
                    dbRFIDParam1Field = tabF8;
                    dbRFIDParam2Field = tabF9;
                    dbRFIDParam3Field = tabF10;
                }

                #endregion

                #region(* RFIDTable methods *)

                #region(* get/set *)
                public string GetRFIDCode()
                {
                    return rfidCode;
                }

                public void SetRFIDCode(string code)
                {
                    rfidCode = code;
                }

                public string GetRFIDModel()
                {
                    return rfidRobotCode;
                }

                public void SetRFIDModel(string model)
                {
                    rfidRobotCode = model;
                }

                public string GetRFIDVariant()
                {
                    return rfidVariant;
                }

                public void SetRFIDVariant(string variant)
                {
                    rfidVariant = variant;
                }

                public string GetRFIDNote()
                {
                    return rfidNote;
                }

                public void SetRFIDNote(string note)
                {
                    rfidNote = note;
                }

                public string GetRFIDSize()
                {
                    return rfidSize;
                }

                public void SetRFIDSize(string size)
                {
                    rfidSize = size;
                }

                public int GetRFIDStatus()
                {
                    return rfidStatus;
                }

                public void SetRFIDStatus(int size)
                {
                    rfidStatus = size;
                }

                public string GetRFIDType()
                {
                    return rfidType;
                }

                public void SetRFIDType(string type)
                {
                    rfidType = type;
                }

                public string GetRFIDParam1()
                {
                    return rfidParam1;
                }

                public void SetRFIDParam1(string type)
                {
                    rfidParam1 = type;
                }

                public string GetRFIDParam2()
                {
                    return rfidParam2;
                }

                public void SetRFIDParam2(string type)
                {
                    rfidParam2 = type;
                }

                public string GetRFIDParam3()
                {
                    return rfidParam3;
                }

                public void SetRFIDParam3(string type)
                {
                    rfidParam3 = type;
                }
                #endregion

                /// <summary>
                /// Find rfid assoctiation info.
                /// </summary>
                /// 
                public DBRetCode FindRFIDAssociationInfo(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;
                    
                    //reset size and type info
                    SetRFIDSize("");
                    SetRFIDType("");

                  
                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfidCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            //lastModelName = dr[dbRFIDModelField].ToString();
                            rfidSize = dr[dbRFIDSizeField].ToString();
                            rfidType = dr[dbRFIDTypeField].ToString();
                            rfidNote = dr[dbRFIDNoteField].ToString();
                            rfidVariant = dr[dbRFIDVariantField].ToString();
                            rfidStatus = Convert.ToInt32(dr[dbRFIDStatusField]);
                            rfidParam1 = dr[dbRFIDParam1Field].ToString();
                            rfidParam2 = dr[dbRFIDParam2Field].ToString();
                            rfidParam3 = dr[dbRFIDParam3Field].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch  (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindRFIDAssociationInfo failed on: " + Ex.Message);
                    }
                    
                    return ret;
                }

                public DBRetCode FindRFIDAssociationInfoByRFID(string rfid, ref string robotCode, ref string size, ref string type, ref string variant, ref string note, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;

                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            //lastModelName = dr[dbRFIDModelField].ToString();
                            size = dr[dbRFIDSizeField].ToString();
                            type = dr[dbRFIDTypeField].ToString();
                            variant = dr[dbRFIDVariantField].ToString();
                            robotCode = dr[dbRFIDRobotCodeField].ToString();
                            note = dr[dbRFIDNoteField].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindRFIDAssociationInfoByRFID failed on: " + Ex.Message);
                    }

                    return ret;
                }

                public DBRetCode FindParam2ByRFID(string rfid, ref string param2, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;

                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            param2 = dr[dbRFIDParam2Field].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindParam2ByRFID failed on: " + Ex.Message);
                    }

                    return ret;
                }

                public DBRetCode FindParam3ByRFID(string rfid, ref string param3, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;

                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            param3 = dr[dbRFIDParam3Field].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindParam3ByRFID failed on: " + Ex.Message);
                    }

                    return ret;
                }

                public DBRetCode FindNoteCodeByRFID(string rfid, ref string noteCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;

                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            noteCode = dr[dbRFIDNoteField].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindNoteCodeByRFID failed on: " + Ex.Message);
                    }

                    return ret;
                }

                public DBRetCode FindSizeByRFID(string rfid, ref string size, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;

                    if (rfidCode == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            size = dr[dbRFIDSizeField].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        //todo log message
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindSizeByRFID failed on: " + Ex.Message);
                    }

                    return ret;
                }

                /// <summary>
                /// Find rfid assoctiation info.
                /// </summary>
                /// 
                public DBRetCode FindRFIDAssociationInfoReadOnly(string rfid, ref string modelName, ref string noteCode, ref string size, ref string type, ref string variant, ref int status, ref string param1, ref string param2, ref string param3, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RFID_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RFID_NEW;


                    if (rfid == "--------------------")
                    {
                        ret = DBRetCode.RFID_NOT_READ;

                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbRFIDTableName + " WHERE " + dbRFIDCodeField + " = " + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            modelName = dr[dbRFIDRobotCodeField].ToString();
                            noteCode = dr[dbRFIDNoteField].ToString();
                            size = dr[dbRFIDSizeField].ToString();
                            type = dr[dbRFIDTypeField].ToString();
                            variant = dr[dbRFIDVariantField].ToString();
                            status = Convert.ToInt32(dr[dbRFIDStatusField]);
                            param1 = dr[dbRFIDParam1Field].ToString();
                            param2 = dr[dbRFIDParam2Field].ToString();
                            param3 = dr[dbRFIDParam3Field].ToString();
                            ret = DBRetCode.RFID_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //@close db connection
                        conn.Close();
                        ret = DBRetCode.RFID_ERROR;
                        throw new System.Exception("FindRFIDAssociationInfoReadOnly failed on: " + ex.Message);
                    }

                    return ret;
                }

                /// <summary>
                /// Update rfid assoctiation info.
                /// </summary>
                ///
                public void UpdateRFIDAssociationInfo(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDSizeField + "]" + "=" + "'" + rfidSize + "'" + "," + "[" + dbRFIDTypeField + "]" + "=" + "'" + rfidType + "'" + "," + "[" + dbRFIDRobotCodeField + "]" + "=" + "'" + rfidRobotCode + "'" + "," + "[" + dbRFIDNoteField + "]" + "=" + "'" + rfidNote + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfidCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDAssociationInfo failed on: " + ex.Message);
                    }
                }

                public void UpdateRFIDAssociationInfo(OleDbConnection conn, string rfidCode, string rfidModelName, string rfidSize, string rfidFoot)
                {
                    rfidNote = "--";
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDSizeField + "]" + "=" + "'" + rfidSize + "'" + "," + "[" + dbRFIDTypeField + "]" + "=" + "'" + rfidFoot + "'" + "," + "[" + dbRFIDRobotCodeField + "]" + "=" + "'" + rfidModelName + "'" + "," + "[" + dbRFIDNoteField + "]" + "=" + "'" + rfidNote + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfidCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDAssociationInfo failed on: " + ex.Message);
                    }
                }

                public void UpdateRFIDAssociationInfoMN(OleDbConnection conn, string rfidCode, string rfidModelName, string size)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDRobotCodeField + "]" + "=" + "'" + rfidModelName + "'" + "," + "[" + dbRFIDSizeField + "]" + "=" + "'" + size + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfidCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDAssociationInfoMN failed on: " + ex.Message);
                    }
                }

                /// <summary>
                /// Update rfid assoctiation info.
                /// </summary>
                ///
                public void UpdateRFIDParam2ByCode(string rfid, string rfidPar2, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDParam2Field + "]" + "=" + "'" + rfidPar2 + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDParam2ByCode failed on: " + ex.Message);
                    }
                }

                public void UpdateRFIDParam1ByCode(string rfid, string rfidPar1, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDParam1Field + "]" + "=" + "'" + rfidPar1 + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDParam1ByCode failed on: " + ex.Message);
                    }
                }

                public void UpdateRFIDStatusByCode(string rfid, string rfidStatus, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDStatusField + "]" + "=" + "'" + rfidStatus + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDStatusByCode failed on: " + ex.Message);
                    }
                }

                public void UpdateRFIDParam3ByCode(string rfid, string rfidPar3, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "UPDATE " + dbRFIDTableName + " SET " + "[" + dbRFIDParam3Field + "]" + "=" + "'" + rfidPar3 + "'" + " WHERE " + dbRFIDCodeField + "=" + "'" + rfid + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateRFIDParam3ByCode failed on: " + ex.Message);
                    }
                }

                

                /// <summary>
                /// Add new rfid assoctiation info.
                /// </summary>
                ///

                public void AddNewRFIDAssociationInfo(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //robot 1
                        string queryString = "INSERT INTO " + dbRFIDTableName + " (" + dbRFIDCodeField + "," + dbRFIDSizeField + "," + dbRFIDTypeField + "," + dbRFIDRobotCodeField + "," + dbRFIDNoteField + "," + dbRFIDStatusField + "," + dbRFIDParam1Field + "," + dbRFIDParam2Field + "," + dbRFIDParam3Field + ")" + " VALUES (" + "'" + rfidCode + "'" + "," + "'" + rfidSize + "'" + "," + "'" + rfidType + "'" + "," + "'" + rfidRobotCode + "'" + "," + "'" + rfidNote + "'" + "," + "'" + rfidStatus + "'" + "," + "'" + rfidParam1 + "'" + "," + "'" + rfidParam2 + "'" + "," + "'" + rfidParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRFIDAssociation failed on: " + Ex.Message);

                    }
                }

                public void AddNewRFIDAssociationInfo(OleDbConnection conn, string rfid, string modelName, string size, string foot)
                {
                    rfidNote = "--";
                    rfidStatus = 0;
                    rfidParam1 = "--";
                    rfidParam2 = "--";
                    rfidParam3 = "--";
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //robot 1
                        string queryString = "INSERT INTO " + dbRFIDTableName + " (" + dbRFIDCodeField + "," + 
                            dbRFIDSizeField + "," + dbRFIDTypeField + "," + dbRFIDRobotCodeField + "," + dbRFIDNoteField
                            + "," + dbRFIDStatusField + "," + dbRFIDParam1Field + "," + dbRFIDParam2Field
                            + "," + dbRFIDParam3Field + ")" + " VALUES (" + "'" +
                            rfid + "'" + "," + "'" + size + "'" + "," + "'" + foot + "'" + "," + "'" +
                            modelName + "'" + "," + "'" + rfidNote + "'" + "," + "'" + 
                            rfidStatus + "'" + "," + "'" + rfidParam1 + "'" + "," + "'" + 
                            rfidParam2 + "'" + "," + "'" + rfidParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRFIDAssociation failed on: " + Ex.Message);

                    }
                }

                #endregion
            }
            #endregion

            #region (*--- archive table class ---*)
            public class ArchiveTable
            {
                #region (* archive table attributes *)
                protected string archiveAux0;
                protected string archiveAux1;
                protected string archiveAux2;
                protected string archiveAux3;
                protected string rfidProgramName;
                protected string dbArchiveTableRobot;
                protected string dbArchiveCodeField;
                protected string dbArchiveAux0Field;
                protected string dbArchiveAux1Field;
                protected string dbArchiveAux2Field;
                protected string dbArchiveAux3Field;
                #endregion

                #region (* archive table constructor *)
                public ArchiveTable()
                {
                    rfidProgramName = "PR0000-000-0000";
                    archiveAux0 = "0";
                    archiveAux1 = "0";
                    archiveAux2 = "0";
                    archiveAux3 = "0";
                    dbArchiveTableRobot = "";
                    dbArchiveCodeField = "";
                    dbArchiveAux0Field = "";
                    dbArchiveAux1Field = "";
                    dbArchiveAux2Field = "";
                    dbArchiveAux3Field = "";
                }
                #endregion

                #region (* archive table copy constructor *)
                public ArchiveTable(string tabR, string tabCodeField, string aux0CodeField, string aux1CodeField, string aux2CodeField, string aux3CodeField)
                {
                    dbArchiveTableRobot = tabR;
                    dbArchiveCodeField = tabCodeField;
                    dbArchiveAux0Field = aux0CodeField;
                    dbArchiveAux1Field = aux1CodeField;
                    dbArchiveAux2Field = aux2CodeField;
                    dbArchiveAux3Field = aux3CodeField;
                }
                #endregion

                #region (* methods *)
                public string GetProgramName()
                {
                    return rfidProgramName;
                }

                public void SetProgrmName(string prg)
                {
                    rfidProgramName = prg;
                }

                public DBRetCode FindArchive(OleDbConnection conn)
                {
                    if (conn.DataSource == "") 
                    {
                        return DBRetCode.ARCHIVE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.ARCHIVE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbArchiveTableRobot + " WHERE " + dbArchiveCodeField + " = " + "'" + rfidProgramName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            archiveAux0 = dr[dbArchiveAux0Field].ToString();
                            archiveAux1 = dr[dbArchiveAux1Field].ToString();
                            archiveAux2 = dr[dbArchiveAux2Field].ToString();
                            archiveAux3 = dr[dbArchiveAux3Field].ToString();
                            ret = DBRetCode.ARCHIVE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.ARCHIVE_NOT_PRESENT;
                        throw new System.Exception("FindArchive failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewArchive(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        archiveAux0 = "1";
                        archiveAux1 = "1";
                        archiveAux2 = "1";
                        archiveAux3 = "1";
                        //robot 1
                        string queryString = "INSERT INTO " + dbArchiveTableRobot + " (" + dbArchiveCodeField + "," + dbArchiveAux0Field + "," + dbArchiveAux1Field + "," + dbArchiveAux2Field + "," + dbArchiveAux3Field + ")" + " VALUES (" + "'" + rfidProgramName + "'" + "," + "'" + archiveAux0 + "'" + "," + "'" + archiveAux1 + "'" + "," + "'" + archiveAux2 + "'" + "," + "'" + archiveAux3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewArchive failed on: " + ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (*--- parameter table class ---*)
            public class ParameterTable
            {
                #region (* parameter table attributes *)
                protected string parKey;
                protected string par1;
                protected string par2;
                protected string par3;
                protected string par4;
                protected string par5;
                protected string par6;
                protected string par7;
                protected string dbParKeyField;
                protected string dbPar1Field;
                protected string dbPar2Field;
                protected string dbPar3Field;
                protected string dbPar4Field;
                protected string dbPar5Field;
                protected string dbPar6Field;
                protected string dbPar7Field;
                protected string dbParameterTable;
                #endregion

                #region (* parameter table constructor *)
                public ParameterTable()
                {
                    parKey = "PR0000-000-0000";
                    par1 = "0.0";
                    par2 = "0.0";
                    par3 = "0.0";
                    par4 = "0.0";
                    par5 = "0.0";
                    par6 = "0.0";
                    par7 = "0.0";
                    dbParKeyField = "Codice";
                    dbPar1Field = "X";
                    dbPar2Field = "Y";
                    dbPar3Field = "Z";
                    dbPar4Field = "RX";
                    dbPar5Field = "RY";
                    dbPar6Field = "RZ";
                    dbPar7Field = "LENGHT";
                    dbParameterTable = "PARAMETRI";
                }
                #endregion                

                #region (* methods *)
                public string GetProgramName()
                {
                    return parKey;
                }

                public void SetProgrmName(string prg)
                {
                    parKey = prg;
                }

                public DBRetCode FindParameter(OleDbConnection conn, string prgName)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.PARAMETER_ERROR;
                    }

                    DBRetCode ret = DBRetCode.PARAMETER_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + dbParameterTable + " WHERE " + dbParKeyField + " = " + "'" + prgName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            par1 = dr[dbPar1Field].ToString();
                            par2 = dr[dbPar2Field].ToString();
                            par3 = dr[dbPar3Field].ToString();
                            par4 = dr[dbPar4Field].ToString();
                            par5 = dr[dbPar5Field].ToString();
                            par6 = dr[dbPar6Field].ToString();
                            par7 = dr[dbPar7Field].ToString();
                            ret = DBRetCode.PARAMETER_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.PARAMETER_ERROR;
                        throw new System.Exception("FindParameter failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewParameter(OleDbConnection conn, string programName, string par1, string par2, string par3, string par4, string par5, string par6, string par7)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        int i = 0;
                        string insStr = dbParKeyField + ",";
                        string insValues = "";
                        
                        i++;
                        insStr = insStr + dbPar1Field + ",";
                        insValues = insValues + "'" + par1 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar2Field + ",";
                        insValues = insValues + "'" + par2 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar3Field + ",";
                        insValues = insValues + "'" + par3 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar4Field + ",";
                        insValues = insValues + "'" + par4 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar5Field + ",";
                        insValues = insValues + "'" + par5 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar6Field + ",";
                        insValues = insValues + "'" + par6 + "'" + ",";
                        i++;
                        insStr = insStr + dbPar7Field;
                        insValues = insValues + "'" + par7 + "'";
                        
                        insStr = dbParameterTable + " (" + insStr;
                        //open connection
                  
                        string queryString = "INSERT INTO " + insStr + ")" + " VALUES (" + "'" + programName + "'" + "," + insValues + ")";                      

                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewParameter failed on: " + ex.Message);
                    }
                }

                public void UpdateParameter(OleDbConnection conn, string programName, string par1, string par2, string par3, string par4, string par5, string par6, string par7)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = "[" + "X" + "]" + "=" + "'" + par1 + "'" + ","; ;
                        insStr = insStr + "[" + "Y" + "]" + "=" + "'" + par2 + "'" + ","; ;
                        insStr = insStr + "[" + "Z" + "]" + "=" + "'" + par3 + "'" + ","; ;
                        insStr = insStr + "[" + "RX" + "]" + "=" + "'" + par4 + "'" + ","; ;
                        insStr = insStr + "[" + "RY" + "]" + "=" + "'" + par5 + "'" + ","; ;
                        insStr = insStr + "[" + "RZ" + "]" + "=" + "'" + par6 + "'" + ","; ;
                        insStr = insStr + "[" + "LENGHT" + "]" + "=" + "'" + par7 + "'";
                        //open connection
                        conn.Close();
                        conn.Open();
                        // UPDATE Table_1 SET [text_col]='Updated text', [int_col]=2014 WHERE id=2;
                        //ROBOT 1
                        string queryString = "UPDATE " + dbParameterTable + " SET " + insStr + " WHERE " + dbParKeyField + "=" + "'" + programName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (*--- ModelTable class ---*)
            public class ModelTable
            {
                #region (* attributes *)
                protected string TableName;
                protected List<string> TableFields = new List<string>();
                protected string CodeField;
                #endregion

                #region (* copy constructor *)
                public ModelTable(
                    string mod_tablename = "", string mod_code_field = "", string mod_r1_field1 = "", string mod_r1_field2 = "", string mod_r1_field3 = "", string mod_r1_field4 = "", string mod_r1_field5 = "", string mod_r1_field6 = "",            
                    string mod_r2_field1 = "", string mod_r2_field2 = "", string mod_r2_field3 = "", string mod_r2_field4 = "", string mod_r2_field5 = "", string mod_r2_field6 = "",
                    string mod_r3_field1 = "", string mod_r3_field2 = "", string mod_r3_field3 = "", string mod_r3_field4 = "", string mod_r3_field5 = "", string mod_r3_field6 = "", 
                    string mod_r4_field1 = "", string mod_r4_field2 = "", string mod_r4_field3 = "", string mod_r4_field4 = "", string mod_r4_field5 = "", string mod_r4_field6 = "")
                {
                    TableName = mod_tablename;
                    CodeField = mod_code_field;
                    TableFields.Add(mod_r1_field1);
                    TableFields.Add(mod_r1_field2);
                    TableFields.Add(mod_r1_field3);
                    TableFields.Add(mod_r1_field4);
                    TableFields.Add(mod_r1_field5);
                    TableFields.Add(mod_r1_field6);

                    TableFields.Add(mod_r2_field1);
                    TableFields.Add(mod_r2_field2);
                    TableFields.Add(mod_r2_field3);
                    TableFields.Add(mod_r2_field4);
                    TableFields.Add(mod_r2_field5);
                    TableFields.Add(mod_r2_field6);

                    TableFields.Add(mod_r3_field1);
                    TableFields.Add(mod_r3_field2);
                    TableFields.Add(mod_r3_field3);
                    TableFields.Add(mod_r3_field4);
                    TableFields.Add(mod_r3_field5);
                    TableFields.Add(mod_r3_field6);

                    TableFields.Add(mod_r4_field1);
                    TableFields.Add(mod_r4_field2);
                    TableFields.Add(mod_r4_field3);
                    TableFields.Add(mod_r4_field4);
                    TableFields.Add(mod_r4_field5);
                    TableFields.Add(mod_r4_field6);
                }
                #endregion

                #region (* constructor *)
                public ModelTable()
                {
                }
                #endregion

                #region (* methods *)
                /// <summary>
                /// get model name flag list.
                /// </summary>
                ///
                public List<string> GetModelNameRecord(string mName, OleDbConnection conn)
                {
                    List<string> ret = new List<string>();

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + TableName + " WHERE " + CodeField + " = " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret.Add(dr[TableFields[0]].ToString());
                            ret.Add(dr[TableFields[1]].ToString());
                            ret.Add(dr[TableFields[2]].ToString());

                            ret.Add(dr[TableFields[3]].ToString());
                            ret.Add(dr[TableFields[4]].ToString());
                            ret.Add(dr[TableFields[5]].ToString());

                            ret.Add(dr[TableFields[6]].ToString());
                            ret.Add(dr[TableFields[7]].ToString());
                            ret.Add(dr[TableFields[8]].ToString());

                            ret.Add(dr[TableFields[9]].ToString());
                            ret.Add(dr[TableFields[10]].ToString());
                            ret.Add(dr[TableFields[11]].ToString());

                            ret.Add(dr[TableFields[12]].ToString());
                            ret.Add(dr[TableFields[13]].ToString());
                            ret.Add(dr[TableFields[14]].ToString());

                            ret.Add(dr[TableFields[15]].ToString());
                            ret.Add(dr[TableFields[16]].ToString());
                            ret.Add(dr[TableFields[17]].ToString());

                            ret.Add(dr[TableFields[18]].ToString());
                            ret.Add(dr[TableFields[19]].ToString());
                            ret.Add(dr[TableFields[20]].ToString());

                            ret.Add(dr[TableFields[21]].ToString());
                            ret.Add(dr[TableFields[22]].ToString());
                            ret.Add(dr[TableFields[23]].ToString());
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                public List<string> GetModelNameRecordByRobot(string mName, int robot, OleDbConnection conn)
                {
                    List<string> ret = new List<string>();

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Close();
                        conn.Open();
                        string queryString = "SELECT * FROM " + TableName + " WHERE " + CodeField + " = " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            if (robot == 1)
                            {
                                ret.Add(dr[TableFields[0]].ToString());
                                ret.Add(dr[TableFields[1]].ToString());
                                ret.Add(dr[TableFields[2]].ToString());

                                ret.Add(dr[TableFields[3]].ToString());
                                ret.Add(dr[TableFields[4]].ToString());
                                ret.Add(dr[TableFields[5]].ToString());
                            }

                            if (robot == 2)
                            {
                                ret.Add(dr[TableFields[6]].ToString());
                                ret.Add(dr[TableFields[7]].ToString());
                                ret.Add(dr[TableFields[8]].ToString());

                                ret.Add(dr[TableFields[9]].ToString());
                                ret.Add(dr[TableFields[10]].ToString());
                                ret.Add(dr[TableFields[11]].ToString());
                            }

                            if (robot == 3)
                            {
                                ret.Add(dr[TableFields[12]].ToString());
                                ret.Add(dr[TableFields[13]].ToString());
                                ret.Add(dr[TableFields[14]].ToString());

                                ret.Add(dr[TableFields[15]].ToString());
                                ret.Add(dr[TableFields[16]].ToString());
                                ret.Add(dr[TableFields[17]].ToString());
                            }

                            if (robot == 4)
                            {
                                ret.Add(dr[TableFields[18]].ToString());
                                ret.Add(dr[TableFields[19]].ToString());
                                ret.Add(dr[TableFields[20]].ToString());

                                ret.Add(dr[TableFields[21]].ToString());
                                ret.Add(dr[TableFields[22]].ToString());
                                ret.Add(dr[TableFields[23]].ToString());
                            }
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void DeleteModelNameRecordByName(string mName, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "DELETE * FROM " + TableName + " WHERE " + CodeField + " = " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteModelNameRecordByName failed on: " + ex.Message);
                    }
                }

                public List<string> GetAllModelNameRecord(OleDbConnection conn)
                {
                    List<string> ret = new List<string>();

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + TableName;
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret.Add(dr[CodeField].ToString());
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetAllModelNameRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                /// <summary>
                /// get model name flag list.
                /// </summary>
                ///
                public int GetModelNameRecordR1Param1(string mName, OleDbConnection conn)
                {
                    int ret = 0;

                    if (conn.DataSource == "")
                    {
                        return 0;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r1_param1 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = Convert.ToInt32( dr["r1_param1"].ToString());
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam0(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param0 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param0"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam0 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam1(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param1 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param1"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam1 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam2(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param2 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param2"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam2 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam3(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param3 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param3"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam3 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam4(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param4 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param4"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam4 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam5(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param5 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param5"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam5 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam6(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param6 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param6"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam6 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam7(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param7 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param7"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam7 failed on: " + ex.Message);
                    }

                    return ret;
                }

                public string GetModelNameRecordParam8(OleDbConnection conn, string mName)
                {
                    string ret = "";

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "SELECT r_param8 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = dr["r_param7"].ToString();
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecordParam8 failed on: " + ex.Message);
                    }

                    return ret;
                }
                /// <summary>
                /// get model name flag list.
                /// </summary>
                ///
                public int GetModelNameRecordR2Param1(string mName, OleDbConnection conn)
                {
                    int ret = 0;

                    if (conn.DataSource == "")
                    {
                        return 0;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //SELECT article_name, model_name, article_description FROM cust_model_name";
                        //string queryString = "SELECT article_name, model_name, article_description FROM cust_model_name WHERE article_name LIKE '" + textBoxNoteArticleName1.Text + "'";

                        string queryString = "SELECT r2_param1 FROM " + TableName + " WHERE " + CodeField + " LIKE " + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret = Convert.ToInt32(dr["r2_param1"].ToString());
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetModelNameRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                /// <summary>
                /// add new model name flag list.
                /// </summary>
                ///
                public void AddNewModelNameRecord(string mName, List<string> flags, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";
                    string insValues = "";

                    try
                    {
                        for (int i = 0; i <= flags.Count - 1; i++)
                        {
                            if (i == flags.Count - 1)
                            {
                                insStr = insStr + TableFields[i];
                                insValues = insValues + "'" + flags[1] + "'";
                            }
                            else
                            {
                                insStr = insStr + TableFields[i] + ",";
                                insValues = insValues + "'" + flags[i] + "'" + ",";
                            }
                        }

                        insStr = TableName + " (" + CodeField + "," + insStr;
                        //open connection
                        conn.Open();
                        //INSERT INTO Table_1 (text_col, int_col) VALUES ('Text', 9);
                        //robot 1
                        string queryString = "INSERT INTO " + insStr + ")" + " VALUES (" + "'" + mName + "'" + "," + insValues + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// update model name flag list.
                /// </summary>
                ///
                public void UpdateModelNameR1Param1(string mName, int param1, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = "[" + "r1_param1" + "]" + "=" + "'" + param1 + "'";
                            
                        //open connection
                        conn.Close();
                        conn.Open();
                        // UPDATE Table_1 SET [text_col]='Updated text', [int_col]=2014 WHERE id=2;
                        //ROBOT 1
                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// update model name flag list.
                /// </summary>
                ///
                public void UpdateModelNameR2Param1(string mName, int param1, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = "[" + "r2_param1" + "]" + "=" + "'" + param1 + "'";

                        //open connection
                        conn.Close();
                        conn.Open();
                        // UPDATE Table_1 SET [text_col]='Updated text', [int_col]=2014 WHERE id=2;
                        //ROBOT 1
                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// update model name flag list.
                /// </summary>
                ///
                public void UpdateModelNameRParams(OleDbConnection conn, string mName, string rParam0, string rParam1, string rParam2, string rParam3,
                    string rParam4, string rParam5, string rParam6, string rParam7, string rParam8)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = "[" + "r_param0" + "]" + "=" + "'" + rParam0 + "'" + "," +
                            "[" + "r_param1" + "]" + "=" + "'" + rParam1 + "'" + "," +
                            "[" + "r_param2" + "]" + "=" + "'" + rParam2 + "'" + "," +
                            "[" + "r_param3" + "]" + "=" + "'" + rParam3 + "'" + "," +
                            "[" + "r_param4" + "]" + "=" + "'" + rParam4 + "'" + "," +
                            "[" + "r_param5" + "]" + "=" + "'" + rParam5 + "'" + "," +
                            "[" + "r_param6" + "]" + "=" + "'" + rParam6 + "'" + "," +
                            "[" + "r_param7" + "]" + "=" + "'" + rParam7 + "'" + "," +
                            "[" + "r_param8" + "]" + "=" + "'" + rParam8 + "'";

                        //open connection
                        conn.Close();
                        conn.Open();
                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// update model name flag list.
                /// </summary>
                ///
                public void UpdateModelNameRecord(string mName, List<string> flags, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        for (int i = 0; i <= flags.Count - 1; i++)
                        {
                            if (i == flags.Count - 1)
                            {
                                insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + flags[i] + "'";
                            }
                            else
                            {
                                insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + flags[i] + "'" + ",";
                            }

                        }

                        //open connection
                        conn.Close();
                        conn.Open();
                        // UPDATE Table_1 SET [text_col]='Updated text', [int_col]=2014 WHERE id=2;
                        //ROBOT 1
                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + mName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                    
                }

               
                #endregion
            }
            #endregion

            #region (*--- BarcodeTable class ---*)
            public class BarcodeTable
            {
                #region (* attributes *)
                protected string TableName;
                protected List<string> TableFields = new List<string>();
                protected string CodeField;
                #endregion

                #region (* copy constructor *)
                public BarcodeTable(
                    string bar_tablename = "", string bar_code_field = "", string bar_robot_code_field = "", string bar_article_code_field = "", string bar_tn_field = "", string bar_dt_field = "", string bar_ln_field = "", string bar_param1 = "", string bar_param2 = "", string bar_param3 = "", string bar_param4 = "", string bar_param5 = "")
                {
                    TableName = bar_tablename;      //table name
                    CodeField = bar_code_field;     //barcode
                    TableFields.Add(bar_code_field);  //robot code
                    TableFields.Add(bar_robot_code_field);  //robot code
                    TableFields.Add(bar_article_code_field);  //article code
                    TableFields.Add(bar_tn_field);  //target numbered
                    TableFields.Add(bar_dt_field);  //done numbered
                    TableFields.Add(bar_ln_field);  //loaded numbered
                    TableFields.Add(bar_param1);        //param1
                    TableFields.Add(bar_param2);        //param2
                    TableFields.Add(bar_param3);        //param3
                    TableFields.Add(bar_param4);        //param4
                    TableFields.Add(bar_param5);        //param5
                }
                #endregion

                #region (* constructor *)
                public BarcodeTable()
                {
                }
                #endregion

                #region (* methods *)

                public bool DeleteBarcodeRecord(OleDbConnection conn, string bName)
                {
                    bool ret = false;

                    if (conn.DataSource == "")
                    {
                        ret = false;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "DELETE * FROM " + TableName + " WHERE " + CodeField + "=" + "'" + bName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteBarcodeRecord failed on: " + ex.Message);
                    }

                    return ret;
                }
                /// <summary>
                /// get "open" barcode db record.
                /// </summary>
                ///
                public List<string> GetOpenBarcodeList(OleDbConnection conn)
                {
                    List<string> ret = new List<string>();

                    if (conn.DataSource == "")
                    {
                        return ret;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + TableName + " WHERE " + "param3" + " = " + "'" + "aperto" + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret.Add(dr[TableFields[0]].ToString());                            
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetBarcodeRecord failed on: " + ex.Message);
                    }

                    return ret;
                }

                /// <summary>
                /// add new barcode to database.
                /// </summary>
                ///
                public void AddNewBarcodeRecord(string bCode, string bRobotCode, string bArticleCode, string bTN, string bDN, string bLN, string bParam1, string bParam2, string bParam3, string bParam4, string bParam5, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";
                    string insValues = "";
                    int i = 0;
                    try
                    {
                        insStr = insStr + TableFields[i] + ",";
                        //insValues = insValues + "'" + bCode + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bRobotCode + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bArticleCode + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bTN + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bDN + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bLN + "'" + ",";                        
                        i++;
                        insStr = insStr + TableFields[i] + ","; 
                        insValues = insValues + "'" + bParam1 + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bParam2 + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bParam3 + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i] + ",";
                        insValues = insValues + "'" + bParam4 + "'" + ",";
                        i++;
                        insStr = insStr + TableFields[i]; 
                        insValues = insValues + "'" + bParam5 + "'";

                        insStr = TableName + " (" + insStr;
                        //open connection
                        conn.Open();
                        string queryString = "INSERT INTO " + insStr + ")" + " VALUES (" + "'" + bCode + "'" + "," + insValues + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// update barcode to database.
                /// </summary>
                ///

                public void UpdateBarcodeRecordLoadedList(string bCode, string bLN, OleDbConnection conn)
                {
                    int i = 1;
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = insStr + "[" + TableFields[5] + "]" + "=" + "'" + bLN + "'";                        

                        //open connection
                        conn.Open();

                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                public void UpdateBarcodeRecordProducedList(string bCode, string bDN, OleDbConnection conn)
                {
                    int i = 1;
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = insStr + "[" + TableFields[4] + "]" + "=" + "'" + bDN + "'";

                        //open connection
                        conn.Open();

                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                public void UpdateBarcodeParam5(string bCode, string param5, OleDbConnection conn)
                {
                    int i = 1;
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = insStr + "[" + TableFields[10] + "]" + "=" + "'" + param5 + "'";

                        //open connection
                        conn.Open();

                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                public void UpdateBarcodeParam3(string bCode, string param3, OleDbConnection conn)
                {
                    int i = 1;
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = insStr + "[" + TableFields[8] + "]" + "=" + "'" + param3 + "'";

                        //open connection
                        conn.Open();

                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                public void UpdateBarcodeRecord(string bCode, string bRobotCode, string bArticleCode, string bTN, string bDN, string bLN, string bParam1, string bParam2, string bParam3, string bParam4, string bParam5, OleDbConnection conn)
                {
                    int i = 1;
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    string insStr = "";

                    try
                    {
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bRobotCode + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bArticleCode + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bTN + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bDN + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bLN + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bParam1 + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bParam2 + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bParam3 + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bParam4 + "'" + ",";
                        i++;
                        insStr = insStr + "[" + TableFields[i] + "]" + "=" + "'" + bParam5 + "'";
                        
                        //open connection
                        conn.Open();
                                                
                        string queryString = "UPDATE " + TableName + " SET " + insStr + " WHERE " + CodeField + "=" + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception Ex)
                    {
                        conn.Close();
                        throw new System.Exception(Ex.Message);
                    }
                }

                /// <summary>
                /// get barcode list
                /// </summary>
                ///
                public List<string> GetBarcodeRecord(string bCode, OleDbConnection conn)
                {
                    List<string> ret = new List<string>();
                    if (conn == null) return ret;

                    

                    try
                    {
                        if (conn.DataSource == "")
                        {
                            return ret;
                        }
                    }

                    catch(Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("GetBarcodeRecord failed on: " + ex.Message);
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + TableName + " WHERE " + CodeField + " = " + "'" + bCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            ret.Add(dr[TableFields[0]].ToString());
                            ret.Add(dr[TableFields[1]].ToString());
                            ret.Add(dr[TableFields[2]].ToString());
                            ret.Add(dr[TableFields[3]].ToString());
                            ret.Add(dr[TableFields[4]].ToString());
                            ret.Add(dr[TableFields[5]].ToString());
                            ret.Add(dr[TableFields[6]].ToString());
                            ret.Add(dr[TableFields[7]].ToString());
                            ret.Add(dr[TableFields[8]].ToString());
                            ret.Add(dr[TableFields[9]].ToString());
                            ret.Add(dr[TableFields[10]].ToString());
                        }

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("GetBarcodeRecord failed on: " + ex.Message);
                    }

                    return ret;
                }
                #endregion
            }
            #endregion

            #region (*--- recipe D1 table class ---*)
            public class RecipeD1Table
            {
                #region (* recipe D1 table attributes *)
                protected string recipeTableName;
                protected string recipeCode;
                protected string recipeParam1;
                protected string recipeParam2;
                protected string recipeParam3;
                protected string recipeParam4;
                protected string recipeCodeField;
                protected string recipeParam1Field;
                protected string recipeParam2Field;
                protected string recipeParam3Field;
                protected string recipeParam4Field;

                #endregion

                #region (* recipeD1 table constructor *)
                public RecipeD1Table()
                {
                    recipeCode = "test";
                    recipeParam1 = "0";
                    recipeParam2 = "0";
                    recipeParam3 = "0";
                    recipeParam4 = "0";
                }
                #endregion

                #region (* recipeD1 table copy constructor *)
                public RecipeD1Table(string tabName, string rCodeField, string rParam1Field, string rParam2Field, string rParam3Field, string rParam4Field)
                {
                    recipeTableName = tabName;
                    recipeCodeField = rCodeField;
                    recipeParam1Field = rParam1Field;
                    recipeParam2Field = rParam2Field;
                    recipeParam3Field = rParam3Field;
                    recipeParam4Field = rParam4Field;
                }
                #endregion

                #region (* methods *)
                public string GetRecipeCode()
                {
                    return recipeCode;
                }

                public void SetRecipeCode(string rCode)
                {
                    recipeCode = rCode;
                }

                public string GetRecipeParam1()
                {
                    return recipeParam1;
                }

                public void SetRecipeParam1(string rP1)
                {
                    recipeParam1 = rP1;
                }

                public string GetRecipeParam2()
                {
                    return recipeParam2;
                }

                public void SetRecipeParam2(string rP2)
                {
                    recipeParam2 = rP2;
                }

                public string GetRecipeParam3()
                {
                    return recipeParam3;
                }

                public void SetRecipeParam3(string rP3)
                {
                    recipeParam3 = rP3;
                }

                public string GetRecipeParam4()
                {
                    return recipeParam4;
                }

                public void SetRecipeParam4(string rP4)
                {
                    recipeParam4 = rP4;
                }

                public DBRetCode FindRecipeByCode(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + rCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();
                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewRecipe(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();                                               
                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + recipeParam1 + "'" + "," + "'" + recipeParam2 + "'" + "," + "'" + recipeParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipe failed on: " + ex.Message);
                    }
                }

                public void AddNewRecipeByRobotCode(string rCode, string rParam1, string rParam2, string rParam3, string rParam4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + "," + recipeParam4Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + rParam1 + "'" + "," + "'" + rParam2 + "'" + "," + "'" + rParam3 + "'" + "," + "'" + rParam4 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipeByRobotCode failed on: " + ex.Message);
                    }
                }

                public void UpdateDeviceByRecipeName(string recipeName, string param1, string param2, string param3, string param4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + recipeTableName + " SET " + "[" + recipeParam1Field + "]" + "=" + "'" + param1 + "'" + "," + "[" + recipeParam2Field + "]" + "=" + "'" + param2 + "'" + "," + "[" + recipeParam3Field + "]" + "=" + "'" + param3 + "'" + "," + "[" + recipeParam4Field + "]" + "=" + "'" + param4 + "'" + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateDevice1ByRecipeName failed on: " + ex.Message);
                    }
                }

                public void DeleteDeviceByRecipeName(string recipeName, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "DELETE FROM " + recipeTableName + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";

                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteDeviceByRecipeName failed on: " + ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (*--- recipe D2 table class ---*)
            public class RecipeD2Table
            {
                #region (* recipe D2 table attributes *)
                protected string recipeTableName;
                protected string recipeCode;
                protected string recipeParam1;
                protected string recipeParam2;
                protected string recipeParam3;
                protected string recipeParam4;
                protected string recipeCodeField;
                protected string recipeParam1Field;
                protected string recipeParam2Field;
                protected string recipeParam3Field;
                protected string recipeParam4Field;
                #endregion

                #region (* recipeD2 table constructor *)
                public RecipeD2Table()
                {
                    recipeCode = "test";
                    recipeParam1 = "0";
                    recipeParam2 = "0";
                    recipeParam3 = "0";
                    recipeParam4 = "0";
                }
                #endregion

                #region (* recipeD2 table copy constructor *)
                public RecipeD2Table(string tabName, string rCodeField, string rParam1Field, string rParam2Field, string rParam3Field, string rParam4Field)
                {
                    recipeTableName = tabName;
                    recipeCodeField = rCodeField;
                    recipeParam1Field = rParam1Field;
                    recipeParam2Field = rParam2Field;
                    recipeParam3Field = rParam3Field;
                    recipeParam4Field = rParam4Field;
                }
                #endregion

                #region (* methods *)
                public string GetRecipeCode()
                {
                    return recipeCode;
                }

                public void SetRecipeCode(string rCode)
                {
                    recipeCode = rCode;
                }

                public string GetRecipeParam1()
                {
                    return recipeParam1;
                }

                public void SetRecipeParam1(string rP1)
                {
                    recipeParam1 = rP1;
                }

                public string GetRecipeParam2()
                {
                    return recipeParam2;
                }

                public void SetRecipeParam2(string rP2)
                {
                    recipeParam2 = rP2;
                }

                public string GetRecipeParam3()
                {
                    return recipeParam3;
                }

                public void SetRecipeParam3(string rP3)
                {
                    recipeParam3 = rP3;
                }

                public string GetRecipeParam4()
                {
                    return recipeParam4;
                }

                public void SetRecipeParam4(string rP4)
                {
                    recipeParam4 = rP4;
                }

                public DBRetCode FindRecipeByCode(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + rCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();
                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewRecipe(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + recipeParam1 + "'" + "," + "'" + recipeParam2 + "'" + "," + "'" + recipeParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipe failed on: " + ex.Message);
                    }
                }

                public void AddNewRecipeByRobotCode(string rCode, string rParam1, string rParam2, string rParam3, string rParam4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + "," + recipeParam4Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + rParam1 + "'" + "," + "'" + rParam2 + "'" + "," + "'" + rParam3 + "'" + "," + "'" + rParam4 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipeByRobotCode failed on: " + ex.Message);
                    }
                }

                public void UpdateDeviceByRecipeName(string recipeName, string param1, string param2, string param3, string param4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + recipeTableName + " SET " + "[" + recipeParam1Field + "]" + "=" + "'" + param1 + "'" + "," + "[" + recipeParam2Field + "]" + "=" + "'" + param2 + "'" + "," + "[" + recipeParam3Field + "]" + "=" + "'" + param3 + "'" + "," + "[" + recipeParam4Field + "]" + "=" + "'" + param4 + "'" + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateDeviceByRecipeName failed on: " + ex.Message);
                    }
                }

                public void DeleteDeviceByRecipeName(string recipeName, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "DELETE FROM " + recipeTableName + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";

                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteDeviceByRecipeName failed on: " + ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (*--- recipe D3 table class ---*)
            public class RecipeD3Table
            {
                #region (* recipe D3 table attributes *)
                protected string recipeTableName;
                protected string recipeCode;
                protected string recipeParam1;
                protected string recipeParam2;
                protected string recipeParam3;
                protected string recipeParam4;
                protected string recipeCodeField;
                protected string recipeParam1Field;
                protected string recipeParam2Field;
                protected string recipeParam3Field;
                protected string recipeParam4Field;

                #endregion

                #region (* recipeD3 table constructor *)
                public RecipeD3Table()
                {
                    recipeCode = "test";
                    recipeParam1 = "0";
                    recipeParam2 = "0";
                    recipeParam3 = "0";
                    recipeParam4 = "0";
                }
                #endregion

                #region (* recipeD3 table copy constructor *)
                public RecipeD3Table(string tabName, string rCodeField, string rParam1Field, string rParam2Field, string rParam3Field, string rParam4Field)
                {
                    recipeTableName = tabName;
                    recipeCodeField = rCodeField;
                    recipeParam1Field = rParam1Field;
                    recipeParam2Field = rParam2Field;
                    recipeParam3Field = rParam3Field;
                    recipeParam4Field = rParam4Field;
                }
                #endregion

                #region (* methods *)
                public string GetRecipeCode()
                {
                    return recipeCode;
                }

                public void SetRecipeCode(string rCode)
                {
                    recipeCode = rCode;
                }

                public string GetRecipeParam1()
                {
                    return recipeParam1;
                }

                public void SetRecipeParam1(string rP1)
                {
                    recipeParam1 = rP1;
                }

                public string GetRecipeParam2()
                {
                    return recipeParam2;
                }

                public void SetRecipeParam2(string rP2)
                {
                    recipeParam2 = rP2;
                }

                public string GetRecipeParam3()
                {
                    return recipeParam3;
                }

                public void SetRecipeParam3(string rP3)
                {
                    recipeParam3 = rP3;
                }

                public string GetRecipeParam4()
                {
                    return recipeParam4;
                }

                public void SetRecipeParam4(string rP4)
                {
                    recipeParam4 = rP4;
                }

                public DBRetCode FindRecipeByCode(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + rCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();
                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }

                public DBRetCode FindRecipeByCode(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + recipeCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();

                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewRecipe(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + recipeParam1 + "'" + "," + "'" + recipeParam2 + "'" + "," + "'" + recipeParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipe failed on: " + ex.Message);
                    }
                }

                public void AddNewRecipeByRobotCode(string rCode, string rParam1, string rParam2, string rParam3, string rParam4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + "," + recipeParam4Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + rParam1 + "'" + "," + "'" + rParam2 + "'" + "," + "'" + rParam3 + "'" + "," + "'" + rParam4 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipeByRobotCode failed on: " + ex.Message);
                    }
                }

                public void UpdateDeviceByRecipeName(string recipeName, string param1, string param2, string param3, string param4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + recipeTableName + " SET " + "[" + recipeParam1Field + "]" + "=" + "'" + param1 + "'" + "," + "[" + recipeParam2Field + "]" + "=" + "'" + param2 + "'" + "," + "[" + recipeParam3Field + "]" + "=" + "'" + param3 + "'" + "," + "[" + recipeParam4Field + "]" + "=" + "'" + param4 + "'" + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateDeviceByRecipeName failed on: " + ex.Message);
                    }
                }

                public void DeleteDeviceByRecipeName(string recipeName, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "DELETE FROM " + recipeTableName + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";

                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteDeviceByRecipeName failed on: " + ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (*--- recipe D4 table class ---*)
            public class RecipeD4Table
            {
                #region (* recipe D4 table attributes *)
                protected string recipeTableName;
                protected string recipeCode;
                protected string recipeParam1;
                protected string recipeParam2;
                protected string recipeParam3;
                protected string recipeParam4;
                protected string recipeCodeField;
                protected string recipeParam1Field;
                protected string recipeParam2Field;
                protected string recipeParam3Field;
                protected string recipeParam4Field;

                #endregion

                #region (* recipeD4 table constructor *)
                public RecipeD4Table()
                {
                    recipeCode = "test";
                    recipeParam1 = "0";
                    recipeParam2 = "0";
                    recipeParam3 = "0";
                    recipeParam4 = "0";
                }
                #endregion

                #region (* recipeD4 table copy constructor *)
                public RecipeD4Table(string tabName, string rCodeField, string rParam1Field, string rParam2Field, string rParam3Field, string rParam4Field)
                {
                    recipeTableName = tabName;
                    recipeCodeField = rCodeField;
                    recipeParam1Field = rParam1Field;
                    recipeParam2Field = rParam2Field;
                    recipeParam3Field = rParam3Field;
                    recipeParam4Field = rParam4Field;
                }
                #endregion

                #region (* methods *)
                public string GetRecipeCode()
                {
                    return recipeCode;
                }

                public void SetRecipeCode(string rCode)
                {
                    recipeCode = rCode;
                }

                public string GetRecipeParam1()
                {
                    return recipeParam1;
                }

                public void SetRecipeParam1(string rP1)
                {
                    recipeParam1 = rP1;
                }

                public string GetRecipeParam2()
                {
                    return recipeParam2;
                }

                public void SetRecipeParam2(string rP2)
                {
                    recipeParam2 = rP2;
                }

                public string GetRecipeParam3()
                {
                    return recipeParam3;
                }

                public void SetRecipeParam3(string rP3)
                {
                    recipeParam3 = rP3;
                }

                public string GetRecipeParam4()
                {
                    return recipeParam4;
                }

                public void SetRecipeParam4(string rP4)
                {
                    recipeParam4 = rP4;
                }

                public DBRetCode FindRecipeByCode(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + rCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();
                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }


                public DBRetCode FindRecipeByCode(OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return DBRetCode.RECIPE_ERROR;
                    }

                    DBRetCode ret = DBRetCode.RECIPE_NEW;

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "SELECT * FROM " + recipeTableName + " WHERE " + recipeCodeField + " = " + "'" + recipeCode + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        OleDbDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            recipeParam1 = dr[recipeParam1Field].ToString();
                            recipeParam2 = dr[recipeParam2Field].ToString();
                            recipeParam3 = dr[recipeParam3Field].ToString();
                            recipeParam4 = dr[recipeParam4Field].ToString();

                            ret = DBRetCode.RECIPE_OLD;
                        }
                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ret = DBRetCode.RECIPE_ERROR;
                        throw new System.Exception("FindRecipeByCode failed on: " + ex.Message);
                    }

                    return ret;
                }

                public void AddNewRecipe(string rCode, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + recipeParam1 + "'" + "," + "'" + recipeParam2 + "'" + "," + "'" + recipeParam3 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipe failed on: " + ex.Message);
                    }
                }

                public void AddNewRecipeByRobotCode(string rCode, string rParam1, string rParam2, string rParam3, string rParam4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();
                        string queryString = "INSERT INTO " + recipeTableName + " (" + recipeCodeField + "," + recipeParam1Field + "," + recipeParam2Field + "," + recipeParam3Field + "," + recipeParam4Field + ")" + " VALUES (" + "'" + rCode + "'" + "," + "'" + rParam1 + "'" + "," + "'" + rParam2 + "'" + "," + "'" + rParam3 + "'" + "," + "'" + rParam4 + "'" + ")";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new System.Exception("AddNewRecipeByRobotCode failed on: " + ex.Message);
                    }
                }

                public void UpdateDeviceByRecipeName(string recipeName, string param1, string param2, string param3, string param4, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "UPDATE " + recipeTableName + " SET " + "[" + recipeParam1Field + "]" + "=" + "'" + param1 + "'" + "," + "[" + recipeParam2Field + "]" + "=" + "'" + param2 + "'" + "," + "[" + recipeParam3Field + "]" + "=" + "'" + param3 + "'" + "," + "[" + recipeParam4Field + "]" + "=" + "'" + param4 + "'" + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("UpdateDeviceByRecipeName failed on: " + ex.Message);
                    }
                }

                public void DeleteDeviceByRecipeName(string recipeName, OleDbConnection conn)
                {
                    if (conn.DataSource == "")
                    {
                        return;
                    }

                    try
                    {
                        //open connection
                        conn.Open();

                        //ROBOT 1
                        string queryString = "DELETE FROM " + recipeTableName + " WHERE " + recipeCodeField + "=" + "'" + recipeName + "'";

                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = queryString;
                        command.Connection = conn;
                        command.ExecuteNonQuery();

                        //close connection
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //close connection
                        conn.Close();
                        throw new System.Exception("DeleteDeviceByRecipeName failed on: " + ex.Message);
                    }
                }
                #endregion
            }
            #endregion

            #region (* DB attributes *)

            //database connection object
            OleDbConnection dbConnection;
            
            ModelTable modelTable = new ModelTable();
            BarcodeTable barcodeTable = new BarcodeTable();
            RFIDTable rfidTable = new RFIDTable();
            ArchiveTable archiveTable = new ArchiveTable();
            ParameterTable parameterTable = new ParameterTable();
            RecipeD1Table recipeD1Table = new RecipeD1Table();
            RecipeD2Table recipeD2Table = new RecipeD2Table();
            RecipeD3Table recipeD3Table = new RecipeD3Table();
            RecipeD4Table recipeD4Table = new RecipeD4Table();
            #endregion

            #region (* DB copy constructor *)

            public DB(ModelTable mTable, RFIDTable rTable, BarcodeTable bTable, RecipeD1Table rD1Table, RecipeD2Table rD2Table, RecipeD3Table rD3Table, RecipeD4Table rD4Table, OleDbConnection oleConn)
            {
                dbConnection = oleConn;
                modelTable = mTable;
                barcodeTable = bTable;
                rfidTable = rTable;
                recipeD1Table = rD1Table;
                recipeD2Table = rD2Table;
                recipeD3Table = rD3Table;
                recipeD4Table = rD4Table;
            }

            public DB(ArchiveTable aTable, OleDbConnection oleConn)
            {
                dbConnection = oleConn;
                archiveTable = aTable;
            }

            public DB(ParameterTable aTable, OleDbConnection oleConn)
            {
                dbConnection = oleConn;
                parameterTable = aTable;
            }
            #endregion

            #region (* DB constructor *)
            public DB()
            {
            }

            public DB(ModelTable mTable, RFIDTable rTable, ArchiveTable aTable, BarcodeTable bTable, OleDbConnection oleConn)
            {
                dbConnection = oleConn;
                modelTable = mTable;
                rfidTable = rTable;
                archiveTable = aTable;
                barcodeTable = bTable;
            }

            public OleDbConnection GetConnection()
            {
                return dbConnection;
            }
            #endregion

            #region (* DB methods *)
            public ModelTable GetModelTable()
            {
                return modelTable;
            }

            public RFIDTable GetRFIDTable()
            {
                return rfidTable;
            }

            public ParameterTable GetParameterTable()
            {
                return parameterTable;
            }

            public ArchiveTable GetArchiveTable()
            {
                return archiveTable;
            }

            public BarcodeTable GetBarcodeTable()
            {
                return barcodeTable;
            }

            public RecipeD1Table GetRecipeD1Table()
            {
                return recipeD1Table;
            }

            public RecipeD2Table GetRecipeD2Table()
            {
                return recipeD2Table;
            }

            public RecipeD3Table GetRecipeD3Table()
            {
                return recipeD3Table;
            }

            public RecipeD4Table GetRecipeD4Table()
            {
                return recipeD4Table;
            }
            #endregion
        }
        #endregion

        #region (* PLServer class *)
        public class PLServer
        {
            [StructLayout(LayoutKind.Sequential)]
            public class CLIENT_Info
            {
                //client object
                internal TcpClient ClientObject;
                //client automatic status
                internal bool ClientAutoStatus;
                //client localization string
                internal string ClientLocalizationInfo;
                //message sent
                internal string ClientMsgSent;
                //ack received
                internal bool ClientAckReceived;                
            };

            #region (* attributes *)
            //sockets number
            Thread thdListener;
            Thread thdListener2;
            List<CLIENT_Info> clList = new List<CLIENT_Info>();
            //scanner inclusion status
            bool scannerInclusionStatus = false;
            #endregion

            #region (* constructor *)
            public PLServer()
            {
                
            }
            #endregion

            #region (* methods *)


            /// <summary>
            /// server in listening mode.
            /// </summary>
            /// 
            public void ServerStart()
            {
                thdListener = new Thread(new ThreadStart(listenerThread));
                thdListener.Start();
            }

            public void ServerStart2()
            {
                thdListener2 = new Thread(new ThreadStart(listenerThread2));
                thdListener2.Start();
            }

            /// <summary>
            /// server stop listening.
            /// </summary>
            /// 
            public void ServerStop()
            {
                try
                {
                    
                    thdListener.Abort();
                    thdListener = null;
                }
                catch(Exception ex)
                {
                    int i = 0;
                }
                
            }

            public void ServerStop2()
            {
                try
                {

                    thdListener2.Abort();
                    thdListener2 = null;
                }
                catch (Exception ex)
                {

                }
            }

            public void listenerThread()
            {
                while (true)
                {
                    int requestCount = 0;

                    try
                    {
                        TcpListener serverSocket = new TcpListener(IPAddress.Any, 9999);
                        
                        serverSocket.Start();
                        

                        while ((true))
                        {
                            try
                            {
                                CLIENT_Info clInfo = new CLIENT_Info();
                                TcpClient client = serverSocket.AcceptTcpClient();
                                
                                clInfo.ClientObject = client;
                                
                                clList.Add(clInfo);
                                requestCount++;
                                Console.WriteLine("Connection accepted from " + client.Client.RemoteEndPoint.ToString());
                                Console.WriteLine("----------");
                                Console.WriteLine("Client list");
                                for (int i = 0; i <= clList.Count - 1; i++)
                                {
                                    Console.WriteLine("Client " + i);
                                    Console.WriteLine(clList[i].ClientObject.Client.RemoteEndPoint.ToString());
                                }

                                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnection));
                                clientThread.IsBackground = true;
                                clientThread.Start(client);
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                            Thread.Sleep(10);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Thread.Sleep(10);
                }
            }

            public void listenerThread2()
            {
                while (true)
                {
                    int requestCount = 0;

                    try
                    {
                        TcpListener serverSocket = new TcpListener(IPAddress.Any, 9998);

                        serverSocket.Start();


                        while ((true))
                        {
                            try
                            {
                                CLIENT_Info clInfo = new CLIENT_Info();
                                TcpClient client = serverSocket.AcceptTcpClient();
                                clInfo.ClientObject = client;
                                clList.Add(clInfo);
                                requestCount++;
                                Console.WriteLine("Connection accepted from " + client.Client.RemoteEndPoint.ToString());
                                Console.WriteLine("----------");
                                Console.WriteLine("Client list");
                                for (int i = 0; i <= clList.Count - 1; i++)
                                {
                                    Console.WriteLine("Client " + i);
                                    Console.WriteLine(clList[i].ClientObject.Client.RemoteEndPoint.ToString());
                                }

                                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnection2));
                                clientThread.IsBackground = true;
                                clientThread.Start(client);
                            }
                            catch (Exception ex)
                            {
                                break;
                    
                            }
                            Thread.Sleep(10);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Thread.Sleep(10);
                }
            }

            public void HandleClientConnection(object client)
            {
                TcpClient tcpClient = (TcpClient)client;

                byte[] messageBytes = new byte[1024];
                int bytesRead = 0;
                int messageCounter = 0;

                string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
                while (true)
                {
                    try
                    {
                        if (tcpClient.Connected == true)
                        {
                            NetworkStream clientStream = tcpClient.GetStream();
                            bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo add to log
                        //MessageBox.Show(ex.Message);

                        RemoveClientFromList(tcpClient);
                        break;
                    }

                    if (bytesRead == 0)
                    {

                        //MessageBox.Show(clientEndPoint + " Client has disconnected");
                        //clList.Remove();
                        Console.WriteLine(clientEndPoint + " Client has disconnected");
                        RemoveClientFromList(tcpClient);
                        Console.WriteLine("----------");
                        Console.WriteLine("Client list");
                        for (int i = 0; i <= clList.Count - 1; i++)
                        {
                            Console.WriteLine("Client " + i);
                            Console.WriteLine(clList[i].ClientObject.Client.RemoteEndPoint.ToString());
                        }
                        break;
                    }

                    else
                    {
                        messageCounter++;
                        string message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);

                        //MessageBox.Show((clientEndPoint.Split(':'))[0] + " " + message);
                        ParseClientMessage(tcpClient, message);
                    }
                    //ControlShowConnectionR1()
                    
                }
            }

            public void HandleClientConnection2(object client)
            {
                TcpClient tcpClient = (TcpClient)client;

                byte[] messageBytes = new byte[1024];
                int bytesRead = 0;
                int messageCounter = 0;

                string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
                while (true)
                {
                    try
                    {
                        if (tcpClient.Connected == true)
                        {
                            NetworkStream clientStream = tcpClient.GetStream();
                            bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo add to log
                        //MessageBox.Show(ex.Message);

                        RemoveClientFromList(tcpClient);
                        break;
                    }

                    if (bytesRead == 0)
                    {

                        //MessageBox.Show(clientEndPoint + " Client has disconnected");
                        //clList.Remove();
                        Console.WriteLine(clientEndPoint + " Client has disconnected");
                        RemoveClientFromList(tcpClient);
                        Console.WriteLine("----------");
                        Console.WriteLine("Client list");
                        for (int i = 0; i <= clList.Count - 1; i++)
                        {
                            Console.WriteLine("Client " + i);
                            Console.WriteLine(clList[i].ClientObject.Client.RemoteEndPoint.ToString());
                        }
                        break;
                    }

                    else
                    {
                        messageCounter++;
                        string message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);

                        ParseClientMessage(tcpClient, message);
                    }                    
                }
            }



            private void ParseClientMessage(TcpClient client, string msg)
            {
                IPAddress clAddress = (((IPEndPoint)(client.Client.RemoteEndPoint)).Address);
                int port = (((IPEndPoint)(client.Client.RemoteEndPoint)).Port);
                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(clAddress) & ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Port.Equals(port));

                if (cl.ClientObject != null)
                {
                    foreach (CLIENT_Info obj in clList)
                    {
                        if (((IPEndPoint)(obj.ClientObject.Client.RemoteEndPoint)).Address.Equals(clAddress))
                        {
                            if (msg.Equals("AUTO_ON"))
                            {
                                obj.ClientAutoStatus = true;
                            }
                            else if (msg.Equals("PROGRAM_RECEIVED"))
                            {
                                obj.ClientAckReceived = true;
                            }
                            else if (msg.Substring(0,4).Equals("SCAN"))
                            {
                                obj.ClientLocalizationInfo = msg;                                
                            }
                            else
                            {
                            }
                            break;
                        }
                    }
                }
                else
                {
                    
                }
            }

            private void RemoveClientFromList(TcpClient client)
            {
                IPAddress clAddress = (((IPEndPoint)(client.Client.RemoteEndPoint)).Address);
                int port = ((IPEndPoint)(client.Client.RemoteEndPoint)).Port;
                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(clAddress) & ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Port.Equals(port));

                if (cl.ClientObject != null)
                {
                    clList.Remove(cl);
                }
            }

            public void SetScannerInclusionStatus(bool scannerStatus)
            {
                scannerInclusionStatus = scannerStatus;
            }

            public void SetClientLocalizationInfo(IPAddress ipAddress, int port, string locString)
            {
                string ret = "";

                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress));

                if (cl != null)
                {
                    cl.ClientLocalizationInfo = locString;
                }
                else
                {
                }
            }

            public string GetClientLocalizationInfo(IPAddress ipAddress, int port)
            {
                string ret = "";

                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress));                

                if (cl != null)
                {
                    ret = cl.ClientLocalizationInfo;
                }
                else
                {
                    ret = "";
                }

                return ret;
            }

            public bool GetClientConnectionStatus(IPAddress ipAddress, int port)
            {
                bool ret = false;

                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress) );
                //CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress) & ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Port.Equals(port));

                if (cl != null)
                {
                    ret = cl.ClientObject.Connected;                    
                }
                else
                {
                    ret = false;
                }
                
                return ret;
            }

            public bool GetClientAutomaticStatus(IPAddress ipAddress, int port)
            {
                bool ret = false;

                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress) );

                try
                {
                    if (cl != null)
                    {
                        ret = cl.ClientAutoStatus;
                        byte[] bytes = Encoding.ASCII.GetBytes("GETAUTO;");
                        cl.ClientObject.Client.Send(bytes);
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch
                {

                }

                return ret;
            }

            public bool GetClientAckReceived(IPAddress ipAddress, int port)
            {
                bool ret = false;

                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress) );

                try
                {
                    if (cl != null)
                    {
                        ret = cl.ClientAckReceived;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch
                {

                }

                return ret;

            }

            

            public void SendCommandToRobot(IPAddress ipAddress, string command, int port)
            {
                CLIENT_Info cl = clList.Find(item => ((IPEndPoint)item.ClientObject.Client.RemoteEndPoint).Address.Equals(ipAddress));

                if (cl != null)
                {
                    //if (cl.ClientMsgSent == command)
                    //{
                    //    //@message already sent
                        
                    //}
                    //else
                    //{
                        byte[] bytes = Encoding.ASCII.GetBytes(command);
                        cl.ClientObject.Client.Send(bytes);
                        cl.ClientMsgSent = command;
                    
                    //}
                }
                else
                {
                }
            }



           
            #endregion
        }
        #endregion

        #region(* client *)
        // State object for receiving data from remote device.
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public class AsynchronousClient
        {
            // The port number for the remote device.
            private const int port = 11000;

            // ManualResetEvent instances signal completion.
            private static ManualResetEvent connectDone =
                new ManualResetEvent(false);
            private static ManualResetEvent sendDone =
                new ManualResetEvent(false);
            private static ManualResetEvent receiveDone =
                new ManualResetEvent(false);

            // The response from the remote device.
            private static String response = String.Empty;

            public static void StartClient()
            {
                // Connect to a remote device.
                try
                {
                    // Establish the remote endpoint for the socket.
                    // The name of the 
                    // remote device is "host.contoso.com".
                    IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
                    IPAddress ipAddress = ipHostInfo.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                    // Create a TCP/IP socket.
                    Socket client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the remote endpoint.
                    client.BeginConnect(remoteEP,
                        new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();

                    // Send test data to the remote device.
                    Send(client, "This is a test<EOF>");
                    sendDone.WaitOne();

                    // Receive the response from the remote device.
                    Receive(client);
                    receiveDone.WaitOne();

                    // Write the response to the console.
                    Console.WriteLine("Response received : {0}", response);

                    // Release the socket.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void ConnectCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete the connection.
                    client.EndConnect(ar);

                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());

                    // Signal that the connection has been made.
                    connectDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void Receive(Socket client)
            {
                try
                {
                    // Create the state object.
                    StateObject state = new StateObject();
                    state.workSocket = client;

                    // Begin receiving the data from the remote device.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the state object and the client socket 
                    // from the asynchronous state object.
                    StateObject state = (StateObject)ar.AsyncState;
                    Socket client = state.workSocket;

                    // Read data from the remote device.
                    int bytesRead = client.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        // There might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                        // Get the rest of the data.
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // All the data has arrived; put it in response.
                        if (state.sb.Length > 1)
                        {
                            response = state.sb.ToString();
                        }
                        // Signal that all bytes have been received.
                        receiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void Send(Socket client, String data)
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), client);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = client.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                    // Signal that all bytes have been sent.
                    sendDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            //public static int Main(String[] args)
            //{
            //    StartClient();
            //    return 0;
            //}
        }
        #endregion

        #region (*FL serial communication class *)
        /// <summary> CommPort class creates a singleton instance
        /// of SerialPort (System.IO.Ports) </summary>
        /// <remarks> When ready, you open the port.
        ///   <code>
        ///   CommPort com = CommPort.Instance;
        ///   com.StatusChanged += OnStatusChanged;
        ///   com.DataReceived += OnDataReceived;
        ///   com.Open();
        ///   </code>
        ///   Notice that delegates are used to handle status and data events.
        ///   When settings are changed, you close and reopen the port.
        ///   <code>
        ///   CommPort com = CommPort.Instance;
        ///   com.Close();
        ///   com.PortName = "COM4";
        ///   com.Open();
        ///   </code>
        /// </remarks>
        //public sealed class CommPort
        public class CommPort
        {
            SerialPort _serialPort;
            Thread _readThread;
            volatile bool _keepReading;

            //begin Singleton pattern
            static readonly CommPort instance = new CommPort();

            string portName = "COM1";
            int baudRate = 9600;
            Parity parity = new Parity();
            int dataBit = 8;
            StopBits stopBit = new StopBits();
            Handshake hndShake = new Handshake();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            //static CommPort()
            //{
            //}

            public CommPort()
            {
                _serialPort = new SerialPort();
                _readThread = null;
                _keepReading = false;
            }

            //public static CommPort Instance
            //{
            //    get
            //    {
            //        return instance;
            //    }
            //}
            //end Singleton pattern

            //begin Observer pattern
            public delegate void EventHandler(string param);
            public EventHandler StatusChanged;
            public EventHandler DataReceived;
            //end Observer pattern

            private void StartReading()
            {
                if (!_keepReading)
                {
                    _keepReading = true;
                    _readThread = new Thread(ReadPort);
                    _readThread.Start();
                }
            }

            private void StopReading()
            {
                if (_keepReading)
                {
                    _keepReading = false;
                    _readThread.Join();	//block until exits
                    _readThread = null;
                }
            }

            /// <summary> Get the data and pass it on. </summary>
            private void ReadPort()
            {
                while (_keepReading)
                {
                    if (_serialPort.IsOpen)
                    {
                        byte[] readBuffer = new byte[_serialPort.ReadBufferSize + 1];
                        try
                        {
                            // If there are bytes available on the serial port,
                            // Read returns up to "count" bytes, but will not block (wait)
                            // for the remaining bytes. If there are no bytes available
                            // on the serial port, Read will block until at least one byte
                            // is available on the port, up until the ReadTimeout milliseconds
                            // have elapsed, at which time a TimeoutException will be thrown.
                            int count = _serialPort.Read(readBuffer, 0, _serialPort.ReadBufferSize);
                            String SerialIn = System.Text.Encoding.ASCII.GetString(readBuffer, 0, count);
                            DataReceived(SerialIn);
                        }
                        catch (TimeoutException) { }
                    }
                    else
                    {
                        TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 50);
                        Thread.Sleep(waitTime);
                    }
                }
            }

            public void SetParameters(string port, int baud, Parity par, int data, StopBits stop, Handshake hnd)
            {
                portName = port;
                baudRate = baud;
                parity = par;
                dataBit = data;
                stopBit = stop;
                hndShake = hnd;

            }

            /// <summary> Open the serial port with current settings. </summary>
            public void Open()
            {
                Close();

                try
                {
                    _serialPort.PortName = portName;
                    _serialPort.BaudRate = baudRate;
                    _serialPort.Parity = parity;
                    _serialPort.DataBits = dataBit;
                    _serialPort.StopBits = stopBit;
                    _serialPort.Handshake = hndShake;

                    // Set the read/write timeouts
                    _serialPort.ReadTimeout = 50;
                    _serialPort.WriteTimeout = 50;

                    _serialPort.Open();
                    StartReading();
                }
                catch (IOException)
                {
                    StatusChanged(String.Format("{0} does not exist", portName));
                }
                catch (UnauthorizedAccessException)
                {
                    StatusChanged(String.Format("{0} already in use", portName));
                }
                catch (Exception ex)
                {
                    StatusChanged(String.Format("{0}", ex.ToString()));
                }

                // Update the status
                if (_serialPort.IsOpen)
                {
                    string p = _serialPort.Parity.ToString().Substring(0, 1);   //First char
                    string h = _serialPort.Handshake.ToString();
                    if (_serialPort.Handshake == Handshake.None)
                        h = "no handshake"; // more descriptive than "None"

                    StatusChanged(String.Format("{0}: {1} bps, {2}{3}{4}, {5}",
                        _serialPort.PortName, _serialPort.BaudRate,
                        _serialPort.DataBits, p, (int)_serialPort.StopBits, h));
                }
                else
                {
                    StatusChanged(String.Format("{0} already in use", portName));
                }
            }

            /// <summary> Close the serial port. </summary>
            public void Close()
            {
                StopReading();
                _serialPort.Close();
                StatusChanged("connection closed");
            }

            /// <summary> Get the status of the serial port. </summary>
            public bool IsOpen
            {
                get
                {
                    return _serialPort.IsOpen;
                }
            }

            /// <summary> Get a list of the available ports. Already opened ports
            /// are not returend. </summary>
            public string[] GetAvailablePorts()
            {
                return SerialPort.GetPortNames();
            }

            /// <summary>Send data to the serial port after appending line ending. </summary>
            /// <param name="data">An string containing the data to send. </param>
            public void Send(string data)
            {
                if (IsOpen)
                {
                    string lineEnding = "";
                    //switch (Settings.Option.AppendToSend)
                    //{
                    //    case Settings.Option.AppendType.AppendCR:
                    lineEnding = "\r"; //break;
                    //    case Settings.Option.AppendType.AppendLF:
                            //lineEnding = "\n";
                    //    case Settings.Option.AppendType.AppendCRLF:
                            //lineEnding = "\r\n";
                    //}

                    _serialPort.Write(data + lineEnding);
                }
            }
        }
        #endregion

        #region  (* RSA attributes *)
        DB databaseR1 = new DB();
        DB databaseR2 = new DB();
        DB databaseR3 = new DB();
        DB databaseR4 = new DB();
        DB databaseL = new DB();

        PLServer plserver = new PLServer();
        RSABB.BLUEBOXLibClass bbmain = new BLUEBOXLibClass();
        RSABB.BLUEBOXLibClass bbfl1 = new BLUEBOXLibClass();
        RSABB.BLUEBOXLibClass bbfl2 = new BLUEBOXLibClass();
        BBBalluff g1Balluff = new BBBalluff();
        BBBalluff g2Balluff = new BBBalluff();
        BBBalluff g31Balluff = new BBBalluff();
        BBBalluff g32Balluff = new BBBalluff();
        BBBase g1ASE = new BBBase();
        BBBase g2ASE = new BBBase();
        BBBase g3ASE = new BBBase();

        int handle1 = 0;
        int handle2 = 0;
        int handlemain = 0;
        DataCollectionList dcCollection = new DataCollectionList();
        EventTrackerList eventTracker = new EventTrackerList();

        Kawasaki myKawasakiR1;
        Kawasaki myKawasakiR3;
        #endregion

        #region (* RSA copy constructor *)
        public RSA(DB dBase1, DB dBase2, DB dBase3, DB dBase4, DB dBaseL, Kawasaki r1, Kawasaki r3)
        {
            databaseR1 = dBase1;
            databaseR2 = dBase2;
            databaseR3 = dBase3;
            databaseR4 = dBase4;
            databaseL = dBaseL;
            myKawasakiR1 = r1;
            myKawasakiR3 = r3;
        }
     
        #endregion

        #region (* RSA constructor *)
        public RSA()
        {
        }
        #endregion

        #region (* RSA methods *)
        public Kawasaki GetCommuKawasakiR1()
        {
            return myKawasakiR1;
        }

        public Kawasaki GetCommuKawasakiR2()
        {
            return null;
        }

        public Kawasaki GetCommuKawasakiR3()
        {
            return myKawasakiR3;
        }

        public PLServer GetPLServer()
        {
            return plserver;
        }

        public DB GetDBR1()
        {
            return databaseR1;
        }

        public DB GetDBR2()
        {
            return databaseR2;
        }

        public DB GetDBR3()
        {
            return databaseR3;
        }

        public DB GetDBR4()
        {
            return databaseR4;
        }

        public DB GetDBL()
        {
            return databaseL;
        }

        public BBBase GetG1ASE()
        {
            return g1ASE;
        }

        public BBBase GetG2ASE()
        {
            return g2ASE;
        }

        public BBBase GetG3ASE()
        {
            return g3ASE;
        }

        public BBBalluff GetG1Balluff()
        {
            return g1Balluff;
        }

        public BBBalluff GetG2Balluff()
        {
            return g2Balluff;
        }

        public BBBalluff GetG31Balluff()
        {
            return g31Balluff;
        }

        public BBBalluff GetG32Balluff()
        {
            return g32Balluff;
        }

        public BLUEBOXLibClass GetBBMain()
        {
            return bbmain;
        }

        public BLUEBOXLibClass GetBBFL1()
        {
            return bbfl1;
        }

        public BLUEBOXLibClass GetBBFL2()
        {
            return bbfl2;
        }

        public void SetHandleFL1(int hnd)
        {
            handle1 = hnd;
        }

        public void SetHandleFL2(int hnd)
        {
            handle2 = hnd;
        }

        public void SetHandleMain(int hnd)
        {
            handlemain = hnd;
        }

        public int GetHandleFL1()
        {
            return handle1;
        }

        public int GetHandleFL2()
        {
            return handle2;
        }

        public int GetHandleMain()
        {
            return handlemain;
        }

        public DataCollectionList GetDataCollectionList()
        {
            return dcCollection;
        }

        public EventTrackerList GetEventTrackerList()
        {
            return eventTracker;
        }

        #endregion

        #region (* Balluff reader *)
        public class BBBalluff
        {
            //   'bisvController' := Pointer Declaration type of T_Controller 
            //   reference library  BIS_V_TCPIP_DLL 
            private T_Controller bisvController;

            //   'myPoint' := Pointer Declaration type of IPEndPoint 
            //   reference library  System.Net
            private IPEndPoint bisvIpPoint;

            private int iPort1 = 10001;
            private int iPort2 = 10002;
            private int iPort3 = 10003;
            private int iPort4 = 10004;
            private string iAddress = "192.168.72.223";

            #region(* constructors *)
            public BBBalluff()
            {
                iPort1 = 10001;
                iPort2 = 10002;
                iAddress = "172.31.10.150";
            }

            public BBBalluff(int port1, int port2, string address)
            {
                iPort1 = port1;
                iPort1 = port2;
                iAddress = address;
            }
            #endregion

            #region(* methods *))
            public int IPort1
            {
                get
                {
                    return iPort1;
                }
                set
                {
                    iPort1 = value;
                }
            }

            public int IPort2
            {
                get
                {
                    return iPort2;
                }
                set
                {
                    iPort2 = value;
                }
            }

            public int IPort3
            {
                get
                {
                    return iPort3;
                }
                set
                {
                    iPort3 = value;
                }
            }

            public int IPort4
            {
                get
                {
                    return iPort4;
                }
                set
                {
                    iPort4 = value;
                }
            }

            public string IAddress
            {
                get
                {
                    return iAddress;
                }
                set
                {
                    iAddress = value;
                }
            }

            public bool Connect(int connID)
            {
                bool ret = false;

                try
                {
                    //   Create Instance of EndPoint Network Object witch references IP and Port of the BIS-V 
                    bisvIpPoint = new IPEndPoint(IPAddress.Parse(iAddress), connID + 10000);

                    //   Create Instance of BIS-V 'T_Controller' Object 
                    //   Constructor parameter defines by 'IPEndPoint', IP Address and Port for BIS-V Connection
                    this.bisvController = new T_Controller(this.bisvIpPoint);

                    //   bisvController Object Method for connecting request
                    this.bisvController.Open();

                    ret = true;
                }
                catch (Exception ex)
                {
                    // it destroys bisvController object
                    this.bisvController = null;
                    throw new System.Exception("Connect failed on: " + ex.Message);
                }

                return ret;
            }

            public bool Disconnect()
            {
                bool ret = false;
                try
                {
                    //   bisvController Object Method for disconnecting request
                    this.bisvController.Close();
                    this.bisvController = null;
                    ret = true;
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Disconnect failed on: " + ex.Message);
                }
                return ret;
            }

            public bool Read(ref string rfidStr, int portNumber)
            {
                rfidStr = "";
                bool ret = false;

                try
                {
                   // portNumber = 10004;
                    byte[] byArray = this.bisvController.ReadTypeAndSerial(portNumber);

                    //balluff
                    rfidStr = BitConverter.ToString(byArray);
                    string[] line = rfidStr.Split('-');
                    rfidStr = "000000" + line[4] + line[5] + line[6] + line[7] + line[8];
                    ret = true;
                    return true;
                    //balluff


                    //SE
                    int i = 0;
                    for (i = 0; i <= 4; i++)
                    {
                        byte revByte = RevertByte(byArray[i + 4]); //byArray[i + 4];//
                        string revByteStr = (revByte.ToString("X"));
                        string byteString = ReverseString(revByteStr);



                        //if ((byteString.Length == 1)) byteString = "0" + byteString;
                        if ((byteString.Length == 1)) byteString = byteString + "0";


                        rfidStr = rfidStr + byteString;
                    }
                    rfidStr = "000000" + rfidStr;
                    ret = true;
                    //ASE

                    return ret;
                }
                catch (Exception ex)
                {
                    // In case of error, message on diagnostic labael 
                    ret = false;
                    throw new System.Exception("Read RFID failed on: " + ex.Message);
                }

                return ret;
            }

            public string ReverseString(string text)
            {
                char[] cArray = text.ToCharArray();
                string reverse = String.Empty;
                for (int i = cArray.Length - 1; i > -1; i--)
                {
                    reverse += cArray[i];
                }
                return reverse;
            }

            private byte RevertByte(byte bitString)
            {
                byte RevertByteRet = default;
                const byte BIN_MASK = 255;
                const byte NUM_OF_BITS = 8;
                byte FilteredString;
                FilteredString = (byte)(bitString & BIN_MASK); // not really useful, actually... :S
                var RevertedBitString = default(string); // it will contain the reverted bit string
                int I;
                for (I = 0; I <= NUM_OF_BITS - 1; I++)
                {
                    RevertedBitString = RevertedBitString + Convert.ToString((FilteredString & Convert.ToByte(Math.Pow(2, I))) / Math.Pow(2, I));
                    if ((RevertedBitString.Substring(RevertedBitString.Length - 1, 1) == "1"))
                        RevertByteRet = Convert.ToByte(RevertByteRet + Math.Pow(2, NUM_OF_BITS - I - 1));
                }

                return RevertByteRet;
            }

            public bool isConnected()
            {

                return true;
            }
            #endregion
        }

        #endregion

        #region(* ase reader *)
        public class BBBase
        {
            public int bHandle = 0;
            RSABB.BLUEBOXLibClass bInstance = new BLUEBOXLibClass();
            private string iAddress = "192.168.72.223";
            public BBBase()
            {

            }

            public int BBConnect(string ipaddress, int port, byte address)
            {
                int err = -1;

                System.Net.IPAddress Ip = System.Net.IPAddress.Parse(ipaddress);                
                UInt16 Port = (UInt16)port;

                bInstance.Init(out bHandle);
                err = bInstance.SetAddress(ref bHandle, address);

                if (err != 0)
                {
                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);

                    return err;
                }

                err = bInstance.SetChannel(ref bHandle, "TCP", Ip + ":" + Port.ToString() + ",60000");
                if (err != 0)
                {
                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);

                    return err;
                }

                err = bInstance.Open(ref bHandle);
                if (err != 0)
                {
                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);

                    return err;
                }

                System.Threading.Thread.Sleep(100);

                System.Text.StringBuilder FwRel = new System.Text.StringBuilder(64);
                err = bInstance.GetFwRelease(ref bHandle, 0, FwRel);
                if (err != 0)
                {

                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);

                    return err;
                }

                System.Threading.Thread.Sleep(100);

                // Read the general parameters.
                byte[] Params = new byte[7];

                err = bInstance.ReadParameters(ref bHandle, Params);

                if (err != 0)
                {
                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);

                    return err;
                }
                
                return err;
            }

            public int BBDataRequest(ref string tag_read, ref int antenna)
            {
                int err = -1;
                int TagsNo = 0;
                IntPtr Tags = IntPtr.Zero;
                err = bInstance.DataRequest(ref bHandle, out Tags, out TagsNo);

                if (err != 0) // check if we got errors from Bluebox
                {
                    bInstance.Close(ref bHandle);
                    bInstance.End(ref bHandle);
                }
                else
                {

                    IntPtr Tmp = new IntPtr((int)Tags + (0 * System.Runtime.InteropServices.Marshal.SizeOf(typeof(RSABB.BLUEBOX_Tag))));
                    RSABB.BLUEBOX_Tag tag;

                    // now got the pointer to the struct, get the structure from the memory
                    tag = (RSABB.BLUEBOX_Tag)System.Runtime.InteropServices.Marshal.PtrToStructure(Tmp, typeof(RSABB.BLUEBOX_Tag));
                    tag_read = "";
                    for (int k = 0; k < tag.Length; k++)
                    {
                        tag_read += System.Runtime.InteropServices.Marshal.ReadByte(tag.Id, k).ToString("X2").ToUpper();
                    }

                    antenna = tag.Antenna;
                    err = bInstance.FreeTagsMemory(ref bHandle, ref Tags, TagsNo);
                }

                return err;
            }

          
        }
        #endregion      

        #region (* data collection list class *)
        public class DataCollectionList
        {
            List<DataCollection> dcList = new List<DataCollection>();

            #region (* data collection object *)
            public class DataCollection
            {
                public string dcSize = "";
                public string dcType = "";

                public DataCollection(string s, string t)
                {
                    dcSize = s;
                    dcType = t;
                }

                public string GetDCSize()
                {
                    return dcSize;
                }

                public string GetDCType()
                {
                    return dcType;
                }
            #endregion
            }

            public void AddItem(string size, string type)
            {
                DataCollection dt = new DataCollection(size, type);
                dcList.Add(dt);
            }

           public int GetCounter(DataCollection dt)
            {

                List<DataCollection> filteredList = dcList.Where(x => (x.GetDCSize() == dt.dcSize) && (x.GetDCType() == dt.dcType)).ToList();

                return filteredList.Count;
            }

           public List<DataCollection> GetDistinctItem()
           {
               List<DataCollection> d = new List<DataCollection>();
               //calcolo numero di elementi unici
               var distinctItems = (from c in dcList
                                    orderby c.GetDCSize()
                                    select new
                                    {
                                       c.dcSize,
                                       c.dcType
                                    }
                                  ).Distinct().ToList();

               foreach (var a in distinctItems)
               {
                   d.Add(new DataCollection(a.dcSize, a.dcType));
               }
               
               return d;
           }

           public List<DataCollection> GetDistinctItemBySizeList()
           {
               List<DataCollection> d = new List<DataCollection>();
               //calcolo numero di elementi unici
               var distinctItems = (from c in dcList
                                    orderby c.GetDCSize()
                                    select new
                                    {
                                        c.dcSize                                        
                                    }
                                  ).Distinct().ToList();

               foreach (var a in distinctItems)
               {
                   d.Add(new DataCollection(a.dcSize, ""));
               }

               return d;
           }

           public List<DataCollection> GetDistinctItemBySize(string size)
           {
               List<DataCollection> d = new List<DataCollection>();              

               var distinctItems = dcList.Where(x => x.GetDCSize() == size);

               foreach (var a in distinctItems)
               {
                   d.Add(new DataCollection(a.dcSize, a.dcType));
               }

               return d;
           }

           public List<DataCollection> GetDistinctItemBySizeAndType(string size, string type)
           {
               List<DataCollection> d = new List<DataCollection>();

               var distinctItems = dcList.Where(x => (x.GetDCSize() == size) && (x.GetDCType() == type));

               foreach (var a in distinctItems)
               {
                   d.Add(new DataCollection(a.dcSize, a.dcType));
               }

               return d;
           }

           public List<DataCollection> GetList()
           {
               return dcList;
           }

           public void RemoveAll()
           {
               dcList.Clear();
           }
        }
        #endregion

        #region (* event tracker collection class *)
        public class EventTrackerList
        {
            #region(* Event tracker object class *)
            public class EventTracker
            {
                #region(* attributes *)
                //event date and event time
                DateTime eDateTime = new DateTime();
                //station identification of event
                StationID eStation = StationID.NONE;
                //event description
                string eDescription = "";
                //event tag or severity
                EventTrackerID eID = EventTrackerID.Info;
                //event start
                DateTime evStart = new DateTime();
                //event stop
                DateTime evStop = new DateTime();

                #endregion

                #region(* constructor *)
                public EventTracker()
                {

                }
                #endregion

                #region(* copy constructor *)
                public EventTracker(DateTime dt, StationID stID, string descr, EventTrackerID evID, DateTime evDateTime)
                {
                    eDateTime = dt;
                    eStation = stID;
                    eDescription = descr;
                    eID = evID;
                    evStart = evDateTime;
                }

                #endregion

                #region(* methods *)
                public EventTrackerID GetEventID()
                {
                    return eID;
                }

                public string GetEventDescription()
                {
                    return eDescription;
                }

                public StationID GetEventStation()
                {
                    return eStation;
                }

                public DateTime GetEventDateTime()
                {
                    return eDateTime;
                }

                #endregion
            }
            #endregion

            #region (* attributes *)
            List<EventTracker> etList = new List<EventTracker>();
            #endregion

            #region(* methods *)
            public List<EventTracker> GetList()
            {
                return etList;
            }

            public void AddElementToList(EventTracker et)
            {
                etList.Add(et);
            }

            public void RemoveElementFromList(EventTracker et)
            {
                etList.Remove(et);
            }

            public List<EventTracker> FilterByDate(DateTime dt)
            {
                List<EventTracker> retList = new List<EventTracker>();

                return retList;
            }

            public List<EventTracker> FilterByStation(StationID id)
            {
                List<EventTracker> retList = new List<EventTracker>();

                //retList = etList.Where(eStation => eStation = id).ToList();
                return retList;
            }

            public List<EventTracker> FilterBySeverity(EventTrackerID sv)
            {
                List<EventTracker> retList = new List<EventTracker>();

                //retList = etList.Where(el. == sv)).ToList();
                return retList;
            }
            #endregion
        }
        #endregion
    }

    public class RFIDReader
    {
        public string antenna1 = "";
        public string antenna2 = "";
        public string antenna3 = "";
        public string antenna4 = "";
        public RFIDReader()
        {

        }
    }

    public class PLCMaster
    {
        //line status: manual/automatic
        public bool automatic = false;
        //line emergency status
        public bool emercencyOk = false;
        //line pressure status
        public bool airPressureOk = false;
        //line manual/auto selector status
        public bool autoManSelectorStatus = false;
        //modbus connection status
        public bool connected = false;
        //cobol tp status
        public bool CobolTPOn = false;
        //G1 reading request
        public bool G1A1ReadingRequest = false;
        public bool G1A2ReadingRequest = false;
        public bool G1A3ReadingRequest = false;
        public bool G1A4ReadingRequest = false;
        public bool G1A1ManageRequest = false;
        public bool G1A2ManageRequest = false;
        public bool G1A3ManageRequest = false;
        public bool G1A3ProcessedRequest = false;
        public bool G1A4ManageRequest = false;
        //G2 reading request
        public bool G2A1ReadingRequest = false;
        public bool G2A2ReadingRequest = false;
        public bool G2A3ReadingRequest = false;
        public bool G2A4ReadingRequest = false;
        public bool G2A1ManageRequest = false;
        public bool G2A2ManageRequest = false;
        public bool G2A3ManageRequest = false;
        public bool G2A4ManageRequest = false;
        //G3 reading request
        public bool G3A1ReadingRequest = false;
        public bool G3A2ReadingRequest = false;
        public bool G3A3ReadingRequest = false;
        public bool G3A4ReadingRequest = false;
        public bool G3A1ManageRequest = false;
        public bool G3A2ManageRequest = false;
        public bool G3A3ManageRequest = false;
        public bool G3A4ManageRequest = false;
        //pl read result
        public bool G1A1ReadResult = false;
        public bool G1A2ReadResult = false;
        public bool G1A3ReadResult = false;
        public bool G1A4ReadResult = false;
        public ushort G1A3ReadResultAddress = 0;
        public ushort G1A2ReadResultAddress = 0;
        public ushort G2A2ReadResultAddress = 0;
        public ushort G2A2IsWasteAddress = 0;
        public ushort G2A2IsNoWasteAddress = 0;
        //public ushort G1A2IsNoWasteAddress = 0;
        //device ready
        public bool previousReadRequest = false;
        public string previousG1A3RFID = "";
        public string previousG1A2RFID = "";
        public int G1A3RFIDCounter = 0;

        public PLCMaster()
        {

        }
    }

    public class Robot
    {
        private bool clientConnectionStatus = false;
        private List<string> robotFlag = new List<string>();
        private bool robotReady = false;
        private bool robotInclusion = false;
        private bool robotInclusionCleaning = false;
        private bool robotIsDisconnected = false;
        public Robot()
        {

        }

        public bool GetClientConnectionStatus ()
        {
            return clientConnectionStatus;
        }

        public void SetRobotFlags(List<string> flags)
        {
            robotFlag = flags;
        }

        public bool GetRobotIsDisconnected()
        {
            return robotIsDisconnected;
        }

        public void SetRobotIsDisconnected(bool status)
        {
            robotIsDisconnected = status;
        }

        public List<string> GetRobotFlags()
        {
            return robotFlag;
        }

        public void SetRobotReady(bool status)
        {
            robotReady = status;
        }

        public bool GetRobotReady()
        {
            return robotReady;
        }

        public void SetRobotInclusion(bool inclusion)
        {
            robotInclusion = inclusion;
        }

        public void SetRobotInclusionCleaning(bool inclusion)
        {
            robotInclusionCleaning = inclusion;
        }

        public bool GetRobotInclusion()
        {
            return robotInclusion;
        }

        public bool GetRobotInclusionCleaning()
        {
            return robotInclusionCleaning;
        }

        public void SetClientConnectionStatus(bool status)
        {
            clientConnectionStatus = status;
        }
    }



    public class LineDevice
    {
        #region(* attributes *)
        //device name
        private string dName = "";
        //ready status
        private bool dReady = false;
        //status
        private string dStatus = "";
        //alarm code
        private string dAlarmCode = "";
        //left station
        private bool dLFInclusion = false;
        //right station
        private bool dRGInclusion = false;
        //station mode
        private int dMode = 0;
        //with recipe
        private bool dWithRecipe = false;
        //recipe param 1
        private string drParam1 = "--";
        //recipe param 2
        private string drParam2 = "--";
        //recipe param 3
        private string drParam3 = "--";
        //recipe param 4
        private string drParam4 = "--";
        //custom param 1
        private string dcParam1 = "--";
        //custom param 2
        private string dcParam2 = "--";
        //custom param 3
        private string dcParam3 = "--";
        //custom param 4
        private string dcParam4 = "--";
        //plc addresses
        private ushort read_ok_lf_address = 0;
        private ushort read_ok_rg_address = 0;
        private ushort param1_address = 0;
        private ushort param2_address = 0;
        private ushort param3_address = 0;
        private ushort param4_address = 0;
        private ushort lf_inclusion_address = 0;
        private ushort rg_inclusion_address = 0;
        private ushort mode_address = 0;
        private ushort ready_address = 0;
        public string interWORDV1 = "";
        public string interWORDV2 = "";
        public string interWORDV3 = "";
        public string interWORDV4 = "";
        #endregion

        #region(* methods *)
        public string GetDeviceStatus()
        {
            return dStatus;
        }

        public void SetDeviceStatus(string value)
        {
            dStatus = value;
        }

        public bool GetDeviceWithRecipe()
        {
            return dWithRecipe;
        }

        public void SetDeviceWithRecipe(bool value)
        {
            dWithRecipe = value;
        }

        public string GetDeviceAlarmCode()
        {
            return dAlarmCode;
        }

        public void SetDeviceAlarmCode(string value)
        {
            dAlarmCode = value;
        }

        public ushort GetDeviceReadOKLFAddress()
        {
            return read_ok_lf_address;
        }

        public void SetDeviceReadOKLFAddress(ushort value)
        {
            read_ok_lf_address = value;
        }

        public ushort GetDeviceReadOKRGAddress()
        {
            return read_ok_rg_address;
        }

        public void SetDeviceReadOKRGAddress(ushort value)
        {
            read_ok_rg_address = value;
        }

        public ushort GetDeviceParam1Address()
        {
            return param1_address;
        }

        public void SetDeviceParam1Address(ushort value)
        {
            param1_address = value;
        }

        public ushort GetDeviceParam2Address()
        {
            return param2_address;
        }

        public void SetDeviceParam2Address(ushort value)
        {
            param2_address = value;
        }

        public ushort GetDeviceParam3Address()
        {
            return param3_address;
        }

        public void SetDeviceParam3Address(ushort value)
        {
            param3_address = value;
        }

        public ushort GetDeviceParam4Address()
        {
            return param4_address;
        }

        public void SetDeviceParam4Address(ushort value)
        {
            param4_address = value;
        }

        public ushort GetDeviceModeAddress()
        {
            return mode_address;
        }

        public void SetDeviceModeAddress(ushort value)
        {
            mode_address = value;
        }

        public ushort GetDeviceLFInclusionAddress()
        {
            return lf_inclusion_address;
        }

        public void SetDeviceLFInclusionAddress(ushort value)
        {
            lf_inclusion_address = value;
        }

        public ushort GetDeviceRGInclusionAddress()
        {
            return rg_inclusion_address;
        }

        public void SetDeviceRGInclusionAddress(ushort value)
        {
            rg_inclusion_address = value;
        }

        public ushort GetDeviceReadyAddress()
        {
            return ready_address;
        }

        public void SetDeviceReadyAddress(ushort value)
        {
            ready_address = value;
        }

        public string GetDeviceName()
        {
            return dName;
        }

        public void SetDeviceName(string name)
        {
            dName = name;
        }

        public int GetDeviceMode()
        {
            return dMode;
        }

        public void SetDeviceMode(int mode)
        {
            dMode = mode;
        }

        public string GetRecipeDeviceParam1()
        {
            return drParam1;
        }

        public void SetRecipeDeviceParam1(string param1)
        {
            drParam1 = param1;
        }

        public string GetRecipeDeviceParam2()
        {
            return drParam2;
        }

        public void SetRecipeDeviceParam2(string param2)
        {
            drParam2 = param2;
        }

        public string GetRecipeDeviceParam3()
        {
            return drParam3;
        }

        public void SetRecipeDeviceParam3(string param3)
        {
            drParam3 = param3;
        }

        public string GetRecipeDeviceParam4()
        {
            return drParam4;
        }

        public void SetRecipeDeviceParam4(string param4)
        {
            drParam4 = param4;
        }

        public string GetCustomDeviceParam1()
        {
            return dcParam1;
        }

        public void SetCustomDeviceParam1(string param1)
        {
            dcParam1 = param1;
        }

        public string GetCustomDeviceParam2()
        {
            return dcParam2;
        }

        public void SetCustomDeviceParam2(string param2)
        {
            dcParam2 = param2;
        }

        public string GetCustomDeviceParam3()
        {
            return dcParam3;
        }

        public void SetCustomDeviceParam3(string param3)
        {
            dcParam3 = param3;
        }

        public string GetCustomDeviceParam4()
        {
            return dcParam4;
        }

        public void SetCustomDeviceParam4(string param4)
        {
            dcParam4 = param4;
        }

        public bool GetDeviceReady()
        {
            return dReady;
        }

        public void SetDeviceReady(bool ready)
        {
            dReady = ready;
        }

        public bool GetDeviceLFInclusion()
        {
            return dLFInclusion;
        }

        public void SetDeviceLFInclusion(bool inc)
        {
            dLFInclusion = inc;
        }

        public bool GetDeviceRGInclusion()
        {
            return dRGInclusion;
        }

        public void SetDeviceRGInclusion(bool inc)
        {
            dRGInclusion = inc;
        }
        #endregion

        public LineDevice()
        {

        }
    }

    public class GDevice
    {
        #region(* attributes *)
        private string gdName = "";
        private bool gdIsReady = false;
        private bool gdIsIncluded = false;
        public bool gdIsDisconnected = false;
        public bool gdIsInAlarm = false;
        private string gdParam1 = "--";
        private string gdParam2 = "--";
        private string gdParam3 = "--";
        private string gdParam4 = "--";
        public float gdRuntimeParam1 = 0.0f;
        public float gdRuntimeParam2 = 0.0f;
        public float gdRuntimeParam3 = 0.0f;
        public float gdRuntimeParam4 = 0.0f;
        private int gdMode = 0;
        private ushort param1_address = 0;
        private ushort param2_address = 0;
        private ushort param3_address = 0;
        private ushort param4_address = 0;
        private string gdStatus = "";
        private string gdAlarmCode = "";
        public short machineStatus = -1;
        

        #endregion

        #region(* methods *)
        public string GetDeviceStatus()
        {
            return gdStatus;
        }

        public void SetDeviceStatus(string value)
        {
            gdStatus = value;
        }

        public string GetDeviceAlarmCode()
        {
            return gdAlarmCode;
        }

        public void SetDeviceAlarmCode(string value)
        {
            gdAlarmCode = value;
        }               

        public ushort GetDeviceParam1Address()
        {
            return param1_address;
        }

        public void SetDeviceParam1Address(ushort value)
        {
            param1_address = value;
        }

        public ushort GetDeviceParam2Address()
        {
            return param2_address;
        }

        public void SetDeviceParam2Address(ushort value)
        {
            param2_address = value;
        }

        public ushort GetDeviceParam3Address()
        {
            return param3_address;
        }

        public void SetDeviceParam3Address(ushort value)
        {
            param3_address = value;
        }

        public ushort GetDeviceParam4Address()
        {
            return param4_address;
        }

        public void SetDeviceParam4Address(ushort value)
        {
            param4_address = value;
        }      

        public string GetDeviceName()
        {
            return gdName;
        }

        public void SetDeviceName(string name)
        {
            gdName = name;
        }

        public int GetDeviceMode()
        {
            return gdMode;
        }

        public void SetDeviceMode(int mode)
        {
            gdMode = mode;
        }

        public string GetDeviceParam1()
        {
            return gdParam1;
        }

        public void SetDeviceParam1(string param1)
        {
            gdParam1 = param1;
        }

        public string GetDeviceParam2()
        {
            return gdParam2;
        }

        public void SetDeviceParam2(string param2)
        {
            gdParam2 = param2;
        }

        public string GetDeviceParam3()
        {
            return gdParam3;
        }

        public void SetDeviceParam3(string param3)
        {
            gdParam3 = param3;
        }

        public string GetDeviceParam4()
        {
            return gdParam4;
        }

        public void SetDeviceParam4(string param4)
        {
            gdParam4 = param4;
        }

        public bool GetDeviceReady()
        {
            return gdIsReady;
        }

        public void SetDeviceReady(bool ready)
        {
            gdIsReady = ready;
        }

        public bool GetDeviceInclusion()
        {
            return gdIsIncluded;
        }

        public void SetDeviceInclusion(bool inc)
        {
            gdIsIncluded = inc;
        }        
        #endregion

        public GDevice()
        {

        }
    }
    //classe Singleton di tipo sealed non ereditabile
    public sealed class MachineRuntime
    {
        //membro privato che rappresenta l'instanza della classe
        private static MachineRuntime _instance;

        //reader RFID values list
        public static List<RFIDReader> RFIDReaderList = new List<RFIDReader>();
        
        //reader type
        public static string RFIDReaderType = "BALLUFF";

        //plc status
        public static PLCMaster plcStatus = new PLCMaster();  

        //r1 status
        public static Robot r1 = new Robot();

        //r2 status
        public static Robot r2 = new Robot();

        //r3 status
        public static Robot r3 = new Robot();

        //r4 status
        public static Robot r4 = new Robot();
        
        public static LineDevice device1 = new LineDevice();
        public static LineDevice device2 = new LineDevice();
        public static LineDevice device3 = new LineDevice();
        public static LineDevice device4 = new LineDevice();
        public static GDevice gdevice1 = new GDevice();
        public static GDevice gdevice2 = new GDevice();

        public static string robotCodePL = "";
        public static string robotCodeD1 = "";
        public static string robotCodeD2 = "";
        public static string robotCodeD3 = "";
        public static string robotCodeD4 = "";

        //line status
        public static short lineMachineStatus = -1;
        public static short lineReadingAuto = 0;
        public static short lineUnloadReadingAuto = 0;
        public static short lineTimerReadingAuto = 6;
        public static short lineTimerTimeout = 6;
        public static short lineTimerEmpty = 6;
        public static bool lineInTimeout = false;
        //pl reading
        public static string rfidCodeG1A1 = "";
        public static string rfidModelNameG1A1 = "";
        public static string rfidSizeG1A1 = "";
        public static string rfidFootG1A1 = "";
        public static string rfidVariantG1A1 = "";
        public static bool rfidReadingRequestG1A1 = false;
        public static int rfidReadingResultG1A1 = -1;
        public static string rfidReadingResultStringG1A1 = "";

        //scanner reading
        public static string rfidCodeG1A2 = "";
        public static string rfidModelNameG1A2 = "";
        public static string rfidSizeG1A2 = "";
        public static string rfidTypeFromScannerG1A2 = "";
        public static string rfidTypeFromPlcG1A2 = "";
        public static string rfidVariantG1A2 = "";
        public static bool rfidReadingRequestG1A2 = false;
        public static int rfidReadingResultG1A2 = -1;
        public static string rfidReadingResultStringG1A2 = "";
        public static bool ackFromScanner = false;




        public static bool rfidReadingRequestG1A4 = false;
        public static int rfidReadingResultG1A4 = -1;
        public static string rfidCodeG1A4 = "";
        public static string rfidModelNameG1A4 = "";
        public static string rfidSizeG1A4 = "";
        public static string rfidFootG1A4 = "";
        public static string rfidVariantG1A4 = "";
        public static int lineLastCounter = 0;
        public static string lineModelName = "";

        public static string pallet1Size = "";
        public static string pallet2Size = "";
        public static string pallet3Size = "";
        public static string pallet4Size = "";
        //scanner info
        public static string scannerMessage = "";
        public static bool scannerSolePresence = false;
        public static bool scannerAxisHomingDone = false;
        public static bool scannerAxisInAlarm = false;
        public static int scannerWaitingCycle = 0;
        public static bool triggerToScanner = false;

        //membro privato per la sincronizzaz dei thread
        private static readonly Object _sync = new Object();

        //costruttore privato non accessibile dall'esterno della classe
        private MachineRuntime() { }

        //Entry-Point: proprietà esterna che ritorna l'istanza della classe
        public static MachineRuntime Instance
        {
            get
            {
                //per evitare richieste di lock successive alla prima istanza
                if (_instance == null)
                {
                    lock (_sync) //area critica per la sincronizz dei thread
                    {
                        //vale sempre per la prima istanza
                        if (_instance == null)
                        {
                            _instance = new MachineRuntime();
                        }
                    }
                }
                return _instance;
            }
        }

       

    }

}
