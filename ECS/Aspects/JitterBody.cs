using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Dynamics;

namespace Lotus.ECS.Aspects {
    public class JitterBody : Aspect {

        public JitterBody(int id) : base(id) { }

        public RigidBody Rigidbody;
    }
}
