using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Vector3Value : DataValue<Vector3> {

        public Vector3Value(Component c, string name, Vector3 value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value.X);
            stream.Write(Value.Y);
            stream.Write(Value.Z);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = new Vector3(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
        }

        public override string Export() {
            return Value.X + ", " + Value.Y + ", " + Value.Z;
        }

        public override void Import(string input) {
            input = input.Replace(" ", "");
            string[] bits = input.Split(',');
            Value = new Vector3(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2]));
        }
    }
}
