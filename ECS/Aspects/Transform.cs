using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS.Aspects {
    public class Transform : Aspect {

        public OpenTK.Vector3 Position;
        public OpenTK.Quaternion Rotation;
        public OpenTK.Vector3 Scale;
    }
}
