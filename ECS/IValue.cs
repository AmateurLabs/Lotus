using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public interface IValue {

        string Name {
            get;
            set;
        }

        bool Serialized {
            get;
            set;
        }

        void Serialize(BinaryWriter stream);
        void Deserialize(BinaryReader stream);
        string Export();
        void Import(string input);
    }
}
