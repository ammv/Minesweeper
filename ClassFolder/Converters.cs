using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.ClassFolder.Converters
{
    public class WindowAccountImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "/Sourcefolder/Images/user.png";
            else
                return WindowFolder.GameWindow.UserAccount.Avatar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class AccountStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (WindowFolder.GameWindow.UserAccount != null &&
                WindowFolder.GameWindow.UserAccount.Error != AccountErrorStatus.Ok)
                return "Error: " + WindowFolder.GameWindow.UserAccount.Error;

            if (value != null)
                return ((AccountStatus)value).ToName();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class AccountTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utils.TimeInSecondsToString((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}