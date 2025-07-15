using MySql.Data.EntityFramework;
using RSACommon.Configuration;
using RSACommon.DatabasesUtils;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MySqlDB
{
    internal class Program
    {
        public static string USERNAME = "m.angella";
        public static string PWD = "Robots_RSA";
        public static string HOST = "192.168.0.242";
        public static string DBNAME = "heartlanddb";

        public static RSACommon.Configuration.MySqlConfiguration CreateConfiguration(string user, string pwd, string host, string DB)
        {
            string encryptedString = RSACommon.PasswordSecurity.EncryptString(pwd);

            RSACommon.Configuration.MySqlConfiguration configuration = new RSACommon.Configuration.MySqlConfiguration()
            {
                Host = host,
                User = new SqlUser()
                {
                    UserName = user,
                    Password = encryptedString
                },
                DBName = DB
            };

            return configuration;
        }

        static void Main(string[] args)
        {
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
            var config = CreateConfiguration(USERNAME, PWD, HOST, DBNAME);
            MySQLService service = new MySQLService(config);           

            service.StartNoAsync();

            Task.Run(async () =>
            {                


            });

            Console.ReadLine();

        }



    }
}
