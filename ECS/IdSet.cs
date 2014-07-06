using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class IdSet<T> where T : IdSet<T> {
        public static HashSet<int> Set;
    }
}
