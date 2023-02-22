using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.ClassFolder
{
    internal class Utils
    {
        public static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace("-", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool CheckInternetConnection()
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

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static string TimeInSecondsToString(int seconds)
        {
            int minutes = seconds / 60;
            int hours = minutes / 60;

            if (minutes == 0 && hours == 0)
                return $"{seconds}s";
            else if (minutes != 0 && hours == 0)
                return $"{minutes}m";
            else if (minutes != 0 && hours != 0)
                return $"{hours}h {minutes}m";
            else
                return $"{hours}h";
        }
    }
}