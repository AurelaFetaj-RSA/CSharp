using MySql.Data.MySqlClient;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.DatabasesUtils
{
    public interface IDButils<out T> where T: IDBSet
    {
        Task<MySqlResult> InsertAutomaticAsync(object[] values);
        MySqlResult InsertAutomatic(object[] values);
        Task<MySqlResult> DeleteRowPrimaryKeyAsync(string key);
        MySqlResult DeleteRowPrimaryKey(string key);
        MySqlResult UpdateAutomaticPrimaryKey(string key, string[] paramToUpdate, object[] values);
    }

    public class DBUtilsMySql: IDButils<IDBSet>
    {
        public MySqlTable tableForData { get; private set; } = null;
        public int ColumnSize { get; private set; } = 0;
        public DBUtilsMySql(MySqlTable table)
        {
            tableForData = table;
            ColumnSize = table.Columns.Count;
        }

        public async Task<MySqlResult> InsertAutomaticAsync(object[] values)
        {
            string allcolumns = "";
            string allNameParam = "";
            string[] names = tableForData.Columns.ToArray();
            //tableForData.Columns.ForEach(x => allcolumns += x + ",");
            //tableForData.Column.ForEach(x => allNameParam += $"?{x},");

            for (int i = 0; i < ColumnSize; i++)
            {
                if (i != ColumnSize - 1)
                {
                    allcolumns += names[i] + ",";
                    allNameParam += $"?{names[i]},";
                }
                else
                {
                    allcolumns += names[i];
                    allNameParam += $"?{names[i]}";
                }
            }

            string insert = $"INSERT INTO {tableForData.Name}({allcolumns}) VALUES({allNameParam})";

            MySqlParameter[] arryaOfParams = new MySqlParameter[ColumnSize];

            for (int i = 0; i < ColumnSize; i++)
            {
                if (tableForData.Columns.Contains(names[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(names[i], values[i]);
                }
            }

            return await tableForData.ExecuteNotQueryAsync(insert, arryaOfParams);
        }

        public async Task<MySqlResult> InsertAutomaticAIAsync(object[] values)
        {
            string allcolumns = "";
            string allNameParam = "";
            string[] names = tableForData.Columns.ToArray();

            for (int i = 0; i < ColumnSize; i++)
            {
                if (i != 0)
                {
                    if (i != ColumnSize - 1)
                    {
                        allcolumns += names[i] + ",";
                        allNameParam += $"?{names[i]},";
                    }
                    else
                    {
                        allcolumns += names[i];
                        allNameParam += $"?{names[i]}";
                    }
                }
            }

            string insert = $"INSERT INTO {tableForData.Name}({allcolumns}) VALUES({allNameParam})";

            MySqlParameter[] arryaOfParams = new MySqlParameter[ColumnSize - 1];

            for (int i = 0; i < ColumnSize - 1; i++)
            {
                if (tableForData.Columns.Contains(names[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(names[i + 1], values[i]);
                }
            }

            return await tableForData.ExecuteNotQueryAsync(insert, arryaOfParams);
        }

        public MySqlResult InsertAutomatic(object[] values)
        {
            string allcolumns = "";
            string allNameParam = "";
            string[] names = tableForData.Columns.ToArray();
            //tableForData.Columns.ForEach(x => allcolumns += x + ",");
            //tableForData.Column.ForEach(x => allNameParam += $"?{x},");

            for (int i = 0; i < ColumnSize; i++)
            {
                if (i != ColumnSize - 1)
                {
                    allcolumns += names[i] + ",";
                    allNameParam += $"?{names[i]},";
                }
                else
                {
                    allcolumns += names[i];
                    allNameParam += $"?{names[i]}";
                }
            }

            string insert = $"INSERT INTO {tableForData.Name}({allcolumns}) VALUES({allNameParam})";

            MySqlParameter[] arryaOfParams = new MySqlParameter[ColumnSize];

            for (int i = 0; i < ColumnSize; i++)
            {
                if (tableForData.Columns.Contains(names[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(names[i], values[i]);
                }
            }

            return tableForData.ExecuteNotQuery(insert, arryaOfParams);
        }

        public async Task<MySqlResult> DeleteRowPrimaryKeyAsync(string key)
        {
            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"DELETE FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";

            return await tableForData.ExecuteNotQueryAsync(query, new MySqlParameter[] { });
        }

        public async Task<MySqlResult<T>> DeleteRowFieldKeyAsync<T>(string fieldKey, string fieldValue) where T : IDBSet
        {
            if (fieldKey == string.Empty)
            {
                return new MySqlResult<T>()
                {
                    Error = RSACommon.Error.MYSQL_FIELD_KEY_MISSING,
                    Message = "Field key is missing",
                    Executed = ""
                };
            }

            string query = $"DELETE FROM {tableForData.Name} WHERE {fieldKey} = '{fieldValue}'";

            return await tableForData.ExecuteQueryAsync<T>(query);
        }

        public MySqlResult DeleteRowPrimaryKey(string key)
        {
            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"DELETE FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";

            return tableForData.ExecuteNotQuery(query, new MySqlParameter[] { });
        }

        /// <summary>
        /// Get data from primary key filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<MySqlResult<U>> SelectByPrimaryKeyAsync<U>(string key) where U : IDBSet
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";

            if (tableForData.PrimaryKey == string.Empty)
            {
                return await Task.Run(() => new MySqlResult<U>()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                });
            }

            string query = $"SELECT * FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";

            return await tableForData.ExecuteQueryAsync<U>(query);
        }

        public MySqlResult UpdateAutomaticPrimaryKey(string key, string[] paramToUpdate, object[] values)
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";
            string allcolumns = "";

            for (int i = 0; i < paramToUpdate.Count(); i++)
            {
                allcolumns += $"{paramToUpdate[i]}=@{paramToUpdate[i]}";

                if (i != paramToUpdate.Count() - 1)
                {
                    allcolumns += ",";
                }
            }

            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"UPDATE {tableForData.Name} SET {allcolumns} WHERE {tableForData.PrimaryKey} = '{key}'";

            MySqlParameter[] arryaOfParams = new MySqlParameter[paramToUpdate.Count()];

            for (int i = 0; i < values.Count(); i++)
            {
                if (tableForData.Columns.Contains(paramToUpdate[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(paramToUpdate[i], values[i]);
                }
            }

            return tableForData.ExecuteNotQuery(query, arryaOfParams);
        }

        /// <summary>
        /// Get data from primary key filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MySqlResult<T> SelectByPrimaryKey<T>(string key) where T : IDBSet
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";

            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult<T>()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"SELECT * FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";

            var result = tableForData.ExecuteQuery<T>(query);

            return result;
        }



        /// <summary>
        /// Get data from field key filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MySqlResult<T> SelectByFieldKey<T>(string fieldKey, string fieldValue) where T : IDBSet
        {
            if (fieldKey == string.Empty)
            {
                return new MySqlResult<T>()
                {
                    Error = RSACommon.Error.MYSQL_FIELD_KEY_MISSING,
                    Message = "Field key is missing",
                    Executed = ""
                };
            }

            string query = $"SELECT * FROM {tableForData.Name} WHERE {fieldKey} = '{fieldValue}'";

            var result = tableForData.ExecuteQuery<T>(query);

            return result;
        }

        //        SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate
        //FROM Orders
        //INNER JOIN Customers ON Orders.CustomerID= Customers.CustomerID;

        public MySqlResult<T> ExecuteQueryString<T>(string query) where T : IDBSet
        {
            return tableForData.ExecuteQuery<T>(query);
        }



        /// <summary>
        /// Get data from field key filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<MySqlResult<T>> SelectByFieldKeyAsync<T>(string fieldKey, string fieldValue) where T : IDBSet
        {
            if (fieldKey == string.Empty)
            {
                return new MySqlResult<T>()
                {
                    Error = RSACommon.Error.MYSQL_FIELD_KEY_MISSING,
                    Message = "Field key is missing",
                    Executed = ""
                };
            }

            string query = $"SELECT * FROM {tableForData.Name} WHERE {fieldKey} = '{fieldValue}'";

            return await tableForData.ExecuteQueryAsync<T>(query);
        }



        public async Task<MySqlResult<T>> SelectAllAsync<T>() where T : IDBSet
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";

            if (tableForData.PrimaryKey == string.Empty)
            {
                return await Task.Run(() => new MySqlResult<T>()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                });
            }

            string query = $"SELECT * FROM {tableForData.Name}";

            return await tableForData.ExecuteQueryAsync<T>(query);
        }

        public MySqlResult<U> SelectAll<U>() where U : IDBSet
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";

            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult<U>()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"SELECT * FROM {tableForData.Name}";

            return tableForData.ExecuteQuery<U>(query); ;
        }
    }
}
