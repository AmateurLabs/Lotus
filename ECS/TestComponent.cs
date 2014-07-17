using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class TestComponent : Component {

        public IntValue Hashtag;
        public StringValue Yolo;
        public BoolValue Swag;
        public Vector3Value Foo;
        public EnumValue<Attractor.AttractionType> Bar;

        public TestComponent(int id) : base(id) {
            Hashtag = new IntValue(this, "Hashtag", 4);
            Yolo = new StringValue(this, "Yolo", "Yolo!");
            Swag = new BoolValue(this, "Swag", true);
            Foo = new Vector3Value(this, "Foo", new Vector3(1f, 2f, 3f));
            Bar = new EnumValue<Attractor.AttractionType>(this, "Bar", Attractor.AttractionType.World);
        }
    }
}
