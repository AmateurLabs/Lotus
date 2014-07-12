using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Component {

        public readonly int Id;

        public Component(int id) {
            Id = id;
        }
    }
}
