using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class AudioSource : Component {

        public EnumValue<Layers> LayerMask;
        public AudioClipValue Clip;
        public FloatValue Pitch;
        public FloatValue Time;
        public EnumValue<AudioSourceState> State;
        public BoolValue Looping;

        public AudioSource(int id) : base(id) {
            LayerMask = new EnumValue<Layers>(this, "LayerMask", Layers.Layer0);
            Clip = new AudioClipValue(this, "Clip", null);
            Pitch = new FloatValue(this, "Pitch", 1f);
            Time = new FloatValue(this, "Time", 0f);
            State = new EnumValue<AudioSourceState>(this, "State", AudioSourceState.None);
            Looping = new BoolValue(this, "Looping", false);
        }
    }

    public enum AudioSourceState {
        None = 0,
        Play = 1,
        Playing = 2,
        Pause = 3,
        Paused = 4,
        Stop = 5,
        Stopped = 6
    }
}
