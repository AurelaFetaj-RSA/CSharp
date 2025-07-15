using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAInterface.Helper
{
    public static class Helper
    {
        public static byte[] ConvertToArray(string strToConvert, int sizeOfByteReturn)
        {
            var array = Encoding.ASCII.GetBytes(strToConvert);

            Array.Resize(ref array, sizeOfByteReturn);
            return array;
        }

        public static string ConvertToString(byte[] bytesToConvert)
        {
            var ret = String.Join("", Encoding.ASCII.GetString(bytesToConvert).Split('\0','\\','\n'));
            return ret;
        }
        public static Uri BuildUri(string scheme, string host)
        {
            return new UriBuilder(scheme, host).Uri;
        }
        public static Uri BuildUri(IServiceConfigurationRemote conf)
        {
            return BuildUri(conf.Scheme, conf.Host, conf.Port);
        }

        public static Uri BuildUri(string scheme, string host, int port)
        {
            return new UriBuilder(scheme, host, port).Uri;
        }

        public static T ConvertObject<T>(object input)
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }

        public static string Save<T>(string fileName, T objectToSave, JsonSerializerSettings settings)
        {
            string jsonString = JsonConvert.SerializeObject(objectToSave, settings);

            //Saving file here
            File.WriteAllText(fileName, jsonString);

            return jsonString;
        }

        public static string Save<T>(string fileName, T objectToSave)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
            string jsonString = JsonConvert.SerializeObject(objectToSave, settings);

            //Saving file here
            File.WriteAllText(fileName, jsonString);

            return jsonString;
        }

        public static T Load<T>(string filename)
        {
            if (File.Exists(filename))
            {
                string fileInput = File.ReadAllText(filename);

                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
                return (T)JsonConvert.DeserializeObject<T>(fileInput, settings);
            }

            return default(T);
        }

        public static T Load<T>(string filename, JsonSerializerSettings settings)
        {
            if (File.Exists(filename))
            {
                string fileInput = File.ReadAllText(filename);
                return (T)JsonConvert.DeserializeObject<T>(fileInput, settings);
            }

            return default(T);

        }
    }
}
