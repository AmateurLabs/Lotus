using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;

namespace Lotus.ECS {
    public class FreelookProcessor : Processor {

        public Vector2 lastMousePos = new Vector2();

        public override void Update(float dt) {
            if (!Window.Main.CursorVisible && Window.Main.Focused) {
                Vector2 delta = lastMousePos - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                foreach (Freelook look in IdMap<Freelook>.Map.Values) {
                    if (!Entity.Has<Transform>(look.Id)) continue;
                    Transform t = Entity.Get<Transform>(look.Id);
                    float amt = dt * look.MoveSpeed;
                    if (Input.IsDown(Key.W)) Move(t, 0f, 0f, amt);
                    if (Input.IsDown(Key.S)) Move(t, 0f, 0f, -amt);
                    if (Input.IsDown(Key.A)) Move(t, -amt, 0, 0f);
                    if (Input.IsDown(Key.D)) Move(t, amt, 0, 0f);
                    if (Input.IsDown(Key.Q)) Move(t, 0f, amt, 0f);
                    if (Input.IsDown(Key.E)) Move(t, 0f, -amt, 0f);
                    Rotate(t, delta.Y * look.RotateSpeed, delta.X * look.RotateSpeed, 0f); //Flipped because moving the mouse horizontally actually rotates on the Y axis, etc.
                }
                Mouse.SetPosition(Window.Main.Bounds.Left + Window.Main.Bounds.Width / 2, Window.Main.Bounds.Top + Window.Main.Bounds.Height / 2);
            }
            lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public void Move(Transform t, float x, float y, float z) {
            var rot = t.RotationMatrix;
            t.Position -= Vector3.TransformPosition(Vector3.UnitX, rot) * x;
            t.Position -= Vector3.TransformPosition(Vector3.UnitY, rot) * y;
            t.Position -= Vector3.TransformPosition(Vector3.UnitZ, rot) * z;
        }

        public void Rotate(Transform t, float x, float y, float z) {
            t.Rotation -= Quaternion.FromAxisAngle(Vector3.UnitX, x); //Yaw
            t.Rotation -= Quaternion.FromAxisAngle(Vector3.UnitY, y); //Pitch
            t.Rotation -= Quaternion.FromAxisAngle(Vector3.UnitZ, z); //Roll
        }
    }
}
