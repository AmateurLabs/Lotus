using System;
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
			float height = (float)Math.Floor(z * 8f);
			Buffer.BlockCopy(BitConverter.GetBytes(x), 0, VBOArr, i + 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(y), 0, VBOArr, i + 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(height), 0, VBOArr, i + 8, 4);
			VBOArr[i + 12] = gray;
			VBOArr[i + 13] = gray;
			VBOArr[i + 14] = gray;
			VBOArr[i + 15] = 255;
			i += 16;
		}

		public void AddHex(int mapX, int mapY) {
			float x = mapX * HEX_WIDTH + mapY * HEX_WIDTH / 2f;
			float y = mapY * HEX_HEIGHT * 0.75f;
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

		public void Draw() {
			GL.PushMatrix();
			GL.Rotate(90f, 1f, 0f, 0f);
			GL.Enable(EnableCap.DepthTest);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.IndexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOID);
			GL.VertexPointer(3, VertexPointerType.Float, 16, 0);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, 16, 12);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOID);
			GL.DrawElements(PrimitiveType.Triangles, t, DrawElementsType.UnsignedInt, 0);
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.ColorArray);
			GL.DisableClientState(ArrayCap.IndexArray);
			GL.Disable(EnableCap.DepthTest);
			GL.PopMatrix();
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

