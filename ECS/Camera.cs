using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Lotus.ECS {
	public class Camera : Component {

		public static Camera Main; //Shortcut to the first camera registered, which should be the 'scene' view
        public static Camera Current; //The camera most recently set up with .Draw()
        Matrix4 projectionMatrix; //The Matrix that determines whether the camera is orthographic, perspective, etc.

        public EnumValue<Layers> LayerMask; //Which layers this camera renders

        bool isOrthographic;
        public BoolValue IsOrthographic; //Whether this camera is orthographic or perspective
        public BoolValue UseAlphaBlend; //Whether to use simple alpha blending for transparency
        public BoolValue UseLighting; //Whether to use lighting; currently the custom lighting

        public FloatValue FieldOfView; //The field-of-view, if the camera is a perspective camera
        public FloatValue ViewDistance; //The maximum viewing distance

        public bool IsPerspective { //Whether this camera uses perspective projection
            get { return !IsOrthographic.Value;  }
            set { IsOrthographic.Value = !value; }
        }

        public Camera(int id) : base(id) {
            if (Main == null) Main = this; //If this is the first created camera, designate it as the Main camera
            LayerMask = new EnumValue<Layers>(this, "LayerMask", Layers.Layer0);
            IsOrthographic = new BoolValue(this, "IsOrthographic", false);
            UseAlphaBlend = new BoolValue(this, "UseAlphaBlend", false);
            UseLighting = new BoolValue(this, "UseLighting", false);
            FieldOfView = new FloatValue(this, "FieldOfView", 60f);
            ViewDistance = new FloatValue(this, "ViewDistance", 256f);
            ResetProjectionMatrix();
        }

        public void ResetProjectionMatrix() {
            if (IsOrthographic.Value) {
                projectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, Window.Main.Width, Window.Main.Height, 0f, 0.1f, ViewDistance.Value);
            }
            else {
                projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FieldOfView.Value), (float)Window.Main.Width / (float)Window.Main.Height, 0.1f, ViewDistance.Value);
                projectionMatrix *= Matrix4.CreateScale(-1f, -1f, 1f); //Invert X and Y to match screen coordinates
            }
        }

        public Matrix4 ProjectionMatrix { //The projection matrix used to create ortho/perspective rendering
            get {
                return projectionMatrix;
            }
        }

		public void Begin(Matrix4 viewMatrix) {
            if (IsOrthographic.Value != isOrthographic) {
                ResetProjectionMatrix();
                isOrthographic = IsOrthographic.Value;
            }
            Current = this;
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projectionMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
			viewMatrix.Invert();
			GL.LoadMatrix(ref viewMatrix);
            if (UseAlphaBlend.Value) {
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);
            }
			//GL.Ortho(-game.Width / 32.0, game.Width / 32.0, -game.Height / 32.0, game.Height / 32.0, 0.0, 4.0);
		}

        public void End() {
            GL.PopMatrix();
            if (UseAlphaBlend.Value) GL.Disable(EnableCap.Blend);
        }
	}
}

