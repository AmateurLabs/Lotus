using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;

namespace Lotus.ECS {
    public class FreelookProcessor : Processor {

        public override void Update(float dt) {
            if (!Window.Main.CursorVisible && Window.Main.Focused) {
                Vector2 delta = Input.MouseDelta;
                foreach (Freelook look in Entity.GetAll<Freelook>()) {
                    if (!Entity.Has<Transform>(look.Id)) continue;
                    Transform t = Entity.Get<Transform>(look.Id);
                    float amt = dt * look.MoveSpeed.Value;
                    if (Input.IsDown(Key.W)) Move(t, 0f, 0f, amt);
                    if (Input.IsDown(Key.S)) Move(t, 0f, 0f, -amt);
                    if (Input.IsDown(Key.A)) Move(t, -amt, 0, 0f);
                    if (Input.IsDown(Key.D)) Move(t, amt, 0, 0f);
                    if (Input.IsDown(Key.Q)) Move(t, 0f, amt, 0f);
                    if (Input.IsDown(Key.E)) Move(t, 0f, -amt, 0f);
                    Rotate(t, -delta.Y * look.RotateSpeed.Value, -delta.X * look.RotateSpeed.Value, 0f); //Flipped because moving the mouse horizontally actually rotates on the Y axis, etc.
                }
            }
        }

        public void Move(Transform t, float x, float y, float z) {
            var rot = t.RotationMatrix;
            t.Position.Value -= Vector3.TransformPosition(Vector3.UnitX, rot) * x;
            t.Position.Value -= Vector3.TransformPosition(Vector3.UnitY, rot) * y;
            t.Position.Value -= Vector3.TransformPosition(Vector3.UnitZ, rot) * z;
        }

        public void Rotate(Transform t, float x, float y, float z) {
            t.Rotation.Value -= Quaternion.FromAxisAngle(Vector3.UnitX, x); //Yaw
            t.Rotation.Value -= Quaternion.FromAxisAngle(Vector3.UnitY, y); //Pitch
            t.Rotation.Value -= Quaternion.FromAxisAngle(Vector3.UnitZ, z); //Roll
        }
    }
}
