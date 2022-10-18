using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper.ClassFolder
{
    /// <summary>
    ///     Contains user name and password from user.data file
    /// </summary>
    internal struct LoginData
    {
        public string Email;
        public string Password;

        public LoginData(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    /// <summary>
    ///     Manages Accounts: getting, creating, changing
    /// </summary>
    internal class AccountManager
    {
        private static readonly string DataFileName = "user.data";

        public static async Task LoadAccount(Account account)
        {
            account.Status = AccountStatus.CheckingInternetConnection;

            var hasInternet = await Task.Run(CheckInternetConnection);

            if (hasInternet)
            {
                account.Status = AccountStatus.OpeningDataFile;

                var loginData = await Task.Run(OpenDataFile);

                if (loginData != null)
                {
                    account.Status = AccountStatus.Filling;
                    await FillAccount(loginData, account, true);
                }
                else
                {
                    account.Error = AccountErrorStatus.IncorrectDataFile;
                }
            }
            else
            {
                account.Error = AccountErrorStatus.NoInternetConnection;
            }
        }

        public static void CreateDataFile(string email, string password)
        {
            using (var sw = new StreamWriter(DataFileName, false))
            {
                sw.WriteLine(email);
                sw.Write(password);
            }
        }

        private static LoginData? OpenDataFile()
        {
            LoginData? loginData = null;
            if (File.Exists(DataFileName))
            {
                var isCorrectFileContent = true;
                using (var sr = new StreamReader(DataFileName))
                {
                    var lines = sr.ReadToEnd().Split('\n');
                    if (lines.Length == 2)
                        loginData = new LoginData(lines[0], lines[1]);
                    else
                        isCorrectFileContent = false;
                }

                if (!isCorrectFileContent)
                    // Clear file
                    new StreamWriter(DataFileName, false).Close();
            }

            return loginData;
        }

        private static async Task FillAccount(LoginData? loginData, Account account, bool isEncrypted = false)
        {
            if (loginData != null)
            {
                account.Status = AccountStatus.DatabaseQuery;

                var result = await DataBaseManager.ContainsAccount(loginData.Value.Email, loginData.Value.Password, isEncrypted);
                if (result == true)
                {
                    account.Status = AccountStatus.LoadsData;
                    var data = await DataBaseManager.GetAccountData(loginData.Value.Email);

                    if (data != null)
                    {
                        account.ID = (int)data[0];
                        account.Nickname = (string)data[1];
                        account.Email = (string)data[2];
                        account.Password = (string)data[3];
                        account.Status = AccountStatus.Complete;
                        account.Error = AccountErrorStatus.Ok;

                        await DataBaseManager.UpdateLastLogin(account.Nickname);
                    }
                    else
                    {
                        account.Error = AccountErrorStatus.NotExists;
                    }
                }
                else
                {
                    account.Error = AccountErrorStatus.NotExists;
                }
            }
            else
            {
                account.Error = AccountErrorStatus.NotExists;
            }
        }

        private static bool CheckInternetConnection()
        {
            try
            {
                var pingSender = new Ping();
                var reply = pingSender.Send("www.google.com.mx");
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}