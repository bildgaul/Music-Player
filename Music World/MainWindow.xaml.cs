using Microsoft.Win32;
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
        MediaPlayer player = new MediaPlayer();
        OpenFileDialog fileSelector = new OpenFileDialog();
        Uri location; // Temporary, will be removed later

        /*
         * Description: Initializes and sets up everything required for the music player to work
         */
        public MainWindow()
        {
            InitializeComponent();
            fileSelector.Filter = "Music Files|*.mp3;*.wav" +
                "|All Files|*.*";
            fileSelector.DefaultExt = ".mp3";
            fileSelector.Title = "Add Audio File";
            fileSelector.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            player.MediaEnded += OnMediaEnded; // Connect the MediaEnded event to the function
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (PlayPause.Tag.ToString() == "Pause")
            {
                if (player.Source == null)
                {
                    player.Open(location);
                }
                player.Play();
                ButtonImage.Source = new BitmapImage(new Uri("assets/Pause.png", UriKind.Relative)); // https://stackoverflow.com/questions/3873027/how-to-change-image-source-on-runtime/40788154
                PlayPause.Tag = "Play";
                Console.WriteLine(player.HasVideo);
                }
            else if (PlayPause.Tag.ToString() == "Play")
            {
                player.Pause();
                ButtonImage.Source = new BitmapImage(new Uri("assets/Play.png", UriKind.Relative));
                PlayPause.Tag = "Pause";
            }
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            player.Stop();
        }

        private void AddAudioFile_Click(object sender, RoutedEventArgs e)
        {
            fileSelector.ShowDialog();
            string fileName = fileSelector.FileName;
            try
            {
                location = new Uri(fileName);
                AudioFactory audioFactory = new AudioFileFactory();
                IAudio audio = audioFactory.CreateAudioFile(location, fileSelector.SafeFileName);
                
                //ViewPanel.Children.Add(fileButton);
            }
            catch (System.UriFormatException)
            {
                MessageBox.Show("Could not open file.");
            }
        }
    }
}
