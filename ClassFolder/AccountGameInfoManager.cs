using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper.ClassFolder
{
    internal class AccountGameInfoManager
    {
        public static Account userAccount { get; set; }

        private static int CalculateExperience(GameInfo gameInfo)
        {
            if (gameInfo.IsWin == false)
                return 0;

            double ratio = 2.7;
            if (gameInfo.IsFlagStyle)
                ratio += 0.5;

            double result = gameInfo.FieldSize
                / 100 + gameInfo.FieldSize
                / gameInfo.TimeInSeconds * ratio;

            return (int)result;
        }

        public static void Update(GameInfo gameInfo)
        {
            if (userAccount != null && userAccount.Status == AccountStatus.Complete)
            {
                AddGameCount(userAccount.Statistic.Games + 1);

                int calculatedExperience = CalculateExperience(gameInfo);
                gameInfo.Experience = calculatedExperience;

                AddExperience(userAccount.Rank.Experience + calculatedExperience);

                AddWinLoss(gameInfo.IsWin);

                AddGameTime(userAccount.Statistic.TimeInGame + gameInfo.TimeInSeconds);

                RefreshStatistics();
            }
        }

        public static async void AddExperience(int experience)
        {
            if (userAccount.Status == AccountStatus.Complete)
                if (experience != userAccount.Rank.Experience)
                {
                    object[] result = await DataBaseManager.UpdateAccountRank(experience,
                        userAccount.SessionID);

                    if (result != null)
                        userAccount.Rank.FromObject(result);
                }
        }

        public static async void AddWinLoss(bool isWin)
        {
            if (userAccount.Status == AccountStatus.Complete)
            {
                string columnName;
                int countWinLoss;
                if (isWin)
                {
                    columnName = "Wins";
                    countWinLoss = userAccount.Statistic.Wins + 1;
                }
                else
                {
                    columnName = "Loss";
                    countWinLoss = userAccount.Statistic.Loss + 1;
                }

                await DataBaseManager.UpdateAccountStatistics(columnName, countWinLoss,
                    userAccount.SessionID);
            }
        }

        public static async void AddGameTime(int timeInSeconds)
        {
            if (userAccount.Status == AccountStatus.Complete)
            {
                await DataBaseManager.UpdateAccountStatistics("TimeInGame", timeInSeconds,
                    userAccount.SessionID);
            }
        }

        public static async void AddGameCount(int gamesCount)
        {
            if (userAccount.Status == AccountStatus.Complete)
            {
                await DataBaseManager.UpdateAccountStatistics("Games", gamesCount,
                    userAccount.SessionID);
            }
        }

        public static async void RefreshStatistics()
        {
            if (userAccount.Status == AccountStatus.Complete)
            {
                object[] result = await DataBaseManager.GetAccountStatistics(
                    userAccount.SessionID);

                if (result != null)
                    userAccount.Statistic.FromObject(result);

                //userAccount.Statistic.FromObject(result);
            }
        }
    }
}