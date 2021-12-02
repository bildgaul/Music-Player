using System;
using System.Windows.Controls;

namespace Music_World
{
    public interface IAudio
    {
        void AddData(Uri location, string fileName, string audioName);
        Button CreateButton();
        void AddAudioFile(AudioFile audioFile);
        void RemoveAudioFile(AudioFile audioFile);
    }
}
