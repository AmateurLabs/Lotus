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
        Vector3[,,] Points;
        int Res;
        float Scale;

        public Cube(Vector3 loc, Quaternion rot, float scl, int res) : base(loc, rot)
        {
           //min res is 3
            if (res < 0) res = -res;
            if (res < 3) res = 3;
            if (scl < 0) scl = -scl;
            PGen();
            
        }

        public override void RenGen()
        {
            Console.WriteLine("1");
            for (int i = 0; i < 6; i++)
            {
                Vector3 normal;
                if (i < 2) normal = Vector3.UnitX;
                else if (i < 4) normal = Vector3.UnitY;
                else normal = Vector3.UnitZ;
                if (i % 2 != 0) normal = -normal;

                for(int j=0; j<Res; j++){
                    int k;
                    k = 0;
                    while (k < Res)
                    {
                        GL.Begin(PrimitiveType.QuadStrip);
                        DrawVertex(Points[i, j, k], normal);
                        DrawVertex(Points[i, j + 1, k++], normal);
                        if (!(k<Res)) break;
                        DrawVertex(Points[i, j + 1, k], normal);
                        DrawVertex(Points[i, j, k++], normal);
                        GL.End();
                    }

                }


            }
            
        }

        public void PGen()
        {
            Points = new Vector3[6,Res,Res];

            float max_val = Scale / 2;
            float step = Scale / Res;
            //X, -X, Y, -Y, Z, -Z
            for (int i = 0; i < Res; i++)
            {
                for (int j = 0; j < Res; j++){
                    Points[0, i, j] = new Vector3(max_val, -max_val + step * i, -max_val + step * j);
                    Points[1, i, j] = new Vector3(-max_val, -max_val + step * i, -max_val + step * j);
                    Points[2, i, j] = new Vector3(-max_val + step * i, max_val, -max_val + step * j);
                    Points[3, i, j] = new Vector3(-max_val + step * i, -max_val, -max_val + step * j);
                    Points[2, i, j] = new Vector3(-max_val + step * i, -max_val + step * j, max_val);
                    Points[3, i, j] = new Vector3(-max_val + step * i, -max_val + step * j, -max_val);
                }
            }
        }
    }
}
