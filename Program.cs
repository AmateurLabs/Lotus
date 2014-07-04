using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Lotus {
	public class Program {

		static HexGrid grid;
		static Camera cam;
        static Text text;

		[STAThread]
		public static void Main(string[] args) {

			using(var game = new GameWindow()) {
				game.Load += (sender, e) => {
					game.VSync = VSyncMode.On;

					grid = new HexGrid(256, 256);
                    cam = new Camera((float)game.Width / (float)game.Height, false);
                    text = new Text();
				};

				game.Resize += (sender, e) => {
					GL.Viewport(0, 0, game.Width, game.Height);
				};

				double time = 0f;

				game.UpdateFrame += (sender, e) => {
					time += e.Time;
					float dt = (float)e.Time;
					if(game.Keyboard[Key.Escape]) {
						game.Exit();
					}
					cam.Update(game, dt);
					//GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					//GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(32), new IntPtr(4), new float[] { (float)Math.Sin(time) });
				};

				game.RenderFrame += (sender, e) => {
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
					GL.ClearColor(Color.CornflowerBlue);
					GL.Enable(EnableCap.DepthTest);
					cam.Draw();
					grid.Draw();
					GL.Begin(PrimitiveType.Lines);
					GL.Color3(1f, 0f, 0f);
					GL.Vertex3(0f, 0f, 0f);
					GL.Color3(1f, 0f, 0f);
					GL.Vertex3(1f, 0f, 0f);
					GL.Color3(0f, 1f, 0f);
					GL.Vertex3(0f, 0f, 0f);
					GL.Color3(0f, 1f, 0f);
					GL.Vertex3(0f, 1f, 0f);
					GL.Color3(0f, 0f, 1f);
					GL.Vertex3(0f, 0f, 0f);
					GL.Color3(0f, 0f, 1f);
					GL.Vertex3(0f, 0f, 1f);
					GL.End();
                    text.Draw("Hello World", Vector2.Zero);
					game.SwapBuffers();
					GL.Disable(EnableCap.DepthTest);
				};

				game.Closed += (sender, e) => {
					grid.Dispose();
				};

				game.Run(60.0);
			}
		}
	}
}

