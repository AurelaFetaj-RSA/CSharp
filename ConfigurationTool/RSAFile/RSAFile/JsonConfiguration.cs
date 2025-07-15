using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RSAFile.Json
{
    public class JsonConfiguration
    {
        public JsonConfiguration() : this(string.Empty, string.Empty)
        {

        }

        public JsonConfiguration(string file, string filePath)
        {

        }

        public string Save<T>(string fileName, T objectToSave)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Newtonsoft.Json.Formatting.Indented };
            string jsonString = JsonConvert.SerializeObject(objectToSave, settings);

            //Saving file here
            File.WriteAllText(fileName, jsonString);

            return jsonString;
        }

        public T Load<T>(string filename)
        {
            if (File.Exists(filename))
            {
                string fileInput = File.ReadAllText(filename);

                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Newtonsoft.Json.Formatting.Indented };
                try
                {
                    return (T)JsonConvert.DeserializeObject<T>(fileInput, settings);
                }
                catch
                {
                    throw new ArgumentException("Config files error", nameof(fileInput));
                }

            }

            return default(T);

        }

    }
}
