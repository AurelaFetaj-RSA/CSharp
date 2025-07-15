using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Points
{
    public interface IPoint
    {
        Procedures Procedures { get; }
        int Index { get; set; }
    }
}
