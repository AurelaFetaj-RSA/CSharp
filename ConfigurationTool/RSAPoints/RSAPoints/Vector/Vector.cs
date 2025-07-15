using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSAPoints.Points;


namespace RSAPoints.Vector
{
    public class Pose
    {
        public Point3 Point3Val { get; set; } = new Point3();
        public Angle AngleVal { get; set; } = new Angle();
    }

    public class Matrix<T>
    {
        private List<T[]> columns { get; set; } = new List<T[]>();
        private List<T> singleValue { get; set; } = new List<T>();
        public Matrix(params T[][] cols)
        {
            ///cols è un array
            cols.ToList().ForEach(x => columns.Add(x));
        }

        public int ColumnsCount => columns.Count;

        public int RowsCount
        {
            get
            {
                if (columns.Count > 0)
                    return (columns[0] as Array).Length;
                else
                    return 0;
            }
        }

        public Matrix()
        {
        }

        public string Print()
        {
            string text = string.Empty;

            var length = (columns[0] as Array).Length;

            for (int i = 0; i < length; i++) //scorro le righe
            {
                foreach (T[] t in columns)
                {
                    text += t[i] + ",";
                }

                text += "\n";
            }

            return text;
        }

        public T this[int row, int col]
        {
            get
            {
                return (T)columns.ElementAt(col)[row];
            }
        }

        public T[] this[int col]
        {
            get
            {
                return (T[])columns.ElementAt(col);
            }
        }
    }


    public static class RobotContourHelper
    {
        public static decimal Angle2(Point3 p1, Point3 p2)
        {
            return Angle2(new Point2(p1.X, p1.Y), new Point2(p2.X, p2.Y));
        }

        public static double[,] Rz(double angle)
        {
            double ct = Math.Cos(angle);
            double st = Math.Sin(angle);

            return new double[3, 3]
            {
                { ct, -st, 0 },
                { st, ct, 0 },
                { 0, 0, 1 },
            };
        }

        /// <summary>
        /// Wrap angle to 0 -360 at -180 180
        /// </summary>
        /// <param name="angleInDeg"></param>
        /// <returns></returns>
        public static double WrapBetween180(double angleInDeg)
        {
            while (angleInDeg > 180 || angleInDeg < -180) 
            {
                if (angleInDeg > 180)
                {
                    angleInDeg -= 360;
                }

                if (angleInDeg < -180)
                {
                    angleInDeg += 360;
                }

            }

            return angleInDeg;
        }

        public static double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            if (cA != rB)
            {
                Console.WriteLine("Matrixes can't be multiplied!!");
                return null;
            }
            else
            {
                double temp = 0;
                double[,] kHasil = new double[rA, cB];

                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }

                return kHasil;
            }
        }


        public static decimal Angle2(Point2 p1, Point2 p2)
        {
            Point2 py = Normalize2(p1 - p2);
            double pyNorm = Norm2(py);
            Angle toFind = new Angle();


            return (decimal)Math.Atan2((double)(p1 * p2), pyNorm);
        }

        public static List<Point3> VerySimpleAngleVector(Point3 pBefore, Point3 p, Point3 pAfter)
        {
            Point3 py = Normalize2(pAfter - pBefore);


            Point3 px = Normalize2(new Point3(-py.Y, py.X, 0));
            Point3 pz = new Point3();
            var scalar = (decimal)(px * py);

            if (scalar == 0)
            {
                pz = CrossProduct(px, py);
            }

            List<Point3> result = new List<Point3>() { px, py, pz };

            return result;
        }

        //public static

        public static Point3 CrossProduct(Point3 p1, Point3 p2)
        {
            return new Point3(p1.Y * p2.Z - p2.Y * p1.Z, -(p1.X * p2.Z - p2.X * p1.Z), p1.X * p2.Y - p2.X * p1.Y);
        }

        public static Angle GetMatrix(Point3 pBefore, Point3 p, Point3 pAfter)
        {
            Point3 py = pAfter - pBefore;
            double pyNorm = Norm2(py);

            Point3 vy = new Point3(py.X / pyNorm, py.Y / pyNorm, py.Z / pyNorm);

            return new Angle();
        }

        public static Point2 Normalize2(Point2 p)
        {
            var norm = Norm2(p);

            return new Point2(p.X / norm, p.Y / norm);
        }
        public static Point3 Normalize2(Point3 p)
        {
            var norm = Norm2(p);

            return new Point3(p.X / norm, p.Y / norm, p.Z / norm);
        }

        public static double Norm2(Point2 p1)
        {
            return Norm2(p1, p1);
        }
        public static double Norm2(Point2 p1, Point2 p2)
        {
            double squareRoot = Math.Sqrt((double)(p1.X * p2.X + p1.Y * p2.Y));
            return Math.Round(squareRoot, p1.DECIMAL_ROUNDS);
        }

        public static double Norm2(Point3 p1)
        {
            return Norm2(p1, p1);
        }
        public static double Norm2(Point3 p1, Point3 p2)
        {
            double squareRoot = Math.Sqrt((double)(p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z));
            return Math.Round(squareRoot, p1.DECIMAL_ROUNDS);
        }

        public static void PrintPoints(string name, double[] rows, double[] cols, double[] distance = null)
        {
            Pose[] undersampledPose = new Pose[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                undersampledPose[i] = new Pose();
                undersampledPose[i].AngleVal = new Angle();
                undersampledPose[i].Point3Val = new Point3();

                undersampledPose[i].Point3Val.X = rows[i];
                undersampledPose[i].Point3Val.Y = cols[i];
            }

            PrintPoints(name, undersampledPose, distance);
        }


        public static void PrintPoints(string name, Pose[] poses, double[] distance = null)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(name))
            {
                for(int i = 0; i < poses.Length; i++)
                {
                    string lineToWrite = $"{poses[i].Point3Val.X.ToString("F2")};";
                    lineToWrite += $"{poses[i].Point3Val.Y.ToString("F2")};";
                    lineToWrite += $"{poses[i].Point3Val.Z.ToString("F2")};";
                    lineToWrite += $"{poses[i].AngleVal.A.ToString("F2")};";
                    lineToWrite += $"{poses[i].AngleVal.B.ToString("F2")};";
                    lineToWrite += $"{poses[i].AngleVal.C.ToString("F2")};";

                    if(distance != null)
                    {
                        lineToWrite += $"{distance[i].ToString("F2")};";
                    }
                    else
                    {
                        lineToWrite += $"-1;";
                    }

                    file.WriteLine(lineToWrite);
                }
            }
        }
    };
};

