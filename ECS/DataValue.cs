using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lotus.ECS {
    public abstract class DataValue<T> : IValue {

        public string Name {
            get;
            set;
        }

        public virtual T Value {
            get;
            set;
        }

        public DataValue(Component c, string name, T value) {
            Name = name;
            Value = value;
            c.Values.Add(this);
        }

        public abstract void Serialize(BinaryWriter stream);
        public abstract void Deserialize(BinaryReader stream);
        public abstract string Export();
        public abstract void Import(string input);
    }
}
