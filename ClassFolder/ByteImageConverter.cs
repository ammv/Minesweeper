using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Data.SqlTypes;

namespace Minesweeper.ClassFolder
{
    public class ByteImageConverter
    {
        public static ImageSource ImageSourceFromBytes(SqlBytes imageData)
        {
            BitmapImage biImg = new BitmapImage();
            biImg.BeginInit();
            if (imageData.IsNull)
                biImg.UriSource = new Uri(@"SourceFolder/Images/no-pictures.png", UriKind.Relative);
            else
                biImg.StreamSource = new MemoryStream(imageData.Value);
            biImg.EndInit();

            return biImg;
        }

        public static ImageSource ImageSourceFromBytes(byte[] bytes)
        {
            BitmapImage biImg = new BitmapImage();
            biImg.BeginInit();
            biImg.StreamSource = new MemoryStream(bytes);
            biImg.EndInit();

            return biImg;
        }

        public static byte[] ImageToByteArray(string imagefilePath)
        {
            Image image = Image.FromFile(imagefilePath);
            byte[] imageByte = ImageToByteArraybyMemoryStream(image);
            return imageByte;
        }

        public static byte[] ImageToByteArraybyMemoryStream(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}