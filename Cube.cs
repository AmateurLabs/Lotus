using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

namespace Lotus
{
    class Cube: Mesh
    {
        Vector3[] points = new Vector3[8];

        public Cube(Vector3 loc, Quaternion rot, float scl) : base(loc, rot)
        {
            points[0] = new Vector3(scl, scl, scl);
            points[1] = new Vector3(scl, scl, -scl);
            points[2] = new Vector3(scl, -scl, scl);
            points[3] = new Vector3(scl, -scl, -scl);
            points[4] = new Vector3(-scl, scl, scl);
            points[5] = new Vector3(-scl, scl, -scl);
            points[6] = new Vector3(-scl, -scl, scl);
            points[7] = new Vector3(-scl, -scl, -scl);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] += loc;
            } 
        }

        public override void RenGen()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Light.GetColor(ToWorldSpace(Vector3.UnitX), ToWorldSpace(Vector3.UnitX), Color4.White));
            GL.Vertex3(points[0]);
            GL.Vertex3(points[1]);
            GL.Vertex3(points[3]);
            GL.Vertex3(points[2]);
            GL.Color4(Light.GetColor(ToWorldSpace(Vector3.UnitY), ToWorldSpace(Vector3.UnitY), Color4.White));
            GL.Vertex3(points[0]);
            GL.Vertex3(points[1]);
            GL.Vertex3(points[5]);
            GL.Vertex3(points[4]);
            GL.Color4(Light.GetColor(ToWorldSpace(Vector3.UnitZ), ToWorldSpace(Vector3.UnitZ), Color4.White));
            GL.Vertex3(points[0]);
            GL.Vertex3(points[2]);
            GL.Vertex3(points[6]);
            GL.Vertex3(points[4]);
            GL.Color4(Light.GetColor(ToWorldSpace(-Vector3.UnitX), ToWorldSpace(-Vector3.UnitX), Color4.White));
            GL.Vertex3(points[4]);
            GL.Vertex3(points[5]);
            GL.Vertex3(points[7]);
            GL.Vertex3(points[6]);
            GL.Color4(Light.GetColor(ToWorldSpace(-Vector3.UnitY), ToWorldSpace(-Vector3.UnitY), Color4.White));
            GL.Vertex3(points[2]);
            GL.Vertex3(points[3]);
            GL.Vertex3(points[7]);
            GL.Vertex3(points[6]);
            GL.Color4(Light.GetColor(ToWorldSpace(-Vector3.UnitZ), ToWorldSpace(-Vector3.UnitZ), Color4.White));
            GL.Vertex3(points[1]);
            GL.Vertex3(points[3]);
            GL.Vertex3(points[7]);
            GL.Vertex3(points[5]);
            GL.End();
            /*GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(points[0]);
            GL.Vertex3(points[1]);
            GL.Vertex3(points[5]);
            GL.Vertex3(points[4]);
            GL.Vertex3(points[6]);
            GL.Vertex3(points[2]);
            GL.Vertex3(points[3]);
            GL.Vertex3(points[1]);
            GL.End();

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(points[7]);
            GL.Vertex3(points[6]);
            GL.Vertex3(points[2]);
            GL.Vertex3(points[3]);
            GL.Vertex3(points[1]);
            GL.Vertex3(points[5]);
            GL.Vertex3(points[4]);
            GL.Vertex3(points[6]);
            GL.End();*/
        }
    }
}
