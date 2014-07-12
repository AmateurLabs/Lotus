using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class MeshFilter : Component {

        public Mesh Mesh;

        public MeshFilter(int id) : base(id) { }
    }
}
