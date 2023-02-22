using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper.SourceFolder.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RoundAvatar.xaml
    /// </summary>
    public partial class RoundAvatar : UserControl
    {
        public RoundAvatar()
        {
            InitializeComponent();
        }

        public ImageSource AvatarSource
        {
            get { return (ImageSource)GetValue(AvatarSourceProperty); }
            set { SetValue(AvatarSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvatarSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register("AvatarSource", typeof(ImageSource), typeof(RoundAvatar),
                new PropertyMetadata(null));

        public string TextIsMouseOver
        {
            get { return (string)GetValue(TextIsMouseOverProperty); }
            set { SetValue(TextIsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextIsMouseOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextIsMouseOverProperty =
            DependencyProperty.Register("TextIsMouseOver", typeof(string), typeof(RoundAvatar), new PropertyMetadata("Change"));
    }
}