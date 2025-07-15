using Newtonsoft.Json;
using RSACommon.Points;
using RSACommon.Shoes;
using RSAFile;
using RSAFile.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAPoints.Points
{

    public interface IObjWithPoint<out T> where T: IPoint
    {
        string Name { get; }
        string Description { get;}
        string FileName { get;}
    }

    public abstract class OWithPoints<T>: IObjWithPoint<T> where T : IPoint
    {
        [JsonIgnore]
        public int PointsN { get => Points.Count; }
        public IDictionary<int, T> Points { get; protected set; } = new Dictionary<int, T>();

        [JsonIgnore]
        public int ZonesN { get => ListOfZones.Count; }
        public List<Zone<T>> ListOfZones { get; protected set; } = new List<Zone<T>>();
        public string Name { get; protected set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public IDictionary<int, T> PR { get; set; } = null;

        public OWithPoints(string name, string description = "")
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

        public virtual bool Load(string fileName, out OWithPoints<T> objToLoad)
        {
            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration("", "");
                objToLoad = (OWithPoints<T>)jsonConfig.Load<OWithPoints<T>>(fileName);

                foreach (Zone<T> zone in objToLoad.ListOfZones)
                {
                    zone.SetParent(objToLoad);
                    zone.MakeZones();
                }

                return true;
            }
            catch
            {
                objToLoad = null;
                return false;
            }
        }

        public virtual bool Save(string name, string filepath, OWithPoints<T> objToSave)
        {
            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration(name, filepath);
                objToSave.FileName = Path.Combine("", name);
                jsonConfig.Save(objToSave.FileName, objToSave);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
