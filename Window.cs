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

        public Queue<Action> WorkQueue = new Queue<Action>();

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

            Engine.Processors.Add(new JitterProcessor());
            Engine.Processors.Add(new PhysicsProcessor());
            Engine.Processors.Add(new FreelookProcessor());
            Engine.Processors.Add(new AudioProcessor());
            Engine.Processors.Add(new RenderProcessor());

            Entity worldEntity = Entity.WrapNew();
            worldEntity.Add<Renderer>();
            worldEntity.Add<Attractor>().Type.Value = Attractor.AttractionType.World;
            //worldEntity.Get<Attractor>().Acceleration.Value = 9.81f;
            worldEntity.Get<Attractor>().Acceleration.Value = 1f;
            worldEntity.Add<DirectionalLight>().Direction.Value = Vector3.UnitY;
            worldEntity.Get<DirectionalLight>().Intensity.Value = 1f;

            Entity ent = Entity.WrapNew();
            ent.Add<Transform>().Position.Value = new Vector3(10f, -10f, 0f);
            ent.Add<Renderer>();
            ent.Add<MeshFilter>();
            ent.Get<MeshFilter>().Mesh.Value = new Sphere(1f);
            //ent.Get<MeshFilter>().Mesh.Value = new Cube(1f, 3);
            ent.Add<Rigidbody>();
            ent.Add<Constraint>().MaxPosition.Value = new Vector3(float.PositiveInfinity, 0f, float.PositiveInfinity);
            ent.Get<Constraint>().MinPosition.Value = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            //ent.Add<PointLight>().Radius.Value = 2f;
            //ent.Get<PointLight>().Color.Value = Color4.Cyan;
            ent.Add<TestComponent>();
            ent.Add<AudioSource>().Clip.Value = new WaveClip(WaveType.Sine);
            ent.Get<AudioSource>().Looping.Value = true;
            ent.Get<AudioSource>().State.Value = AudioSourceState.Play;
            ent.Save(true);
            ent.Load(true);

            int size = 31;
            int f = (int)Math.Floor(size/2.0);
            int c = (int)Math.Ceiling(size/2.0);
            for (int x = -15; x <= 15; x++) {
                for (int y = -15; y <= 15; y++) {
                    int tempX = x;
                    int tempY = y;
                    WorkQueue.Enqueue(() => {
                        Entity terrain = Entity.WrapNew();
                        terrain.Add<Transform>();
                        terrain.Add<Renderer>();
                        terrain.Add<MeshFilter>().Mesh.Value = new HexGrid(size, tempX * size + tempY * -f, tempY * size + tempX * -c);
                    });
                }
            }

            cam = Entity.WrapNew();
            cam.Add<Transform>().Position.Value = new Vector3(-10.71002f, -9.084502f, -7.3577f);
            cam.Get<Transform>().Rotation.Value = new Quaternion(0.282464295625687f, -2.12368106842041f, 0f, 0f);
            cam.Add<Camera>().UseLighting.Value = true;
            cam.Add<Freelook>();
            cam.Add<AudioListener>();

            uiCam = Entity.WrapNew();
            uiCam.Add<Transform>().Position.Value = new Vector3(0f, 0f, 10f);
            uiCam.Add<Camera>().IsOrthographic.Value = true;
            uiCam.Get<Camera>().UseAlphaBlend.Value = true;
            uiCam.Get<Camera>().LayerMask.Value = Layers.Layer1;

            shape = Entity.WrapNew();
            shape.Add<Transform>().Position.Value = new Vector3(100f, 100f, 0f);
            shape.Add<Renderer>().LayerMask.Value = Layers.Layer1;
            shape.Add<MeshFilter>().Mesh.Value = new Spline(
                new Spline.Point(new Vector3(-56f, 32f, 0f)),
                new Spline.Point(new Vector3(-56f, -32f, 0f)),
                new Spline.Point(new Vector3(0f, -64f, 0f)),
                new Spline.Point(new Vector3(56f, -32f, 0f)),
                new Spline.Point(new Vector3(56f, 32f, 0f)),
                new Spline.Point(new Vector3(0f, 64f, 0f))
            );
            shape.Get<MeshFilter>().Color.Value = Color4.Red;
        }

        int pointId;
        int pointType;

        float step = .01f;
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Debug.Update();
            Input.Update();

            time += e.Time;
            float dt = (float)e.Time;
            CalcAvgFrameRate(e.Time);

            if (Input.IsPressed(Key.Escape)) { //For now, setting escape as a switch for cursor visibility
                CursorVisible = !CursorVisible;
                //Exit();
            }

            if (Input.IsPressed(Key.F4) && Input.Alt) Exit();

            /*if (Input.IsDown(Key.Space)) {
                Entity.Get<AJitterBody>(0).Rigidbody.IsActive = true;
                Entity.Get<AJitterBody>(0).Rigidbody.AddTorque(Jitter.LinearMath.JVector.Forward * 100f);
            }*/

            Entity.Get<AudioSource>(1).Pitch.Value = (float)Math.Pow(1.05946, 12 * ((time) % 1));

            if (Input.IsPressed(Key.F1)) DebugEnabled = !DebugEnabled;

            if (Input.IsDown(Key.PageUp)) Light.AmbientColor = new Color4(Light.AmbientColor.R + step, Light.AmbientColor.G + step, Light.AmbientColor.B + step, Light.AmbientColor.A + step);
            if (Input.IsDown(Key.PageDown)) Light.AmbientColor = new Color4(Light.AmbientColor.R - step, Light.AmbientColor.G - step, Light.AmbientColor.B - step, Light.AmbientColor.A - step);

            Spline spline = shape.Get<MeshFilter>().Mesh.Value as Spline;
            Vector3 mPos = new Vector3(Input.MousePosition.X, Input.MousePosition.Y, 0f) - shape.Get<Transform>().Position.Value;
            if (Input.IsPressed(MouseButton.Left)) {
                float minDist = 25f;
                pointType = -1;
                float dist = float.PositiveInfinity;
                for (int i = 0; i < spline.Points.Count; i++) {
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
                dist = (Vector3.Zero - mPos).Length;
                if (dist < minDist) {
                    minDist = dist;
                    pointType = 3;
                }
            }
            if (Input.IsDown(MouseButton.Left)) {
                if (!Input.Control) {
                    mPos.X = (float)Math.Round(mPos.X / 8f) * 8f;
                    mPos.Y = (float)Math.Round(mPos.Y / 8f) * 8f;
                    mPos.Z = (float)Math.Round(mPos.Z / 8f) * 8f;
                }
                if (pointType == 0) spline.Points[pointId].Position = mPos;
                else if (pointType == 1) spline.Points[pointId].LeftControl = mPos - spline.Points[pointId].Position;
                else if (pointType == 2) spline.Points[pointId].RightControl = mPos - spline.Points[pointId].Position;
                else if (pointType == 3) shape.Get<Transform>().Position.Value += mPos;
            }
            Entity.Get<DirectionalLight>(0).Direction.Value = Vector3.TransformNormal(Entity.Get<DirectionalLight>(0).Direction.Value, Matrix4.CreateRotationX((float)(time % 360)/100000f));

            Vector3 camPos = cam.Get<Transform>().Position.Value;
            Quaternion camRot = cam.Get<Transform>().Rotation.Value;

            Debug.AddMsg("DEBUG READOUT");
            Debug.AddMsg("Camera Location || Camera Rotation");
            Debug.AddMsg(("X: " + camPos.X).PadRight(15) + " || X:" + (float)(camRot.X % (2 * Math.PI)));
            Debug.AddMsg(("Y: " + camPos.Y).PadRight(15) + " || Y:" + (float)(camRot.Y % (2 * Math.PI)));
            Debug.AddMsg(("Z: " + camPos.Z).PadRight(15) + " || Z:" + (float)(camRot.Z % (2 * Math.PI)));
            Debug.AddMsg("Frame Rate: " + frameRate); //framerate readout
            Debug.AddMsg("Time: " + time);

            //Raycast from the center of the camera and display cursor
            Vector3 hex;
            if (HexGrid.Raycast(Entity.Get<Transform>(Camera.Main.Id).Position.Value, Entity.Get<Transform>(Camera.Main.Id).Forward, out hex, 256f)) {
                int mapX = (int)hex.X;
                int mapY = (int)hex.Y;
                Vector3 p = HexGrid.ToWorld(mapX, mapY);
                p.Y = 0f;
                Debug.DrawLater(() => {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.Begin(PrimitiveType.TriangleFan);
                    GL.Color3(1f, 0f, 1f);
                    for (int k = 0; k < HexGrid.HexVerts.Length; k++) GL.Vertex3(p + HexGrid.HexVerts[k] + Vector3.UnitY * HexGrid.GetHeight(p.X + HexGrid.HexVerts[k].X, p.Z + HexGrid.HexVerts[k].Z) * 8f - Vector3.UnitY * 0.01f);
                    GL.Vertex3(p + HexGrid.HexVerts[1] + Vector3.UnitY * HexGrid.GetHeight(p.X + HexGrid.HexVerts[1].X, p.Z + HexGrid.HexVerts[1].Z) * 8f - Vector3.UnitY * 0.01f);
                    GL.End();
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.Red);
                    GL.Vertex3(HexGrid.ToWorld(mapX, mapY) - Vector3.UnitY * 0.02f);
                    GL.Vertex3(HexGrid.ToWorld(mapX + 1, mapY) - Vector3.UnitY * 0.02f);
                    GL.Color3(Color.Green);
                    GL.Vertex3(HexGrid.ToWorld(mapX, mapY) - Vector3.UnitY * 0.02f);
                    GL.Vertex3(HexGrid.ToWorld(mapX, mapY + 1) - Vector3.UnitY * 0.02f);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(HexGrid.ToWorld(mapX, mapY) - Vector3.UnitY * 0.02f);
                    GL.Vertex3(HexGrid.ToWorld(mapX - 1, mapY + 1) - Vector3.UnitY * 0.02f);
                    GL.End();
                });
                Debug.AddMsg("Cursor Location: " + hex.X + ", " + hex.Y + ", " + -(hex.X + hex.Y));
            }

            if (WorkQueue.Count > 0) WorkQueue.Dequeue()();

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
            Debug.DrawUIStack();
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
            IList<string> msgs = Debug.GetDebugMsgs();
            float border = 10f;
            int spacing = 12;

            Debug.Depth = -0.9f;
            Debug.DrawRect((float)((Width / 2) - 1), (float)((Height / 2) - 10), 2f, 20f, new Color4(1f, 1f, 1f, 1f));
            Debug.DrawRect((float)((Width / 2) - 10), (float)((Height / 2) - 1), 20f, 2f, new Color4(1f, 1f, 1f, 1f));
            Debug.Depth = -1f;
            Debug.DrawRectFrame(Vector2.Zero, new Vector2(435, border + spacing * msgs.Count + border), Color4.White, new Color4(1f, 0f, 1f, 0.5f), border / 2);
            Debug.Depth = -1.1f;
            Debug.DrawRectFrame(Vector2.Zero, new Vector2(Width, Height), Color4.White, new Color4(0.1f, 0.1f, 0.5f, 0.3f), border / 2);

            int n = 0;
            
            foreach (string msg in msgs) {
                text.Draw(msg, new Vector2(border, spacing * n++ + border));
            }
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
