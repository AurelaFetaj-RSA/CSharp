using log4net;
using Newtonsoft.Json.Linq;
using RSAInterface.Logger;
using RSACommon.Points;
using RSACommon.Shoes;
using RSAInterface;
using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace RSACommon
{


    public class Scanner<T>: IService where T: IPoint
    {
        public string Name => "Scanner Service Control Application";
        public ILog Log => null;
        public Uri ServiceURI { get; private set; }
        public bool IsActive { get; private set; } = false;
        public IServiceConfiguration Configuration => throw new NotImplementedException();

        public IService SetLogger(LoggerConfigurator logger)
        {
            throw new NotImplementedException();
        }

        public Task<IService> Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void MakeJob(Shoe<T> shoeToMakeJob, string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                string toWrite = ShoeJob(shoeToMakeJob, Path.GetFileNameWithoutExtension(filename), new List<int>() { 46});
                byte[] info = new UTF8Encoding(true).GetBytes(toWrite);
                fs.Write(info, 0, info.Length);
            }
        }

        public virtual Shoe<T> OpenFile(StreamReader fileStream, string shoeName)
        {
            //string line;
            Shoe<T> shoe = new Shoe<T>(shoeName);

            /*
            line = fileStream.ReadLine();

            while (line != null)
            {
                var positionArray = line.Split(';');

                double x = Convert.ToDouble(positionArray[0]);
                double y = Convert.ToDouble(positionArray[1]);
                double z = Convert.ToDouble(positionArray[2]);

                //Dummy value for testing
                Point p = new Point(x, y, z, 180.000, 0, -135.00);
                shoe.PR[46] = new Point();

                //end of testing
                p.Speed = 660;
                shoe.AddPoint(p);

                //Read the next line
                line = fileStream.ReadLine();
            }
            */

            return shoe;
        }

        public virtual Shoe<T> OpenFile(string toLoadFile, string filename)
        {
            //string toLoadFile = fileNames[0];

            if (!File.Exists(toLoadFile))
                return null;

            using (StreamReader sr = new StreamReader(toLoadFile))
            {
                return OpenFile(sr, filename);
            }
        }

        public virtual string ShoeJob(Shoe<T> shoe, string jobName, List<int> PR_TO_SET)
        {

            return string.Empty;

            /*
            string l1 = $"/PROG {jobName}\r\n/ATTR\r\nCOMMENT = \"RSAWORK\";\r\nDEFAULT_GROUP = 1,*,*,*,*;\r\nCONTROL_CODE = 00000000 00000000;\r\n/APPL\r\n/MN\r\n";

            l1 += $"1: PR[60] = P[1];\r\n2: R[260] = 1;\r\n3: CALL INIT(1) ;\r\n";
            int index = 4;

            foreach (int registerValue in PR_TO_SET)
            {
                if(shoe.PR.ContainsKey(registerValue))
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
            */
        }


        public bool SendRecipe(string filename, Uri target)
        {
            byte[] responseArray;

            if (!File.Exists(filename))
                return false;

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Credentials = new System.Net.NetworkCredential("rsa", "robots");
                    responseArray = client.UploadFile("ftp://" + target.Host + "/" + filename, System.Net.WebRequestMethods.Ftp.UploadFile, filename); //machine 2
                }
            }
            catch
            {
                return false;
            }


            return true;
        }
    }
}
