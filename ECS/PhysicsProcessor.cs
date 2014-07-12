using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class PhysicsProcessor : Processor {

        float time = 0f;
        const float STEP = 0.025f;

        public override void Update(float dt) {
            time += dt;
            while (time > STEP) {
                time -= STEP;
                DoStuff(STEP);
            }
        }

        public void DoStuff(float timeStep) {
            foreach (Rigidbody rb in IdMap<Rigidbody>.Map.Values) {
                if (!Entity.Has<Transform>(rb.Id)) continue;
                Transform t = Entity.Get<Transform>(rb.Id);

                foreach (Attractor atr in IdMap<Attractor>.Map.Values) {
                    Vector3 pos = Vector3.Zero;
                    Vector3 normal = atr.Normal;
                    if (Entity.Has<Transform>(atr.Id)) {
                        pos = Entity.Get<Transform>(atr.Id).Position;
                        normal = Entity.Get<Transform>(atr.Id).ToWorldNormal(normal);
                    }
                    if (atr.Type == Attractor.AttractionType.World) {
                        rb.Velocity += atr.Normal * atr.Acceleration * (atr.UseMass ? rb.Mass : 1f) * timeStep;
                    }
                    else if (atr.Type == Attractor.AttractionType.Point) {
                        Vector3 diff = (t.Position - pos);
                        float dist = diff.Length;
                        if (dist > 0f) dist = Math.Max(dist, 1f);
                        if (dist < 0f) dist = Math.Min(dist, -1f);
                        rb.Velocity += diff.Normalized() * (atr.Acceleration * (atr.UseMass ? rb.Mass : 1f) / (dist * dist)) * timeStep;
                        Console.WriteLine(diff.Normalized() * (atr.Acceleration * (atr.UseMass ? rb.Mass : 1f) / (dist * dist)) * timeStep);

                    }
                    else if (atr.Type == Attractor.AttractionType.Plane) {
                        float dist = Vector3.Dot(normal, pos - t.Position);
                        float sign = (dist > 0f ? 1f : 0f);
                        dist = Math.Abs(dist);
                        rb.Velocity += normal * sign * (atr.Acceleration * (atr.UseMass ? rb.Mass : 1f) / (dist * dist)) * timeStep;
                    }
                }

                t.Position += rb.Velocity * timeStep;
                t.Rotation *= Quaternion.FromMatrix(Matrix3.CreateRotationZ(rb.AngularVelocity.Z * timeStep) * Matrix3.CreateRotationX(rb.AngularVelocity.X * timeStep) * Matrix3.CreateRotationY(rb.AngularVelocity.Y * timeStep));

                if (Entity.Has<Constraint>(rb.Id)) {
                    Constraint c = Entity.Get<Constraint>(rb.Id);
                    t.Position.X = Math.Max(c.MinPosition.X, Math.Min(c.MaxPosition.X, t.Position.X));
                    t.Position.Y = Math.Max(c.MinPosition.Y, Math.Min(c.MaxPosition.Y, t.Position.Y));
                    t.Position.Z = Math.Max(c.MinPosition.Z, Math.Min(c.MaxPosition.Z, t.Position.Z));
                }
            }
        }
    }
}
