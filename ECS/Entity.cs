using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Entity {

        public static T Get<T>(int id) where T : Aspect {
            T t;
            IdMap<T>.Map.TryGetValue(id, out t);
            return t;
        }

        public static bool Has<T>(int id) where T : Aspect {
            return IdMap<T>.Map.ContainsKey(id);
        }

        public static T Add<T>(int id) where T : Aspect {
            T t = (T)Activator.CreateInstance(typeof(T), id);
            IdMap<T>.Map[id] = t;
            foreach (Module module in Engine.Modules) module.Reveille(t);
            return t;
        }

        public static bool Remove<T>(int id) where T : Aspect {
            foreach (Module module in Engine.Modules) module.Taps(IdMap<T>.Map[id]);
            return IdMap<T>.Map.Remove(id);
        }
    }
}
