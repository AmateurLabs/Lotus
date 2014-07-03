using System;
using OpenTK;
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

		public HexGrid(int width, int height) {
			Width = width;
			Height = height;

			VBOArr = new byte[Width * Height * 7 * 16];
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

		~HexGrid() {
			if(!disposed) Dispose();
		}

		private int i = 0;
		private int v = 0;
		private int t = 0;

		public void AddVertex(float x, float y) {
			float z = (Noise.Generate(x / 16f + 4096f, y / 16f + 4096f) + 1f) / 2f;
			byte gray = (byte)(z * 255f);
			//float height = (float)Math.Floor(z * 8f);
			float height = z * 8f;
			Buffer.BlockCopy(BitConverter.GetBytes(x), 0, VBOArr, i + 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(y), 0, VBOArr, i + 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(height), 0, VBOArr, i + 8, 4);
			VBOArr[i + 12] = gray;
			VBOArr[i + 13] = gray;
			VBOArr[i + 14] = gray;
			VBOArr[i + 15] = 255;
			i += 16;
		}

		static Vector3[] HexVerts = {
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, HEX_HEIGHT / 2f, 0f),
			new Vector3(HEX_WIDTH / 2f, HEX_HEIGHT / 4f, 0f),
			new Vector3(HEX_WIDTH / 2f, -HEX_HEIGHT / 4f, 0f),
			new Vector3(0f, -HEX_HEIGHT / 2f, 0f),
			new Vector3(-HEX_WIDTH / 2f, -HEX_HEIGHT / 4f, 0f),
			new Vector3(-HEX_WIDTH / 2f, HEX_HEIGHT / 4f, 0f),
		};

		public void AddHex(int mapX, int mapY) {
			float x = ToWorld(mapX, mapY).X;
			float y = ToWorld(mapX, mapY).Y;
			AddVertex(x, y);
			AddVertex(x, y + HEX_HEIGHT / 2f);
			AddVertex(x + HEX_WIDTH / 2f, y + HEX_HEIGHT / 4f);
			AddVertex(x + HEX_WIDTH / 2f, y - HEX_HEIGHT / 4f);
			AddVertex(x, y - HEX_HEIGHT / 2f);
			AddVertex(x - HEX_WIDTH / 2f, y - HEX_HEIGHT / 4f);
			AddVertex(x - HEX_WIDTH / 2f, y + HEX_HEIGHT / 4f);
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

		public Vector3 ToHex(float x, float y) {
			Vector3 hexPos = new Vector3();
			hexPos.Y = (float)Math.Round(y / 0.75f / HEX_HEIGHT);
			hexPos.X = (float)Math.Round((x - hexPos.Y * HEX_WIDTH / 2f) / HEX_WIDTH);
			return hexPos;
		}

		public Vector3 ToWorld(int x, int y) {
			Vector3 worldPos = new Vector3(x * HEX_WIDTH + y * HEX_WIDTH / 2f, y * HEX_HEIGHT * 0.75f, 0f);
			worldPos.Z = (Noise.Generate(worldPos.X / 16f + 4096f, worldPos.Y / 16f + 4096f) + 1f) / 2f * 8f;
			return worldPos;
		}

		public void Draw() {
			GL.PushMatrix();
			//GL.Rotate(90f, 1f, 0f, 0f);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.IndexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
			GL.VertexPointer(3, VertexPointerType.Float, 16, 0);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, 16, 12);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOID);
			//GL.DrawElements(PrimitiveType.Triangles, t, DrawElementsType.UnsignedInt, 0);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.DrawElements(PrimitiveType.Triangles, t, DrawElementsType.UnsignedInt, 0);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			Vector3 hex;
			if(Raycast(Camera.Main.Position, Camera.Main.Forward, out hex, 256f)) {
				//int j = (int)(hex.Y * Width + hex.X);
				Vector3 p = ToWorld((int)hex.X, (int)hex.Y);
				GL.Begin(PrimitiveType.TriangleFan);
				for(int k = 0; k < HexVerts.Length; k++) {
					GL.Color3(1f, 0f, 1f);
					GL.Vertex3(p + HexVerts[k]);
				}
				GL.Color3(1f, 0f, 1f);
				GL.Vertex3(p + HexVerts[1]);
				GL.End();
			}

			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.ColorArray);
			GL.DisableClientState(ArrayCap.IndexArray);
			GL.PopMatrix();
		}

		public const float RAY_STEP = 0.1f;

		public bool Raycast(Vector3 origin, Vector3 dir, out Vector3 hexPoint, float dist) {
			bool hit = false;
			Vector3 p = origin;
			Vector3 step = dir * RAY_STEP;
			float d = 0f;
			Vector3 h = ToHex(p.X, p.Y);
			while(d < dist) {
				Vector3 worldPos = ToWorld((int)h.X, (int)h.Y);
				if(h.X >= 0 && h.Y >= 0 && h.X < Width && h.Y < Height && p.Z > worldPos.Z) {
					hit = true;
					break;
				}
				p += step;
				d += RAY_STEP;
				h = ToHex(p.X, p.Y);
			}
			hexPoint = h;
			return hit;
		}

		private bool disposed;

		public void Dispose() {
			if(disposed) return;
			disposed = true;
			GL.DeleteBuffer(VBOID);
			GL.DeleteBuffer(IBOID);
		}
	}
}

