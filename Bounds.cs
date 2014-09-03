using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus {
    public class Bounds {
        public Vector3 Min;
        public Vector3 Max;

        public Bounds(Vector3 min, Vector3 max) {
            this.Min = min;
            this.Max = max;
        }

        public Vector3[] GetCorners() {
            return new Vector3[]{
                new Vector3(Min.X, Min.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Max.Z)
            };
        }
    }
}
