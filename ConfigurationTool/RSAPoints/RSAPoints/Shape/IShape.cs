using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Shape.Interface
{
    public interface IRegion<T>
    {
        bool Intersecate(T pointToCheck);
        bool Intersecate(IShape2D pointToCheck);
        T[] Border { get; }
    }

    public interface IShape2D
    {
        Size Size { get; }
        IRegion<Point2> Region { get; }

    }

    public struct Size
    {
        public double L1 { get; private set; }
        public double L2 { get; private set; }

        public Size(double height, double width)
        {
            L1 = height;
            L2 = width;
        }

        public static readonly Size Empty;
    }
}
