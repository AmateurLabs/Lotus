﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class Engine {

        public static List<Module> Modules = new List<Module>();

        public static void Update(float dt) {
            foreach (Module module in Modules) module.Update(dt);
        }

        public static void Render() {
            foreach (Module module in Modules) module.Render();
        }
    }
}