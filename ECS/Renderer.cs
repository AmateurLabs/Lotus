using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Renderer : Component {

        public EnumValue<RenderLayers> Layers;

        public Renderer(int id) : base(id) {
            Layers = new EnumValue<RenderLayers>(this, "Layers", RenderLayers.Default);
        }

    }
}
