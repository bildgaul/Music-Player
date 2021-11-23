using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_World
{
    public class AudioFileFactory : AudioFactory
    {
        public override IAudio CreateAudioFile(Uri fileUri, string fileName)
        {
            AudioFile audio = new AudioFile();
            audio.AddData(fileUri, fileName);
            return audio;
        }
    }
}
