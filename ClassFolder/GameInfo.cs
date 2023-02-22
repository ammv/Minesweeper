using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.ClassFolder
{
    public class GameInfo
    {
        public int FieldSize;
        public int TimeInSeconds;
        public int Experience { get; set; }

        public bool IsWin;
        public bool IsFlagStyle;

        public string IsWinString
        {
            get => IsWin == true ? "Win" : "Loss";
        }

        public string TimeString
        {
            get => Utils.TimeInSecondsToString(TimeInSeconds);
        }

        public GameInfo(int fieldSize, int timeInSeconds, bool isWin, bool isFlagStyle)
        {
            FieldSize = fieldSize;
            TimeInSeconds = timeInSeconds;
            IsFlagStyle = isFlagStyle;
            IsWin = isWin;
        }
    }
}