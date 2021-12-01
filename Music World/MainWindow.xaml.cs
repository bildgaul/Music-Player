using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TagLib;

namespace Music_World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();
        OpenFileDialog fileSelector = new OpenFileDialog();
        List<AudioFile> storedAudioFiles = new List<AudioFile>();

        Uri currentAudio;
        bool isPlaying = false;

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

        private void Play()
        {
            if (player.Source != currentAudio)
            {
                player.Close();
                player.Open(currentAudio);
            }
            player.Play();
            ButtonImage.Source = new BitmapImage(new Uri("assets/Pause.png", UriKind.Relative)); // https://stackoverflow.com/questions/3873027/how-to-change-image-source-on-runtime/40788154
            isPlaying = true;
        }

        private void Pause()
        {
            if (player.CanPause)
            {
                player.Pause();
                ButtonImage.Source = new BitmapImage(new Uri("assets/Play.png", UriKind.Relative));
                isPlaying = false;
            }
        }


        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying && currentAudio != null)
            {
                Play();
                Console.WriteLine(player.NaturalDuration.TimeSpan);
            }
            else if (isPlaying)
            {
                Pause();
            }
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            player.Stop();
            ButtonImage.Source = new BitmapImage(new Uri("assets/Play.png", UriKind.Relative));
            isPlaying = false;
        }

        private void AddAudioFile_Click(object sender, RoutedEventArgs e)
        {
            fileSelector.ShowDialog();
            string fileName = fileSelector.FileName;

            bool alreadyStored = false;
            foreach (AudioFile audioFile in storedAudioFiles)
            {
                if (fileName == audioFile.GetLocation().OriginalString)
                {
                    alreadyStored = true;
                }
            }
            if (alreadyStored)
            {
                MessageBox.Show("Cannot store two of the same audio.");
            }
            else
            {
                try
                {
                    File fileTags = TagLib.File.Create(fileName);
                    if (currentAudio == null)
                        currentAudio = new Uri(fileName);
                    IAudio audio = new AudioFileFactory().CreateAudioFile(new Uri(fileName), fileSelector.SafeFileName, fileTags.Tag.Title);
                    Button audioFileButton = audio.CreateButton();
                    audioFileButton.MouseDoubleClick += AudioFileButton_MouseDoubleClick;
                    AllAudio.Children.Add(audioFileButton);
                    storedAudioFiles.Add((AudioFile)audio);
                }
                catch (System.UriFormatException)
                {
                    MessageBox.Show("Could not open file.");
                }
            }
            
        }
        
        private void AudioFileButton_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button; // https://stackoverflow.com/questions/14479143/what-is-the-use-of-object-sender-and-eventargs-e-parameters
            AudioFile audioFile = button.Tag as AudioFile;
            currentAudio = audioFile.GetLocation();
            Play();
            // add play icon next to name
        }
    }
}
