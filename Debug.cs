using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
    public static class Debug {
        static Text debugText = new Text();

        public static float Depth;
        public static Random rand = new Random();

        public static void DrawRect(Vector2 p0, Vector2 p1, Color4 color) {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);
            GL.Vertex3(p0.X, p0.Y, Depth);
            GL.Vertex3(p1.X, p0.Y, Depth);
            GL.Vertex3(p1.X, p1.Y, Depth);
            GL.Vertex3(p0.X, p1.Y, Depth);
            GL.End();
        }

        public static void DrawTri(Vector3 p0, Vector3 p1, Vector3 p2, Color4 color)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(color);
            GL.Vertex3(p0);
            GL.Vertex3(p1);
            GL.Vertex3(p2);
            GL.Normal3(Vector3.Normalize(Vector3.Cross(p1 - p0, p2 - p0)));
            GL.End();

        }

        public static void DrawRect(float x, float y, float width, float height, Color4 color) {
            DrawRect(new Vector2(x, y), new Vector2(x + width, y + height), color);
        }

        public static void DrawRect(Vector2 p0, Vector2 p1, float r, float g, float b, float a) {
            DrawRect(p0, p1, new Color4(r, g, b, a));
        }

        public static void DrawRect(float x, float y, float width, float height, float r, float g, float b, float a) {
            DrawRect(new Vector2(x, y), new Vector2(x + width, y + height), new Color4(r, g, b, a));
        }

        public static void DrawRect(Vector2 p0, Vector2 p1, float r, float g, float b) {
            DrawRect(p0, p1, new Color4(r, g, b, 1f));
        }

        public static void DrawRect(float x, float y, float width, float height, float r, float g, float b) {
            DrawRect(new Vector2(x, y), new Vector2(x + width, y + height), new Color4(r, g, b, 1f));
        }
        public static void DrawRectBorder(Vector2 p0, Vector2 p1, Color4 color, float thickness)
        {
            DrawRect(new Vector2(p0.X, p0.Y), new Vector2(thickness, p1.Y - p0.Y), color);
            DrawRect(new Vector2(thickness, p1.Y - p0.Y - thickness), new Vector2(p1.X - p0.X - thickness, p1.Y - p0.Y), color);
            DrawRect(new Vector2(p1.X-p0.X - thickness, p0.Y), new Vector2(p1.X-p0.X, p1.Y-p0.Y), color);
            DrawRect(new Vector2(thickness, p0.Y), new Vector2(p1.X - p0.X - thickness, thickness), color);
        }
        public static void DrawRectFrame(Vector2 p0, Vector2 p1, Color4 color, Color4 fillColor, float thickness)
        {
            Vector2 borderstuff = new Vector2(thickness, thickness);
            DrawRect(p0 + borderstuff, p1 - borderstuff, fillColor);
            DrawRectBorder(p0, p1, color, thickness);
            
        }

        public static void DrawText(Vector2 p, string text) {
            GL.PushMatrix();
            GL.Translate(0f, 0f, Depth);
            debugText.Draw(text, p);
            GL.PopMatrix();
        }

        public static void DrawText(float x, float y, string text) {
            DrawText(new Vector2(x, y), text);
        }

        public static Color4 RandColor(int i)
        {
            rand = new Random(i);
            return new Color4((float)rand.Next(255) / 255, (float)rand.Next(255) / 255, (float)rand.Next(255) / 255, 1f);
        }

        
    }
}
