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
            font = new QFont(new Font(FontFamily.GenericMonospace, 10f, FontStyle.Regular));
        }

        public void Draw(string text, OpenTK.Vector2 position) {
            //QFont.Begin();
            font.Print(text, position, QFontAlignment.Left);
            //QFont.End();
        }

        public void ResetView() {
            QFont.RefreshViewport();
        }
    }
}
