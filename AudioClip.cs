using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Audio.OpenAL;

namespace Lotus {
    public abstract class AudioClip {

        public bool Stereo = false;
        public int SampleRate = 44100;

        public abstract short[] GetData();

        public abstract float Length { get; }
    }
}
