using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public override string ToString()
        {
            return $"name: {name}\nvalue: {value}\ntype: {type}\nsize: {size}";
        }
    }

    internal class DataBaseManager
    {
        private static readonly string connString = @"
            Data Source=DESKTOP-0AEC5RH\SQLEXPRESS;
            Initial Catalog=Minesweeper;
            Integrated Security=True;
            User ID=artem;password=19731982dd;
            ";

        //private static readonly SqlConnection conn = new SqlConnection(connString);

        /// <summary>
        /// Adding parameters to SqlCommand.Parameters
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        public static void AddParameters(SqlCommand cmd, params Parameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                //MessageBox.Show(parameter.ToString());
                SqlParameter sqlParameter = new SqlParameter
                {
                    ParameterName = parameter.name,
                    SqlDbType = parameter.type,
                    Size = parameter.size
                };
                sqlParameter.Value = parameter.value;
                cmd.Parameters.Add(sqlParameter);
            }
            //SqlParameter x = new SqlParameter("@add", SqlDbType.BigInt, 4, 35);
        }

        public static async Task<bool> ContainsAccountData(string data, char dataType)
        {
            bool hasRows = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand containsAccountData = new SqlCommand()
                    {
                        Connection = conn,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = $"ContainsAccountData"
                    };

                    containsAccountData.Parameters.Add("@data", SqlDbType.NVarChar, 1).Value = data;
                    containsAccountData.Parameters.Add("@dataType", SqlDbType.Char, 1).Value = dataType;

                    object result = await containsAccountData.ExecuteScalarAsync();
                    hasRows = result != null;
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "ContainsAccountData. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return hasRows;
        }

        public static async Task<object[]> GetAccountData(string email, string password)
        {
            object[] data = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand getAccountData = new SqlCommand()
                    {
                        Connection = conn,
                        CommandText = "GetAccountData",
                        CommandType = CommandType.StoredProcedure
                    };

                    AddParameters(getAccountData,
                        new Parameter("email", email, SqlDbType.NVarChar, 100),
                        new Parameter("password", password, SqlDbType.NVarChar, 30),
                        new Parameter("createSession", 1, SqlDbType.Bit, 1),
                        new Parameter("ID", -1, SqlDbType.Int, 4)
                    );

                    SqlDataReader reader = await getAccountData.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            data = new object[4];

                            data[0] = reader.GetInt32(0);
                            data[1] = reader.GetString(1);
                            data[2] = ByteImageConverter.ImageSourceFromBytes(reader.GetSqlBytes(2));
                            data[3] = BitConverter.ToString(reader.GetSqlBytes(3).Value);
                        }
                    }

                    if (reader != null) reader.Close();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "GetAccountData. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return data;
        }

        public static async Task<object[]> GetAccountDataBySessionID(string sessionID)
        {
            object[] data = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand getAccountDataBySessionID = new SqlCommand()
                    {
                        Connection = conn,
                        CommandText = "GetAccountDataBySessionID",
                        CommandType = CommandType.StoredProcedure
                    };

                    AddParameters(getAccountDataBySessionID,
                        new Parameter("SessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length)
                    );

                    SqlDataReader reader = await getAccountDataBySessionID.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.FieldCount == 1)
                                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.SessionExpired;
                            else
                            {
                                data = new object[4];

                                data[0] = reader.GetInt32(0);
                                data[1] = reader.GetString(1);
                                data[2] = ByteImageConverter.ImageSourceFromBytes(reader.GetSqlBytes(2));
                                data[3] = sessionID;
                            }
                        }
                    }

                    if (reader != null) reader.Close();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "getAccountDataBySessionID. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return data;
        }

        public static async Task<object[]> GetAccountStatistics(string sessionID)
        {
            object[] data = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand getAccountStatistics = new SqlCommand()
                    {
                        Connection = conn,
                        CommandText = "GetAccountStatistics",
                        CommandType = CommandType.StoredProcedure
                    };

                    AddParameters(getAccountStatistics,
                        new Parameter("SessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length)
                    );

                    SqlDataReader reader = await getAccountStatistics.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.FieldCount == 1)
                                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.SessionExpired;
                            else
                            {
                                data = new object[6];

                                data[0] = reader.GetInt32(1);
                                data[1] = reader.GetInt32(2);
                                data[2] = reader.GetInt32(3);
                                data[3] = reader.GetInt32(4);
                                data[4] = reader.GetInt32(5);
                                data[5] = reader.GetInt32(6);
                            }
                        }
                    }
                    if (reader != null) reader.Close();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "GetAccountStatistic. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public static async Task<int> ContainsAccount(string email, string password)
        {
            int ID = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand containsAccount = new SqlCommand
                    {
                        Connection = conn,
                        CommandType = CommandType.StoredProcedure
                    };
                    containsAccount.CommandText = "ContainsAccount";
                    AddParameters(containsAccount,
                        new Parameter("email", email.Trim(), SqlDbType.NVarChar, 100),
                        new Parameter("password", password.Trim(), SqlDbType.NVarChar, 30));

                    object result = await containsAccount.ExecuteScalarAsync();
                    //MessageBox.Show("ContainsAccount result is null: " + (result == null).ToString());
                    if (result != null)
                        ID = (int)result;
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "ContainsAccount. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ID;
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
            int x = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
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
                    x = await addAccount.ExecuteNonQueryAsync();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "AddAccount. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return x;
        }

        public static async Task<int> UpdateAccountAvatar(string sessionID, byte[] avatar)
        {
            int x = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    SqlCommand updateAccountAvatar = new SqlCommand()
                    {
                        CommandText = "UpdateAccountAvatar",
                        CommandType = CommandType.StoredProcedure,
                        Connection = conn
                    };
                    AddParameters(updateAccountAvatar,
                        new Parameter("SessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length),
                        new Parameter("avatar", avatar, SqlDbType.VarBinary, avatar.Length)
                    );
                    x = await updateAccountAvatar.ExecuteNonQueryAsync();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "UpdateAccountAvatar. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return x;
        }

        public static async Task<bool> UpdateAccountStatistics(string columnName, int newValue, string sessionID)
        {
            bool isSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    SqlCommand updateAccountStatistics = new SqlCommand()
                    {
                        CommandText = "UpdateAccountStatistics",
                        CommandType = CommandType.StoredProcedure,
                        Connection = conn
                    };
                    AddParameters(updateAccountStatistics,
                        new Parameter("columnName", columnName, SqlDbType.NVarChar, columnName.Length),
                        new Parameter("newValue", newValue, SqlDbType.Int, 4),
                        new Parameter("SessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length)
                    );
                    int result = await updateAccountStatistics.ExecuteNonQueryAsync();
                    isSuccess = result != 0;
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "UpdateAccountStatistics. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return isSuccess;
        }

        public static async Task<object[]> GetAccountRank(string sessionID)
        {
            object[] data = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    SqlCommand getAccountRank = new SqlCommand()
                    {
                        CommandText = "GetAccountRank",
                        CommandType = CommandType.StoredProcedure,
                        Connection = conn
                    };
                    AddParameters(getAccountRank,
                        new Parameter("sessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length)
                    );
                    SqlDataReader reader = await getAccountRank.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            data = new object[4];

                            data[0] = reader.GetInt32(0);
                            data[1] = reader.GetString(1);
                            data[2] = reader.GetInt32(2);
                            data[3] = reader.GetInt32(3);
                        }
                    }

                    if (reader != null) reader.Close();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "GetAccountRank. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return data;
        }

        public static async Task<object[]> UpdateAccountRank(int newExperience, string sessionID)
        {
            object[] data = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    SqlCommand updateAccountRank = new SqlCommand()
                    {
                        CommandText = "UpdateAccountRank",
                        CommandType = CommandType.StoredProcedure,
                        Connection = conn
                    };
                    AddParameters(updateAccountRank,
                        new Parameter("newExperience", newExperience, SqlDbType.Int, 4),
                        new Parameter("sessionID", Utils.StringToByteArray(sessionID), SqlDbType.VarBinary, sessionID.Length)
                    );
                    SqlDataReader reader = await updateAccountRank.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            data = new object[4];

                            data[0] = reader.GetInt32(0);
                            data[1] = reader.GetString(1);
                            data[2] = reader.GetInt32(2);
                            data[3] = reader.GetInt32(3);
                        }
                    }

                    if (reader != null) reader.Close();
                }
            }
            catch (Exception err)
            {
                WindowFolder.GameWindow.UserAccount.Error = AccountErrorStatus.DataBaseConnectionFailed;
                MessageBox.Show(err.Message, "UpdateAccountRank. Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return data;
        }
    }
}