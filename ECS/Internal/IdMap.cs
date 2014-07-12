using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS.Internal {
    internal static class IdMap<T> {
        internal static Dictionary<int, T> Map = new Dictionary<int, T>();
    }
}
