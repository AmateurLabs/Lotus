using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;

namespace Lotus.ECS {
    public class Freelook : Component {

        public FloatValue MoveSpeed; //How fast the freelook camera moves around
        public FloatValue RotateSpeed; //How fast the freelook camera rotates

        public Freelook(int id)
            : base(id) {
                MoveSpeed = new FloatValue(this, "MoveSpeed", 10f);
                RotateSpeed = new FloatValue(this, "RotateSpeed", 0.005f);
        }
    }
}
