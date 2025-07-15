using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum NoteStatus
{
    NOTE_EMPTY,
    NOTE_OPEN,
    NOTE_CLOSED,
    NOTE_STANDBY
}

namespace ProductionLaunch
{
    public class Numbered 
    {
        //size
        private float nSize = 70;
        public float NSize
        {
            get
            {
                return this.nSize;
            }
            set
            {
                this.nSize = value;
            }
        }

        //number of pairs
        private int nPairs = 0;
        public int NPairs
        {
            get
            {
                return this.nPairs;
            }
            set
            {
                this.nPairs = value;
            }
        }

        //default constructor
        public Numbered()
        {
            nSize = 70;
            nPairs = 0;
        }
    }
    public class NoteManager
    {
        //notemanager id
        private int idNote;
        public int IDNote
        {
            get
            {
                return this.idNote;
            }
            set
            {
                this.idNote = value;
            }
        }

        //note code
        private string noteCode;
        public string NoteCode
        {
            get
            {
                return this.noteCode;
            }
            set
            {
                this.noteCode = value;
            }
        }

        //note param1
        private string noteParam1;
        public string NoteParam1
        {
            get
            {
                return this.noteParam1;
            }
            set
            {
                this.noteParam1 = value;
            }
        }

        //note param2
        private string noteParam2;
        public string NoteParam2
        {
            get
            {
                return this.noteParam2;
            }
            set
            {
                this.noteParam2 = value;
            }
        }

        //note param3
        private string noteParam3;
        public string NoteParam3
        {
            get
            {
                return this.noteParam3;
            }
            set
            {
                this.noteParam3 = value;
            }
        }


        //note param4
        private string noteParam4;
        public string NoteParam4
        {
            get
            {
                return this.noteParam4;
            }
            set
            {
                this.noteParam4 = value;
            }
        }

        //note param5
        private string noteParam5;
        public string NoteParam5
        {
            get
            {
                return this.noteParam5;
            }
            set
            {
                this.noteParam5 = value;
            }
        }
       
        //barcode model name
        private string noteModelName = "";
        public string NoteModelName
        {
            get
            {
                return this.noteModelName;
            }
            set
            {
                this.noteModelName = value;
            }
        }

        private string noteArticleCode = "";
        public string NoteArticleCode
        {
            get
            {
                return this.noteArticleCode;
            }
            set
            {
                this.noteArticleCode = value;
            }
        }
       
        //target numbered list
        private List<Numbered> noteTargetNumberedList = new List<Numbered>();
        public List<Numbered> NoteTargetList
        {
            get
            {
                return this.noteTargetNumberedList;
            }
            set
            {
                this.noteTargetNumberedList = value;
            }
        }

        //loaded numbered list
        private List<Numbered> noteLoadedNumberedList = new List<Numbered>();
        public List<Numbered> NoteLoadedList
        {
            get
            {
                return this.noteLoadedNumberedList;
            }
            set
            {
                this.noteLoadedNumberedList = value;
            }
        }

        //produced numbered list
        private List<Numbered> noteProducedNumberedList = new List<Numbered>();
        public List<Numbered> NoteProducedList
        {
            get
            {
                return this.noteProducedNumberedList;
            }
            set
            {
                this.noteProducedNumberedList = value;
            }
        }
        
        //default constructor
        public NoteManager()
        {
            idNote = 0;
            noteCode = "";
            noteModelName = "";
            noteArticleCode = "";
            noteParam1 = "";
            noteParam2 = "";
            noteParam3 = "";
            noteParam4 = "";
            noteParam5 = "";
            noteLoadedNumberedList = new List<Numbered>();
            noteProducedNumberedList = new List<Numbered>();
            noteTargetNumberedList = new List<Numbered>();
        }

        public bool SetNoteManagerFromDBRecord(string nCode, string mName, string aCode, string tLN, string dLN, string lLN, string param1, string param2, string param3, string param4, string param5)
        {            
            NoteCode = nCode;
            NoteModelName = mName;
            NoteArticleCode = aCode;

            NoteParam1 = param1;
            NoteParam2 = param2;
            NoteParam3 = param3;
            NoteParam4 = param4;
            NoteParam5 = param5;
            try
            { 
                //target list
                int k = 0;
                string tmpLine = tLN;

                tmpLine = tmpLine.Trim('$');
                if (tmpLine == null || tmpLine == "")
                {

                    return false;
                }

                string[] line = tmpLine.Split('-');

                string sLine = "";
                for (k = 0; k <= line.Count() - 1; k++)
                {
                    sLine = line[k];
                    sLine = sLine.Trim('\t');
                    sLine = sLine.Trim(' ');

                    Numbered nm = new Numbered();

                    string[] numbered = sLine.Split('*');

                    nm.NSize = Convert.ToSingle(numbered[0]);
                    nm.NPairs = Convert.ToInt32(numbered[1]);

                    NoteTargetList.Add(nm);
                }

                //produced list
                k = 0;
                tmpLine = dLN;

                tmpLine = tmpLine.Trim('$');
                if (tmpLine == null || tmpLine == "")
                {                    
                    return false;
                }

                line = tmpLine.Split('-');

                sLine = "";
                for (k = 0; k <= line.Count() - 1; k++)
                {
                    sLine = line[k];
                    sLine = sLine.Trim('\t');
                    sLine = sLine.Trim(' ');

                    Numbered nm = new Numbered();

                    string[] numbered = sLine.Split('*');

                    nm.NSize = Convert.ToSingle(numbered[0]);
                    nm.NPairs = Convert.ToInt32(numbered[1]);

                    NoteProducedList.Add(nm);
                }

                k = 0;
                tmpLine = lLN;

                tmpLine = tmpLine.Trim('$');
                if (tmpLine == null || tmpLine == "")
                {                    
                    return false;
                }

                line = tmpLine.Split('-');

                sLine = "";
                //loaded list = produced + target
                for (k = 0; k <= line.Count() - 1; k++)
                {
                    sLine = line[k];
                    sLine = sLine.Trim('\t');
                    sLine = sLine.Trim(' ');

                    Numbered nm = new Numbered();

                    string[] numbered = sLine.Split('*');

                    nm.NSize = Convert.ToSingle(numbered[0]);
                    nm.NPairs = Convert.ToInt32(numbered[1]);

                    NoteLoadedList.Add(nm);                    
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public NoteStatus GetStatusEnum(string status)
        {
            if (status == "aperto")
            {
                return NoteStatus.NOTE_OPEN;
            }
            else if (status == "chiuso")
            {
                return NoteStatus.NOTE_CLOSED;
            }
            else
            {
                return NoteStatus.NOTE_EMPTY;
            }
            
        }
    }
}
