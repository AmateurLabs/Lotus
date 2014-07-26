using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public abstract class Light : Component {

        protected static List<DirectionalLight> dirLights = new List<DirectionalLight>();
        protected static List<PointLight> pointLights = new List<PointLight>();

        public static Color4 AmbientColor = new Color4(0.125f, 0.125f, 0.125f, 1f);

        public static Color4 GetColor(Vector3 normal, Vector3 pos, Color4 baseColor) {
            Color4 result = AmbientColor;
            normal.NormalizeFast();
            foreach (DirectionalLight light in dirLights) {
                Vector3 dir = -light.Direction.Value;
                dir.NormalizeFast();
                float dot = Vector3.Dot(normal, dir);
                if (dot < 0f) continue;
                dot *= light.Intensity.Value;
                result.R += light.Color.Value.R * dot * baseColor.R;
                result.G += light.Color.Value.G * dot * baseColor.G;
                result.B += light.Color.Value.B * dot * baseColor.B;
            }
            foreach (PointLight light in pointLights) {
                Vector3 lightPos = light.CachedPosition;
                /*if (Entity.Has<Transform>(light.Id)) lightPos = Entity.Get<Transform>(light.Id).Position.Value;*/
                float dist = (pos - lightPos).LengthFast;
                float attenuation = light.Intensity.Value / (1f + (2f / light.Radius.Value) * dist + (1f / (light.Radius.Value * light.Radius.Value)) * dist * dist);
                result.R += light.Color.Value.R * attenuation * baseColor.R;
                result.G += light.Color.Value.G * attenuation * baseColor.G;
                result.B += light.Color.Value.B * attenuation * baseColor.B;

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

        public Vector3 CachedPosition = Vector3.Zero;

        public Color4Value Color;
        public FloatValue Intensity;

        public Light(int id)
            : base(id) {
                Color = new Color4Value(this, "Color", Color4.White);
                Intensity = new FloatValue(this, "Intensity", 1f);
        }
    }

    public class PointLight : Light {

        public FloatValue Radius;

        public PointLight(int id) : base(id) {
            Radius = new FloatValue(this, "Radius", 1f);
            pointLights.Add(this);
        }
    }

    public class DirectionalLight : Light {

        public Vector3Value Direction;

        public DirectionalLight(int id) : base(id) {
            Direction = new Vector3Value(this, "Direction", Vector3.UnitY);
            dirLights.Add(this);
        }
    }
}
