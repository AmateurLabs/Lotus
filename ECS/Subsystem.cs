using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Subsystem {

        public HashSet<int> Entities;
        public HashSet<string> AspectMask;
    }
}
