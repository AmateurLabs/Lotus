using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Constraint : Component {

        public Vector3 MinPosition;
        public Vector3 MaxPosition;

        public Constraint(int id) : base(id) { }
    }
}
