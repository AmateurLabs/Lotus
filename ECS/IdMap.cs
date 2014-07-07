using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public static class IdMap<T> {
        public static Dictionary<int, T> Map = new Dictionary<int, T>();
    }
}
