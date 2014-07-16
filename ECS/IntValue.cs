using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class IntValue : DataValue<int> {

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = stream.ReadInt32();
        }

        public override string Export() {
            return Value.ToString();
        }

        public override void Import(string input) {
            Value = int.Parse(input);
        }
    }
}
