using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Points
{
    public class Point2
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public virtual int DECIMAL_ROUNDS => 5;
        public Point2(Point2 p) : this(p.X, p.Y)
        {
        }

        public Point2(double x, double y)
        {
            X = Math.Round(x, DECIMAL_ROUNDS);
            Y = Math.Round(y, DECIMAL_ROUNDS);
        }
        public Point2() : this(0, 0)
        {
        }

        public static Point2 operator +(Point2 p1, Point2 p2)
        {
            Point2 result = new Point2(p1);

            result.X += p2.X;
            result.Y += p2.Y;

            Point2 resultRound = new Point2(result);
            return resultRound;
        }
        public static double operator *(Point2 p1, Point2 p2) => Math.Round(p1.X * p2.X + p1.Y * p2.Y, p1.DECIMAL_ROUNDS);
        public static Point2 operator -(Point2 p1, Point2 p2)
        {
            Point2 result = new Point2(p1);

            result.X -= p2.X;
            result.Y -= p2.Y;

            Point2 resultRound = new Point2(result);
            return resultRound;
        }

        public Point2(Point3 p):this(p.X, p.Y)
        {
        }
    }
    public class Point3 : Point2
    {
        public override int DECIMAL_ROUNDS => 5;
        public Point3(Point3 p) : this(p.X, p.Y, p.Z)
        {
        }
        public Point3() : this(0, 0, 0)
        {
        }
        public Point3(double x, double y, double z) : base(x, y)
        {
            Z = Math.Round(z, DECIMAL_ROUNDS);
        }

        public double Z { get; set; } = 0;
        public static Point3 operator +(Point3 p1, Point3 p2)
        {
            Point3 result = new Point3(p1);

            result.X += p2.X;
            result.Y += p2.Y;
            result.Z += p2.Z;

            Point3 resultRound = new Point3(result);
            return resultRound;
        }

        /// <summary>
        /// Scalar Product
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double operator *(Point3 p1, Point3 p2) => Math.Round(p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z, p1.DECIMAL_ROUNDS);
        public static Point3 operator -(Point3 p1, Point3 p2)
        {
            Point3 result = new Point3(p1);

            result.X -= p2.X;
            result.Y -= p2.Y;
            result.Z -= p2.Z;

            Point3 resultRound = new Point3(result);
            return resultRound;
        }
    }

    public class Angle
    {
        public double A { get; set; } = 0;
        public double B { get; set; } = 0;
        public double C { get; set; } = 0;
    }

}

