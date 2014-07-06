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

        public static void Update() {
            oldKeys = keys;
            keys = Keyboard.GetState();
        }
    }
}
