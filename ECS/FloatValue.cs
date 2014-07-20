using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class FloatValue : DataValue<float> {

        public FloatValue(Component c, string name, float value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = stream.ReadSingle();
        }

        public override string Export() {
            return Value.ToString();
        }

        public override void Import(string input) {
            Value = float.Parse(input);
        }
    }
}
