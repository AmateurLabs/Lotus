using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class BoolValue : DataValue<bool> {

        public BoolValue(Component c, string name, bool value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = stream.ReadBoolean();
        }

        public override string Export() {
            return Value.ToString();
        }

        public override void Import(string input) {
            Value = bool.Parse(input);
        }
    }
}
