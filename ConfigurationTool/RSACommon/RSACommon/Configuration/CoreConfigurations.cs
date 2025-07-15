using System.IO;
using RSACommon;
using RSAFile;
using RSAFile.Json;
using RSAInterface;

namespace RSACommon.Configuration
{
    public class CoreConfigurationsBuilder
    {
        private JsonConfiguration jsonConfig;
        public string FileName { get; set; }
        public CoreConfigurationsBuilder(string fileName, string filepath)        
        {
            jsonConfig = new JsonConfiguration(fileName, filepath);
            FileName = fileName;
        }

        public CoreConfigurationsBuilder(string fileName)
        {

            jsonConfig = new JsonConfiguration(Path.GetFileName(fileName), Path.GetDirectoryName(fileName));
            FileName = fileName;
        }
        
        public string Save(CoreConfigurations objToSave, string file)
        {
            string toRet = jsonConfig.Save<CoreConfigurations>(file, objToSave);

            return toRet;
        }

        public string Save(CoreConfigurations objToSave)
        {
            string toRet = jsonConfig.Save<CoreConfigurations>(FileName, objToSave);

            return toRet;
        }

        public string Save(User objToSave)
        {
            string toRet = jsonConfig.Save<User>(FileName, objToSave);

            File.WriteAllText(FileName, toRet);

            return toRet;
        }

        public CoreConfigurations LoadConfiguration(string file)
        {            
            return (CoreConfigurations)jsonConfig.Load<CoreConfigurations>(file); ;
        }

    }
}
