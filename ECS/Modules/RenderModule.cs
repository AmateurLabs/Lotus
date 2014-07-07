using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lotus.ECS.Aspects;

namespace Lotus.ECS.Modules {
    public class RenderModule : Module {

        public override void Render() {
            foreach (Renderer r in IdMap<Renderer>.Map.Values) {

            }
        }
    }
}
