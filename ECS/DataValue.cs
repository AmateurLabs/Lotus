using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lotus.ECS {
    public class DataValue<T> : IValue {

        public string Name {
            get;
            set;
        }

        public T Value {
            get;
            set;
        }

        public bool Serialized {
            get;
            set;
        }

        public DataValue() { }

        public DataValue(Component c, string name, T value) : this(c, name, value, true) { }

        public DataValue(Component c, string name, T value, bool serialized) {
            Name = name;
            Value = value;
            c.Values.Add(this);
            Serialized = serialized;
        }

        public virtual void Serialize(BinaryWriter stream) { }
        public virtual void Deserialize(BinaryReader stream) { }
        public virtual string Export() { return ""; }
        public virtual void Import(string input) { }
    }
}
