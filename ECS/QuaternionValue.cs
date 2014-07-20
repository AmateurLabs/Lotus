using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class QuaternionValue : DataValue<Quaternion> {

        public QuaternionValue(Component c, string name, Quaternion value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value.X);
            stream.Write(Value.Y);
            stream.Write(Value.Z);
            stream.Write(Value.W);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = new Quaternion(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
        }

        public override string Export() {
            return "(" + Value.X + ", " + Value.Y + ", " + Value.Z + ", " + Value.W + ")";
        }

        public override void Import(string input) {
            input = input.Substring(1, input.Length - 2);
            input = input.Replace(" ", "");
            string[] bits = input.Split(',');
            Value = new Quaternion(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2]), float.Parse(bits[3]));
        }
    }
}
