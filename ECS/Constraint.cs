using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Constraint : Component {

        public Vector3Value MinPosition;
        public Vector3Value MaxPosition;

        public Constraint(int id)
            : base(id) {
                MinPosition = new Vector3Value(this, "MinPosition", Vector3.One * float.NegativeInfinity);
                MaxPosition = new Vector3Value(this, "MaxPosition", Vector3.One * float.PositiveInfinity);
        }
    }
}
