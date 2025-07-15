using RSACommon.Points;
using RSACommon.Shoes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.Scanner
{
    public class ScannerPointsMulti: Scanner<PointMultiDimensional>
    {

        public override Shoe<PointMultiDimensional> OpenFile(StreamReader fileStream, string shoeName)
        {
            string line;
            Shoe<PointMultiDimensional> shoe = new Shoe<PointMultiDimensional>(shoeName);

            line = fileStream.ReadLine();

            while (line != null)
            {
                var positionArray = line.Split(';');

                double x = Convert.ToDouble(positionArray[0]);
                double y = Convert.ToDouble(positionArray[1]);
                double z = Convert.ToDouble(positionArray[2]);

                //Dummy value for testing
                PointMultiDimensional p = new PointMultiDimensional(x, y, z, 180.000, 0, -135.00);
                shoe.PR[46] = new PointMultiDimensional();

                //end of testing
                p.Speed = 660;
                shoe.AddPoint(p);

                //Read the next line
                line = fileStream.ReadLine();
            }

            return shoe;
        }

        public override string ShoeJob(Shoe<PointMultiDimensional> shoe, string jobName, List<int> PR_TO_SET)
        {
            if (shoe == null)
                return string.Empty;

            string l1 = $"/PROG {jobName}\r\n/ATTR\r\nCOMMENT = \"RSAWORK\";\r\nDEFAULT_GROUP = 1,*,*,*,*;\r\nCONTROL_CODE = 00000000 00000000;\r\n/APPL\r\n/MN\r\n";

            l1 += $"1: PR[60] = P[1];\r\n2: R[260] = 1;\r\n3: CALL INIT(1) ;\r\n";
            int index = 4;

            foreach (int registerValue in PR_TO_SET)
            {
                if (shoe.PR.ContainsKey(registerValue))
                {
                    l1 += $"{index++}: PR[{registerValue}, 1] = {shoe.PR[registerValue].X.ToString("F3")};\r\n";
                    l1 += $"{index++}: PR[{registerValue}, 2] = {shoe.PR[registerValue].Y.ToString("F3")};\r\n";
                    l1 += $"{index++}: PR[{registerValue}, 3] = {shoe.PR[registerValue].Z.ToString("F3")};\r\n";
                }
            }

            l1 += $"{index++}:L P[1] R[5]mm/sec CNT R[6] ACC R[8] TOOL_OFFSET,PR[46] TB    R[10]sec,";

            string pointLogicOperator = "POINT_LOGIC;\r\n------";
            // l'oggetto "-----" serve per il POINT LOGIC
            l1 += pointLogicOperator + ";\r\n";

            string toAppendSpeed = string.Empty;

            foreach (var PointskeyValue in shoe.Points)
            {
                toAppendSpeed = $"{PointskeyValue.Value.Speed}mm/sec CNT R[6] ACC R[8] TOOL_OFFSET,PR[46];";

                if (PointskeyValue.Key == 0 || PointskeyValue.Key == 1)
                    continue;

                l1 += $"{index++}:L P[{PointskeyValue.Key}] {toAppendSpeed}\r\n";
            }

            l1 += $"{index++}: PR[60] = P[{shoe.PointsN}];\r\n";
            l1 += $"{index++}: CALL END(1) ;\r\n";
            l1 += $"{index++}: LBL[999];\r\n";

            string coordText = "";

            l1 += "/POS\r\n";
            foreach (var PointskeyValue in shoe.Points)
            {
                coordText = $"X = {PointskeyValue.Value.X.ToString("F3")} mm, Y = {PointskeyValue.Value.Y.ToString("F3")} mm, Z = {PointskeyValue.Value.Z.ToString("F3")} mm,\r\n";
                coordText += $"W = {PointskeyValue.Value.W.ToString("F3")} deg, P = {PointskeyValue.Value.P.ToString("F3")} deg, R = {PointskeyValue.Value.R.ToString("F3")} deg\r\n" + "};\r\n";
                l1 += $"P[{PointskeyValue.Key + 1}]" + "{\r\nGP1:\r\nUF: 1, UT: 1, CONFIG: 'N U T,0,0,0',\r\n" + coordText;

            }

            l1 += "/END\r\n";

            return l1;
        }

    }
}
