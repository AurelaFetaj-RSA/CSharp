using Newtonsoft.Json;
using RSACommon.Shoes;
using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Points
{
    public class Zone<T> where T : IPoint
    {
        [JsonIgnore]
        public int ElementsN { get => ListOfIndexPoints.Count; }
        [JsonIgnore]
        public Dictionary<int, T> Points { get; private set; } = new Dictionary<int, T>();
        public List<int> ListOfIndexPoints { get; set; } = new List<int>();
        public string Description { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        OWithPoints<T> ParentShoe { get; set; } = null;
        public Zone(OWithPoints<T> shoe, string name, string desscription = "")
        {
            Name = name;
            Description = desscription;
            ParentShoe = shoe;
        }

        /// <summary>
        /// Every zone has the points with internal INDEX, the points maintains the global index
        /// </summary>
        public void MakeZones()
        {
            int i = 0;
            foreach (int index in ListOfIndexPoints)
            {
                if (ParentShoe.Points.Count > index)
                    Points[i++] = ParentShoe.Points[index];
            }
        }

        public void SetParent(OWithPoints<T> parent)
        {
            ParentShoe = parent;
        }
    }

    public class PointMultiDimensional : IPoint
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        public double W { get; set; } = 0;
        public double P { get; set; } = 0;
        public double R { get; set; } = 0;
        public int Index { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public Procedures Procedures { get; private set; } = new Procedures();
        public PointMultiDimensional(double x, double y, double z, double w, double p, double r)
        {
            X = Math.Round(x, 3);
            Y = Math.Round(y, 3);
            Z = Math.Round(z, 3);
            W = Math.Round(w, 3);
            P = Math.Round(p, 3);
            R = Math.Round(r, 3);
        }

        public PointMultiDimensional() : this(0, 0, 0, 0, 0, 0)
        {

        }
    }
}
