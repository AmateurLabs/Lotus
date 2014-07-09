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
            DrawVertex(points[0], Vector3.UnitX);
            DrawVertex(points[1], Vector3.UnitX);
            DrawVertex(points[3], Vector3.UnitX);
            DrawVertex(points[2], Vector3.UnitX);
            DrawVertex(points[0], Vector3.UnitY);
            DrawVertex(points[1], Vector3.UnitY);
            DrawVertex(points[5], Vector3.UnitY);
            DrawVertex(points[4], Vector3.UnitY);
            DrawVertex(points[0], Vector3.UnitZ);
            DrawVertex(points[2], Vector3.UnitZ);
            DrawVertex(points[6], Vector3.UnitZ);
            DrawVertex(points[4], Vector3.UnitZ);
            DrawVertex(points[4], -Vector3.UnitX);
            DrawVertex(points[5], -Vector3.UnitX);
            DrawVertex(points[7], -Vector3.UnitX);
            DrawVertex(points[6], -Vector3.UnitX);
            DrawVertex(points[2], -Vector3.UnitY);
            DrawVertex(points[3], -Vector3.UnitY);
            DrawVertex(points[7], -Vector3.UnitY);
            DrawVertex(points[6], -Vector3.UnitY);
            DrawVertex(points[1], -Vector3.UnitZ);
            DrawVertex(points[3], -Vector3.UnitZ);
            DrawVertex(points[7], -Vector3.UnitZ);
            DrawVertex(points[5], -Vector3.UnitZ);
            GL.End();
        }
    }
}
