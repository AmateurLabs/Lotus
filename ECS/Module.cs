using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Module : IdSet<Module> {

        public HashSet<int> Set = new HashSet<int>();

        public abstract void Update();
    }
}
