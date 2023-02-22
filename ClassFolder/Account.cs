using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

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
        OpeningSessionIDFile,

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
        IncorrectSessionIDFile,

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
        DataBaseConnectionFailed,

        /// <summary>
        /// 7 days have passed since the session was created
        /// </summary>
        SessionExpired
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

    public class AccountRank : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private int experience;
        private int experienceToNewRank;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Experience
        {
            get => experience;
            set
            {
                experience = value;
                OnPropertyChanged("CurrentExperience");
                //OnPropertyChanged("ExperienceString");
            }
        }

        public int ExperienceToNewRank
        {
            get => experienceToNewRank;
            set
            {
                experienceToNewRank = value;
                OnPropertyChanged("ExperienceToNewRank");
                //OnPropertyChanged("ExperienceString");
            }
        }

        public string ExperienceString
        {
            get => $"{experience}/{experienceToNewRank}";
        }

        public void FromObject(object[] data)
        {
            if (data != null)
            {
                id = (int)data[0];
                name = (string)data[1];
                experience = (int)data[2];
                ExperienceToNewRank = (int)data[3];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    /// <summary>
    /// Contains statistic information
    /// </summary>
    public class AccountStatistic : INotifyPropertyChanged
    {
        private int games;
        private int wins;
        private int loss;
        private int timeInGame;
        private int longestGame;
        private int fastestGame;

        public int Games
        {
            get => games;
            set
            {
                games = value;
                OnPropertyChanged("Games");
            }
        }

        public int Wins
        {
            get => wins;
            set
            {
                wins = value;
                OnPropertyChanged("Wins");
            }
        }

        public int Loss
        {
            get => loss;
            set
            {
                loss = value;
                OnPropertyChanged("Loss");
            }
        }

        public int TimeInGame
        {
            get => timeInGame;
            set
            {
                timeInGame = value;
                OnPropertyChanged("TimeInGame");
            }
        }

        public int LongestGame
        {
            get => longestGame;
            set
            {
                longestGame = value;
                OnPropertyChanged("LongestGame");
            }
        }

        public int FastestGame
        {
            get => fastestGame;
            set
            {
                fastestGame = value;
                OnPropertyChanged("FastestGame");
            }
        }

        public void FromObject(object[] data)
        {
            if (data != null)
            {
                games = (int)data[0];
                wins = (int)data[1];
                loss = (int)data[2];
                timeInGame = (int)data[3];
                longestGame = (int)data[4];
                fastestGame = (int)data[5];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
        private ImageSource avatar;
        private AccountStatistic statistic;
        private AccountRank rank;
        private string sessionID;

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

        public ImageSource Avatar
        {
            get => avatar;
            set
            {
                avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        public string SessionID
        {
            get => sessionID;
        }

        public AccountStatistic Statistic
        {
            get => statistic;
            set
            {
                statistic = value;
                OnPropertyChanged("Statistic");
            }
        }

        public AccountRank Rank
        {
            get => rank;
            set
            {
                rank = value;
                OnPropertyChanged("Rank");
            }
        }

        public Account()
        {
            Status = AccountStatus.Initialized;
        }

        public async void FromObject(object[] data)
        {
            if (data != null)
            {
                ID = (int)data[0];
                Nickname = (string)data[1];
                Avatar = (ImageSource)data[2];
                sessionID = (string)data[3];

                statistic = new AccountStatistic();
                rank = new AccountRank();

                statistic.FromObject(await DataBaseManager.GetAccountStatistics(sessionID));
                rank.FromObject(await DataBaseManager.GetAccountRank(sessionID));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}