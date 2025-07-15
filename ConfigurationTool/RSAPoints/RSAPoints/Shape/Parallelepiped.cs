using RSAPoints.Points;
using RSAPoints.Shape.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Shape
{
    public class Parallelepiped : ISolid
    {
        public double Height { get; private set; } = 0;
        public IShape2D Base { get; private set; } = null;
        public Point3[] Border { get; private set; } = null;
        public IRegion<Point3> Region { get; private set; } = null;
        public string Name { get; private set; } = string.Empty;
        public Parallelepiped(IShape2D rect, double height)
        {
            Base = rect;
            Height = height;

        }

        bool ISolid.IsInSolid(Point3 p)
        {
            throw new NotImplementedException();
        }

        bool ISolid.IsInSolid(ISolid solid)
        {
            throw new NotImplementedException();
        }
    }

    public class ParallelepipedRegion : RectRegion
    {
        public double Height { get; set; } = 0;
        public ISolid SolidParent { get; private set; } = null;
        //public ParallelepipedRegion(ISolid rect, double height) : this(rect.Base.Localization, rect.Base.Size.L1, rect.Base.Size.L2, height)
        //{

        //}

        public ParallelepipedRegion(Point2 localization, double l1, double  l2, double Height) : base(localization, l1, l2)
        {

        }
    }
}
