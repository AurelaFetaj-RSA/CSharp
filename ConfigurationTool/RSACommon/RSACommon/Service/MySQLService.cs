using log4net;
using MySql.Data.MySqlClient;
using RSACommon.Configuration;
using RSACommon.DatabasesUtils;
using RSAInterface;
using RSAInterface.Helper;
using RSAInterface.Logger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Service
{
    public class MySqlResult: MySqlResult<IDBSet>
    {

    }

    public class MySqlResult<T> where T : IDBSet
    {
        public Error Error { get; set; } = Error.OK;
        public string Message { get; set; } = "";
        public string Executed { get; set; } = "";
        public List<T> Result { get; set; } = new List<T>();
    }

    /// <summary>
    /// NUGET USED: 8.0.29
    /// </summary>
    public class MySQLService : IService
    {

        string myConnectionStringPattern = "server={0};Port={1};uid={2};pwd={3};database={4}";
        public ILog Log { get; private set; }
        public List<MySqlConnection> SqlConnection { get; private set; } = new List<MySqlConnection>();
        public IServiceConfiguration Configuration { get; private set; }
        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;
        public MySQLService(IServiceConfiguration serviceConfig)
        {
            Configuration = serviceConfig;

            if (serviceConfig is RSACommon.Configuration.MySqlConfiguration mySqlConf)
            {
                _mySqlConfiguration = mySqlConf;
                IsActive = mySqlConf.Active;
                ServiceURI = Helper.BuildUri(_mySqlConfiguration);
                //SqlConnection = ConfigConnection(_mySqlConfiguration.User, ServiceURI, _mySqlConfiguration.DBName);
            }
        }

        public List<DBUtilsMySql> DBTable { get; set; } = new List<DBUtilsMySql>();
        public MySQLService(IServiceConfiguration serviceConfig, ILog logger): this(serviceConfig)
        {
            Log = logger;
        }
        public MySqlConnection ConfigConnection()
        {
            if(Configuration is RSACommon.Configuration.MySqlConfiguration config)
            {
                Uri last = new UriBuilder(config.Scheme, config.Host, config.Port).Uri;
                return ConfigConnection(config.User, last, config.DBName);
            }

            return null;
        }
        public MySqlConnection ConfigConnection(RSACommon.Configuration.MySqlConfiguration config)
        {
            Uri last = new UriBuilder(config.Scheme, config.Host, config.Port).Uri;
            return ConfigConnection(config.User, last, config.DBName);
        }

        public MySqlConnection ConfigConnection(RSACommon.Configuration.SqlUser user, Uri mysqlUri, string dbName)
        {
            if (user == null)
                return null;
            string pwd = user.Password == null?"": RSACommon.PasswordSecurity.DecryptString(user.Password);
            pwd = "Mangella24";
            pwd = "Pippo_123";

            try
            {
                string connectionString = String.Format(myConnectionStringPattern, mysqlUri.Host, mysqlUri.Port, user.UserName, pwd, dbName);
                MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection();
                connection.ConnectionString = connectionString;

                return connection;
            }
            catch (MySqlException ex)
            {
                Log?.Warn(ex.Message);
                return null;
            }
        }

        public async Task<MySqlResult> ConnectAsync()
        {
            return await ConnectAsync(_mySqlConfiguration.User, ServiceURI, _mySqlConfiguration.DBName);
        }


        /// <summary>
        /// Automatic fill of class and search the properties
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="toManage"></param>
        public void AddTable<T>(string tableName)
        {
            ///istanzio la MySqlTable, gli configuro le colonne e la primary key
            MySqlTable modelTable = new MySqlTable(Log, tableName).Configure<T>(this);
            DBTable.Add(new DBUtilsMySql(modelTable));
        }

        public void AddTable(string tableName, List<string> columns, string primaryKey)
        {
            ///istanzio la MySqlTable, gli configuro le colonne e la primary key
            MySqlTable modelTable = new MySqlTable(Log, tableName).Configure(this, columns, primaryKey);
            DBTable.Add(new DBUtilsMySql(modelTable)); 
        }

        public async Task<MySqlResult> ConnectAsync(RSACommon.Configuration.SqlUser user, Uri mysqlUri, string dbName)
        {
            MySqlResult result = new MySqlResult();

            try
            {
                foreach (DBUtilsMySql obj in DBTable)
                {
                    await obj.tableForData.Connection.OpenAsync();
                }

                result.Message = "Connection is OK";

                return result;
            }
            catch (MySqlException ex)
            {
                Log?.Warn(ex.Message);
                result.Error = Error.MYSQL_CONNECTION_ERROR;
                result.Message = ex.Message;

                return result;
            }
        }

        public string Name { get; private set; } = "DB Service";
        public bool IsActive { get; private set; } = false;
        private RSACommon.Configuration.MySqlConfiguration _mySqlConfiguration { get; set; }
        IService IService.SetLogger(LoggerConfigurator logger)
        {
            Log = logger?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }

        public IService StartNoAsync()
        {
            foreach (DBUtilsMySql obj in DBTable)
            {
                obj.tableForData.Connection.Open();
            }
            //await SqlConnection.OpenAsync();

            return this;
        }

        public async Task<IService> Start()
        {
            await Task.Delay(100);

            foreach (DBUtilsMySql obj in DBTable)
            {
                await obj.tableForData.Connection.OpenAsync();
            }
            //await SqlConnection.OpenAsync();

            return this;
        }

        public void Stop()
        {
            //SqlConnection.Close();
        }
    }
}
