using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Dynamics;

namespace Lotus.ECS {
    public class JitterBody : Component {

        public JitterBody(int id) : base(id) { }

        public RigidBody Rigidbody;
    }
}
