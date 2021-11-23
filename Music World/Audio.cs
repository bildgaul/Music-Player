using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Music_World
{
    public interface IAudio
    {
        void AddData(Uri location, string fileName);
        Button CreateButton();
        void AddAudioFile();
        void RemoveAudioFile();
    }
}
