using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music_World
{
    public class Playlist : IAudio
    {

        public List<AudioFile> AudioFiles { get; }
        public string Name { get; }

        public Playlist(string name)
        {
            Name = name;
            AudioFiles = new List<AudioFile>();
        }

        public void AddData(Uri location, string fileName, string audioName)
        {
            // nothing here! not an AudioFile
        }

        public Button CreateButton()
        {
            Button playlistButton = new Button()
            {
                Content = Name,
                Background = Brushes.White,
                BorderBrush = null,
                FontSize = 15,
                Tag = this
            };
            return playlistButton;
        }

        public void AddAudioFile(AudioFile audioFile)
        {
            AudioFiles.Add(audioFile);
        }

        public void RemoveAudioFile(AudioFile audioFile)
        {
            AudioFiles.Remove(audioFile);
        }
    }
}
