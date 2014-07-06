using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Engine {

        public List<Module> Modules = new List<Module>();

        public void Update() {
            foreach (Module module in Modules) module.Update();
        }
    }
}
