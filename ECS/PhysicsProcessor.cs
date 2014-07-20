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
            foreach (Rigidbody rb in Entity.GetAll<Rigidbody>()) {
                if (!Entity.Has<Transform>(rb.Id)) continue;
                Transform t = Entity.Get<Transform>(rb.Id);

                foreach (Attractor atr in Entity.GetAll<Attractor>()) {
                    Vector3 pos = Vector3.Zero;
                    Vector3 normal = atr.Normal.Value;
                    if (Entity.Has<Transform>(atr.Id)) {
                        pos = Entity.Get<Transform>(atr.Id).Position.Value;
                        normal = Entity.Get<Transform>(atr.Id).ToWorldNormal(normal);
                    }
                    if (atr.Type.Value == Attractor.AttractionType.World) {
                        rb.Velocity.Value += atr.Normal.Value * atr.Acceleration.Value * (atr.UseMass.Value ? rb.Mass.Value : 1f) * timeStep;
                    }
                    else if (atr.Type.Value == Attractor.AttractionType.Point) {
                        Vector3 diff = (t.Position.Value - pos);
                        float dist = diff.Length;
                        if (dist > 0f) dist = Math.Max(dist, 1f);
                        if (dist < 0f) dist = Math.Min(dist, -1f);
                        rb.Velocity.Value += diff.Normalized() * (atr.Acceleration.Value * (atr.UseMass.Value ? rb.Mass.Value : 1f) / (dist * dist)) * timeStep;
                        Console.WriteLine(diff.Normalized() * (atr.Acceleration.Value * (atr.UseMass.Value ? rb.Mass.Value : 1f) / (dist * dist)) * timeStep);

                    }
                    else if (atr.Type.Value == Attractor.AttractionType.Plane) {
                        float dist = Vector3.Dot(normal, pos - t.Position.Value);
                        float sign = (dist > 0f ? 1f : 0f);
                        dist = Math.Abs(dist);
                        rb.Velocity.Value += normal * sign * (atr.Acceleration.Value * (atr.UseMass.Value ? rb.Mass.Value : 1f) / (dist * dist)) * timeStep;
                    }
                }

                t.Position.Value += rb.Velocity.Value * timeStep;
                t.Rotation.Value *= Quaternion.FromMatrix(Matrix3.CreateRotationZ(rb.AngularVelocity.Value.Z * timeStep) * Matrix3.CreateRotationX(rb.AngularVelocity.Value.X * timeStep) * Matrix3.CreateRotationY(rb.AngularVelocity.Value.Y * timeStep));

                if (Entity.Has<Constraint>(rb.Id)) {
                    Constraint c = Entity.Get<Constraint>(rb.Id);
                    Vector3 pos = t.Position.Value;
                    pos.X = Math.Max(c.MinPosition.Value.X, Math.Min(c.MaxPosition.Value.X, pos.X));
                    pos.Y = Math.Max(c.MinPosition.Value.Y, Math.Min(c.MaxPosition.Value.Y, pos.Y));
                    pos.Z = Math.Max(c.MinPosition.Value.Z, Math.Min(c.MaxPosition.Value.Z, pos.Z));
                    t.Position.Value = pos;
                }
            }
        }
    }
}
