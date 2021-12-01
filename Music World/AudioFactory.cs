using System;

namespace Music_World
{
    public abstract class AudioFactory
    {
        public abstract IAudio CreateAudioFile(Uri fileUri, string fileName, string audioName);
    }
}
