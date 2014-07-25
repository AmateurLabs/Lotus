using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Renderer : Component {

        public EnumValue<Layers> LayerMask;

        public Renderer(int id) : base(id) {
            LayerMask = new EnumValue<Layers>(this, "LayerMask", Layers.Layer0);
        }

    }
}
