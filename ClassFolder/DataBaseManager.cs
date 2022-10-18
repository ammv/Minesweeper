using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper.ClassFolder
{
    internal struct Parameter
    {
        public string name;
        public object value;
        public SqlDbType type;
        public int size;

        public Parameter(string name, object value, SqlDbType type, int size)
        {
            this.name = "@" + name;
            this.value = value;
            this.type = type;
            this.size = size;
        }
    }

    internal class DataBaseManager
    {
        private static readonly string connString = @"
            Data Source=DESKTOP-0AEC5RH\SQLEXPRESS;
            Initial Catalog=Minesweeper;
            Integrated Security=True";

        private static readonly SqlConnection conn = new SqlConnection(connString);

        /// <summary>
        /// Adding parameters to SqlCommand.Parameters
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        public static void AddParameters(SqlCommand cmd, params Parameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                SqlParameter sqlParameter = new SqlParameter
                {
                    ParameterName = parameter.name,
                    SqlDbType = parameter.type,
                    Size = parameter.size
                };
                sqlParameter.Value = parameter.value;
                cmd.Parameters.Add(sqlParameter);
            }
        }

        public static async Task<bool> ContainsData(string data, char dataType)
        {
            bool hasRows = false;
            try
            {
                await conn.OpenAsync();

                SqlCommand containsData = new SqlCommand()
                {
                    Connection = conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $"ContainsData"
                };
                AddParameters(containsData,

                    new Parameter("data", data, SqlDbType.NVarChar, data.Length),
                    new Parameter("dataType", dataType, SqlDbType.Char, 1));

                object result = await containsData.ExecuteScalarAsync();
                hasRows = result != null;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ContainsData. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
            return hasRows;
        }

        public static async Task<string> GetAccountEncryptedData(string nickname)
        {
            string encryptedData = null;
            try
            {
                await conn.OpenAsync();

                SqlCommand getEncryptedDataAccount = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = "GetAccountEncryptedData",
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(getEncryptedDataAccount,
                    new Parameter("nickname", nickname, SqlDbType.NVarChar, 20)
                );

                SqlDataReader reader = await getEncryptedDataAccount.ExecuteReaderAsync();

                object[] data = new object[2];

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data[0] = reader.GetSqlBinary(0);
                        data[1] = reader.GetSqlBinary(1);
                    }
                }

                encryptedData = data[0].ToString() + '\n' + data[1].ToString();

                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GetAccountEncryptedData. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
            return encryptedData;
        }

        public static async Task<object[]> GetAccountData(string email)
        {
            object[] data = null;
            try
            {
                await conn.OpenAsync();

                SqlCommand getAccountData = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = "GetAccountData",
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(getAccountData,
                    new Parameter("email", email.Trim(), SqlDbType.NVarChar, 100)
                );

                SqlDataReader reader = await getAccountData.ExecuteReaderAsync();

                data = new object[4];

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data[0] = reader.GetInt32(0);
                        data[1] = reader.GetString(1);
                        data[2] = reader.GetSqlBinary(2);
                        data[3] = reader.GetSqlBinary(3);
                    }
                }

                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GetAccountData. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// checks the presence of a Account in the database
        /// </summary>
        /// <param name="email">Email of player</param>
        /// <param name="password">Password of player</param>
        /// <param name="isEncrypted">If Email and Password encrypted</param>
        /// <returns></returns>
        public static async Task<bool?> ContainsAccount(string email, string password, bool isEncrypted = false)
        {
            bool? read = false;
            try
            {
                await conn.OpenAsync();

                SqlCommand containsAccount = new SqlCommand
                {
                    Connection = conn,
                    CommandType = CommandType.StoredProcedure
                };

                if (isEncrypted)
                    containsAccount.CommandText = "ContainsAccountEncrypted";
                else
                    containsAccount.CommandText = "ContainsAccount";

                AddParameters(containsAccount,
                    new Parameter("email", email.Trim(), SqlDbType.NVarChar, 100),
                    new Parameter("password", password.Trim(), SqlDbType.NVarChar, 30));

                object result = await containsAccount.ExecuteScalarAsync();
                if (result != null)
                    read = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "ContainsAccount. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            finally
            {
                conn.Close();
            }
            return read;
        }

        /// <summary>
        /// Add Account into data base
        /// </summary>
        /// <param name="nickname">Nickname of player</param>
        /// <param name="email">Email of player</param>
        /// <param name="password">Password of player</param>
        /// <returns></returns>
        public static async Task<int> AddAccount(string nickname, string email, string password)
        {
            try
            {
                await conn.OpenAsync();
                SqlCommand addAccount = new SqlCommand()
                {
                    CommandText = "AddAccount",
                    CommandType = CommandType.StoredProcedure,
                    Connection = conn
                };
                AddParameters(addAccount,
                    new Parameter("nickname", nickname, SqlDbType.NVarChar, 20),
                    new Parameter("email", email, SqlDbType.NVarChar, 100),
                    new Parameter("password", password, SqlDbType.NVarChar, 30)
                );
                int x = await addAccount.ExecuteNonQueryAsync();
                conn.Close();
                return x;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "AddAccount. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
            return 0;
        }

        public static async Task<int> UpdateLastLogin(string nickname)
        {
            try
            {
                await conn.OpenAsync();
                SqlCommand updateLastLogin = new SqlCommand()
                {
                    CommandText = "UpdateLastLogin",
                    CommandType = CommandType.StoredProcedure,
                    Connection = conn
                };
                AddParameters(updateLastLogin,
                    new Parameter("nickname", nickname, SqlDbType.NVarChar, 20)
                );
                int x = await updateLastLogin.ExecuteNonQueryAsync();
                conn.Close();
                return x;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "UpdateLastLogin. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
            return 0;
        }
    }
}