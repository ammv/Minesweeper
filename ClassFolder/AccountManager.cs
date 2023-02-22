using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper.ClassFolder
{
    /// <summary>
    ///     Manages Accounts: getting, creating, changing
    /// </summary>
    internal class AccountManager
    {
        private static readonly string SessionIDFile = ".session";

        public static async Task LoadAccount(Account account)
        {
            account.Status = AccountStatus.CheckingInternetConnection;

            var hasInternet = await Task.Run(Utils.CheckInternetConnection);

            if (hasInternet)
            {
                account.Status = AccountStatus.OpeningSessionIDFile;

                string SessionID = await Task.Run(OpenSessionFile);

                if (SessionID != null)
                {
                    account.Status = AccountStatus.Filling;
                    await FillAccount(SessionID, account);
                }
                else
                {
                    account.Error = AccountErrorStatus.IncorrectSessionIDFile;
                }
            }
            else
            {
                account.Error = AccountErrorStatus.NoInternetConnection;
            }
        }

        public static async Task CreateSessionFile(string sessionID)
        {
            using (var sw = new StreamWriter(SessionIDFile, false))
            {
                await sw.WriteAsync(sessionID);
            }
        }

        public static string OpenSessionFile()
        {
            string SessionID = null;
            if (File.Exists(SessionIDFile))
            {
                var isCorrectFileContent = true;
                using (var sr = new StreamReader(SessionIDFile))
                    SessionID = sr.ReadToEnd();

                if (!isCorrectFileContent)
                    File.Delete(SessionIDFile);
            }

            return SessionID;
        }

        private static async Task FillAccount(string SessionID, Account account)
        {
            if (SessionID != null)
            {
                account.Status = AccountStatus.DatabaseQuery;

                var data = await DataBaseManager.GetAccountDataBySessionID(SessionID);

                if (data != null)
                {
                    account.FromObject(data);
                    account.Error = AccountErrorStatus.Ok;
                    account.Status = AccountStatus.Complete;
                    return;
                }
                else
                    account.Error = AccountErrorStatus.NotExists;
            }
            else
                account.Error = AccountErrorStatus.IncorrectSessionIDFile;
        }
    }
}