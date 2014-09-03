using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SimplexNoise;

using Lotus.ECS;

namespace Lotus {

    public class HexGrid : Mesh {

        public readonly int OffX;
        public readonly int OffY;
        public readonly int Size;

        readonly int ChunkOffX;
        readonly int ChunkOffY;

        public const float HEX_HEIGHT = 1f;
        public const float HEX_WIDTH = 0.8660254037844386f;
        readonly byte[] VBOArr;
        readonly uint[] IBOArr;
        readonly int VBOID;
        readonly int IBOID;
        Vector3[] Vertices;
        Vector3[] Normals;

        public const int VERT_STRIDE = 32;

        /*
         * Vertex Layout: 
         * 0-3: X Position
         * 4-7: Y Position
         * 8-11: Z Position
         * 12: R
         * 13: G
         * 14: B
         * 15: A
         * 16-19: X Normal
         * 20-23: Y Normal
         * 24-27: Z Normal
         * 28-31: 
         */

        public HexGrid(int size, int offX, int offY) {
            Size = size;
            OffX = offX;
            OffY = offY;
            ChunkOffX = offX - size / 2;
            ChunkOffY = offY - size / 2;

            VBOArr = new byte[Size * Size * 7 * VERT_STRIDE];
            VBOID = GL.GenBuffer();
            IBOArr = new uint[Size * Size * 6 * 3];
            IBOID = GL.GenBuffer();

            Vertices = new Vector3[Size * Size * 7];
            Normals = new Vector3[Size * Size * 7];

            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    AddHex(ChunkOffX + x, ChunkOffY + y);
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(VBOArr.Length), VBOArr, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(IBOArr.Length * 4), IBOArr, BufferUsageHint.DynamicDraw);

            Window.Main.Closed += (sender, e) => {
                Dispose();
            };

            /*id = chunkId++;
            chunkColor = Debug.RandColor(id);
            if (firstChunk == null) {
                lastChunk = this;
                firstChunk = this;
                nextChunk = this;
                curChunk = this;
            }
            else {
                nextChunk = firstChunk;
                lastChunk.nextChunk = this;
                lastChunk = this;
            }*/
        }

        private int i = 0;
        private int v = 0;
        private int t = 0;

        public void AddVertex(float x, float z) {
            float y = GetHeight(x, z) * 8f;
            float dx = GetHeight(x - 0.5f, z) * 8f - GetHeight(x + 0.5f, z) * 8f;
            float dz = GetHeight(x, z - 0.5f) * 8f - GetHeight(x, z + 0.5f) * 8f;

            Vector3 vertex = new Vector3(x, y, z);
            Vector3 normal = new Vector3(-dx, -1f, -dz).Normalized();
            Color4 color = Light.GetColor(normal, vertex, chunkColor);

            Buffer.BlockCopy(BitConverter.GetBytes(x), 0, VBOArr, i + 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, VBOArr, i + 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(z), 0, VBOArr, i + 8, 4);
            VBOArr[i + 12] = (byte)(Math.Min(Math.Max(255f * color.R, 0f), 255f));
            VBOArr[i + 13] = (byte)(Math.Min(Math.Max(255f * color.G, 0f), 255f));
            VBOArr[i + 14] = (byte)(Math.Min(Math.Max(255f * color.B, 0f), 255f));
            VBOArr[i + 15] = 255;
            Buffer.BlockCopy(BitConverter.GetBytes(normal.X), 0, VBOArr, i + 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(normal.Y), 0, VBOArr, i + 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(normal.Z), 0, VBOArr, i + 24, 4);
            i += VERT_STRIDE;

            Normals[v] = normal;
            Vertices[v] = vertex;
            v++;
        }

