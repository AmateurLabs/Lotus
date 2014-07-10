using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS.Aspects {
    public class AMesh : Aspect {

        public Mesh Mesh;

        public AMesh(int id) : base(id) { }
    }
}
