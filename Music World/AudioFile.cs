﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music_World
{
    public class AudioFile : IAudio
    {
        public struct AudioData
        {
            public Uri location;
            public string fileName; // Contains the name of the file presented on the computer
            public string audioName; // Actual audio name vs the name with it's extention, include if possible
            // {image} albumCover; include if possible, maybe alongside audioName
        }
        AudioData data;

        public void AddData(Uri location, string fileName, string audioName)
        {
            data.location = location;
            data.fileName = fileName;
            data.audioName = audioName;
        }

        public Button CreateButton()
        {
            Button audioFileButton = new Button()
            {
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right,
                Content = data.audioName, // switch with audioName if possible
                Background = Brushes.White,
                Tag = this
            };
            if (audioFileButton.Content == null)
            {
                audioFileButton.Content = data.fileName;
            }

            audioFileButton.MouseDoubleClick += AudioFileButton_MouseDoubleClick;
            return audioFileButton;
        }

        public Uri GetLocation()
        {
            return data.location;
        }

        private void AudioFileButton_MouseDoubleClick(object sender, RoutedEventArgs e)
        {

            // check for playing song
            // if song playing stop it
            // start this song
            // add visual thingy
        }

        public void AddAudioFile()
        {
            // leaves do not need children
        }
        public void RemoveAudioFile()
        {
            // leaves do not need children
        }
    }
}
