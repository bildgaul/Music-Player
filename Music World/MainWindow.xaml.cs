using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        Mp3FileReader mp3Reader;
        WaveFileReader waveReader;
        TimeSpan duration;
        double totalTime;

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

            mp3Reader = new Mp3FileReader(currentAudio.OriginalString);
            duration = mp3Reader.TotalTime;
            //Referene: https://stackoverflow.com/questions/16252911/c-how-do-i-convert-a-timespan-value-to-a-double
            totalTime = (((duration.Minutes / 1.0)*60) + (duration.Seconds / 100.0)*100) * (duration > TimeSpan.Zero ? 1 : -1);
            Console.WriteLine(duration);
            Console.WriteLine(totalTime);
            ProgressBar();

            /*waveReader = new WaveFileReader(currentAudio.OriginalString);
            duration = waveReader.TotalTime;
            totalTime = (duration.Minutes / 1.0 + duration.Seconds / 100.0) * (duration > TimeSpan.Zero ? 1 : -1);
            Console.WriteLine(duration);
            Console.WriteLine(totalTime);
            ProgressBar();*/
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
                    if (currentAudio == null)
                        currentAudio = new Uri(fileName);
                    IAudio audio = new AudioFileFactory().CreateAudioFile(new Uri(fileName), fileSelector.SafeFileName);
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

        private void ProgressBar()
        {
            ProgressBar pBar = new ProgressBar();

            pBar.Minimum = 1;
            pBar.Maximum = totalTime;
            pBar.Value = 1;
            Console.WriteLine(pBar.Maximum);

            for (int progress = 1; progress <= totalTime; progress++)
            {
                if (currentAudio == null)
                {
                    pBar.Value = 1;
                }
                else if (isPlaying == true)
                {
                    Label.Content = Convert.ToString(progress);
                    pBar.Value += 1;
                }
                else
                {
                    pBar.Value = pBar.Value;
                }
            }
        }

        private void Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isPlaying && currentAudio != null)
            {

            }
            else if (isPlaying == true)
            {
                ProgressBar();
            }
        }
    }
}
