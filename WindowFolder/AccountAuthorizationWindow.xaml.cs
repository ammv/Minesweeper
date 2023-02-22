using Minesweeper.ClassFolder;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper.WindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AccountAuthorization.xaml
    /// </summary>
    public partial class AccountAuthorization : Window
    {
        public AccountAuthorization()
        {
            InitializeComponent();
        }

        private async void Authorization_Click(object sender, RoutedEventArgs e)
        {
            int result = await DataBaseManager.ContainsAccount(EmailTB.Text.Trim(), PasswordPB.Password.Trim());
            if (result != -1)
            {
                object[] data = await DataBaseManager.GetAccountData(EmailTB.Text, PasswordPB.Password);
                GameWindow.UserAccount.FromObject(data);
                GameWindow.UserAccount.Error = AccountErrorStatus.Ok;
                GameWindow.UserAccount.Status = AccountStatus.Complete;
                MessageBox.Show("Авторизация прошла успешно");
                await AccountManager.CreateSessionFile(GameWindow.UserAccount.SessionID);
                Close();
            }
            else
                MessageBox.Show("Неправильная почта или пароль");
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            new AccountRegistration().Show();
            Close();
        }

        private int GetGridIndex(int rowProperty)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                if ((int)((Grid)grid.Children[i]).GetValue(Grid.RowProperty) == rowProperty)
                    return i;
            }
            return -1;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            int gridIndex = GetGridIndex((int)((Control)sender).Parent.GetValue(Grid.RowProperty));
            if (gridIndex != -1)
            {
                var uiElementCollection = ((Grid)grid.Children[gridIndex]).Children;
                {
                    if (uiElementCollection[1] is Label lbl) lbl.Visibility = Visibility.Hidden;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Visibility visibleMode = Visibility.Visible;

            int gridIndex = GetGridIndex((int)((Control)sender).Parent.GetValue(Grid.RowProperty));

            if (gridIndex == -1) return;

            Label lbl = (Label)((Grid)grid.Children[gridIndex]).Children[1];

            lbl.Visibility = Visibility.Hidden;

            if (sender is TextBox tb)
            {
                if (tb.Text.Length != 0)
                    visibleMode = Visibility.Hidden;
            }
            else
            if (((PasswordBox)sender).Password.Length != 0)
            {
                visibleMode = Visibility.Hidden;
            }

            lbl.Visibility = visibleMode;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grid.Focus();
        }
    }
}