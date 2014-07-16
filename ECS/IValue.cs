using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public interface IValue {

        void Serialize(BinaryWriter stream);
        void Deserialize(BinaryReader stream);
        string Export();
        void Import(string input);
    }
}
