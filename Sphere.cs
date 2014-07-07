using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus
{
    class Sphere
    {
        private Vector3[,] circles;
        private Vector3 north, south;
        Matrix4 projectionMatrix; //The Matrix that determines whether the camera is orthographic, perspective, etc.
        public Vector3 Position; //The position in 3D space that the camera occupies
        public Quaternion Rotation; //The quaternion rotation of the camera, applied in YXZ order

        //TODO: move camera controls to separate class

        public Sphere(float radius, Vector3 position, Quaternion rotation)
        { //Creates a new camera, using the width and height of the screen and whether it is orthographic
            circles = new Vector3[8, 18];
            for (int i = 1; i < 9; i++)
            {
                float rad = (float)Math.Sin(i * Math.PI / 9) * radius;
                float y = (float)Math.Cos(i * Math.PI / 9) * radius;
                for (int j = 0; j < 18; j++)
                {
                    float x = (float)(Math.Cos(j * Math.PI / 9) * rad);
                    float z = (float)(Math.Sin(j * Math.PI / 9) * rad);
                    circles[i-1, j] = new Vector3(x, y, z);
                }

            }
            north = new Vector3(0f, radius, 0f);
            south = new Vector3(0f, -radius, 0f);
            Position = position;
            Rotation = rotation;
        }

        public Matrix4 ViewMatrix
        { //The final view matrix used to draw the world
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

        /*public Vector3 Forward { //The direction the camera is facing in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitZ, RotationMatrix);
            }
        }

        public Vector3 Right { //The direction to the right of the camera in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitX, RotationMatrix);
            }
        }

        public Vector3 Up { //The direction to the top of the camera in worldspace
            get {
                return Vector3.TransformPosition(-Vector3.UnitY, RotationMatrix);
            }
        }

        /*public void Move(float x, float y, float z) {
            var rot = RotationMatrix;
            Position -= Vector3.TransformPosition(Vector3.UnitX, rot) * x;
            Position -= Vector3.TransformPosition(Vector3.UnitY, rot) * y;
            Position -= Vector3.TransformPosition(Vector3.UnitZ, rot) * z;
        }

        public void Rotate(float x, float y, float z) {
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitX, x); //Yaw
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitY, y); //Pitch
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitZ, z); //Roll
        }*/

        public void Draw()
        {
            GL.PushMatrix();
            var viewMatrix = ViewMatrix;
            //viewMatrix.Invert();
            GL.MultMatrix(ref viewMatrix);
            RenGen();

            GL.PopMatrix();
            //GL.Ortho(-game.Width / 32.0, game.Width / 32.0, -game.Height / 32.0, game.Height / 32.0, 0.0, 4.0);
        }

        public void RenGen()
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color4(Color4.Wheat);
            GL.Vertex3(north);// rendering the north pole of the sphere
            for (int i = 0; i < 18; i++)
            {
                GL.Vertex3(circles[0, i]);
            }
            GL.Vertex3(circles[0, 0]);
            GL.End();

            //rendering the south pole of the sphere
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(south);
            for (int i = 0; i < 18; i++)
            {
                GL.Vertex3(circles[7, i]);
            }
            GL.Vertex3(circles[7, 0]);
            GL.End();

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Vertex3(circles[0, 17]);
            for (int row = 0; row < 7; row++)
            {
                int n = 0;
                int m = 0;
                int c = 0;
                
                for (int i = 0; i < 17; i++)
                {
                    
                    GL.Vertex3(circles[row, n]);
                    GL.Color4(Debug.RandColor(c++));
                    GL.Vertex3(circles[row + 1, m++]);
                    GL.Color4(Debug.RandColor(c++));
                    GL.Vertex3(circles[row + 1, m]);
                    GL.Color4(Debug.RandColor(c++));
                    GL.Vertex3(circles[row, n++]);
                    GL.Color4(Debug.RandColor(c++));

                }
                GL.Vertex3(circles[row, n]);
                GL.Color4(Debug.RandColor(c++));
                GL.Vertex3(circles[row + 1, m]);
                GL.Color4(Debug.RandColor(c++));
                //GL.Vertex3(circles[row, 0]);
                //GL.Vertex3(circles[row + 1, 0]);
            }
            GL.Vertex3(circles[7, 0]);
            GL.End();

        }


    }
}
        
