using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    public class MeshValue : DataValue<Mesh> {

        public MeshValue(Component c, string name, Mesh value) : base(c, name, value) { }

        public enum MeshType {
            None = 0,

            //3D primitives
            Cube = 1,
            Sphere = 2,

            //2D primitives
            Square = 8,

            //Special meshes
            Spline = 16,
            HexGrid = 17,
            File = 32
        }

        public override void Serialize(BinaryWriter stream) {
            if (Value == null) {
                stream.Write((int)MeshType.None);
                return;
            }
            if (Value is Cube) {
                stream.Write((int)MeshType.Cube);
                Cube cube = Value as Cube;
                stream.Write(cube.Scale);
                stream.Write(cube.Res);
            }
            else if (Value is Sphere) {
                stream.Write((int)MeshType.Sphere);
                Sphere sphere = Value as Sphere;
                stream.Write(sphere.Radius);
            }
            else if (Value is Square) {
                stream.Write((int)MeshType.Square);
                Square square = Value as Square;
                stream.Write(square.Scale);
            }
            else if (Value is Spline) {
                stream.Write((int)MeshType.Spline);
                Spline spline = Value as Spline;
                stream.Write(spline.Points.Count);
                foreach (Spline.Point pt in spline.Points) {
                    stream.Write(pt.Position.X);
                    stream.Write(pt.Position.Y);
                    stream.Write(pt.Position.Z);
                    stream.Write(pt.LeftControl.X);
                    stream.Write(pt.LeftControl.Y);
                    stream.Write(pt.LeftControl.Z);
                    stream.Write(pt.RightControl.X);
                    stream.Write(pt.RightControl.Y);
                    stream.Write(pt.RightControl.Z);
                }
            }
            else if (Value is HexGrid) {
                stream.Write((int)MeshType.HexGrid);
                HexGrid hexGrid = Value as HexGrid;
                stream.Write(hexGrid.Size);
                stream.Write(hexGrid.OffX);
                stream.Write(hexGrid.OffY);
            }
            else {
                throw new NotImplementedException("Mesh type " + Value.GetType().Name + " doesn't have a serializer defined.");
            }
        }

        public override void Deserialize(BinaryReader stream) {
            MeshType type = (MeshType)stream.ReadInt32();
            if (type == MeshType.Cube) {
                Value = new Cube(stream.ReadSingle(), stream.ReadInt32());
            }
            else if (type == MeshType.Sphere) {
                Value = new Sphere(stream.ReadSingle());
            }
            else if (type == MeshType.Square) {
                Value = new Square(stream.ReadSingle());
            }
            else if (type == MeshType.Spline) {
                int ptCount = stream.ReadInt32();
                Spline.Point[] pts = new Spline.Point[ptCount];
                for (int i = 0; i < ptCount; i++) {
                    pts[i] = new Spline.Point(new Vector3(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle()), new Vector3(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle()), new Vector3(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle()));
                }
                Value = new Spline(pts);
            }
            else if (type == MeshType.HexGrid) {
                Value = new HexGrid(stream.ReadInt32(), stream.ReadInt32(), stream.ReadInt32());
            }
            else {
                throw new NotImplementedException("Mesh type " + type + " doesn't have a deserializer defined.");
            }
        }

        public override string Export() {
            if (Value == null) return MeshType.None.ToString();
            string output = "";
            if (Value is Cube) {
                Cube cube = Value as Cube;
                output += MeshType.Cube + "(" + cube.Scale + ", " + cube.Res + ")";
            }
            else if (Value is Sphere) {
                Sphere sphere = Value as Sphere;
                output += MeshType.Sphere + "(" + sphere.Radius + ")";
            }
            else if (Value is Square) {
                Square square = Value as Square;
                output += MeshType.Square + "(" + square.Scale + ")";
            }
            else if (Value is Spline) {
                Spline spline = Value as Spline;
                output += MeshType.Spline + "([";
                for (int i = 0; i < spline.Points.Count; i++) {
                    Spline.Point pt = spline.Points[i];
                    output += "(" + pt.Position.X + ", " + pt.Position.Y + ", " + pt.Position.Z + "); ";
                    output += "(" + pt.LeftControl.X + ", " + pt.LeftControl.Y + ", " + pt.LeftControl.Z + "); ";
                    output += "(" + pt.RightControl.X + ", " + pt.RightControl.Y + ", " + pt.RightControl.Z + ")";
                    if (i < spline.Points.Count - 1) output += "; ";
                }
                output += "])";
            }
            else if (Value is HexGrid) {
                HexGrid hexGrid = Value as HexGrid;
                output += MeshType.HexGrid + "(" + hexGrid.Size + ", " + hexGrid.OffX + ", " + hexGrid.OffY + ")";
            }
            else {
                throw new NotImplementedException("Mesh type " + Value.GetType().Name + " doesn't have an exporter defined.");
            }
            return output;
        }

        public override void Import(string input) {
            int splitPt = input.IndexOf('(');
            string typeStr = (splitPt == -1) ? input : input.Substring(0, splitPt);
            MeshType type = (MeshType)Enum.Parse(typeof(MeshType), typeStr);
            if (type == MeshType.None) return;
            string data = input.Substring(splitPt);
            data = data.Substring(1, data.Length - 2);
            if (type == MeshType.Cube) {
                string[] bits = data.Replace(", ", ",").Split(',');
                Value = new Cube(float.Parse(bits[0]), int.Parse(bits[1]));
            }
            else if (type == MeshType.Sphere) {
                Value = new Sphere(float.Parse(data));
            }
            else if (type == MeshType.Square) {
                Value = new Square(float.Parse(data));
            }
            else if (type == MeshType.Spline) {
                data = data.Substring(1, data.Length - 1);
                data = data.Replace("; ", ";");
                string[] ptData = data.Split(';');
                List<Spline.Point> points = new List<Spline.Point>();
                for (int i = 0; i < ptData.Length; i += 3) {
                    string[] posBits = ptData[i].Substring(1, ptData[i].Length - 2).Replace(", ", ",").Split(',');
                    string[] leftBits = ptData[i+1].Substring(1, ptData[i+1].Length - 2).Replace(", ", ",").Split(',');
                    string[] rightBits = ptData[i+2].Substring(1, ptData[i+2].Length - 2).Replace(", ", ",").Split(',');
                    points.Add(new Spline.Point(new Vector3(float.Parse(posBits[0]), float.Parse(posBits[1]), float.Parse(posBits[2])), new Vector3(float.Parse(leftBits[0]), float.Parse(leftBits[1]), float.Parse(leftBits[2])), new Vector3(float.Parse(rightBits[0]), float.Parse(rightBits[1]), float.Parse(rightBits[2]))));
                }
                Value = new Spline(points.ToArray());
            }
            else if (type == MeshType.HexGrid) {
                string[] bits = data.Replace(", ", ",").Split(',');
                Value = new HexGrid(int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));
            }
            else {
                throw new NotImplementedException("Mesh type " + type + " doesn't have an importer defined.");
            }
        }
    }
}
