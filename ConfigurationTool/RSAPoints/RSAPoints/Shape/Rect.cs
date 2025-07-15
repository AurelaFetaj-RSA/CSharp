using RSAPoints.Points;
using RSAPoints.Shape.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Shape
{
    public class Rect: IShape2D
    {
        public Point3 Localization { get; set; } = null;
        public IRegion<Point2> Region { get; private set; } = null;
        public string Name { get; private set; } = string.Empty;
        public Size Size { get; private set; } = new Size();

        public Rect(double width, double height)
        {
            //Localization = localization;
            Size = new Size(height, width);

            //Region = new RectRegion(this);
        }

        public bool IsPointInRegion(Point2 p)
        {
            return Region.Intersecate(p);
        }

        public bool IsRegionInConflict(IShape2D p)
        {
            return Region.Intersecate(p);
        }

        public void SetPoint(Point2 point)
        {
            Localization = new Point3(point.X, point.Y, 0);
        }

        public void SetPoint(Point3 point)
        {
            Localization = point;
        }
    }

    /// <summary>
    /// Convex region
    /// </summary>
    public class RectRegion : IRegion<Point2>
    {
        public static double Step { get => 0.25; }
        public Point2[] Border { get; private set; } = null;
        public IShape2D Parent { get; private set; } = null;

        public RectRegion(Rect rect): this(rect.Localization, rect.Size.L1, rect.Size.L2)
        {
            Parent = rect;
        }

        public RectRegion(Point2 localization, double width, double height)
        {
            double maxX = localization.X + width;
            double maxY = localization.Y + height;

            List<Point2> points = new List<Point2>();

            for (double x = localization.X; x < maxX; x += Step)
            {
                for (double y = localization.Y; y < maxY; y += Step)
                {
                    points.Add(new Point2(x, y));
                }
            }

            Border = points.ToArray();
        }

        /// <summary>
        /// Be on the border, is in region
        /// </summary>
        /// <param name="pointToCheck"></param>
        /// <returns></returns>
        public bool Intersecate(Point2 pointToCheck)
        {
            if (pointToCheck == null)
                return false;

            double minX = Border.Min(p => p.X);
            double maxX = Border.Max(p => p.X);
            double minY = Border.Min(p => p.Y);
            double maxY = Border.Max(p => p.Y);

            if (pointToCheck.X < minX || pointToCheck.X > maxX)
                return false;

            if (pointToCheck.Y < minY || pointToCheck.Y > maxY)
                return false;

            return true;
        }

        public bool Intersecate(IShape2D pointToCheck)
        {
            foreach (Point2 p in pointToCheck.Region.Border)
            {
                if (Intersecate(p))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
