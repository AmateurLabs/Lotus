using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class AudioClipValue : DataValue<AudioClip> {

        public AudioClipValue(Component c, string name, AudioClip value) : base(c, name, value) { }

        public enum AudioClipType {
            None=0,
            Waveform=1,
            Wav=2
        }

        public override void Serialize(System.IO.BinaryWriter stream) {
            if (Value == null) {
                stream.Write((int)AudioClipType.None);
                return;
            }
            if (Value is WaveClip) {
                stream.Write((int)AudioClipType.Waveform);
                WaveClip clip = Value as WaveClip;
                stream.Write((int)clip.Wave);
                stream.Write(clip.Frequency);
            }
        }

        public override void Deserialize(System.IO.BinaryReader stream) {
            AudioClipType type = (AudioClipType)stream.ReadInt32();
            if (type == AudioClipType.Waveform) {
                WaveType waveType = (WaveType)stream.ReadInt32();
                double frequency = stream.ReadDouble();
                Value = new WaveClip(waveType, frequency);
            }
        }

        public override string Export() {
            if (Value == null) return AudioClipType.None.ToString();
            string output = "";
            if (Value is WaveClip) {
                WaveClip clip = Value as WaveClip;
                output += AudioClipType.Waveform + "(" + clip.Wave + ", " + clip.Frequency + ")";
            }
            else {
                throw new NotImplementedException("AudioClip type " + Value.GetType().Name + " doesn't have an exporter defined.");
            }
            return output;
        }

        public override void Import(string input) {
            int splitPt = input.IndexOf('(');
            string typeStr = (splitPt == -1) ? input : input.Substring(0, splitPt);
            AudioClipType type = (AudioClipType)Enum.Parse(typeof(AudioClipType), typeStr);
            if (type == AudioClipType.None) return;
            string data = input.Substring(splitPt);
            data = data.Substring(1, data.Length - 2);
            if (type == AudioClipType.Waveform) {
                string[] bits = data.Replace(", ", ",").Split(',');
                Value = new WaveClip((WaveType)Enum.Parse(typeof(WaveType), bits[0]), double.Parse(bits[1]));
            }
            else {
                throw new NotImplementedException("AudioClip type " + type + " doesn't have an importer defined.");
            }
        }
    }
}
