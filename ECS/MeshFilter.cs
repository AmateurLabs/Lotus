using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class MeshFilter : Component {

        public MeshValue Mesh;
        public Color4Value Color;

        public MeshFilter(int id) : base(id) {
            Mesh = new MeshValue(this, "Mesh", null);
            Color = new Color4Value(this, "Color", Color4.White);
        }
    }
}
