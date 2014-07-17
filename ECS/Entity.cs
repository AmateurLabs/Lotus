using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lotus.ECS.Internal;

namespace Lotus.ECS {
    public sealed class Entity {

        static Dictionary<int, Dictionary<string, Component>> entityHasFlags = new Dictionary<int, Dictionary<string, Component>>();
        static Dictionary<string, Dictionary<int, Component>> components = new Dictionary<string, Dictionary<int, Component>>();
        static int nextEntityId;

        public static int Allocate() {
            int id = nextEntityId++;
            return id;
        }

        public static Entity WrapNew() {
            return new Entity(Allocate());
        }

        public static Entity Wrap(int id) {
            return new Entity(id);
        }

        public static T Get<T>(int id) where T : Component {
            return (T)Get(typeof(T).Name, id);
        }

        public static Component Get(string type, int id) {
            Component c;
            components.GetOrCreate(type).TryGetValue(id, out c);
            return c;
        }

        public static IEnumerable<Component> GetAll<T>() where T : Component {
            return GetAll(typeof(T).Name);
        }

        public static IEnumerable<Component> GetAll(string type) {
            return components.GetOrCreate(type).Values;
        }

        public static bool Has<T>(int id) where T : Component {
            return Has(typeof(T).Name, id);
        }

        public static bool Has(string type, int id) {
            return entityHasFlags.GetOrCreate(id).ContainsKey(type);
        }

        public static T Add<T>(int id) where T : Component {
            return (T)Add(typeof(T).Name, id);
        }

        public static Component Add(string type, int id) {
            Component c = (Component)Activator.CreateInstance(Type.GetType("Lotus.ECS."+type, true), id);
            components.GetOrCreate(type).Add(id, c);
            entityHasFlags.GetOrCreate(id).Add(type, c);
            foreach (Processor processor in Engine.Processors) processor.Reveille(c);
            return c;
        }

        public static bool Remove<T>(int id) where T : Component {
            foreach (Processor module in Engine.Processors) module.Taps(IdMap<T>.Map[id]);
            IdMap<List<Component>>.Map[id].Remove(IdMap<T>.Map[id]);
            return IdMap<T>.Map.Remove(id);
        }

        public static bool Remove(string type, int id) {
            foreach (Processor processor in Engine.Processors) processor.Taps(components.GetOrCreate(type)[id]);
            entityHasFlags.GetOrCreate(id).Remove(type);
            return components.GetOrCreate(type).Remove(id);
        }

        public static void RemoveAll(int id) {
            var keys = entityHasFlags.GetOrCreate(id).Keys.ToArray();
            foreach (string type in keys) {
                Remove(type, id);
            }
        }

        public static void Export(int id) {
            string output = "";
            foreach (string type in entityHasFlags[id].Keys) {
                Component c = entityHasFlags[id][type];
                output += type + "\n";
                foreach (IValue value in c.Values) {
                    output += "  " + value.Name + "=" + value.Export() + "\n";
                }
            }
            System.IO.File.WriteAllText(id + ".ald", output);
        }

        public static void Import(int id) {
            RemoveAll(id);
            string[] lines = System.IO.File.ReadAllLines(id + ".ald");
            int i = 0;
            Component c;
            while (i < lines.Length) {
                c = Add(lines[i], id);
                i++;
                while (i < lines.Length && lines[i].StartsWith("  ")) {
                    string line = lines[i].Substring(2);
                    string[] bits = line.Split('=');
                    c.GetDataValue(bits[0]).Import(bits[1]);
                    i++;
                }
            }
        }

        public readonly int Id;

        private Entity(int id) {
            Id = id;
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

        public void Export() {
            Export(Id);
        }

        public void Import() {
            Import(Id);
        }
    }
}
