using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

namespace Lotus
{
    class Cube
    {
        Vector3[] points = new Vector3[8];

        public Cube(Vector3 loc, float scl){
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

        public void Draw()
        {
            Debug.DrawTri(points[0], points[3], points[1], Color4.White);
            Debug.DrawTri(points[0], points[3], points[2], Color4.Red);
            Debug.DrawTri(points[0], points[5], points[1], Color4.Blue);
            Debug.DrawTri(points[0], points[5], points[4], Color4.Green);
            Debug.DrawTri(points[0], points[6], points[2], Color4.Yellow);
            Debug.DrawTri(points[0], points[6], points[4], Color4.Purple);
            Debug.DrawTri(points[1], points[7], points[3], Color4.Cyan);
            Debug.DrawTri(points[1], points[7], points[5], Color4.Magenta);
            Debug.DrawTri(points[2], points[7], points[3], Color4.Pink);
            Debug.DrawTri(points[2], points[7], points[6], Color4.Black);
            Debug.DrawTri(points[4], points[7], points[6], Color4.Maroon);
            Debug.DrawTri(points[4], points[7], points[5], Color4.Violet);
        }
    }
}
