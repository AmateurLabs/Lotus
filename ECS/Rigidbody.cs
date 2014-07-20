using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Rigidbody : Component {

        public Vector3Value Velocity;
        public Vector3Value AngularVelocity;
        public FloatValue Mass;

        public Rigidbody(int id) : base(id) {
            Velocity = new Vector3Value(this, "Velocity", Vector3.Zero);
            AngularVelocity = new Vector3Value(this, "AngularVelocity", Vector3.Zero);
            Mass = new FloatValue(this, "Mass", 1f);
        }
    }
}
