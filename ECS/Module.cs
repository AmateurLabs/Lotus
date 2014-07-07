using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Module {

        public virtual void Update() { }
        public virtual void Render() { }
    }
}
