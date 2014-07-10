using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Dynamics;

namespace Lotus.ECS.Aspects {
    public class AJitterBody : Aspect {

        public AJitterBody(int id) : base(id) { }

        public RigidBody Rigidbody;
    }
}
