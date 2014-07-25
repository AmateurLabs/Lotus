using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Lotus.ECS;

namespace Lotus
{
    public abstract class Mesh
    {

        public abstract void RenGen(); // here you would program the GL to render the object at Vector3.Zero and as Quaternion.Identity for the rotation.

        private Matrix4 viewMatrix;
        private Matrix4 normalMatrix;

        public Vector3 ToWorld(Vector3 p) {
            return Vector3.Transform(p, viewMatrix);
        }

        protected Color4 baseColor;

        public Color4 GetColor(Vector3 vertex, Vector3 normal) {
            if (Camera.Current.UseLighting.Value) {
                return Light.GetColor(ToWorld(normal), ToWorld(vertex), baseColor);
            }
            else {
                return baseColor;
            }
        }

        public void DrawVertex(Vector3 vertex, Vector3 normal) {
            GL.Color4(GetColor(vertex, normal));
            GL.Vertex3(vertex);
        }

        public virtual void Update() { }

        public void Draw(Matrix4 viewMatrix, Matrix4 normalMatrix, Color4 baseColor)
        {
            GL.PushMatrix();
            this.viewMatrix = viewMatrix;
            this.normalMatrix = normalMatrix;
            this.baseColor = baseColor;
            GL.MultMatrix(ref viewMatrix);
            RenGen();
            GL.PopMatrix();
        }
    }
}
