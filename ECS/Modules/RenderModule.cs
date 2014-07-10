using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Lotus.ECS.Aspects;

namespace Lotus.ECS.Modules {
    public class RenderModule : Module {

        public override void Render() {
            foreach (ARenderer r in IdMap<ARenderer>.Map.Values) {
                GL.PushMatrix();
                ATransform t = Entity.Get<ATransform>(r.Id);
                Matrix4 viewMatrix = t.ViewMatrix;
                GL.MultMatrix(ref viewMatrix);
                if (Entity.Has<AMesh>(r.Id)) {
                    Entity.Get<AMesh>(r.Id).Mesh.Draw();
                }
                else {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(1f, 0f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Color3(1f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 0f);
                    GL.Color3(0f, 1f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Color3(0f, 1f, 0f);
                    GL.Vertex3(0f, 1f, 0f);
                    GL.Color3(0f, 0f, 1f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Color3(0f, 0f, 1f);
                    GL.Vertex3(0f, 0f, 1f);
                    GL.End();
                }
                //GL.Translate(t.Position);
                //Vector3 axis;
                //float angle;
                //t.Rotation.ToAxisAngle(out axis, out angle);
                //GL.Rotate(MathHelper.DegreesToRadians(angle), axis);
                //new Sphere(1f, t.Position, t.Rotation).Draw();
                GL.PopMatrix();
            }
        }
    }
}
