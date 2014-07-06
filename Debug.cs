using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
    public static class Debug {
        static Text text = new Text();

        public static float Depth;

        public static void DrawRect(Vector2 p0, Vector2 p1, Color4 color) {
            GL.Begin(BeginMode.Quads);
            GL.Color4(color);
            GL.Vertex3(p0.X, p0.Y, Depth);
            GL.Vertex3(p1.X, p0.Y, Depth);
            GL.Vertex3(p1.X, p1.Y, Depth);
            GL.Vertex3(p0.X, p1.Y, Depth);
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
    }
}
