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
            Color4 result = AmbientColor;
            foreach (Light light in List) {
                if (light is DirectionalLight) {
                    float dot = Vector3.Dot(normal, -((DirectionalLight)light).Direction);
                    if (dot < 0f) continue;
                    dot *= light.Intensity;
                    result.R += light.Color.R * dot * baseColor.R;
                    result.G += light.Color.G * dot * baseColor.G;
                    result.B += light.Color.B * dot * baseColor.B;
                }
                else if (light is PointLight) {
                    PointLight pLight = light as PointLight;
                    float dist = (pos - pLight.Position).Length;
                    float attenuation = light.Intensity / (1f + (2f / pLight.Radius) * dist + (1f / (pLight.Radius * pLight.Radius)) * dist * dist);
                    result.R += light.Color.R * attenuation * baseColor.R;
                    result.G += light.Color.G * attenuation * baseColor.G;
                    result.B += light.Color.B * attenuation * baseColor.B;

                    Debug.DrawLater(() => {
                        GL.Begin(PrimitiveType.Lines);
                        GL.Color4(Color4.Magenta);
                        GL.Vertex3(pos);
                        GL.Vertex3(pLight.Position);
                        GL.Color4(Color4.White);
                        GL.End();
                    });
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

        public Light() {
            Color = Color4.White;
        }

        public Light(Color4 color) {
            Color = color;
        }

        public Light(Color4 color, float intensity) {
            Color = color;
            Color.A = intensity;
        }
    }

    public class PointLight : Light {

        public Vector3 Position;
        public float Radius;

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
            Direction = direction.Normalized();
        }

        public DirectionalLight(Vector3 direction, Color4 color, float intensity)
            : base(color, intensity) {
            Direction = direction.Normalized();
        }
    }
}
