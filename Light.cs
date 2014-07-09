using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
    public class Light {

        public static Color4 AmbientColor = new Color4(0.125f, 0.125f, 0.125f, 1f);
        public static List<Light> List = new List<Light>();

        public static Color4 GetColor(Vector3 normal, Color4 baseColor) {
            normal.Normalize();
            Color4 result = AmbientColor;
            foreach (Light light in List) {
                float dot = Vector3.Dot(normal, -light.Direction);
                dot *= light.Intensity;
                result.R += light.Color.R * dot * baseColor.R;
                result.G += light.Color.G * dot * baseColor.G;
                result.B += light.Color.B * dot * baseColor.B;
            }
            //Pythagorean theorem for addition?
            return result;
        }

        public Vector3 Direction;
        public Color4 Color;

        public float Intensity {
            get { return Color.A; }
            set { Color.A = value; }
        }

        public Light(Vector3 direction, Color4 color) {
            Direction = direction;
            Color = color;
        }

        public Light(Vector3 direction, Color4 color, float intensity) {
            Direction = direction;
            Color = color;
            color.A = intensity;
        }
    }
}
