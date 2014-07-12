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
                ATransform t = Entity.Get<ATransform>(r.Id);
                if (Entity.Has<AMesh>(r.Id)) { //If there is a Mesh aspect, draw that
                    Entity.Get<AMesh>(r.Id).Mesh.Draw(t.ViewMatrix, t.ScalingMatrix*t.RotationMatrix);
                }
                else { //Otherwise, draw an XYZ axis gizmo so we can see where it is
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
            }
        }
    }
}
