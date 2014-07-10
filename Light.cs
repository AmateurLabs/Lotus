using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
    public class Light {

        public static Color4 AmbientColor = new Color4(0.25f, 0.25f, 0.25f, 1f);
        public static List<Light> List = new List<Light>();

        public static Color4 GetColor(Vector3 normal, Vector3 pos, Color4 baseColor) {
            normal.Normalize();
            Color4 result = AmbientColor;
            foreach (Light light in List) {
                if (light is DirectionalLight) {
                    float dot = Vector3.Dot(normal, -((DirectionalLight)light).Direction);
                    dot *= light.Intensity;
                    result.R += light.Color.R * dot * baseColor.R;
                    result.G += light.Color.G * dot * baseColor.G;
                    result.B += light.Color.B * dot * baseColor.B;
                }
                else if (light is PointLight) {
                    PointLight pLight = light as PointLight;
                    /*float dot = Vector3.Dot(normal, -((PointLight)light).Direction(pos));
                    Vector3 difference;
                    Vector3.Subtract(ref pos, ref ((PointLight)light).Position, out difference);
                    light.Intensity = (float)Math.Sqrt(MathHelper.Clamp(((PointLight)light).Size * ((PointLight)light).Size - difference.Length*difference.Length, 0, Double.MaxValue));*/
                    float dist = (pos - pLight.Position).Length;
                    //if (dist > pLight.Radius) continue;
                    float attenuation = light.Intensity / (1f + (2f / pLight.Radius) * dist + (1f / (pLight.Radius * pLight.Radius)) * dist * dist);
                    result.R += light.Color.R * attenuation * baseColor.R;
                    result.G += light.Color.G * attenuation * baseColor.G;
                    result.B += light.Color.B * attenuation * baseColor.B;
                }
            }

            //Pythagorean theorem for addition?
            return result;
        }

        public Color4 Color;

        public float Intensity {
            get { return Color.A; }
            set { Color.A = value; }
        }

        public Light(Color4 color) {
            Color = color;
        }

        public Light(Color4 color, float intensity) {
            Color = color;
            color.A = intensity;
        }

    }

    public class PointLight : Light {
        public Vector3 Position;
        public float Radius;
        public Vector3 Direction(Vector3 pt) {
            Vector3 dir = new Vector3(pt.X - Position.X, pt.Y - Position.Y, pt.Z - Position.Z);
            dir.Normalize();
            return dir;

            //return Vector3.Zero;
        }
        public PointLight(Vector3 pos, Color4 color, float radius)
            : base(color) {
            Position = pos;
            Radius = radius;
        }
    }

    public class DirectionalLight : Light {
        public Vector3 Direction;
        public DirectionalLight(Vector3 direction, Color4 color)
            : base(color) {
            Direction = direction;

        }
        public DirectionalLight(Vector3 direction, Color4 color, float intensity)
            : base(color) {
            Direction = direction;
            color.A = intensity;

        }
    }
}
