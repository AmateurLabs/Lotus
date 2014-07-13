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

namespace Lotus {
    public class Window : GameWindow {

        public static Window Main;
        static Entity cam;
        static Entity uiCam;
        static Text text;
        static Entity shape;

        static double time = 0.0;
        double timeLeft = 0.0;
        double accumFPS = 0.0;
        int frameCount = 0;
        double frameRate = 0.0;
        public bool DebugEnabled = false;

        public static double Time {
            get { return time; }
        }

        public Window()
            : base(1024, 768, GraphicsMode.Default, "Lotus", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible) {
                Main = this;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            VSync = VSyncMode.On;
            text = new Text();
            CursorVisible = false;

            Engine.Processors.Add(new RenderProcessor());
            Engine.Processors.Add(new JitterProcessor());
            Engine.Processors.Add(new PhysicsProcessor());
            Engine.Processors.Add(new FreelookProcessor());

            Entity worldEntity = new Entity();
            worldEntity.Add<Attractor>().Type = Attractor.AttractionType.World;
            worldEntity.Get<Attractor>().Acceleration = 9.81f;
            worldEntity.Add<DirectionalLight>().Direction = Vector3.UnitY;
            worldEntity.Get<DirectionalLight>().Intensity = 0.125f;
            Entity light = new Entity();
            light.Add<Transform>();
            light.Add<PointLight>().Radius = 2f;
            light.Get<PointLight>().Color = Color4.Cyan;
            Entity ent = new Entity();
            ent.Add<Transform>().Position = new Vector3(0f, -10f, 0f);
            ent.Add<Renderer>();
            ent.Add<MeshFilter>();
            ent.Get<MeshFilter>().Mesh = new Sphere(1f);
            //ent.Get<MeshFilter>().Mesh = new Cube(1f, 3);
            ent.Add<Rigidbody>();
            ent.Add<Constraint>().MaxPosition = new Vector3(float.PositiveInfinity, 0f, float.PositiveInfinity);
            ent.Get<Constraint>().MinPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            Entity terrain = new Entity();
            terrain.Add<Transform>();
            terrain.Add<Renderer>();
            terrain.Add<MeshFilter>().Mesh = new HexGrid(256, 256);

            cam = new Entity();
            cam.Add<Transform>().Position = new Vector3(-10.71002f, -9.084502f, -7.3577f);
            cam.Get<Transform>().Rotation = new Quaternion(0.282464295625687f, -2.12368106842041f, 0f, 0f);
            cam.Add<Camera>().UseLighting = true;
            cam.Add<Freelook>();

            uiCam = new Entity();
            uiCam.Add<Transform>().Position = new Vector3(0f, 0f, 10f);
            uiCam.Add<Camera>().IsOrthographic = true;
            uiCam.Get<Camera>().UseAlphaBlend = true;
            uiCam.Get<Camera>().Layers = RenderLayers.GUI;

            shape = new Entity();
            shape.Add<Renderer>().Layers = RenderLayers.GUI;
            shape.Add<MeshFilter>().Mesh = new Spline(
                new Spline.Point(new Vector3(100f, 100f, 0f)),
                new Spline.Point(new Vector3(200f, 200f, 0f)),
                new Spline.Point(new Vector3(300f, 100f, 0f))
            );
        }

        int pointId;
        int pointType;

        float step = .01f;
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Input.Update();

            if (Input.IsPressed(Key.Escape)) { //For now, setting escape as a switch for cursor visibility
                CursorVisible = !CursorVisible;
                //Exit();
            }

            if (Input.IsPressed(Key.F4) && Input.Alt) Exit();

            /*if (Input.IsDown(Key.Space)) {
                Entity.Get<AJitterBody>(0).Rigidbody.IsActive = true;
                Entity.Get<AJitterBody>(0).Rigidbody.AddTorque(Jitter.LinearMath.JVector.Forward * 100f);
            }*/

            if (Input.IsPressed(Key.F1)) DebugEnabled = !DebugEnabled;

