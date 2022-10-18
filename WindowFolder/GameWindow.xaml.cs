using Minesweeper.ClassFolder;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Minesweeper.WindowFolder
{
    /// <summary>
    /// Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public struct Mode
    {
        public int Width;
        public int Height;
        public int Mines;

        public Mode(int width, int height, int mines)
        {
            Width = width;
            Height = height;
            Mines = mines;
        }
    }

    public partial class GameWindow : Window
    {
        private static RoutedCommand _toStartMenuCommand;
        private readonly GameGridField gameGridField;
        private Mode[] gameModes;
        private int mode;
        public string Text { get; set; } = "Hello";
        public static Account UserAccount { get; set; }

        public GameWindow()
        {
            InitializeComponent();
            LoadAccount();
            SetBinding();
            SetMaxWindowSize();
            SetCommand();
            SetGameModes();
            gameGridField = new GameGridField(GameBoard, MinesLbl, TimeLbl, RestartSmile);
        }

        private void SetBinding()
        {
            Binding binding = new Binding
            {
                Source = UserAccount, // элемент-источник
                Path = new PropertyPath("Status"), // свойство элемента-источника
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                TargetNullValue = "Ошибка",
                Converter = new AccountStatusConverter()
            };
            //BindingOperations.SetBinding(AccountStateLbl, TextBox.TextProperty, binding);
            AccountStateLbl.SetBinding(Label.ContentProperty, binding);
        }

        private async void LoadAccount()
        {
            UserAccount = new Account();
            await AccountManager.LoadAccount(UserAccount);
        }

        private void SetCommand()
        {
            _toStartMenuCommand = new RoutedCommand();
            _toStartMenuCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding commandBind = new CommandBinding(_toStartMenuCommand, ToStartMenu);
            CommandBinding commandBind2 = new CommandBinding(_toStartMenuCommand, ToStartMenuFromMenu);

            Game.CommandBindings.Add(commandBind);
            AccountMenu.CommandBindings.Add(commandBind2);
        }

        private void SetMaxWindowSize()
        {
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            //Calculate half of the offset to move the form

            // MessageBox.Show($"{sizeInfo.NewSize.Height}\n{sizeInfo.NewSize.Width}");

            if (sizeInfo.HeightChanged)
                Top += (sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height) / 2;

            if (sizeInfo.WidthChanged)
                Left += (sizeInfo.PreviousSize.Width - sizeInfo.NewSize.Width) / 2;
        }

        private void SetGameModes()
        {
            mode = 0;

            gameModes = new Mode[4];
            gameModes[0] = new Mode(10, 10, 10);
            gameModes[1] = new Mode(16, 16, 40);
            gameModes[2] = new Mode(16, 30, 100);
            gameModes[3] = new Mode(10, 10, 0);

            WidthLbl.Content = gameModes[3].Width;
            HeightLbl.Content = gameModes[3].Height;
        }

        private void ToStartMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (gameGridField.Initialized)
            {
                if (!gameGridField.IsWin())
                {
                    MessageBoxResult result = MessageBox.Show("Вы не закончили игру, вы уверены?", "Выйти в главное меню", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        gameGridField.ClearGrid();
                        SHGameMenu();
                        SHStartMenu();
                    }
                }
                else
                {
                    gameGridField.ClearGrid();
                    SHGameMenu();
                    SHStartMenu();
                }
            }
        }

        private void ToStartMenuFromMenu(object sender, ExecutedRoutedEventArgs e)
        {
            SHStartMenu();
            SHAccountMenu();
        }

        // SH - ShowHide
        private void SHStartMenu()
        {
            Visibility visibleMode = Visibility.Visible == Menu.Visibility ? Visibility.Hidden : Visibility.Visible;
            Menu.Visibility = visibleMode;
            Menu.IsEnabled = !Menu.IsEnabled;

            if (visibleMode == Visibility.Visible)
            {
                MaxWidth = 280;
            }
            else
            {
                SetMaxWindowSize();
            }
        }

        private void SHGameMenu()
        {
            Visibility visibleMode = Visibility.Visible == Game.Visibility ? Visibility.Hidden : Visibility.Visible;
            Game.Visibility = visibleMode;
            Game.IsEnabled = !Game.IsEnabled;
        }

        private void SHAccountMenu()
        {
            Visibility visibleMode = Visibility.Visible == AccountMenu.Visibility ? Visibility.Hidden : Visibility.Visible;
            AccountMenu.Visibility = visibleMode;
            AccountMenu.IsEnabled = !AccountMenu.IsEnabled;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            SHStartMenu();

            gameGridField.Init(gameModes[mode].Width, gameModes[mode].Height, gameModes[mode].Mines);
            gameGridField.CreateCells();

            SHGameMenu();
        }

        private void RestartSmile_Click(object sender, RoutedEventArgs e)
        {
            gameGridField.Reset();
        }

        private void ModeMenu_Checked(object sender, RoutedEventArgs e)
        {
            mode = ((int)((RadioButton)sender).GetValue(Grid.RowProperty)) - 1;
        }

        private void SetWidth_Click(object sender, RoutedEventArgs e)
        {
            if (UserModeRadioButton.IsChecked == false)
                UserModeRadioButton.IsChecked = true;
            string op = ((Button)sender)?.Content.ToString();
            ChangeGameFieldWidth(op);
        }

        private void SetHeight_Click(object sender, RoutedEventArgs e)
        {
            if (UserModeRadioButton.IsChecked == false)
                UserModeRadioButton.IsChecked = true;
            string op = ((Button)sender).Content.ToString();
            ChangeGameFieldHeight(op);
        }

        private async void OpenAccount_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show($"{UserAccount.Status}\n{UserAccount.Error}");
            if (UserAccount == null || UserAccount.Status == AccountStatus.Initialized)
                MessageBox.Show("Ошибка! Попробуйте позже");
            else
            {
                if (UserAccount.Error != AccountErrorStatus.Ok)
                    await AccountManager.LoadAccount(UserAccount);

                // MessageBox.Show(UserAccount.Error.ToString());

                switch (UserAccount.Error)
                {
                    case AccountErrorStatus.NoInternetConnection:
                        MessageBox.Show("Отсутствует соеденение с интернетом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case AccountErrorStatus.DataBaseConnectionFailed:
                        MessageBox.Show("Не удалось соеденение с базой данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case AccountErrorStatus.IncorrectDataFile:
                        new AccountAuthorization().ShowDialog();
                        new AccountAuthorization().ShowDialog();
                        break;

                    case AccountErrorStatus.NotExists:
                        new AccountAuthorization().ShowDialog();
                        break;

                    case AccountErrorStatus.Ok:
                        SHStartMenu();
                        SHAccountMenu();
                        //MessageBox.Show(Account.ToString());
                        break;
                }
            }
        }

        private void ChangeGameFieldHeight(string op)
        {
            if (op.Equals("+"))
            {
                if (gameModes[3].Height <= 100)
                    gameModes[3].Height++;
            }
            else
                if (gameModes[3].Height > 10)
                gameModes[3].Height--;

            HeightLbl.Content = gameModes[3].Height.ToString();
        }

        private void ChangeGameFieldWidth(string op)
        {
            if (op.Equals("+"))
            {
                if (gameModes[3].Width <= 100)
                    gameModes[3].Width++;
            }
            else
                if (gameModes[3].Width > 10)
                gameModes[3].Width--;

            WidthLbl.Content = gameModes[3].Width.ToString();
        }

        public class AccountStatusConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (UserAccount != null &&
                    UserAccount.Error != AccountErrorStatus.Ok)
                    return "Error: " + UserAccount.Error;
                if (value != null) return ((AccountStatus)value).ToName();
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены что, хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question))
                App.Current.Shutdown();
        }
    }
}