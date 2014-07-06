using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class IdMap<T> where T : IdMap<T> {
        public static Dictionary<int, T> Map;
    }
}
