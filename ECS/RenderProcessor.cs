using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
    public class RenderProcessor : Processor {

        public override void Render() {
            int culled = 0;
            foreach (PointLight light in Entity.GetAll<PointLight>()) {
                if (Entity.Has<Transform>(light.Id)) {
                    light.CachedPosition = Entity.Get<Transform>(light.Id).Position.Value;
                }
            }
            foreach (Camera cam in Entity.GetAll<Camera>()) {
                Matrix4 camMatrix = Matrix4.Identity;
                Vector3 camFwd = Vector3.UnitZ;
                Vector3 camPos = Vector3.Zero;
                if (Entity.Has<Transform>(cam.Id)) {
                    Transform camTrans = Entity.Get<Transform>(cam.Id);
                    cam.Begin(camTrans.ViewMatrix);
                    camMatrix = camTrans.ViewMatrix;
                    camFwd = camTrans.Forward;
                    camPos = camTrans.Position.Value;
                }
                else {
                    cam.Begin(Matrix4.Identity);
                }
                foreach (Renderer r in Entity.GetAll<Renderer>()) {
                    if ((r.LayerMask.Value & cam.LayerMask.Value) == 0) continue; //If the camera and renderer use different layers, don't draw
                    Matrix4 viewMatrix = Matrix4.Identity;
                    Matrix4 normalMatrix = Matrix4.Identity;
                    if (Entity.Has<Transform>(r.Id)) {
                        Transform t = Entity.Get<Transform>(r.Id);
                        viewMatrix = t.ViewMatrix;
                        normalMatrix = t.ScalingMatrix * t.RotationMatrix;
                    }
                    if (Entity.Has<MeshFilter>(r.Id)) { //If there is a Mesh component, draw that
                        MeshFilter mf = Entity.Get<MeshFilter>(r.Id);
                        if (mf.Mesh.Value != null) {
                            bool cull = true;
                            Bounds b = mf.Mesh.Value.GetBounds();
                            GL.PushMatrix();
                            GL.Translate(b.Min + (b.Max - b.Min) / 2f);
                            //new Sphere(((b.Max - b.Min)/2f).LengthFast).Draw(viewMatrix, normalMatrix, OpenTK.Graphics.Color4.Magenta); //Draw bounding boxes as spheres
                            GL.PopMatrix();
                            foreach (Vector3 corner in b.GetCorners()) {
                                Vector3 t = Vector3.Transform(corner, viewMatrix);
                                Vector3 diff = (t - camPos);
                                if (cam.IsPerspective && diff.LengthFast > cam.ViewDistance.Value) continue; //Out of viewing range, no need to check against FoV
                                double angle = Math.Acos(Vector3.Dot(camFwd, diff.Normalized())); //Get angle between camera forward direction and direction to bound corner
                                if (cam.IsPerspective && angle < MathHelper.DegreesToRadians(cam.FieldOfView.Value)) { //If angle is inside field of view, don't cull
                                    cull = false;
                                    break;
                                }
                            }
                            if (!cull)
                                mf.Mesh.Value.Draw(viewMatrix, normalMatrix, mf.Color.Value);
                            else
                                culled++;
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
            Debug.AddMsg("Culled: " + culled);
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
