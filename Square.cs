using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

namespace Lotus
{
    class Square : Mesh
    {
        Vector3[] points = new Vector3[8];

        public Square(float scl)
        {
            points[0] = new Vector3(scl, scl, 0);
            points[1] = new Vector3(scl, -scl, 0);
            points[2] = new Vector3(-scl, scl, 0);
            points[3] = new Vector3(-scl, -scl, 0);
        }

        public override void RenGen()
        {
            GL.Begin(PrimitiveType.Quads);
            DrawVertex(points[0], -Vector3.UnitZ);
            DrawVertex(points[1], -Vector3.UnitZ);
            DrawVertex(points[3], -Vector3.UnitZ);
            DrawVertex(points[2], -Vector3.UnitZ);
            GL.End();
        }
    }
}
