using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lotus.ECS.Aspects;

using Jitter;
using Jitter.Dynamics;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS.Modules {
    public class JitterModule : Module {

        World world;
        CollisionSystem collisionSystem;
        RigidBody floor;

        public JitterModule() {
            collisionSystem = new CollisionSystemSAP();
            world = new World(collisionSystem);
            world.Gravity = new JVector(world.Gravity.X, -world.Gravity.Y, world.Gravity.Z);
            BoxShape box = new BoxShape(100000f, 2f, 100000f);
            floor = new RigidBody(box);
            floor.IsStatic = true;
            world.AddBody(floor);
        }

        public override void Update(float dt) {
            world.Step(dt, false);
            foreach (AJitterBody body in IdMap<AJitterBody>.Map.Values) {
                ATransform t = Entity.Get<ATransform>(body.Id);
                t.Position = Convert(body.Rigidbody.Position);
                JQuaternion rot = JQuaternion.CreateFromMatrix(body.Rigidbody.Orientation);
                t.Rotation = Convert(rot);
            }
        }

        public override void Render() {
            foreach (AJitterBody body in IdMap<AJitterBody>.Map.Values) {
                body.Rigidbody.EnableDebugDraw = true;
                body.Rigidbody.DebugDraw(new DebugDraw());
            }
        }

        class DebugDraw : IDebugDrawer {

            public void DrawLine(JVector start, JVector end) {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(1f, 1f, 0f);
                GL.Vertex3(start.X, start.Y, start.Z);
                GL.Vertex3(end.X, end.Y, end.Z);
                GL.End();
            }

            public void DrawPoint(JVector pos) {
                GL.PointSize(4f);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(pos.X, pos.Y, pos.Z);
                GL.End();
            }

            public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3) {
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(1f, 0f, 1f);
                GL.Vertex3(pos1.X, pos1.Y, pos1.Z);
                GL.Vertex3(pos2.X, pos2.Y, pos2.Z);
                GL.Vertex3(pos3.X, pos3.Y, pos3.Z);
                GL.End();
            }
        }

        public override void Reveille(Aspect aspect) {
            if (aspect is AJitterBody) {
                AJitterBody body = aspect as AJitterBody;
                body.Rigidbody = new RigidBody(new SphereShape(1f));
                body.Rigidbody.Position = Convert(Entity.Get<ATransform>(aspect.Id).Position);
                world.AddBody(body.Rigidbody);
            }
        }

        public override void Taps(Aspect aspect) {
            if (aspect is AJitterBody) {
                AJitterBody body = aspect as AJitterBody;
                world.RemoveBody(body.Rigidbody);
            }
        }

        public static Vector3 Convert(JVector v) {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static JVector Convert(Vector3 v) {
            return new JVector(v.X, v.Y, v.Z);
        }

        public static Quaternion Convert(JQuaternion q) {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }
    }
}
