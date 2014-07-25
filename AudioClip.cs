using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Audio.OpenAL;

namespace Lotus {
    public abstract class AudioClip {

        public abstract short[] GetData(out int channels, out int bits, out int sampleRate);

        public abstract float Length { get; }
    }
}
