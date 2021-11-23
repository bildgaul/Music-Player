using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_World
{
    public abstract class AudioFactory
    {
        public abstract IAudio CreateAudioFile(Uri fileUri, string fileName);
    }
}
