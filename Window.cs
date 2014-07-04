using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.Off;

            grid = new HexGrid(256, 256);
            cam = new Camera((float)Width, (float)Height, false);
            uiCam = new Camera((float)Width, (float)Height, true);
            uiCam.Move(0f, 0f, -10f);
            cam.FreelookEnabled = true;
            text = new Text();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape]) {
                Exit();
            }

            time += e.Time;
            float dt = (float)e.Time;

            lastTime += e.Time;
            accumFPS += e.Time;
            frameCount++;
            if (lastTime > 0.25) {
                frameRate = Math.Round(1.0/(accumFPS/frameCount));
                accumFPS = 0.0;
                frameCount = 0;
                lastTime = 0.0;
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
            text.Draw("Hello World", Vector2.Zero);
            text.Draw("Frame Rate: " + frameRate, new Vector2(0,18)); //framerate readout
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
