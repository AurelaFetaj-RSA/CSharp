using log4net;
using Opc.UaFx.Client;
using OpcCustom.OPCLicense;
using RSACommon.Configuration;
using RSACommon.RecipeParser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSACommon.Points;
using System.Runtime.Remoting.Channels;
using RSACommon.ProgramParser;
using KRcc;
using System.Diagnostics;
using RSAPoints.Points;
using RSAPoints.ConcretePoints;
using RSAInterface.Logger;
using RSAInterface;

namespace RSACommon.Service
{
    public class ReadProgramsService : IService
    {
        public string Name { get; set; } = "Read Program Service";

        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;

        public bool IsActive { get; protected set; } = false;
        public ILog Log { get; private set; }
        public IParser<IObjProgram> ProgramParser { get; protected set; }

        public IServiceConfiguration Configuration { get; protected set; }
        public int ProgramNumbers { get => ModelDictionary.Values.Sum(l => l.Count); }
        /// <summary>
        /// Model name - List of string filename
        /// </summary>
        public Dictionary<string, List<IObjProgram>> ModelDictionary { get; protected set; } = new Dictionary<string, List<IObjProgram>>();

        public IService SetLogger(LoggerConfigurator logger)
        {
            Log = logger?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");


            return this;
        }

        public async Task<IService> Start()
        {
            await Task.Delay(200);
            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }

        public void Stop()
        {

        }

        public ReadProgramsService(IServiceConfiguration config)
        {

            if (config is ReadProgramsConfiguration clientConfig)
            {
                Configuration = clientConfig;
                Name = clientConfig.ServiceName;
            }
        }

        public ReadProgramsService SetProgramParser(IParser<IObjProgram> programParser)
        {
            ProgramParser = programParser;
            return this;
        }

        public ReadProgramsService(string name)
        {
            Name = name;
        }


        public async Task<List<string>> GetModelAsync(string[] paths, string[] fileExtensions, string filterModel = "")
        {
            if (fileExtensions == null)
                return null;

            List<string> result = new List<string>();

            await Task.Run(() =>
            {
                if (Configuration is ReadProgramsConfiguration progRed)
                {
                    Stopwatch t = new Stopwatch();

                    t.Start();

                    result = GetModel(paths, fileExtensions);

                    t.Stop();

                    RSACommon.Event.RSACustomEvents.OnServiceLoadedProgramEndEvent(new Event.RSACustomEvents.ProgramsReadEndedEventArgs(this, ProgramNumbers, t.ElapsedMilliseconds));
                    return result;
                }

                return result;

            });

            return result;
        }


        /// <summary>
        /// Given the path, this fuction provide the return of List<IObjProgram> not filled
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtensions"></param>
        /// <param name="modelNameFilter"></param>
        /// <returns></returns>
        public async Task<List<IObjProgram>> GetProgramAsync(string path, string[] fileExtensions, string modelNameFilter = "")
        {
            List<IObjProgram> listOfFilteredByModelNameObject = new List<IObjProgram>();

            await Task.Run(() =>
            {
                listOfFilteredByModelNameObject = GetProgram(path, fileExtensions, modelNameFilter);

                return listOfFilteredByModelNameObject;
            });

            return listOfFilteredByModelNameObject;
        }

        public async Task<List<IObjProgram>> GetProgramsAsync(string[] paths, string[] fileExtensions, string modelNameFilter = "")
        {
            List<IObjProgram> listOfFilteredByModelNameObject = new List<IObjProgram>();

            await Task.Run(() =>
            {
                listOfFilteredByModelNameObject = GetPrograms(paths, fileExtensions, modelNameFilter);

                return listOfFilteredByModelNameObject;
            });

            return listOfFilteredByModelNameObject;
        }


        public List<IObjProgram> GetPrograms(string[] paths, string[] fileExtensions, string modelNameFilter = "")
        {

            List<IObjProgram> listOfFilteredByModelNameObject = new List<IObjProgram>();

            foreach(string path in paths)
            {
                listOfFilteredByModelNameObject.AddRange(GetProgram(path, fileExtensions, modelNameFilter));
            }

            return listOfFilteredByModelNameObject;
        }

