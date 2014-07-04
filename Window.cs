using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Lotus {
    public class Window : GameWindow {

        public static Window Main;
        HexGrid grid;
        Camera cam;
        Camera uiCam;
        Text text;
        public string DebugMessage = "Hello World!";

        double time = 0f;

        public Window() : base() {
                Main = this;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.On;

            grid = new HexGrid(256, 256);
            cam = new Camera((float)Width, (float)Height, false);
            uiCam = new Camera((float)Width, (float)Height, true);
            uiCam.Move(0f, 0f, -10f);
            cam.FreelookEnabled = true;
            text = new Text();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            time += e.Time;
            float dt = (float)e.Time;
            if (Keyboard[Key.Escape]) {
                Exit();
            }
            cam.Update(this, dt);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            cam.Draw();
            grid.Draw();
            GL.Begin(PrimitiveType.Lines); //Draws the axis widget for debugging
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
            uiCam.Draw();
            text.Draw(DebugMessage, Vector2.Zero);
            SwapBuffers();
            GL.Disable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            grid.Dispose();
        }
    }
}
