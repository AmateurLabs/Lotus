using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public class RenderProcessor : Processor {

        public override void Render() {
            foreach (Camera cam in Entity.GetAll<Camera>()) {
                if (Entity.Has<Transform>(cam.Id))
                    cam.Begin(Entity.Get<Transform>(cam.Id).ViewMatrix);
                else
                    cam.Begin(Matrix4.Identity);

                foreach (Renderer r in Entity.GetAll<Renderer>()) {
                    if ((r.Layers & cam.Layers) == 0) continue; //If the camera and renderer use different layers, don't draw
                    Matrix4 viewMatrix = Matrix4.Identity;
                    Matrix4 normalMatrix = Matrix4.Identity;
                    if (Entity.Has<Transform>(r.Id)) {
                        Transform t = Entity.Get<Transform>(r.Id);
                        viewMatrix = t.ViewMatrix;
                        normalMatrix = t.ScalingMatrix * t.RotationMatrix;
                    }
                    if (Entity.Has<MeshFilter>(r.Id)) { //If there is a Mesh aspect, draw that
                        Entity.Get<MeshFilter>(r.Id).Mesh.Draw(viewMatrix, normalMatrix);
                    }
                    else { //Otherwise, draw an XYZ axis gizmo so we can see where it is
                        GL.PushMatrix();
                        
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
                cam.End();
            }
        }
    }
}
