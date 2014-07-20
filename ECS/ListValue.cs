using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class ListValue<T> : DataValue<List<T>> where T : IValue, new() {

        public ListValue() { }
        public ListValue(Component c, string name, List<T> value) : base(c, name, value) { }

        public override void Serialize(BinaryWriter stream) {
            stream.Write(Value.Count);
            for (int i = 0; i < Value.Count; i ++) {
                Value[i].Serialize(stream);
            }
        }

        public override void Deserialize(BinaryReader stream) {
            int count = stream.ReadInt32();
            Value = new List<T>(count);
            for (int i = 0; i < count; i++) {
                Value[i] = new T();
                Value[i].Deserialize(stream);
            }
        }

        public override string Export() {
            string output = "[";
            for (int i = 0; i < Value.Count; i++) {
                output += Value[i].Export();
                if (i < Value.Count - 1) output += "; ";
            }
            output += "]";
            return output;
        }

        public override void Import(string input) {
            input = input.Substring(1, input.Length - 2);
            input = input.Replace("; ", ";");
            string[] bits = input.Split(';');
            Value = new List<T>(bits.Length);
            for (int i = 0; i < bits.Length; i++) {
                Value[i] = new T();
                Value[i].Import(bits[i]);
            }
        }
    }
}
