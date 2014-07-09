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
        Color4 BaseColor;

        public Mesh(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            BaseColor = Color4.White;
        }

        public Mesh(Vector3 position, Quaternion rotation, Color4 baseColor) : this(position, rotation) {
            BaseColor = baseColor;
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

        public Vector3 ToWorldSpace(Vector3 p) {
            return Vector3.Transform(p, ViewMatrix);
        }

        public Color4 GetColor(Vector3 vertex, Vector3 normal) {
            if (Camera.Current.UseLighting) {
                return Light.GetColor(ToWorldSpace(normal), ToWorldSpace(vertex), BaseColor);
            }
            else {
                return BaseColor;
            }
        }

        public void DrawVertex(Vector3 vertex, Vector3 normal) {
            GL.Color4(GetColor(vertex, normal));
            GL.Vertex3(vertex);
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
