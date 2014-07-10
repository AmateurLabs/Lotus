﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Lotus.ECS.Aspects {
    public class ATransform : Aspect {

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        public ATransform(int id) : base(id) { }

        public Matrix4 ViewMatrix {
            get {
                return ScalingMatrix * RotationMatrix * TranslationMatrix;
            }
        }

        public Matrix4 TranslationMatrix {
            get {
                return Matrix4.CreateTranslation(Position);
            }
        }

        public Matrix4 RotationMatrix {
            get {
                return Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y);
            }
        }

        public Matrix4 ScalingMatrix {
            get {
                return Matrix4.CreateScale(Scale);
            }
        }
    }
}
