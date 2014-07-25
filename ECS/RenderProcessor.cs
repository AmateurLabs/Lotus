using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public class RenderProcessor : Processor {

        public override void Render() {
            foreach (PointLight light in Entity.GetAll<PointLight>()) {
                if (Entity.Has<Transform>(light.Id)) {
                    light.CachedPosition = Entity.Get<Transform>(light.Id).Position.Value;
                }
            }
            foreach (Camera cam in Entity.GetAll<Camera>()) {
                if (Entity.Has<Transform>(cam.Id))
                    cam.Begin(Entity.Get<Transform>(cam.Id).ViewMatrix);
                else
                    cam.Begin(Matrix4.Identity);

                foreach (Renderer r in Entity.GetAll<Renderer>()) {
                    if ((r.LayerMask.Value & cam.LayerMask.Value) == 0) continue; //If the camera and renderer use different layers, don't draw
                    Matrix4 viewMatrix = Matrix4.Identity;
                    Matrix4 normalMatrix = Matrix4.Identity;
                    if (Entity.Has<Transform>(r.Id)) {
                        Transform t = Entity.Get<Transform>(r.Id);
                        viewMatrix = t.ViewMatrix;
                        normalMatrix = t.ScalingMatrix * t.RotationMatrix;
                    }
                    if (Entity.Has<MeshFilter>(r.Id)) { //If there is a Mesh aspect, draw that
                        MeshFilter mf = Entity.Get<MeshFilter>(r.Id);
                        if (mf.Mesh.Value != null) {
                            mf.Mesh.Value.Draw(viewMatrix, normalMatrix, mf.Color.Value);
                        }
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

        public override void Update(float dt) {
            foreach (Renderer r in Entity.GetAll<Renderer>()) {
                MeshFilter filter = Entity.Get<MeshFilter>(r.Id);
                if (filter != null && filter.Mesh.Value != null) {
                    Entity.Get<MeshFilter>(r.Id).Mesh.Value.Update();
                }
            }
        }
    }
}
