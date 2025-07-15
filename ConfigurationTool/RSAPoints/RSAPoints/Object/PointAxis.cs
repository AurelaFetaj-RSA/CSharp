using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Points
{
    public class PointAxis : IPoint
    {
        public int Index { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public float CustomFloatParam { get; set; } = 0.0f;
        public Procedures Procedures { get; private set; } = new Procedures();

        public float Q1 { get; set; } = 0.0f;
        public float Q2 { get; set; } = 0.0f;
        public float Q3 { get; set; } = 0.0f;
        public float Q4 { get; set; } = 0.0f;
        public int V1 { get; set; } = 0;
        public int V2 { get; set; } = 0;
        public int V3 { get; set; } = 0;
        public int V4 { get; set; } = 0;


        public PointAxis(float q1, float q2, float q3, float q4, int v1, int v2, int v3, int v4)
        {
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
        }

        public PointAxis() : this(0, 0, 0, 0, 0, 0, 0,0)
        {

        }

        /// <summary>
        /// Costruttore copia
        /// </summary>
        /// <param name="p"></param>
        public PointAxis(PointAxis p) : this(p.Q1, p.Q2, p.Q3, p.Q4, p.V1, p.V2, p.V3, p.V4)
        {

        }
    }

    public class PointAxisInt : IPoint
    {
        public int Index { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public int CustomFloatParam { get; set; } = 0;
        public Procedures Procedures { get; private set; } = new Procedures();

        public int Q1 { get; set; } = 0;
        public int Q2 { get; set; } = 0;
        public int Q3 { get; set; } = 0;
        public int Q4 { get; set; } = 0;
        public int V1 { get; set; } = 0;
        public int V2 { get; set; } = 0;
        public int V3 { get; set; } = 0;
        public int V4 { get; set; } = 0;


        public PointAxisInt(int q1, int q2, int q3, int q4, int v1, int v2, int v3, int v4)
        {
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
        }

        public PointAxisInt() : this(0, 0, 0, 0, 0, 0, 0, 0)
        {

        }

        /// <summary>
        /// Costruttore copia
        /// </summary>
        /// <param name="p"></param>
        public PointAxisInt(PointAxisInt p) : this(p.Q1, p.Q2, p.Q3, p.Q4, p.V1, p.V2, p.V3, p.V4)
        {

        }
    }
}
