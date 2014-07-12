using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Engine {

        public static List<Processor> Processors = new List<Processor>();

        public static void Update(float dt) {
            foreach (Processor processor in Processors) processor.Update(dt);
        }

        public static void Render() {
            foreach (Processor processor in Processors) processor.Render();
        }
    }
}
