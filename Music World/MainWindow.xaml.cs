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
        List<Playlist> storedPlaylists = new List<Playlist>();

        AudioFile currentAudio;
        Playlist currentPlaylist;

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
            if (player.Source != currentAudio.GetLocation())
            {
                player.Close();
                player.Open(currentAudio.GetLocation());
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

        private void CreateNewPlaylist(string name)
        {
            foreach (Playlist playlist1 in storedPlaylists)
            {
                if (name == playlist1.Name)
                {
                    MessageBox.Show("Cannot have two playlists with the same name.");
                    return;
                }
            }
            Playlist playlist = new Playlist(name);
            storedPlaylists.Add(playlist);
            currentPlaylist = playlist;

            Button playlistButton = playlist.CreateButton();
            playlistButton.Click += SwitchPlaylists;
            View.Items.Add(playlistButton);

            AllAudio.Children.Clear();
        }

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistNameBox.Visibility = Visibility.Visible;
        }

        private void PlaylistNameBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CreateNewPlaylist(PlaylistNameBox.Text);
                PlaylistNameBox.Text = "Playlist Name";
                PlaylistNameBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AddAudioFileToPlayList(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            AudioFile audioFile = menuItem.Tag as AudioFile;

            AudioMenu.Height = 0;
            AudioMenu.Items.Clear();
            AudioScroll.Visibility = Visibility.Collapsed;

            if (currentPlaylist.AudioFiles.Contains(audioFile))
            {
                MessageBox.Show("Cannot add two of the same audio to a playlist.");
                return;
            }
            
            currentPlaylist.AddAudioFile(audioFile);
            Button audioFileButton = audioFile.CreateButton();
            audioFileButton.MouseDoubleClick += AudioFileButton_MouseDoubleClick;
            AllAudio.Children.Add(audioFileButton);
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Add_Create.IsDropDownOpen = false;

            if (currentPlaylist == null)
            {
                MessageBox.Show("No playlist selected.");
                return;
            }
            if (storedAudioFiles.Count == 0)
            {
                MessageBox.Show("No audio files to add.");
                return;
            }
            if (AudioMenu.Height != 0)
                return; // prevent readding all audio files
            
            AudioScroll.Visibility = Visibility.Visible;
            
            foreach (AudioFile audio in storedAudioFiles)
            {
                MenuItem menuItem = new MenuItem()
                {
                    Header = audio.GetAudioName(),
                    Width = 100,
                    Height = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Tag = audio
                };
                menuItem.Click += AddAudioFileToPlayList;
                
                AudioMenu.Height += 20;
                AudioMenu.Items.Add(menuItem);
            }
        }

        private void SwitchPlaylists(object sender, RoutedEventArgs e)
        {
            Button playlistButton = sender as Button;
            AllAudio.Children.Clear();

            if (playlistButton.Name == "ViewAllAudio")
            {
                currentPlaylist = null;

                foreach (AudioFile audioFile in storedAudioFiles)
                {
                    Button audioFileButton = audioFile.CreateButton();
                    audioFileButton.MouseDoubleClick += AudioFileButton_MouseDoubleClick;
                    AllAudio.Children.Add(audioFileButton);
                }
            }
            else
            {
                currentPlaylist = playlistButton.Tag as Playlist;

                foreach (AudioFile audioFile in currentPlaylist.AudioFiles)
                {
                    Button audioFileButton = audioFile.CreateButton();
                    audioFileButton.MouseDoubleClick += AudioFileButton_MouseDoubleClick;
                    AllAudio.Children.Add(audioFileButton);
                }
            }
        }

        private void RemoveAudioFileFromList(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            AudioFile audioFile = menuItem.Tag as AudioFile;

            AudioMenu.Height = 0;
            AudioMenu.Items.Clear();
            AudioScroll.Visibility = Visibility.Collapsed;

            foreach (Button button in AllAudio.Children)
            {
                if ((AudioFile)button.Tag == audioFile)
                {
                    button.Tag = null;
                    AllAudio.Children.Remove(button);
                    break;
                }
            }

            foreach (Playlist playlist in storedPlaylists)
            {
                if (playlist.AudioFiles.Contains(audioFile))
                {
                    playlist.RemoveAudioFile(audioFile);
                }
            }

            storedAudioFiles.Remove(audioFile);
        }

        private void RemoveAudioFile_Click(object sender, RoutedEventArgs e)
        {
            Remove.IsDropDownOpen = false;

            if (AudioMenu.Height != 0)
                return;
            if (storedAudioFiles.Count == 0)
            {
                MessageBox.Show("No audio files to remove.");
                return;
            }
            
            AudioScroll.Visibility = Visibility.Visible;
            
            foreach (AudioFile audioFile in storedAudioFiles)
            {
                MenuItem menuItem = new MenuItem()
                {
                    Header = audioFile.GetAudioName(),
                    Width = 100,
                    Height = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Tag = audioFile
                };
                menuItem.Click += RemoveAudioFileFromList;
                AudioMenu.Height += 20;
                AudioMenu.Items.Add(menuItem);
            }
        }

        private void RemoveStoredPlaylist(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            Playlist playlist = menuItem.Tag as Playlist;

            AudioMenu.Height = 0;
            AudioMenu.Items.Clear();
            AudioScroll.Visibility = Visibility.Collapsed;

            storedPlaylists.Remove(playlist);

            if (currentPlaylist == playlist)
            {
                currentPlaylist = null;
                SwitchPlaylists(ViewAllAudio, new RoutedEventArgs());
            }

            foreach (Button button in View.Items)
            {
                if ((Playlist)button.Tag == playlist)
                {
                    View.Items.Remove(button);
                    break;
                }
            }
        }

        private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            Remove.IsDropDownOpen = false;
            
            if (AudioMenu.Height != 0)
                return;
            if (storedPlaylists.Count == 0)
            {
                MessageBox.Show("No playlists to remove.");
                return;
            }

            AudioScroll.Visibility = Visibility.Visible;
            
            foreach (Playlist playlist in storedPlaylists)
            {
                MenuItem menuItem = new MenuItem()
                {
                    Header = playlist.Name,
                    Width = 100,
                    Height = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Tag = playlist
                };
                menuItem.Click += RemoveStoredPlaylist;
                AudioMenu.Height += 20;
                AudioMenu.Items.Add(menuItem);
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
            if (currentPlaylist != null)
            {
                MessageBox.Show("Cannot add audio files when a playlist is open.");
                return;
            }

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
            currentAudio = audioFile;
            Play();
            // add play icon next to name
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                if (currentAudio == storedAudioFiles[0])
                    currentAudio = storedAudioFiles[storedAudioFiles.Count - 1];
                else
                    currentAudio = storedAudioFiles[storedAudioFiles.IndexOf(currentAudio) - 1];
            }
            else
            {
                if (currentAudio == currentPlaylist.AudioFiles[0])
                    currentAudio = currentPlaylist.AudioFiles[currentPlaylist.AudioFiles.Count - 1];
                else
                    currentAudio = currentPlaylist.AudioFiles[currentPlaylist.AudioFiles.IndexOf(currentAudio) - 1];
            }

            Play();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                if (currentAudio == storedAudioFiles[storedAudioFiles.Count - 1])
                    currentAudio = storedAudioFiles[0];
                else
                    currentAudio = storedAudioFiles[storedAudioFiles.IndexOf(currentAudio) + 1];
            }
            else
            {
                if (currentAudio == currentPlaylist.AudioFiles[currentPlaylist.AudioFiles.Count - 1])
                    currentAudio = currentPlaylist.AudioFiles[0];
                else
                    currentAudio = currentPlaylist.AudioFiles[currentPlaylist.AudioFiles.IndexOf(currentAudio) + 1];
            }

            Play();
        }
    }
}
