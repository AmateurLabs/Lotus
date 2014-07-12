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
        Color4 BaseColor;

        public Mesh()
        {
            BaseColor = Color4.White;
        }

        public abstract void RenGen(); // here you would program the GL to render the object at Vector3.Zero and as Quaternion.Identity for the rotation.

        private Matrix4 viewMatrix;
        private Matrix4 normalMatrix;

        public Vector3 ToWorldPoint(Vector3 p) {
            return Vector3.Transform(p, viewMatrix);
        }

        public Vector3 ToWorldNormal(Vector3 n) {
            return Vector3.TransformNormal(n, normalMatrix);
        }

        public Color4 GetColor(Vector3 vertex, Vector3 normal) {
            if (Camera.Current.UseLighting) {
                return Light.GetColor(ToWorldNormal(normal), ToWorldPoint(vertex), BaseColor);
            }
            else {
                return BaseColor;
            }
        }

        public void DrawVertex(Vector3 vertex, Vector3 normal) {
            GL.Color4(GetColor(vertex, normal));
            GL.Vertex3(vertex);
        }

        public void Draw(Matrix4 viewMatrix, Matrix4 normalMatrix)
        {
            GL.PushMatrix();
            this.viewMatrix = viewMatrix;
            this.normalMatrix = normalMatrix;
            GL.MultMatrix(ref viewMatrix);
            RenGen();
            GL.PopMatrix();
        }
    }
}
