using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Attractor : Component {

        public enum AttractionType {
            Point,
            Plane,
            World
        }

        public EnumValue<AttractionType> Type;
        public FloatValue Acceleration;
        public Vector3Value Normal;
        public BoolValue UseMass;

        public Attractor(int id) : base(id) {
            Type = new EnumValue<AttractionType>(this, "Type", AttractionType.World);
            Acceleration = new FloatValue(this, "Acceleration", 0f);
            Normal = new Vector3Value(this, "Normal", Vector3.UnitY);
            UseMass = new BoolValue(this, "UseMass", false);
        }

    }
}
