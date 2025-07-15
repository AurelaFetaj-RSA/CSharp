﻿using RSACommon.Shoes;
using RSAFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSAPoints.Points;
using RSAFile.Json;

namespace RSAPoints.ConcretePoints
{
    public class ConcretePointsContainer<T> : Points.OWithPoints<T> where T : IPoint
    {
        public ConcretePointsContainer(string name, string description = "") : base(name, description)
        {
        }

        public bool Save(string name, string filepath, bool force = false)
        {
            string fileName = Path.Combine(filepath, name);

            if (File.Exists(fileName) && !force)
                return false; 

            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration(name, filepath);
                FileName = Path.Combine(filepath, name);
                jsonConfig.Save(FileName, this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Load(string fileNamed)
        {

            if (!File.Exists(fileNamed))
                return false;

            try
            {
                JsonConfiguration jsonConfig = new JsonConfiguration("", "");
                ConcretePointsContainer<T> objLoaded = jsonConfig.Load<ConcretePointsContainer<T>>(fileNamed);

                if (objLoaded == null || objLoaded.GetType() != GetType())
                    return false;

                FileName = objLoaded.FileName;
                Name = objLoaded.Name;
                Description = objLoaded.Description;
                ListOfZones = objLoaded.ListOfZones;    
                PR = objLoaded.PR;
                Points = objLoaded.Points;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoadAsync(string fileNamed)
        {
            await Task.Run(() =>
            {
                return Load(fileNamed);
            });

            return false;
        }
    }
}
