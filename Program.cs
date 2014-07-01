using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Lotus {
	public class Program {

		[STAThread]
		public static void Main(string[] args) {

			using(var game = new GameWindow()) {

				int vbo = 0;

				game.Load += (sender, e) => {
					game.VSync = VSyncMode.On;

					float[] vertData = new float[3 * 3];
					vertData[0] = -1f;
					vertData[1] = -1f;
					vertData[2] = 0f;
					vertData[3] = 1f;
					vertData[4] = -1f;
					vertData[5] = 0f;
					vertData[6] = 0f;
					vertData[7] = 1f;
					vertData[8] = 0f;

					vbo = GL.GenBuffer();
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertData.Length * sizeof(float)), vertData, BufferUsageHint.DynamicDraw);
				};

				game.Resize += (sender, e) => {
					GL.Viewport(0, 0, game.Width, game.Height);
				};

				game.UpdateFrame += (sender, e) => {
					if(game.Keyboard[Key.Escape]) {
						game.Exit();
					}
				};

				game.RenderFrame += (sender, e) => {
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

					GL.EnableVertexAttribArray(0);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
					GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
					GL.DisableVertexAttribArray(0);

					game.SwapBuffers();
				};

				game.Closed += (sender, e) => {
					GL.DeleteBuffer(vbo);
				};

				game.Run(60.0);
			}
		}

		public static int STRIDE = 4 + 4 + 4 + 1 + 1 + 1 + 1;

		public int AddVertex(int i, byte[] buffer, float x, float y, float z, byte r, byte g, byte b, byte a) {

			i += STRIDE;
			return i;
		}
	}
}

