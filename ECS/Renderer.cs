using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Renderer : Component {

        public RenderLayers Layers = RenderLayers.Default;

        public Renderer(int id) : base(id) { }

    }
}
