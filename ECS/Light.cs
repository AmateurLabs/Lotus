using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public abstract class Light : Component {

        public Light(int id) : base(id) { }

        public static Color4 AmbientColor = new Color4(0.25f, 0.25f, 0.25f, 1f);

        public static Color4 GetColor(Vector3 normal, Vector3 pos, Color4 baseColor) {
            Color4 result = AmbientColor;
            foreach (DirectionalLight light in Entity.GetAll<DirectionalLight>()) {
                float dot = Vector3.Dot(normal, -light.Direction);
                if (dot < 0f) continue;
                dot *= light.Intensity;
                result.R += light.Color.R * dot * baseColor.R;
                result.G += light.Color.G * dot * baseColor.G;
                result.B += light.Color.B * dot * baseColor.B;
            }
            foreach (PointLight light in Entity.GetAll<PointLight>()) {
                Vector3 lightPos = Vector3.Zero;
                if (Entity.Has<Transform>(light.Id)) lightPos = Entity.Get<Transform>(light.Id).Position;
                float dist = (pos - lightPos).Length;
                float attenuation = light.Intensity / (1f + (2f / light.Radius) * dist + (1f / (light.Radius * light.Radius)) * dist * dist);
                result.R += light.Color.R * attenuation * baseColor.R;
                result.G += light.Color.G * attenuation * baseColor.G;
                result.B += light.Color.B * attenuation * baseColor.B;

                /*Debug.DrawLater(() => {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(Color4.Magenta);
                    GL.Vertex3(pos);
                    GL.Vertex3(lightPos);
                    GL.Color4(Color4.White);
                    GL.End();
                });*/
            }
            return result;
        }

        public Color4 Color = Color4.White;

        public float Intensity = 1f;
    }

    public class PointLight : Light {

        public float Radius = 1f;

        public PointLight(int id) : base(id) { }
    }

    public class DirectionalLight : Light {

        public Vector3 Direction = Vector3.UnitY;

        public DirectionalLight(int id) : base(id) { }
    }
}
