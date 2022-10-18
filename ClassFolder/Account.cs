using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minesweeper.ClassFolder
{
    /// <summary>
    /// Statuses of loading Account
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// An object of the Account class has been created and initialized, but not filled in
        /// </summary>
        [Description("Initializing")]
        Initialized,

        /// <summary>
        /// Check internet connection
        /// </summary>
        [Description("Checks for internet availability")]
        CheckingInternetConnection,

        /// <summary>
        /// Open file with user data to login
        /// </summary>
        [Description("Opens user.data file")]
        OpeningDataFile,

        /// <summary>
        /// Filling out the Account
        /// </summary>
        [Description("Uploads account data")]
        Filling,

        /// <summary>
        /// Account loaded from data base succesfully
        /// </summary>
        [Description("Successful login to the account")]
        Complete,

        /// <summary>
        /// Checks the existence of the Account in the database
        /// </summary>
        [Description("Makes a request to the database")]
        DatabaseQuery,

        /// <summary>
        /// Loads Account data from the database
        /// </summary>
        [Description("Retrieves data from the database")]
        LoadsData
    }

    /// <summary>
    /// Error statuses of Account loading
    /// </summary>
    public enum AccountErrorStatus
    {
        /// <summary>
        /// Data file exists and have inccorect content or data file not exists
        /// </summary>
        IncorrectDataFile,

        /// <summary>
        /// Data file exist but Account not exists in database
        /// </summary>
        NotExists,

        /// <summary>
        /// Not have internet connection
        /// </summary>
        NoInternetConnection,

        /// <summary>
        /// No errors
        /// </summary>
        Ok,

        /// <summary>
        /// Connection to the database failed
        /// </summary>
        DataBaseConnectionFailed
    }

    public static class EnumExtensions
    {
        /// <summary>
        /// This extension method is broken out so you can use a similar pattern with
        /// other MetaData elements in the future. This is your base method for each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>Attribute</returns>

        public static T GetAttribute<T>(this AccountStatus value) where T : DescriptionAttribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }

        /// <summary>
        /// This method creates a specific call to the above method, requesting the
        /// Description MetaData attribute.
        /// </summary>
        public static string ToName(this AccountStatus value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }

    /// <summary>
    /// Contains information about user
    /// </summary>
    public class Account : INotifyPropertyChanged
    {
        private AccountStatus status;
        private AccountErrorStatus error;
        private int id;
        private string nickname;
        private string email;
        private string password;

        public AccountErrorStatus Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged("Error");
            }
        }

        public AccountStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string StatusString
        {
            get => status.ToString();
        }

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        public string Nickname
        {
            get => nickname;
            set
            {
                nickname = value;
                OnPropertyChanged("Nickname");
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public Account()
        {
            Status = AccountStatus.Initialized;
        }

        public override string ToString()
        {
            return string.Format(
                "ID: {0}\n" +
                "Nickname: {1}\n" +
                "Mail: {2}\n" +
                "Password: {3}",
                ID, Nickname, Email, Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}