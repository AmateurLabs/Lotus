using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Attractor : Component {

        public Attractor(int id) : base(id) { }

        public enum AttractionType {
            Point,
            Plane,
            World
        }

        public AttractionType Type;
        public float Acceleration;
        public Vector3 Normal = Vector3.UnitY;
        public bool UseMass;
    }
}
