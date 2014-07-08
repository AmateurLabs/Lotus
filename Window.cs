using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using Lotus.ECS;
using Lotus.ECS.Aspects;
using Lotus.ECS.Modules;

namespace Lotus {
    public class Window : GameWindow {

        static HexGrid grid;
        static Camera cam;
        static Camera uiCam;
        static Text text;
        static Shader shader;

        static double time = 0.0;
        double lastTime = 0.0;
        double accumFPS = 0.0;
        int frameCount = 0;
        double frameRate = 0.0;
        public bool DebugEnabled = false;

        public static double Time {
            get { return time; }
        }
       
        public Window()
            : base(1024, 768, GraphicsMode.Default, "Lotus", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible) {

        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.On;

            grid = new HexGrid(256, 256);
            cam = new Camera((float)Width, (float)Height, false, false, true);
            cam.Position = new Vector3(-10.71002f, -9.084502f, -7.3577f);
            cam.Rotation = new Quaternion(0.282464295625687f, -2.12368106842041f, 0f, 0f);
            uiCam = new Camera((float)Width, (float)Height, true, true, false);
            uiCam.Position = new Vector3(0, 0, 10);
            cam.FreelookEnabled = true;
            text = new Text();
            CursorVisible = false;
            shader = new Shader();

            Engine.Modules.Add(new RenderModule());
            Engine.Modules.Add(new JitterModule());

            Entity.Add<Transform>(0);
            Entity.Get<Transform>(0).Position = new Vector3(0f, -10f, 0f);
            Entity.Add<Renderer>(0);
            Entity.Add<JitterBody>(0);            
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Input.Update();

            if (Input.IsPressed(Key.Escape)) { //For now, setting escape as a switch for cursor visibility
                CursorVisible = !CursorVisible;
                //Exit();
            }

            if (Input.IsPressed(Key.F4) && Input.Alt) Exit();

            if (Input.IsDown(Key.Space)) {
                Entity.Get<JitterBody>(0).Rigidbody.IsActive = true;
                Entity.Get<JitterBody>(0).Rigidbody.AddTorque(Jitter.LinearMath.JVector.Forward * 100f);
            }

            if (Input.IsPressed(Key.F1))
                DebugEnabled = !DebugEnabled;

            time += e.Time;
            float dt = (float)e.Time;

            CalcAvgFrameRate(e);

            cam.Update(this, dt);

            Engine.Update(dt); //Insert engine rev here VROOOOOOM
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
            cam.Begin();
            shader.Draw();
            grid.Draw();
            HexagonCursorThingie();
            //new Cube(Vector3.Zero, Quaternion.Identity, 4f).Draw();
            Engine.Render();
            new Sphere(1, new Vector3((float)Math.Cos(time), (float)Math.Sin(time), 0f), Quaternion.FromAxisAngle(Vector3.UnitX, (float)time)).Draw();
            Shader.Reset();
            cam.End();
            uiCam.Begin();
            if(DebugEnabled)
                DebugReadout();
            uiCam.End();
            SwapBuffers();
            GL.Disable(EnableCap.DepthTest);
        }

        private void HexagonCursorThingie() {
            GL.Begin(PrimitiveType.Lines); //Hexagon cursor thingie
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
        }

        private void DebugReadout()//basic dubug readout.
        {
            float border = 10f;
            Debug.Depth = -0.9f;
            Debug.DrawRect((float)((Width / 2) - 1), (float)((Height / 2) - 10), 2f, 20f, new Color4(1f, 1f, 1f, 1f));
            Debug.DrawRect((float)((Width / 2) - 10), (float)((Height / 2) - 1), 20f, 2f, new Color4(1f, 1f, 1f, 1f));
            Debug.Depth = -1f;
            Debug.DrawRectFrame(Vector2.Zero, new Vector2(435, 108), Color4.White, new Color4(1f, 0f, 1f, 0.5f), border / 2);
            Debug.Depth = -1.1f;
            Debug.DrawRectFrame(Vector2.Zero, new Vector2(Width, Height), Color4.White, new Color4(0.1f, 0.1f, 0.5f, 0.3f), border / 2);
            
            int n = 0;
            int spacing = 12;
            
            text.Draw("DEBUG READOUT", new Vector2(border, spacing * n++ + border));
            text.Draw("Camera Location || Camera Rotation", new Vector2(border, spacing * n++ + border));
            text.Draw(("X: " + cam.Position.X).PadRight(15) + " || X:" + (float)(cam.Rotation.X % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw(("Y: " + cam.Position.Y).PadRight(15) + " || Y:" + (float)(cam.Rotation.Y % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw(("Z: " + cam.Position.Z).PadRight(15) + " || Z:" + (float)(cam.Rotation.Z % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw("Frame Rate: " + frameRate, new Vector2(border, spacing * n++ + border)); //framerate readout
            text.Draw("Time: " + time, new Vector2(border, spacing * n++ + border));
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
            shader.Dispose();
        }
    }
}
