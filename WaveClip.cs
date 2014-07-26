using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus {
    public class WaveClip : AudioClip {

        public WaveType Wave;
        public double Frequency = 261.626;

        public WaveClip(WaveType wave) {
            Wave = wave;
        }

        public WaveClip(WaveType wave, double frequency)
            : this(wave) {
                Frequency = frequency;
        }

        short[] data;

        public override short[] GetData() {
            if (data == null) {
                data = new short[(int)(SampleRate / Frequency)];
                for (int i = 0; i < data.Length; i++) {
                    data[i] = (short)(short.MaxValue * Sample(Wave, ((double)i / (double)data.Length)));
                }
            }
            return data;
        }

        public static double Sample(WaveType wave, double t) {
            if (wave == WaveType.Sine) return Math.Sin(t * 2.0 * Math.PI);
            else if (wave == WaveType.Sawtooth) return (t - Math.Floor(t + 0.5));
            else if (wave == WaveType.Triangle) return Math.Abs(t/2.0 - Math.Floor(t/2.0 + 0.5)) * ((t > 0.5) ? -1.0 : 1.0);
            else if (wave == WaveType.Square) return Math.Sign(Math.Sin(t * 2.0 * Math.PI));
            return 0.0;
        }

        public override float Length {
            get { return (float)(1f / Frequency); }
        }
    }

    public enum WaveType {
        None=0,
        Sine=1,
        Square=2,
        Triangle=3,
        Sawtooth=4
    }
}
