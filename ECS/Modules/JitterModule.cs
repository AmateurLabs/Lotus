using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jitter;
using Jitter.Collision;

namespace Lotus.ECS.Modules {
    public class JitterModule : Module {

        World world;
        CollisionSystem collisionSystem;

        public JitterModule() {
            collisionSystem = new CollisionSystemSAP();
            world = new World(collisionSystem);
        }
    }
}
