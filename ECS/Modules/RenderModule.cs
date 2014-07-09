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
            foreach (Renderer r in IdMap<Renderer>.Map.Values) {
                GL.PushMatrix();
                Transform t = Entity.Get<Transform>(r.Id);
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
