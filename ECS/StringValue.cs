using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class StringValue : DataValue<string> {

        public StringValue(Component c, string name, string value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = stream.ReadString();
        }

        public override string Export() {
            return Value;
        }

        public override void Import(string input) {
            Value = input;
        }
    }
}
