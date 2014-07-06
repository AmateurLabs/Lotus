using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Lotus {
    public class Window : GameWindow {

        static HexGrid grid;
        static Camera cam;
        static Camera uiCam;
        static Text text;

        bool cursorShow = true; // DO NOT MODIFY THIS OUTSIDE OF cursorCurtain()!
        public bool ShowCursor {
            get { return cursorShow; }
            set { if (cursorShow != value) CursorCurtain(); }
        }

        double time = 0.0;
        double lastTime = 0.0;
        double accumFPS = 0.0;
        int frameCount = 0;
        double frameRate = 0.0;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.On;

            grid = new HexGrid(256, 256);
            cam = new Camera((float)Width, (float)Height, false);
            uiCam = new Camera((float)Width, (float)Height, true);
            uiCam.Move(0f, 0f, -10f);
            cam.FreelookEnabled = true;
            text = new Text();
            CursorCurtain();
            
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape]) {
                CursorCurtain();
                //Exit();
            }

            if (Keyboard[Key.F4] && Keyboard[Key.LAlt]) Exit();

            time += e.Time;
            float dt = (float)e.Time;

            CalcAvgFrameRate(e);

            cam.Update(this, dt);
        }

        private void CalcAvgFrameRate(FrameEventArgs e)
        {
            lastTime += e.Time;
            accumFPS += e.Time;
            frameCount++;
            if (lastTime > 0.25)
            {
                frameRate = Math.Round(1.0 / (accumFPS / frameCount));
                accumFPS = 0.0;
                frameCount = 0;
                lastTime = 0.0;
            }
        }// Calculates the average framerate every quarter second. Relies on the OnUpdateFrame Method.

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
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
            uiCam.Draw();
            DebugReadout();
            SwapBuffers();
            GL.Disable(EnableCap.DepthTest);
        }

        private void DebugReadout()//basic dubug readout.
        {
            int n = 0;
            int spacing = 18;
            text.Draw("DEBUG READOUT", new Vector2(0, spacing * n++));
            text.Draw("Camera Location || Camera Rotation", new Vector2(0, spacing * n++));
            text.Draw(("X: " + cam.Position.X).PadRight(15) + " || X:" + cam.Rotation.X % (2 * Math.PI), new Vector2(0, spacing * n++));
            text.Draw(("Y: " + cam.Position.Y).PadRight(15) + " || Y:" + cam.Rotation.Y % (2 * Math.PI), new Vector2(0, spacing * n++));
            text.Draw(("Z: " + cam.Position.Z).PadRight(15) + " || Z:" + cam.Rotation.Z % (2 * Math.PI), new Vector2(0, spacing * n++));
            text.Draw("Frame Rate: " + frameRate, new Vector2(0, spacing * n++)); //framerate readout
            Vector3 hex;
            if (grid.Raycast(Camera.Main.Position, Camera.Main.Forward, out hex, 256f))
            {
                text.Draw("(" + hex.X + ", " + hex.Y + ")", new Vector2(0, spacing * n++));
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            grid.Dispose();
        }

        protected bool CursorCurtain() // Shows cursor if hidden. Hides cursor in showing. Self correcting after one call.
        {
            if (cursorShow == false) System.Windows.Forms.Cursor.Show();
            else System.Windows.Forms.Cursor.Hide();
            cursorShow = !cursorShow;
            return cursorShow;

        }
    }
}
