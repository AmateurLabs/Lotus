using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lotus.ECS.Internal;

namespace Lotus.ECS {
    public sealed class Entity {

        static int nextEntityId;

        public static int Allocate() {
            return nextEntityId++;
        }

        public static Entity Wrap(int id) {
            Entity ent;
            if (!IdMap<Entity>.Map.TryGetValue(id, out ent)) {
                ent = new Entity(id);
            }
            return ent;
        }

        public static T Get<T>(int id) where T : Component {
            T t;
            IdMap<T>.Map.TryGetValue(id, out t);
            return t;
        }

        public static IEnumerable<T> GetAll<T>() where T : Component {
            return IdMap<T>.Map.Values;
        }

        public static bool Has<T>(int id) where T : Component {
            return IdMap<T>.Map.ContainsKey(id);
        }

        public static T Add<T>(int id) where T : Component {
            T t = (T)Activator.CreateInstance(typeof(T), id);
            IdMap<T>.Map.Add(id, t);
            foreach (Processor module in Engine.Processors) module.Reveille(t);
            return t;
        }

        public static bool Remove<T>(int id) where T : Component {
            foreach (Processor module in Engine.Processors) module.Taps(IdMap<T>.Map[id]);
            return IdMap<T>.Map.Remove(id);
        }

        public readonly int Id;

        private Entity(int id) {
            Id = id;
            IdMap<Entity>.Map.Add(id, this);
        }

        public Entity() : this(Allocate()) {
            
        }

        public T Get<T>() where T : Component {
            return Get<T>(Id);
        }

        public bool Has<T>() where T : Component {
            return Has<T>(Id);
        }

        public T Add<T>() where T : Component {
            return Add<T>(Id);
        }
        public bool Remove<T>() where T : Component {
            return Remove<T>(Id);
        }
    }
}
