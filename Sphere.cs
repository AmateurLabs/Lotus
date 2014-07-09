using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus
{
    class Sphere : Mesh
    {
        private Vector3[,] circles;
        private Vector3 north, south;
        private int sec=18;

        public Sphere(float radius, Vector3 position, Quaternion rotation) : base(position, rotation)
        { //Creates a new camera, using the width and height of the screen and whether it is orthographic
            circles = new Vector3[sec-1, 2*sec];
            for (int i = 1; i < sec; i++)
            {
                float rad = (float)Math.Sin(i * Math.PI / sec) * radius;
                float y = (float)Math.Cos(i * Math.PI / sec) * radius;
                for (int j = 0; j < 2*sec; j++)
                {
                    float x = (float)(Math.Cos(j * Math.PI / sec) * rad);
                    float z = (float)(Math.Sin(j * Math.PI / sec) * rad);
                    circles[i-1, j] = new Vector3(x, y, z);
                }

            }
            north = new Vector3(0f, radius, 0f);
            south = new Vector3(0f, -radius, 0f);
        }

        public override void RenGen()
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color4(Light.GetColor(ToWorldSpace(north), Color4.White));
            GL.Vertex3(north);// rendering the north pole of the sphere
            for (int i = 0; i < 2*sec; i++)
            {
                GL.Color4(Light.GetColor(ToWorldSpace(circles[0, i]), Color4.White));
                GL.Vertex3(circles[0, i]);
            }
            GL.Color4(Light.GetColor(ToWorldSpace(circles[0,0]), Color4.White));
            GL.Vertex3(circles[0, 0]);
            GL.End();

            //rendering the south pole of the sphere
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color4(Light.GetColor(ToWorldSpace(south), Color4.White));
            GL.Vertex3(south);
            for (int i = 0; i < 2*sec; i++)
            {
                GL.Color4(Light.GetColor(ToWorldSpace(circles[sec-2, i]), Color4.White));
                GL.Vertex3(circles[sec-2, i]);
            }
            GL.Color4(Light.GetColor(ToWorldSpace(circles[sec-2, 0]), Color4.White));
            GL.Vertex3(circles[sec-2, 0]);
            GL.End();
            //rendering the middle part

            GL.Begin(PrimitiveType.TriangleStrip);
            for (int row = 0; row < sec-2; row++)
            {
                int n = 0;
                int m = 0;
                int c = 0;


                GL.Color4(Light.GetColor(ToWorldSpace(circles[row, sec*2-1]), Color4.White));
                GL.Vertex3(circles[row, sec*2-1]);
                for (int i = 0; i < sec*2-1; i++)
                {
                    GL.Color4(Light.GetColor(ToWorldSpace(circles[row, n]), Color4.White));
                    GL.Vertex3(circles[row, n]);
                    GL.Color4(Light.GetColor(ToWorldSpace(circles[row+1, m]), Color4.White));
                    GL.Vertex3(circles[row + 1, m++]);
                    GL.Color4(Light.GetColor(ToWorldSpace(circles[row+1, m]), Color4.White));
                    GL.Vertex3(circles[row + 1, m]);
                    GL.Color4(Light.GetColor(ToWorldSpace(circles[row, n]), Color4.White));
                    GL.Vertex3(circles[row, n++]);
                }
                GL.Color4(Light.GetColor(ToWorldSpace(circles[row, n]), Color4.White));
                GL.Vertex3(circles[row, n]);
                GL.Color4(Light.GetColor(ToWorldSpace(circles[row + 1, m]), Color4.White));
                GL.Vertex3(circles[row + 1, m]);
                GL.Color4(Light.GetColor(ToWorldSpace(circles[row + 1, 0]), Color4.White));
                GL.Vertex3(circles[row + 1, 0]);
                //GL.Color4(Light.GetColor(ToWorldSpace(circles[row, n]), Color4.White));
                //GL.Vertex3(circles[row, 0]);
                
                //GL.End();
            }
            //GL.Vertex3(circles[sec-2, 0]);
            GL.End();

        }


    }
}
        
