using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
	public class Camera : Component {

		public static Camera Main; //Shortcut to the first camera registered, which should be the 'scene' view
        public static Camera Current; //The camera most recently set up with .Draw()
        Matrix4 projectionMatrix; //The Matrix that determines whether the camera is orthographic, perspective, etc.

        public RenderLayers Layers = RenderLayers.Default; //Which layers this camera renders

        bool isOrthographic = false;

        public bool IsOrthographic { //Whether this camera is orthographic or perspective
            get { return isOrthographic; }
            set { if (value != isOrthographic) { isOrthographic = value; ResetProjectionMatrix(); } }
        }

        public bool UseAlphaBlend = false; //Whether to use simple alpha blending for transparency
        public bool UseLighting = false; //Whether to use lighting; currently the custom lighting

        public bool IsPerspective { //Whether this camera uses perspective projection
            get { return !IsOrthographic;  }
            set { IsOrthographic = !value; }
        }

        public Camera(int id) : base(id) {
            if (Main == null) Main = this; //If this is the first created camera, designate it as the Main camera
            ResetProjectionMatrix();
        }

        public void ResetProjectionMatrix() {
            if (IsOrthographic) {
                projectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, Window.Main.Width, Window.Main.Height, 0f, 0.1f, 256f);
            }
            else {
                projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), (float)Window.Main.Width / (float)Window.Main.Height, 0.1f, 256f);
                projectionMatrix *= Matrix4.CreateScale(-1f, -1f, 1f); //Invert X and Y to match screen coordinates
            }
        }

        public Matrix4 ProjectionMatrix { //The projection matrix used to create ortho/perspective rendering
            get {
                return projectionMatrix;
            }
        }

		public void Begin(Matrix4 viewMatrix) {
            Current = this;
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projectionMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
			viewMatrix.Invert();
			GL.LoadMatrix(ref viewMatrix);
            if (UseAlphaBlend) {
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);
            }
			//GL.Ortho(-game.Width / 32.0, game.Width / 32.0, -game.Height / 32.0, game.Height / 32.0, 0.0, 4.0);
		}

        public void End() {
            GL.PopMatrix();
            if (UseAlphaBlend) GL.Disable(EnableCap.Blend);
        }
	}
}

