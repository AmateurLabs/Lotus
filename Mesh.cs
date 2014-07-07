using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus
{
    public abstract class Mesh
    {
        Vector3 Position;
        Quaternion Rotation;

        public Mesh(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public abstract void RenGen();// here you would program the GL to render the object at Vector3.Zero and as Quaternion.Identity for the rotation.

        public Matrix4 ViewMatrix
        { //The final view matrix used to draw the object
            get
            {
                return RotationMatrix * TranslationMatrix;
            }
        }

        public Matrix4 TranslationMatrix
        { //A matrix of the current position
            get
            {
                return Matrix4.CreateTranslation(Position);
            }
        }

        public Matrix4 RotationMatrix
        { //A matrix of the current rotation
            get
            {
                return Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y);
            }
        }

        public void Draw()
        {
            GL.PushMatrix();
            var viewMatrix = ViewMatrix;
            GL.MultMatrix(ref viewMatrix);
            RenGen();
            GL.PopMatrix();
        }
        
    }
}
