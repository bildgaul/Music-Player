using System;

namespace Music_World
{
    public class AudioFileFactory : AudioFactory
    {
        public override IAudio CreateAudioFile(Uri fileUri, string fileName, string audioName)
        {
            AudioFile audio = new AudioFile();
            audio.AddData(fileUri, fileName, audioName);
            return audio;
        }
    }
}