        public List<IObjProgram> GetProgram(string path, string[] fileExtensions, string modelName = "")
        {
            if (!Directory.Exists(path))
                return null;

            if (fileExtensions == null)
                return null;

            List<IObjProgram> listOfFilteredByModelNameObject = new List<IObjProgram>();

            string[] dizin = Directory.GetFiles(path, "*.*")
                .Where(f => fileExtensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach (string filename in dizin)
            {
                var parsed = ProgramParser.Parse(filename);

                if (parsed.ModelName != null)
                {
                    if (parsed.ModelName.Contains(modelName) || modelName == "")
                    {
                        listOfFilteredByModelNameObject.Add(parsed);
                    }
                }
            }

            return listOfFilteredByModelNameObject;
        }


        public List<string> GetModel(string[] paths, string[] fileExtensions)
        {
            if (fileExtensions == null)
                return null;

            List<string> result = new List<string>();

            foreach (string path in paths)
            {
                result.AddRange(GetModel(path, fileExtensions));
            }

            return result;
        }

        public List<string> GetModel(string path, string[] fileExtensions)
        {
            if (!Directory.Exists(path))
                return null;

            if (fileExtensions == null)
                return null;

            List<string> result = new List<string>();

            string[] dizin = Directory.GetFiles(path, "*.*").Where(f => fileExtensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach (string filename in dizin)
            {
                var parsed = ProgramParser.Parse(filename);

                if(!result.Contains(parsed.ModelName))
                    result.Add(parsed.ModelName);

            }

            return result;
        }



        public void LoadProgramInDictionary<T>(string path, string[] fileExtensions, string filterModel = "") where T : IPoint
        {
            if (!Directory.Exists(path))
                return;

            if (fileExtensions == null)
                return;

            string[] dizin = Directory.GetFiles(path, "*.*")
                .Where(f => fileExtensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach (string filename in dizin)
            {
                var parsed = ProgramParser.Parse(filename);
                parsed.Container = LoadFileProgram<T>(filename) as IObjWithPoint<IPoint>;

                if (parsed != null)
                {
                    if (ModelDictionary.ContainsKey(parsed.ModelName))
                    {
                        ModelDictionary[parsed.ModelName].Add(parsed);
                    }
                    else
                    {
                        ModelDictionary[parsed.ModelName] = new List<IObjProgram>() { parsed };
                    }
                }
            }

        }


        public void LoadProgramInDictionary<T>(List<string> paths, string[] fileExtensions) where T : IPoint
        {
            foreach (string path in paths)
            {
                LoadProgramInDictionary<T>(path, fileExtensions);
            }
        }


        public void LoadProgramInDictionary<T>(string path, string[] fileExtensions) where T: IPoint
        { 

            if (!Directory.Exists(path))
                return;

            if (fileExtensions == null)
                return;

            string[] dizin = Directory.GetFiles(path, "*.*")
                .Where(f => fileExtensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();

            foreach(string filename in dizin)
            {
                var parsed = ProgramParser.Parse(filename);
                parsed.Container = LoadFileProgram<T>(filename) as IObjWithPoint<IPoint>;

                if(parsed != null)
                {
                    if(ModelDictionary.ContainsKey(parsed.ModelName))
                    {
                        ModelDictionary[parsed.ModelName].Add(parsed);
                    }
                    else
                    {
                        ModelDictionary[parsed.ModelName] = new List<IObjProgram>() { parsed };
                    }
                }
            }
        }

        public async Task<IObjWithPoint<T>> LoadProgramByNameAsync<T>(string filename) where T : IPoint
        {
            ConcretePointsContainer<T> objToLoad = new ConcretePointsContainer<T>("dummy");

            await Task.Run(() =>
            {
                if (Configuration is ReadProgramsConfiguration progRed)
                {
                    Stopwatch t = new Stopwatch();

                    t.Start();

                    objToLoad = (ConcretePointsContainer<T>)LoadFileProgram<T>(filename);

                    t.Stop();

                    RSACommon.Event.RSACustomEvents.OnServiceLoadedProgramEndEvent(new Event.RSACustomEvents.ProgramsReadEndedEventArgs(this, ProgramNumbers, t.ElapsedMilliseconds));
                    return objToLoad;
                }

                return null;

            });

            return objToLoad;
        }

        public IObjWithPoint<T> LoadFileProgram<T>(string filename) where T: IPoint
        {
            ConcretePointsContainer<T> objToLoad = new ConcretePointsContainer<T>("dummy");

            if(objToLoad.Load(filename))
            {
                return objToLoad;
            }

            return null;
        }

        public async Task LoadProgramAsync<T>() where T : IPoint
        {
            await Task.Run(() =>
            {
                if (Configuration is ReadProgramsConfiguration progRed)
                {
                    Stopwatch t = new Stopwatch();

                    t.Start();

                    foreach (string path in progRed.ProgramsPath)
                    {
                        LoadProgramInDictionary<T>(path, progRed.Extensions);
                    }

                    t.Stop();

                    RSACommon.Event.RSACustomEvents.OnServiceLoadedProgramEndEvent(new Event.RSACustomEvents.ProgramsReadEndedEventArgs(this, ProgramNumbers, t.ElapsedMilliseconds));
                }

            });
        }
    }
}
