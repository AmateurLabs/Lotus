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

        double time = 0.0;
        double lastTime = 0.0;
        double accumFPS = 0.0;
        int frameCount = 0;
        double frameRate = 0.0;

        public Window()
            : base(1024, 768) {

        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.On;

            grid = new HexGrid(256, 256);
            cam = new Camera((float)Width, (float)Height, false);
            cam.Position = new Vector3(-10.71002f, -9.084502f, -7.3577f);
            cam.Rotation = new Quaternion(0.282464295625687f, -2.12368106842041f, 0f, 0f);
            uiCam = new Camera((float)Width, (float)Height, true);
            uiCam.Position = new Vector3(0, 0, 10);
            cam.FreelookEnabled = true;
            text = new Text();
            CursorVisible = false;
            
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Input.Update();

            if (Input.IsPressed(Key.Escape)) { //For now, setting escape as a switch for cursor visibility
                CursorVisible = !CursorVisible;
                //Exit();
            }

            if (Input.IsPressed(Key.F4) && Input.Alt) Exit();

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
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
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
            GL.Disable(EnableCap.Blend);
        }

        private void DebugReadout()//basic dubug readout.
        {
            float sc = 120;
            float h = 108;
            float w = 435;
            float off = -1f;
            float border = 10;
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(1f, 0f, 1f, 0.5f);
            GL.Vertex3(0f, 0f, off);
            GL.Vertex3(w, 0f, off);
            GL.Vertex3(w, h, off);
            GL.Vertex3(0, h, off);
            off += 0.01f;
            GL.Color4(1f, 1f, 1f,1f);
            GL.Vertex3(0f, 0f, off);
            GL.Vertex3(border/2, 0f, off);
            GL.Vertex3(border/2, h, off);
            GL.Vertex3(0, h, off);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.Vertex3(border/2, h-border/2, off);
            GL.Vertex3(w-border/2, h-border/2, off);
            GL.Vertex3(w-border/2, h, off);
            GL.Vertex3(border/2, h, off);
            GL.Vertex3(border / 2, h - border / 2, off);
            GL.Vertex3(w - border / 2, h - border / 2, off);
            GL.Vertex3(w - border / 2, h, off);
            GL.Vertex3(border / 2, h, off);
            GL.End();
            int n = 0;
            int spacing = 12;
            
            text.Draw("DEBUG READOUT", new Vector2(border, spacing * n++ + border));
            text.Draw("Camera Location || Camera Rotation", new Vector2(border, spacing * n++ + border));
            text.Draw(("X: " + cam.Position.X).PadRight(15) + " || X:" + cam.Rotation.X % (2 * Math.PI), new Vector2(border, spacing * n++ + border));
            text.Draw(("Y: " + cam.Position.Y).PadRight(15) + " || Y:" + cam.Rotation.Y % (2 * Math.PI), new Vector2(border, spacing * n++ + border));
            text.Draw(("Z: " + cam.Position.Z).PadRight(15) + " || Z:" + cam.Rotation.Z % (2 * Math.PI), new Vector2(border, spacing * n++ + border));
            text.Draw("Frame Rate: " + frameRate, new Vector2(border, spacing * n++ + border)); //framerate readout
            Vector3 hex;
            if (grid.Raycast(Camera.Main.Position, Camera.Main.Forward, out hex, 256f))
            {
                text.Draw("(" + hex.X + ", " + hex.Y + ")", new Vector2(border, spacing * n++ + border));
            }
            
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            cam.ResetProjectionMatrix(Width, Height);
            uiCam.ResetProjectionMatrix(Width, Height);
            text.ResetView();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            grid.Dispose();
        }
    }
}
