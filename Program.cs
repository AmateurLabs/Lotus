using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Lotus {
	public class Program {

		static HexGrid grid;
		static Camera cam;

		[STAThread]
		public static void Main(string[] args) {

			using(var game = new GameWindow()) {
				game.Load += (sender, e) => {
					game.VSync = VSyncMode.On;

					grid = new HexGrid(32, 32);
					cam = new Camera(game);
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
					cam.Draw();
					grid.Draw();
					game.SwapBuffers();
				};

				game.Closed += (sender, e) => {
					grid.Dispose();
				};

				game.Run(60.0);
			}
		}
	}
}

