using RSAPoints.Points;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.ProgramParser
{

    public interface IObjProgram
    {
        string ModelName { get; set; }
        string Size { get; set; }
        string Foot { get; set; }
        string Index { get; set; }
        IObjWithPoint<IPoint> Container { get; set; }
        string FilePath { get; set; }
        string ProgramName { get; set; }
    }

    public class ObjProgram : IObjProgram
    {
        public virtual string ModelName { get; set; }
        public virtual string Size { get; set; }
        public virtual string Foot { get; set; }
        public virtual string Index { get; set; }
        public virtual IObjWithPoint<IPoint> Container { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string ProgramName { get; set; }
    }

    public class StandardObjProgram : ObjProgram
    {
        /// <summary>
        /// AAAA
        /// </summary>
        public override string ModelName { get; set; }
        /// <summary>
        /// ZZZ
        /// </summary>
        public override string Size { get; set; }
        /// <summary>
        /// TT
        /// </summary>
        public override string Foot { get; set;}
        /// <summary>
        /// XX
        /// </summary>
        public override string Index { get; set; }
        public override IObjWithPoint<IPoint> Container { get; set; }
        /// <summary>
        /// PRAAAA-ZZZ-TTXX
        /// </summary>
        public override string ProgramName { get; set; }
    }


    public interface IParser<out T> where T : IObjProgram
    {
        string Description { get; }
        T Parse(string programName);
    }

    public class StandardProgramParser: ProgramParser<StandardObjProgram>
    {
        public override string Description { get; protected set; } = "Standard Parser, PRAAAA-ZZZ-XX00";

        public override StandardObjProgram Parse(string programName)
        {
            StandardObjProgram toRet = new StandardObjProgram();

            FileInfo f = new FileInfo(programName);

            string[] splittedString = f.Name.Split(new char[] { '-', '.' });
            string programNameToRemove = f.Name;

            if (splittedString.Length > 2)
            {
                toRet.Size = splittedString[1];
                //if (int.TryParse(splittedString[1], out int size))
                //{
                //    toRet.Size = size;
                //}

                toRet.ModelName = splittedString[0].Substring(2);

                toRet.Foot = splittedString[2].Substring(0, 2);

                toRet.Index = splittedString[2].Substring(2, 2);
                //if (int.TryParse(splittedString[2].Substring(2, 2), out int index))
                //{
                //    toRet.Index = index;
                //}

                toRet.FilePath = programName;

                toRet.ProgramName = programNameToRemove.Replace(f.Extension, "");
            }

            return toRet;
        }
    }


    public abstract class ProgramParser<T>: IParser<T> where T : IObjProgram
    {
        public virtual string Description { get; protected set; } = "";
        public virtual T Parse(string programName)
        {
            //T toRet = (T)Activator.CreateInstance(typeof(T));

            //FileInfo f = new FileInfo(programName);

            //string[] splittedString = f.Name.Split(new char[] { '-','.' });

            //if(splittedString.Length > 2 )
            //{
            //    if (int.TryParse(splittedString[1], out int size))
            //    {
            //        toRet.Size = size;
            //    }

            //    toRet.ModelName = splittedString[0].Substring(2);

            //    toRet.Orientation = splittedString[2].Substring(0, 2);

            //    if (int.TryParse(splittedString[2].Substring(2,2), out int index))
            //    {
            //        toRet.Index = index;
            //    }

            //    toRet.FilePath = programName;
            //}

            //return toRet;

            return default(T);
        }
    }
}
