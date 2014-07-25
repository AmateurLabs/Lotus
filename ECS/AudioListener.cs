using OpenTK.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class AudioListener : Component {

        public EnumValue<Layers> LayerMask;

        public AudioListener(int id) : base(id) {
            LayerMask = new EnumValue<Layers>(this, "LayerMask", Layers.Layer0);
        }
    }
}
