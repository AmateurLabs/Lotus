using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using QuickFont;

namespace Lotus {

    public class Text {

        QFont font;

        public Text() {
            font = new QFont(new Font(FontFamily.GenericMonospace, 16f));
        }

        public void Draw(string text, OpenTK.Vector2 position) {
            font.Print(text, position);
        }
    }
}
