using Minesweeper.ClassFolder;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper.WindowFolder
{
    /// <summary>
    /// Логика взаимодействия для AccountRegistration.xaml
    /// </summary>
    public partial class AccountRegistration : Window
    {
        public AccountRegistration()
        {
            InitializeComponent();
        }

        private int GetGridIndex(int rowProperty)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                if ((int)(grid.Children[i] as Grid).GetValue(Grid.RowProperty) == rowProperty)
                    return i;
            }
            return -1;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            int gridIndex = GetGridIndex((int)(sender as Control).Parent.GetValue(Grid.RowProperty));
            if (gridIndex != -1)
            {
                Label lbl = (grid.Children[gridIndex] as Grid).Children[1] as Label;
                lbl.Visibility = Visibility.Hidden;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Visibility visibleMode = Visibility.Visible;
            int gridIndex = GetGridIndex((int)(sender as Control).Parent.GetValue(Grid.RowProperty));
            if (gridIndex != -1)
            {
                Label lbl = (grid.Children[gridIndex] as Grid).Children[1] as Label;
                lbl.Visibility = Visibility.Hidden;
                if (sender is TextBox tb)
                {
                    if (tb.Text.Length != 0)
                        visibleMode = Visibility.Hidden;
                }
                else
               if ((sender as PasswordBox).Password.Length != 0)
                    visibleMode = Visibility.Hidden;
                lbl.Visibility = visibleMode;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            grid.Focus();
        }

        private async Task<bool> CheckNickname()
        {
            return NicknameTB.Text.Length >= 4 && await DataBaseManager.ContainsData(NicknameTB.Text, 'N');
        }

        private async Task<bool> CheckMail()
        {
            return await DataBaseManager.ContainsData(MailTB.Text, 'E');
        }

        private bool CheckPassword()
        {
            return PasswordPB.Password.Length >= 8;
        }

        private bool CheckPasswordRepeat()
        {
            return PasswordRepeatPB.Password == PasswordPB.Password;
        }

        private bool IsValidEmail()
        {
            var email = MailTB.Text;
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckNickname())
                MessageBox.Show("Данный ник уже занят");
            else if (!IsValidEmail())
                MessageBox.Show("Некорректный адрес почты");
            else if (await CheckMail())
                MessageBox.Show("Данная почта уже занята");
            else if (!CheckPassword())
                MessageBox.Show("Пароль должен быть не менее 8 символов");
            else if (!CheckPasswordRepeat())
                MessageBox.Show("Пароли не совпадают");
            else
            {
                await DataBaseManager.AddAccount(NicknameTB.Text, MailTB.Text, PasswordPB.Password);
                MessageBox.Show("Успешная регистрация");
                new AccountAuthorization().Show();
                Close();
            }
        }
    }
}