            if (Input.IsDown(Key.PageUp)) Light.AmbientColor = new Color4(Light.AmbientColor.R + step, Light.AmbientColor.G + step, Light.AmbientColor.B + step, Light.AmbientColor.A + step);
            if (Input.IsDown(Key.PageDown)) Light.AmbientColor = new Color4(Light.AmbientColor.R - step, Light.AmbientColor.G - step, Light.AmbientColor.B - step, Light.AmbientColor.A - step);

            Spline spline = shape.Get<MeshFilter>().Mesh as Spline;
            Vector3 mPos = new Vector3(Input.MousePosition.X, Input.MousePosition.Y, 0f);
            if (Input.IsPressed(MouseButton.Left)) {
                float minDist = 25f;
                pointType = -1;
                for (int i = 0; i < spline.Points.Count; i++) {
                    float dist = float.PositiveInfinity;
                    if (Input.Shift) {
                        dist = (spline.Points[i].Position + spline.Points[i].LeftControl - mPos).Length;
                        if (dist < minDist) {
                            minDist = dist;
                            pointId = i;
                            pointType = 1;
                        }
                        dist = (spline.Points[i].Position + spline.Points[i].RightControl - mPos).Length;
                        if (dist < minDist) {
                            minDist = dist;
                            pointId = i;
                            pointType = 2;
                        }
                    }
                    else {
                        dist = (spline.Points[i].Position - mPos).Length;
                        if (dist < minDist) {
                            minDist = dist;
                            pointId = i;
                            pointType = 0;
                        }
                    }
                }
            }
            if (Input.IsDown(MouseButton.Left)) {
                if (pointType == 0) spline.Points[pointId].Position = mPos;
                else if (pointType == 1) spline.Points[pointId].LeftControl = mPos - spline.Points[pointId].Position;
                else if (pointType == 2) spline.Points[pointId].RightControl = mPos - spline.Points[pointId].Position;
            }

            time += e.Time;
            float dt = (float)e.Time;

            CalcAvgFrameRate(e.Time);

            Engine.Update(dt); //Insert engine rev here VROOOOOOM
        }

        private void CalcAvgFrameRate(double dt) {
            timeLeft -= dt;
            accumFPS += dt;
            frameCount++;
            if (timeLeft <= 0) {
                frameRate = Math.Round(1f / (accumFPS / frameCount));
                accumFPS = 0.0;
                frameCount = 0;
                timeLeft = 0.5;
                Title = "Lotus - " + Math.Round(RenderFrequency) + " FPS";
            }
        }// Calculates the average framerate every quarter second. Relies on the OnUpdateFrame Method.

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            GL.Enable(EnableCap.DepthTest);
            Engine.Render();
            cam.Get<Camera>().Begin(cam.Get<Transform>().ViewMatrix);
            TransformGizmo();
            Debug.DrawStack();
            cam.Get<Camera>().End();
            uiCam.Get<Camera>().Begin(uiCam.Get<Transform>().ViewMatrix);
            if (DebugEnabled)
                DebugReadout();
            uiCam.Get<Camera>().End();
            SwapBuffers();
            GL.Disable(EnableCap.DepthTest);
        }

        public static void TransformGizmo() {
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

            Vector3 camPos = cam.Get<Transform>().Position;
            Quaternion camRot = cam.Get<Transform>().Rotation;

            text.Draw("DEBUG READOUT", new Vector2(border, spacing * n++ + border));
            text.Draw("Camera Location || Camera Rotation", new Vector2(border, spacing * n++ + border));
            text.Draw(("X: " + camPos.X).PadRight(15) + " || X:" + (float)(camRot.X % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw(("Y: " + camPos.Y).PadRight(15) + " || Y:" + (float)(camRot.Y % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw(("Z: " + camPos.Z).PadRight(15) + " || Z:" + (float)(camRot.Z % (2 * Math.PI)), new Vector2(border, spacing * n++ + border));
            text.Draw("Frame Rate: " + frameRate, new Vector2(border, spacing * n++ + border)); //framerate readout
            text.Draw("Time: " + time, new Vector2(border, spacing * n++ + border));
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            foreach (Camera cam in Entity.GetAll<Camera>()) cam.ResetProjectionMatrix();
            text.ResetView();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
        }
    }
}
