using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
	public class Camera {

		public static Camera Main;
		Matrix4 ProjectionMatrix;
		public Vector3 Position;
		public Quaternion Rotation;
		float MoveSpeed = 10f;
		float MouseSensitivity = 0.005f;

		public Camera(GameWindow game) {
			Main = this;
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), (float)game.Width / (float)game.Height, 0.1f, 256f);
			ProjectionMatrix *= Matrix4.CreateScale(-1f, -1f, 1f);
		}

		public Matrix4 ViewMatrix {
			get {
				return RotationMatrix * TranslationMatrix;
			}
		}

		public Matrix4 TranslationMatrix {
			get {
				return Matrix4.CreateTranslation(Position);
			}
		}

		public Matrix4 RotationMatrix {
			get {
				return Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y);
			}
		}

		public Vector3 Forward {
			get {
				return Vector3.TransformPosition(-Vector3.UnitZ, RotationMatrix);
			}
		}

		public Vector3 Right {
			get {
				return Vector3.TransformPosition(-Vector3.UnitX, RotationMatrix);
			}
		}

		public Vector3 Up {
			get {
				return Vector3.TransformPosition(-Vector3.UnitY, RotationMatrix);
			}
		}

		public void Move(float x, float y, float z) {
			var rot = RotationMatrix;
			Position -= Vector3.TransformPosition(Vector3.UnitX, rot) * x * MoveSpeed;
			Position -= Vector3.TransformPosition(Vector3.UnitY, rot) * y * MoveSpeed;
			Position -= Vector3.TransformPosition(Vector3.UnitZ, rot) * z * MoveSpeed;
		}

		public void Rotate(float x, float y) {
			x *= MouseSensitivity;
			y *= MouseSensitivity;
			Rotation.Y = (Rotation.Y - x) % ((float)Math.PI * 2f);
			Rotation.X = (Rotation.X - y) % ((float)Math.PI * 2f);
		}

		Vector2 lastMousePos = new Vector2();

		public void Update(GameWindow game, float dt) {
			if(game.Keyboard[Key.W]) Move(0f, 0f, dt);
			if(game.Keyboard[Key.S]) Move(0f, 0f, -dt);
			if(game.Keyboard[Key.A]) Move(-dt, 0, 0f);
			if(game.Keyboard[Key.D]) Move(dt, 0, 0f);
			if(game.Keyboard[Key.Q]) Move(0f, dt, 0f);
			if(game.Keyboard[Key.E]) Move(0f, -dt, 0f);

			if(game.Focused) {
				//game.Title = "" + MathHelper.RadiansToDegrees(Rotation.X) + ", " + MathHelper.RadiansToDegrees(Rotation.Y) + ", " + MathHelper.RadiansToDegrees(Rotation.Z);
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
			viewMatrix.Invert();
			GL.LoadMatrix(ref viewMatrix);
			//GL.Ortho(-game.Width / 32.0, game.Width / 32.0, -game.Height / 32.0, game.Height / 32.0, 0.0, 4.0);
		}
	}
}

