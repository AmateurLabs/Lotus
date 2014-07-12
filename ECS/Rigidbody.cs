using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Rigidbody : Component {

        public Vector3 Velocity = Vector3.Zero;
        public Vector3 AngularVelocity = Vector3.Zero;

        public float Mass = 1f;

        public Rigidbody(int id) : base(id) { }
    }
}
