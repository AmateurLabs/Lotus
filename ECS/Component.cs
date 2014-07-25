using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Component {

        public readonly int Id;
        public readonly List<IValue> Values = new List<IValue>();

        public Component(int id) {
            Id = id;
        }

        public IValue GetDataValue(string name) {
            foreach (IValue value in Values) {
                if (value.Name == name) return value;
            }
            return null;
        }

        /*public T GetValue<T>(string name) {
            foreach (IValue value in Values) {
                if (value.Name == name && value is DataValue<T>) return ((DataValue<T>)value).Value;
            }
            return default(T);
        }

        public void SetValue<T>(string name, T newValue) {
            foreach (IValue value in Values) {
                if (value.Name == name && value is DataValue<T>) ((DataValue<T>)value).Value = newValue;
            }
        }*/
    }
}
