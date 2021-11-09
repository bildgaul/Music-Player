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

namespace Music_World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (PlayPause.Tag.ToString() == "Pause")
            {
                ButtonImage.Source = new BitmapImage(new Uri("assets/Play.png", UriKind.Relative)); // https://stackoverflow.com/questions/3873027/how-to-change-image-source-on-runtime/40788154
                PlayPause.Tag = "Play";
            }
            else if (PlayPause.Tag.ToString() == "Play")
            {
                ButtonImage.Source = new BitmapImage(new Uri("assets/Pause.png", UriKind.Relative));
                PlayPause.Tag = "Pause";
            }
        }
    }
}
