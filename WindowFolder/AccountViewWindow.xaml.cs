using System;
using System.Windows;
using System.Drawing;
using Minesweeper.ClassFolder;

namespace Minesweeper.WindowFolder
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class AccountViewWindow : Window
    {
        private GameWindow mainWindow;

        public AccountViewWindow(GameWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            DataContext = GameWindow.UserAccount;
            ExperienceLbl.Content = GameWindow.UserAccount.Rank.ExperienceString;
        }

        private async void RoundAvatar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension
                DefaultExt = ".png",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg"
            };

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                byte[] bytes = null;
                if (new System.IO.FileInfo(dlg.FileName).Length > 256 * 256)
                {
                    if (MessageBoxResult.Yes == MessageBox.Show(
                        "Изображение будет уменьшено до 64КБ. Уменьшить?", "Большой размер изображения",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        Image img = Image.FromFile(dlg.FileName);
                        Image scaledImage = Utils.ScaleImage(img, 256, 256);
                        bytes = ByteImageConverter.ImageToByteArraybyMemoryStream(scaledImage);
                    }
                }
                else
                    bytes = ByteImageConverter.ImageToByteArray(dlg.FileName);

                if (bytes != null)
                {
                    await DataBaseManager.UpdateAccountAvatar(GameWindow.UserAccount.SessionID, bytes);
                    //MessageBox.Show((bytes.Length / 1024).ToString() + "КБ");
                    GameWindow.UserAccount.Avatar = ByteImageConverter.ImageSourceFromBytes(bytes);
                    mainWindow.UpdateOpenAccountImg(ByteImageConverter.ImageSourceFromBytes(bytes));
                    //((SourceFolder.UserControls.RoundAvatar)sender).AvatarSource =;
                }
            }
        }
    }
}