        public static Vector3[] HexVerts = {
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, HEX_HEIGHT / 2f),
			new Vector3(HEX_WIDTH / 2f, 0f, HEX_HEIGHT / 4f),
			new Vector3(HEX_WIDTH / 2f, 0f, -HEX_HEIGHT / 4f),
			new Vector3(0f, 0f, -HEX_HEIGHT / 2f),
			new Vector3(-HEX_WIDTH / 2f, 0f, -HEX_HEIGHT / 4f),
			new Vector3(-HEX_WIDTH / 2f, 0f, HEX_HEIGHT / 4f),
		};

        public void AddHex(int mapX, int mapY) {
            float x = ToWorld(mapX, mapY).X;
            float z = ToWorld(mapX, mapY).Z;
            for (int j = 0; j < 7; j++) {
                AddVertex(x + HexVerts[j].X, z + HexVerts[j].Z);
            }
            if (OnGrid(mapX, mapY)) {
                for (int j = 1; j < 7; j++) {
                    int k = (j + 1);
                    if (k > 6) k = 1;
                    IBOArr[t] = (uint)(v - 7);
                    IBOArr[t + 1] = (uint)(v - 7 + j);
                    IBOArr[t + 2] = (uint)(v - 7 + k);
                    t += 3;
                }
            }
            else {
                IBOArr[t] = 0;
                IBOArr[t + 1] = 0;
                IBOArr[t + 2] = 0;
                t += 3;
            }
        }

        public bool OnGrid(int mapX, int mapY) {
            return HexDist(mapX, mapY, ChunkOffX + Size / 2, ChunkOffY + Size / 2) <= Size / 2;
        }

        public static float GetHeight(float x, float z) {
            return (Noise.Generate(x / 16f + 4096f, z / 16f + 4096f) + 1f) / 2f;
        }

        //Gets the array indices of the hex column that the given worldspace point is in
        public static Vector3 ToHex(float x, float z) {
            Vector3 hexPos = new Vector3();
            hexPos.Y = (float)Math.Round(z / 0.75f / HEX_HEIGHT);
            hexPos.X = (float)Math.Round((x - hexPos.Y * HEX_WIDTH / 2f) / HEX_WIDTH);
            return hexPos;
        }

        //Gets the worldspace location of the center vertex of the given hex column
        public static Vector3 ToWorld(int x, int y) {
            Vector3 worldPos = new Vector3(x * HEX_WIDTH + y * HEX_WIDTH / 2f, 0f, y * HEX_HEIGHT * 0.75f);
            worldPos.Y = GetHeight(worldPos.X, worldPos.Z) * 8f;
            return worldPos;
        }

        public static int HexDist(int x0, int y0, int x1, int y1) {
            int z0 = -(x0 + y0);
            int z1 = -(x1 + y1);
            int dX = Math.Abs(x1 - x0);
            int dY = Math.Abs(y1 - y0);
            int dZ = Math.Abs(z1 - z0);
            return Math.Max(dX, Math.Max(dY, dZ));
        }

        Color4 chunkColor = Color4.White;

        /*HexGrid nextChunk;
        static HexGrid curChunk;
        static HexGrid firstChunk;
        static HexGrid lastChunk;
        const int UPDATES_PER_FRAME = 1;
        static int updates;
        static int chunkId;
        int id;

        public override void Update() {
            if (curChunk == this) {
                if (updates == 0) {
                    updates = UPDATES_PER_FRAME;
                    return;
                }
                updates--;
                curChunk = nextChunk;

                int l = 0;
                for (int j = 0; j < Vertices.Length; j++) {
                    Color4 color = Light.GetColor(Normals[j], Vertices[j], chunkColor);
                    VBOArr[l + 12] = (byte)(color.R * 255f);
                    VBOArr[l + 13] = (byte)(color.G * 255f);
                    VBOArr[l + 14] = (byte)(color.B * 255f);
                    l += VERT_STRIDE;
                }
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(VBOArr.Length), VBOArr, BufferUsageHint.StreamDraw);
            }

            //Primitive way of updating VBO data without resending everything; probably impractical
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(32), new IntPtr(4), new float[] { (float)Math.Sin(time) });

            //Raycasting!
            Vector3 hex;
            if (Raycast(Camera.Main.Position, Camera.Main.Forward, out hex, 256f)) {
                Debug.DrawText(new Vector2(10f, 200f), "(" + hex.X + ", " + hex.Y + ")");
            }
        }*/

        public override void RenGen() {
            //We are drawing using vertex position data, color data, normal data, and triangle index data
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            //Bind vertex buffer and load each type of info from it
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
            GL.VertexPointer(3, VertexPointerType.Float, VERT_STRIDE, 0);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, VERT_STRIDE, 12);
            GL.NormalPointer(NormalPointerType.Float, VERT_STRIDE, 16);

            //Bind triangle index buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOID);

            //Draw polygons as specified by the triangle index buffer
            GL.DrawElements(PrimitiveType.Triangles, t, DrawElementsType.UnsignedInt, 0);

            //Stop using arrays so we don't contaminate future draws
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.IndexArray);
        }

        public override Bounds GetBounds() {
            Vector3 min = ToWorld(ChunkOffX, ChunkOffY);
            Vector3 max = ToWorld(ChunkOffX + Size - 1, ChunkOffY + Size - 1);
            min.Y = 0;
            max.Y = 8;
            return new Bounds(min, max);
        }

        //How far the ray in Raycast() travels each loop; smaller values are more precise but slower
        public const float RAY_STEP = 0.1f;

        //Casts a ray from an origin point toward a direction for a maximum distance. Returns true if it intersected any hex column, and if so 'out's the hex-grid coordinates
        public static bool Raycast(Vector3 origin, Vector3 dir, out Vector3 hexPoint, float dist) {
            bool hit = false;
            Vector3 p = origin;
            Vector3 step = dir * RAY_STEP;
            float d = 0f;
            Vector3 h = ToHex(p.X, p.Z);
            while (d < dist) {
                Vector3 worldPos = ToWorld((int)h.X, (int)h.Y);
                //If the point is actually on the grid and the ray has intersected the top of the column
                if (p.Y > worldPos.Y) {
                    hit = true;
                    break;
                }
                p += step;
                d += RAY_STEP;
                h = ToHex(p.X, p.Z);
            }
            hexPoint = h;
            return hit;
        }

        //Has the grid already been Dispose()'d?
        private bool disposed;

        //De-allocate the buffers so we don't cause a VRAM memory leak
        public void Dispose() {
            if (disposed) return;
            disposed = true;
            GL.DeleteBuffer(VBOID);
            GL.DeleteBuffer(IBOID);
        }

        public static Vector3 NearestIntersect(Vector3 Pos)//this is intended to find the nearest point to Pos where 3 chunks meet.
        {
            float Pointer = -1;//this is merely to keep the raycast consistently pointed at the terrain
            Vector3 hex;
            if (Pos.Y < 0) Pointer = 1;
            Raycast(Pos, Pointer * Vector3.UnitY, out hex, 256f);//get the position of whatever (specifically, a camera.) in gridspace


            return Vector3.Zero;
        }
    }
}

