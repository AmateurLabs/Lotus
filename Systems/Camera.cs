using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
	public class Camera {

		public static Camera Main; //Shortcut to the first camera registered, which should be the 'scene' view
        public static Camera Current; //The camera most recently set up with .Draw()
        Matrix4 ProjectionMatrix; //The Matrix that determines whether the camera is orthographic, perspective, etc.
		public Vector3 Position; //The position in 3D space that the camera occupies
        public Quaternion Rotation; //The quaternion rotation of the camera, applied in YXZ order

        //TODO: move camera controls to separate class
        public bool FreelookEnabled;
		float MoveSpeed = 10f; //How fast the freelook camera moves around
        float RotateSpeed = 0.005f; //How fast the freelook camera rotates

        public readonly bool IsOrthographic; //Whether this camera is orthographic; cannot be changed after initialization

        public bool IsPerspective { //Whether this camera uses perspective projection
            get { return !IsOrthographic;  }
        }

		public Camera(float width, float height, bool ortho) { //Creates a new camera, using the width and height of the screen and whether it is orthographic
            IsOrthographic = ortho;
            if (Main == null) Main = this; //If this is the first created camera, designate it as the Main camera
            if (IsOrthographic) {
                ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, 0.1f, 256f);
                //ProjectionMatrix *= Matrix4.CreateScale(-1f, -1f, 1f); //Invert X and Y to match screen coordinates
            }
            else {
                ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), width / height, 0.1f, 256f);
                ProjectionMatrix *= Matrix4.CreateScale(-1f, -1f, 1f); //Invert X and Y to match screen coordinates
            }
		}

		public Matrix4 ViewMatrix { //The final view matrix used to draw the world
			get {
				return RotationMatrix * TranslationMatrix;
			}
		}

		public Matrix4 TranslationMatrix { //A matrix of the current position
			get {
				return Matrix4.CreateTranslation(Position);
			}
		}

		public Matrix4 RotationMatrix { //A matrix of the current rotation
			get {
				return Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y);
			}
		}

		public Vector3 Forward { //The direction the camera is facing in worldspace
			get {
				return Vector3.TransformPosition(-Vector3.UnitZ, RotationMatrix);
			}
		}

		public Vector3 Right { //The direction to the right of the camera in worldspace
			get {
				return Vector3.TransformPosition(-Vector3.UnitX, RotationMatrix);
			}
		}

		public Vector3 Up { //The direction to the top of the camera in worldspace
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

		public void Rotate(float x, float y, float z) {
			x *= RotateSpeed;
            y *= RotateSpeed;
            z *= RotateSpeed;
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitX, x); //Yaw
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitY, y); //Pitch
            Rotation -= Quaternion.FromAxisAngle(Vector3.UnitZ, z); //Roll
		}

		Vector2 lastMousePos = new Vector2();

		public void Update(GameWindow game, float dt) {
            if (FreelookEnabled) {
                if (game.Keyboard[Key.W]) Move(0f, 0f, dt);
                if (game.Keyboard[Key.S]) Move(0f, 0f, -dt);
                if (game.Keyboard[Key.A]) Move(-dt, 0, 0f);
                if (game.Keyboard[Key.D]) Move(dt, 0, 0f);
                if (game.Keyboard[Key.Q]) Move(0f, dt, 0f);
                if (game.Keyboard[Key.E]) Move(0f, -dt, 0f);

                if (game.Focused) {
                    //game.Title = "" + MathHelper.RadiansToDegrees(Rotation.X) + ", " + MathHelper.RadiansToDegrees(Rotation.Y) + ", " + MathHelper.RadiansToDegrees(Rotation.Z);
                    Vector2 delta = lastMousePos - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    Rotate(delta.Y, delta.X, 0f); //Flipped because moving the mouse horizontally actually rotates on the Y axis, etc.
                    Mouse.SetPosition(game.Bounds.Left + game.Bounds.Width / 2, game.Bounds.Top + game.Bounds.Height / 2);
                    lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                }
            }
		}

		public void Draw() {
            Current = this;
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

