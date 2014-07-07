using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public abstract class Aspect {

        public readonly int Id;

        public Aspect(int id) {
            Id = id;
        }
    }
}
