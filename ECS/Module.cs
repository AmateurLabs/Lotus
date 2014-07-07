using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Module {

        public virtual void Update(float dt) { }
        public virtual void Render() { }
        public virtual void Reveille(Aspect aspect) { }
        public virtual void Taps(Aspect aspect) { }
    }
}
