using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class EnumValue<T> : DataValue<T> {

        public EnumValue(Component c, string name, T value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write((int)(object)Value);
        }

        public override void Deserialize(BinaryReader stream) {
            Value = (T)Enum.ToObject(typeof(T), stream.ReadInt32());
        }

        public override string Export() {
            return Value.ToString();
        }

        public override void Import(string input) {
            Value = (T)Enum.Parse(typeof(T), input);
        }
    }
}
