using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RSACommon.Points;
using RSAPoints.Points;

namespace RSACommon.Shoes
{
    public class PositionRegisterList
    {
        public PositionRegisterList() 
        {
            Items = new List<PointMultiDimensional>();
        }

        public List<PointMultiDimensional> Items { get; private set; } = null;
    }   

    public class Shoe<T> where T : IPoint
    {
        [JsonIgnore]
        public int PointsN { get => Points.Count; }
        public IDictionary<int, T> Points { get; private set; } = new Dictionary<int, T>();

        [JsonIgnore]
        public int ZonesN { get => ListOfZones.Count; }
        public List<Zone<T>> ListOfZones { get; private set; } = new List<Zone<T>>();
        public string Name {  get; private set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;        
        public IDictionary<int, T> PR { get; set; } = null;

        public Shoe(string name, string description = "") 
        {
            Name = name;
            Description = description; 
            PR = new Dictionary<int, T>();  
        }

        public void AddPoint(T p, int position = -1)
        {
            //Last
            if (position == -1)
            {
                p.Index = Points.Count;
                Points[p.Index] = p;

            }
        }

        /*
        public static bool Load(string fileName, out Shoe<T> shoe)
        {
            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration("", "");
                shoe = (Shoe<T>)jsonConfig.Load<Shoe<T>>(fileName);

                foreach(Zone<T> zone in shoe.ListOfZones)
                {
                    zone.SetParent(shoe);
                    zone.MakeZones();
                }

                return true;
            }
            catch
            {
                shoe = null;
                return false;
            }
        }


        public static bool Save(string name, string filepath, Shoe<T> shoe)
        {
            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration(name, filepath);
                shoe.FileName = Path.Combine("", name);
                jsonConfig.Save(shoe.FileName, shoe);
                return true;
            }
            catch
            {
                return false;
            }
        }

         */
    }
}
