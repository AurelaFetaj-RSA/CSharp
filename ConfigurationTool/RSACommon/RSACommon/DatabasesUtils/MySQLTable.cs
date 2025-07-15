using Google.Protobuf.WellKnownTypes;
using log4net;
using MySql.Data.MySqlClient;
using Opc.Ua;
using RSACommon.Service;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.DatabasesUtils
{
    public class MySqlTable: IDBTable 
    {
        public string Name { get; set; } = "SQLTABLE";
        public ILog Logger { get; set; } = null;
        public MySqlConnection Connection { get; private set; } = null;
        public List<string> Columns = new List<string>();
        public string PrimaryKey { get; set; } = string.Empty;
        public System.Type RecordsType { get; set; } = null;


        public MySqlTable(ILog log, string name)
        {
            Logger = log;
            Logger?.Info($"Table {name} configured");
            Name = name;
        }

        public MySqlTable Configure<T>(IService service)
        {
            int keyCount = 0;

            if (service is MySQLService serviceMySql)
            {
                Connection = serviceMySql.ConfigConnection();

                foreach(var prop in typeof(T).GetProperties())
                {
                    Columns.Add(prop.Name);

                    var att = prop.GetCustomAttributes(typeof(KeyAttribute), false);

                    if (att.Length > 0)
                    {
                        PrimaryKey = prop.Name;
                        keyCount++;
                    }
                }
                //Columns = column;
                //PrimaryKey = primaryKey;
            };

            if (keyCount > 1)
                throw new Exception($"Set too much primary key in {Name}");

            return this;
        }

        public MySqlTable Configure(IService service, List<string> column = null, string primaryKey = null)
        {
            if (service is MySQLService serviceMySql)
            {
                Connection = serviceMySql.ConfigConnection();
                Columns = column;
                PrimaryKey = primaryKey;
            }

            return this;
        }

        public async Task<MySqlResult<T>> ExecuteQueryAsync<T>(string commandText, params MySqlParameter[] values) where T : RSACommon.DatabasesUtils.IDBSet
        {
            MySqlResult<T> result1 = new MySqlResult<T>();

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }

            List<T> toReturn = new List<T>();

            MySqlCommand cmd = new MySqlCommand(commandText);
            cmd.Connection = Connection;

            int result = -1;

            try
            {
                cmd.Parameters.AddRange(values);

                await Task.Run(async () =>
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            bool readExecuted = false;

                            while (readExecuted = await reader.ReadAsync())
                            {
                                T toFill = (T)Activator.CreateInstance<T>();
                                toFill.CopyFromDB(reader);
                                toReturn.Add(toFill);
                            }

                            result = toReturn.Count;
                        }
                        catch (MySqlException ex)
                        {
                            result = -1;
                            Logger?.Error(ex.Message);
                            result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                            result1.Message = ex.Message;

                            //await Connection.CloseAsync();
                        }

                    }
                });

                result1.Executed = cmd.CommandText;
                result1.Result = toReturn;
            }
            catch (MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = ex.Message;

                //await Connection.CloseAsync();

                return result1;
            }

            if (result == 0)
            {
                result1.Error = Error.OK;
                result1.Message = $"No row founds";
                return result1;
            }
            else if (result != -1)
            {
                result1.Error = Error.OK;
                result1.Message = "Ok, execute is going well";
                return result1;
            }
            else
            {
                result1.Error = Error.MYSQL_READ_ERROR;
                result1.Message = $"Bad, execute {result} lines";
                return result1;
            }
        }

        public MySqlResult<T> ExecuteQuery<T>(string commandText, params MySqlParameter[] values) where T : RSACommon.DatabasesUtils.IDBSet
        {
            MySqlResult<T> result1 = new MySqlResult<T>();

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }

            List<T> toReturn = new List<T>();

            MySqlCommand cmd = new MySqlCommand(commandText);
            cmd.Connection = Connection;

            int result = -1;

            try
            {
                cmd.Parameters.AddRange(values);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        T toFill = (T)Activator.CreateInstance<T>();
                        toFill.CopyFromDB(reader);
                        toReturn.Add(toFill);
                    }
                }

                result1.Executed = cmd.CommandText;
                result1.Result = toReturn;
            }
            catch (MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = ex.Message;

                //Connection.Close();

                return result1;
            }

            if (result == 0)
            {
                result1.Error = Error.OK;
                result1.Message = $"No row founds";
                return result1;
            }
            else if (result != -1)
            {
                result1.Error = Error.OK;
                result1.Message = "Ok, execute is going well";
                return result1;
            }
            else
            {
                result1.Error = Error.MYSQL_READ_ERROR;
                result1.Message = $"Bad, execute {result} lines";
                return result1;
            }
        }

        public MySqlResult ExecuteNotQuery(string commandText, params MySqlParameter[] values)
        {
            MySqlResult result1 = new MySqlResult();

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();

                //result1.Error = Error.MYSQL_NOT_CONNECTED;
                //result1.Message = "Connection is not opened";
                //return result1;
            }

            MySqlCommand cmd = new MySqlCommand(commandText);
            cmd.Connection = Connection;

            int result = -1;

            try
            {
                cmd.Parameters.AddRange(values);

                result = cmd.ExecuteNonQuery();
                result1.Executed = cmd.CommandText;
            }
            catch (MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = ex.Message;

                //Connection.Close();

                return result1;
            }

            try
            {
                //Connection.Close();
            }
            catch(MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_CONNECTION_ERROR;
                result1.Message = ex.Message;
            }


            if (result == 0)
            {
                result1.Error = Error.OK;
                result1.Message = $"No row founds";
                return result1;
            }
            else if (result != -1)
            {
                result1.Error = Error.OK;
                result1.Message = "Ok, execute is going well";
                return result1;
            }
            else
            {
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = $"Bad, execute {result} lines";
                return result1;
            }
        }

        public async Task<MySqlResult> ExecuteNotQueryAsync(string commandText, params MySqlParameter[] values)
        {
            MySqlResult result1 = new MySqlResult();

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();

                //result1.Error = Error.MYSQL_NOT_CONNECTED;
                //result1.Message = "Connection is not opened";
                //return result1;
            }

            MySqlCommand cmd = new MySqlCommand(commandText);
            cmd.Connection = Connection;

            int result = -1;
            try
            {
                foreach (MySqlParameter value in values)
                {
                    cmd.Parameters.Add(value);
                }

                result = await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = ex.Message;
                //Connection.Close();

                return result1;
            }

            try
            {
                //Connection.Close();
            }
            catch (MySqlException ex)
            {
                Logger?.Error(ex.Message);
                result1.Error = Error.MYSQL_CONNECTION_ERROR;
                result1.Message = ex.Message;
            }

            if (result != -1)
            {
                result1.Error = Error.OK;
                result1.Message = "Ok, execution is going well";
                return result1;
            }
            else
            {
                result1.Error = Error.MYSQL_NON_QUERY_ERROR;
                result1.Message = $"Bad, executed {result} lines";
                return result1;
            }
        }


    }
}
