using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using System.IO;

namespace Lotus.ECS {
    public class Color4Value : DataValue<Color4> {
        public Color4Value(Component c, string name, Color4 value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value.R);
            stream.Write(Value.G);
            stream.Write(Value.B);
            stream.Write(Value.A);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = new Color4(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
        }

        public override string Export() {
            return "(" + Value.R + ", " + Value.G + ", " + Value.B + ", " + Value.A + ")";
        }

        public override void Import(string input) {
            input = input.Substring(1, input.Length - 2);
            input = input.Replace(" ", "");
            string[] bits = input.Split(',');
            Value = new Color4(float.Parse(bits[0]), float.Parse(bits[1]), float.Parse(bits[2]), float.Parse(bits[3]));
        }
    }
}
