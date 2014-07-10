using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SimplexNoise;

namespace Lotus {
	public class HexGrid : IDisposable {

		public readonly int Width;
		public readonly int Height;
		public const float HEX_HEIGHT = 1f;
		public const float HEX_WIDTH = 0.8660254037844386f;
		public readonly byte[] VBOArr;
		public readonly uint[] IBOArr;
		public readonly int VBOID;
		public readonly int IBOID;

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

        public HexGrid(int width, int height) {
			Width = width;
			Height = height;

            VBOArr = new byte[Width * Height * 7 * VERT_STRIDE];
			VBOID = GL.GenBuffer();
			IBOArr = new uint[Width * Height * 6 * 3];
			IBOID = GL.GenBuffer();

			for(int x = 0; x < Width; x++) {
				for(int y = 0; y < Height; y++) {
					AddHex(x, y);
				}
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(VBOArr.Length), VBOArr, BufferUsageHint.DynamicDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(IBOArr.Length * 4), IBOArr, BufferUsageHint.DynamicDraw);
		}

        //If the hexgrid gets removed from memory without cleaning up after itself, prevent memory leaks
		~HexGrid() {
			if(!disposed) Dispose();
		}

        public float GetHeight(float x, float z) {
            return (Noise.Generate(x / 16f + 4096f, z / 16f + 4096f) + 1f) / 2f;
        }

		private int i = 0;
		private int v = 0;
		private int t = 0;

		public void AddVertex(float x, float z) {
			float y = GetHeight(x, z);
            float dx = GetHeight(x - 0.5f, z)*8f - GetHeight(x + 0.5f, z)*8f;
            float dz = GetHeight(x, z - 0.5f)*8f - GetHeight(x, z + 0.5f)*8f;
            Vector3 normal = new Vector3(-dx, -1f, -dz).Normalized();
            float dot = Vector3.Dot(normal, Vector3.UnitY);
			byte gray = (byte)(Math.Min(Math.Max(255f * dot, 0f), 255f));
            Color4 color = Light.GetColor(normal, new Vector3(x, y, z), Color4.White);
			//float height = (float)Math.Floor(z * 8f);
			float height = y * 8f;
			Buffer.BlockCopy(BitConverter.GetBytes(x), 0, VBOArr, i + 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(height), 0, VBOArr, i + 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(z), 0, VBOArr, i + 8, 4);
            VBOArr[i + 12] = (byte)(Math.Min(Math.Max(255f * color.R, 0f), 255f));
            VBOArr[i + 13] = (byte)(Math.Min(Math.Max(255f * color.G, 0f), 255f));
            VBOArr[i + 14] = (byte)(Math.Min(Math.Max(255f * color.B, 0f), 255f));
			VBOArr[i + 15] = 255;
            Buffer.BlockCopy(BitConverter.GetBytes(normal.X), 0, VBOArr, i + 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(normal.Y), 0, VBOArr, i + 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(normal.Z), 0, VBOArr, i + 24, 4);
            i += VERT_STRIDE;
		}

		static Vector3[] HexVerts = {
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
			for(int j = 1; j < 7; j++) {
				int k = (j + 1);
				if(k > 6) k = 1;
				IBOArr[t] = (uint)(v);
				IBOArr[t + 1] = (uint)(v + j);
				IBOArr[t + 2] = (uint)(v + k);
				t += 3;
			}
			v += 7;
		}

        //Gets the array indices of the hex column that the given worldspace point is in
		public Vector3 ToHex(float x, float z) {
			Vector3 hexPos = new Vector3();
			hexPos.Y = (float)Math.Round(z / 0.75f / HEX_HEIGHT);
			hexPos.X = (float)Math.Round((x - hexPos.Y * HEX_WIDTH / 2f) / HEX_WIDTH);
			return hexPos;
		}

        //Gets the worldspace location of the center vertex of the given hex column
		public Vector3 ToWorld(int x, int y) {
			Vector3 worldPos = new Vector3(x * HEX_WIDTH + y * HEX_WIDTH / 2f, 0f, y * HEX_HEIGHT * 0.75f);
			worldPos.Y = GetHeight(worldPos.X, worldPos.Z) * 8f;
			return worldPos;
		}

        public void Update() {
            //Primitive way of updating VBO data without resending everything; probably impractical
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(32), new IntPtr(4), new float[] { (float)Math.Sin(time) });
        }

		public void Draw() {

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

            //Raycast from the center of the camera and display cursor
			Vector3 hex;
			if(Raycast(Camera.Main.Position, Camera.Main.Forward, out hex, 256f)) {
				//int j = (int)(hex.Y * Width + hex.X);

				Vector3 p = ToWorld((int)hex.X, (int)hex.Y);
				p.Y = 0f;
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
				GL.Begin(PrimitiveType.TriangleFan);
				for(int k = 0; k < HexVerts.Length; k++) {
					GL.Color3(1f, 0f, 1f);
					GL.Vertex3(p + HexVerts[k] + Vector3.UnitY * GetHeight(p.X + HexVerts[k].X, p.Z + HexVerts[k].Z) * 8f);
				}
				GL.Color3(1f, 0f, 1f);
                GL.Vertex3(p + HexVerts[1] + Vector3.UnitY * GetHeight(p.X + HexVerts[1].X, p.Z + HexVerts[1].Z) * 8f);
				GL.End();
				GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
			}

            //Stop using arrays so we don't contaminate future draws
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.NormalArray);
			GL.DisableClientState(ArrayCap.IndexArray);
		}

        //How far the ray in Raycast() travels each loop; smaller values are more precise but slower
		public const float RAY_STEP = 0.1f;

        //Casts a ray from an origin point toward a direction for a maximum distance. Returns true if it intersected any hex column, and if so 'out's the hex-grid coordinates
		public bool Raycast(Vector3 origin, Vector3 dir, out Vector3 hexPoint, float dist) {
			bool hit = false;
			Vector3 p = origin;
			Vector3 step = dir * RAY_STEP;
			float d = 0f;
			Vector3 h = ToHex(p.X, p.Z);
			while(d < dist) {
				Vector3 worldPos = ToWorld((int)h.X, (int)h.Y);
                //If the point is actually on the grid and the ray has intersected the top of the column
				if(h.X >= 0 && h.Y >= 0 && h.X < Width && h.Y < Height && p.Y > worldPos.Y) {
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
			if(disposed) return;
			disposed = true;
			GL.DeleteBuffer(VBOID);
			GL.DeleteBuffer(IBOID);
		}
	}
}

