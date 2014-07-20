using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS {
    public class Transform : Component {

        public Vector3Value Position;
        public QuaternionValue Rotation;
        public Vector3Value Scale;

        public Transform(int id) : base(id) {
            Position = new Vector3Value(this, "Position", Vector3.Zero);
            Rotation = new QuaternionValue(this, "Rotation", Quaternion.Identity);
            Scale = new Vector3Value(this, "Scale", Vector3.One);
        }

        public Matrix4 ViewMatrix {
            get {
                return ScalingMatrix * RotationMatrix * TranslationMatrix;
            }
        }

        public Matrix4 TranslationMatrix {
            get {
                return Matrix4.CreateTranslation(Position.Value);
            }
        }

        public Matrix4 RotationMatrix {
            get {
                return Matrix4.CreateRotationZ(Rotation.Value.Z) * Matrix4.CreateRotationX(Rotation.Value.X) * Matrix4.CreateRotationY(Rotation.Value.Y);
            }
        }

        public Matrix4 ScalingMatrix {
            get {
                return Matrix4.CreateScale(Scale.Value);
            }
        }

        public Vector3 Forward { //The direction the entity is facing in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitZ, RotationMatrix);
            }
        }

        public Vector3 Right { //The direction to the right of the entity in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitX, RotationMatrix);
            }
        }

        public Vector3 Up { //The direction to the top of the entity in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitY, RotationMatrix);
            }
        }

        public Vector3 ToWorldPoint(Vector3 p) {
            return Vector3.Transform(p, ViewMatrix);
        }

        public Vector3 ToWorldNormal(Vector3 n) {
            return Vector3.TransformNormal(n, RotationMatrix).Normalized();
        }
    }
}
