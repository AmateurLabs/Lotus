using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;

namespace Lotus {
    public static class Input {

        static KeyboardState oldKeys = new KeyboardState();
        static KeyboardState keys = new KeyboardState();
        static MouseState oldMouse = new MouseState();
        static MouseState mouse = new MouseState();

        public static bool IsDown(Key key) {
            return keys.IsKeyDown(key);
        }

        public static bool IsPressed(Key key) {
            return !oldKeys.IsKeyDown(key) && keys.IsKeyDown(key);
        }

        public static bool IsUp(Key key) {
            return keys.IsKeyUp(key);
        }

        public static bool IsReleased(Key key) {
            return oldKeys.IsKeyDown(key) && !keys.IsKeyDown(key);
        }

        public static bool Alt {
            get { return IsDown(Key.AltLeft) || IsDown(Key.AltRight); }
        }

        public static bool Shift {
            get { return IsDown(Key.ShiftLeft) || IsDown(Key.ShiftRight); }
        }

        public static bool Control {
            get { return IsDown(Key.ControlLeft) || IsDown(Key.ControlRight); }
        }

        public static bool IsDown(MouseButton btn) {
            return mouse.IsButtonDown(btn);
        }

        public static bool IsPressed(MouseButton btn) {
            return !oldMouse.IsButtonDown(btn) && mouse.IsButtonDown(btn);
        }

        public static bool IsUp(MouseButton btn) {
            return mouse.IsButtonUp(btn);
        }

        public static bool IsReleased(MouseButton btn) {
            return oldMouse.IsButtonDown(btn) && !oldMouse.IsButtonDown(btn);
        }

        public static Vector2 MousePosition {
            get {
                return new Vector2(Window.Main.Mouse.X, Window.Main.Mouse.Y);
            }
        }

        public static Vector2 MouseAbsPosition {
            get {
                return new Vector2(mouse.X, mouse.Y);
            }
        }

        public static Vector2 MouseDelta {
            get {
                return new Vector2(mouse.X - oldMouse.X, mouse.Y - oldMouse.Y);
            }
        }

        public static void Update() {
            oldKeys = keys;
            keys = Keyboard.GetState();
            oldMouse = mouse;
            mouse = Mouse.GetState();
            if (!Window.Main.CursorVisible)
                Mouse.SetPosition(Window.Main.Bounds.Left + Window.Main.Bounds.Width / 2, Window.Main.Bounds.Top + Window.Main.Bounds.Height / 2);
        }
    }
}
