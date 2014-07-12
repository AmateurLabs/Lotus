using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public class RenderProcessor : Processor {

        public override void Render() {
            foreach (Renderer r in IdMap<Renderer>.Map.Values) {
                if (!Entity.Has<Transform>(r.Id)) continue;
                Transform t = Entity.Get<Transform>(r.Id);
                if (Entity.Has<MeshFilter>(r.Id)) { //If there is a Mesh aspect, draw that
                    Entity.Get<MeshFilter>(r.Id).Mesh.Draw(t.ViewMatrix, t.ScalingMatrix*t.RotationMatrix);
                }
                else { //Otherwise, draw an XYZ axis gizmo so we can see where it is
                    GL.PushMatrix();
                    Matrix4 viewMatrix = t.ViewMatrix;
                    GL.MultMatrix(ref viewMatrix);
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
                    GL.PopMatrix();
                }
            }
        }
    }
}
