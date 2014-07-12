using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;

namespace Lotus.ECS {
    public class Freelook : Component {

        public Freelook(int id) : base(id) { }

        public float MoveSpeed = 10f; //How fast the freelook camera moves around
        public float RotateSpeed = 0.005f; //How fast the freelook camera rotates
    }
}
