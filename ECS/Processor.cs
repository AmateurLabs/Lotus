using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Processor {

        public virtual void Update(float dt) { }
        public virtual void Render() { }
        public virtual void Reveille(Component component) { }
        public virtual void Taps(Component component) { }
    }
}
