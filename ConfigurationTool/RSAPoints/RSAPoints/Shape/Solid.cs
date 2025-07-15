using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Shape.Interface
{
    public interface ISolid
    {
        double Height { get; }
        Interface.IShape2D Base { get; }
        Point3[] Border { get; }
        bool IsInSolid(Point3 p);
        bool IsInSolid(ISolid solid);
        IRegion<Point3> Region { get; }
        string Name { get; }
    }
}
