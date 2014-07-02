using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
	public class Camera {

		Matrix4 ProjectionMatrix;
		Vector3 Position = -Vector3.UnitZ * 32f;
		Vector3 Rotation = new Vector3((float)Math.PI, 0f, 0f);
		float MoveSpeed = 0.1f;
		float MouseSensitivity = 0.005f;

		public Camera(GameWindow game) {
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), (float)game.Width / (float)game.Height, 0.1f, 256f);
		}

		public Matrix4 ViewMatrix {
			get {
				Vector3 lookAt = new Vector3();
				lookAt.X = (float)(Math.Sin(Rotation.X) * Math.Cos(Rotation.Y));
				lookAt.Y = (float)(Math.Sin(Rotation.Y));
				lookAt.Z = (float)(Math.Cos(Rotation.X) * Math.Cos(Rotation.Y));
				return Matrix4.LookAt(Position, Position + lookAt, Vector3.UnitY);
			}
		}

		public void Move(float x, float y, float z) {
			Vector3 forward = new Vector3((float)Math.Sin(Rotation.X), 0f, (float)Math.Cos(Rotation.X));
			Vector3 right = new Vector3(-forward.Z, 0f, forward.X);
			Vector3 offset = new Vector3();
			offset += right * x;
			offset += forward * y;
			offset.Y += z;

			offset.NormalizeFast();
			offset = Vector3.Multiply(offset, MoveSpeed);
			Position += offset;
		}

		public void Rotate(float x, float y) {
			x *= MouseSensitivity;
			y *= MouseSensitivity;
			Rotation.X = (Rotation.X + x) % ((float)Math.PI * 2f);
			Rotation.Y = Math.Max(Math.Min(Rotation.Y + y, (float)Math.PI / 2f - 0.1f), (float)-Math.PI / 2f + 0.1f);
		}

		Vector2 lastMousePos = new Vector2();

		public void Update(GameWindow game, float dt) {
			if(game.Keyboard[Key.W]) Move(0f, dt, 0f);
			if(game.Keyboard[Key.S]) Move(0f, -dt, 0f);
			if(game.Keyboard[Key.A]) Move(-dt, 0, 0f);
			if(game.Keyboard[Key.D]) Move(dt, 0, 0f);
			if(game.Keyboard[Key.Q]) Move(0f, 0f, dt);
			if(game.Keyboard[Key.E]) Move(0f, 0f, -dt);

			if(game.Focused) {
				Vector2 delta = lastMousePos - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
				Rotate(delta.X, delta.Y);
				Mouse.SetPosition(game.Bounds.Left + game.Bounds.Width / 2, game.Bounds.Top + game.Bounds.Height / 2);
				lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
			}
		}

		public void Draw() {
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref ProjectionMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
			var viewMatrix = ViewMatrix;
			GL.LoadMatrix(ref viewMatrix);
			//GL.Ortho(-game.Width / 32.0, game.Width / 32.0, -game.Height / 32.0, game.Height / 32.0, 0.0, 4.0);
		}
	}
}

