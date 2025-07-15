using RSACommon.Points;
using RSACommon.Shoes;
using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSAPoints.FanucOld
{
    public class FanucScanner
    {
        public static void MakeJob(Shoe<PointMultiDimensional> shoeToMakeJob, string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                string toWrite = FanucScannerJob(shoeToMakeJob, Path.GetFileNameWithoutExtension(filename), new List<int>() { 46 });
                byte[] info = new UTF8Encoding(true).GetBytes(toWrite);
                fs.Write(info, 0, info.Length);
            }
        }

        public static Shoe<PointMultiDimensional> ParseOldJob(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return null;
            }

            // Open the file to read from.
            string readText = File.ReadAllText(filepath);

            string[] splitted = readText.Split('\n');

            int i = 0;
            foreach(string splittedString in splitted)
            {
                i++;
                if (splittedString.Contains("/POS"))
                {

                    break;
                }
            }

            string[] newSplitted = new string[splitted.Length - i];

            Array.Copy(splitted, i, newSplitted, 0, splitted.Length - i);

            foreach(string newSplittedString in newSplitted)
            {
                if(newSplittedString.Contains('P'))
                {

                }
            }

            return null;

        }

        public static string FanucScannerJob(Shoe<PointMultiDimensional> shoe, string jobName, List<int> PR_TO_SET)
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