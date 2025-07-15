using log4net;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json.Linq;
using Opc.UaFx;
using Opc.UaFx.Client;
using OpcCustom;
using OpcCustom.OPCLicense;
using Org.BouncyCastle.Asn1.X509;
using RSACommon.Configuration;
using RSAInterface.Logger;
using RSAInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using RSAInterface.Helper;


namespace RSACommon.Service
{
    public class ClientResult
    {
        /// <summary>
        /// Operation opc description, external dll parse
        /// </summary>
        public string OpcResultDescription { get; set; } = string.Empty;
        /// <summary>
        /// Operation result, bool ok, false bad
        /// </summary>
        public bool OpcResult { get; set; } = false;
        /// <summary>
        /// The opc value returned from the opc read
        /// </summary>
        public object Value { get; set; } = null;

        /// <summary>
        /// Internal key for call the node id
        /// </summary>
        public string Key { get; set; } = string.Empty;
        public string NodeString { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; } = null;
        public long? Elapsed { get; set; } = null;
    }

    public class OpcClientService : IService
    {
        protected OpcClient _client;
        public OpcClient Client { get; private set; }
        //public OpcClient Client { get; private set; }
        public string Name { get; private set; } = "Op Client Default Name";
        public ILog Log { get; private set; }
        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;
        public IServiceConfiguration Configuration { get; private set; }
        public bool IsActive { get; private set; }
        protected ILicenseInfo _opcLicenseInfo { get; set; } = null;
        public IOpcClientConfigurator ObjectsData { get; private set; } = null;
        public bool ClientIsConnected
        {
            get
            {
                if (_client != null)
                    return _client.State == OpcClientState.Connected ? true : false;

                return false;
            }
        }

        public void SetObjectData(IOpcClientConfigurator opcDataClientObject)
        {
            ObjectsData = opcDataClientObject;
        }

        public OpcClientService(IServiceConfiguration config)
        {

            if(config is OpcClientConfiguration clientConfig)
            {
                Configuration = clientConfig;
                Name = clientConfig.ServiceName;
                ServiceURI = Helper.BuildUri(clientConfig);

                Client = _client = new OpcClient(ServiceURI);
                Client.DisconnectTimeout = clientConfig.DisconnectionTimeoutMilliseconds;

                Client.Security.AutoAcceptUntrustedCertificates = true;
                Client.CertificateValidationFailed += HandleCertificateValidationFailed;

                IsActive = clientConfig.Active;

            }

            if(OpcClientKey.Key != string.Empty)
            {
                Opc.UaFx.Client.Licenser.LicenseKey = OpcClientKey.Key;
                _opcLicenseInfo = Opc.UaFx.Client.Licenser.LicenseInfo;
            }
        }

        private void HandleCertificateValidationFailed(object sender, OpcCertificateValidationFailedEventArgs e)
        {
            e.Accept = true;
        }

        public OpcClientService(string name)
        {
            Name = name;
        }

        public async Task<bool> Connect(string user = "", string password = "")
        {
            return await Connect(ServiceURI, user, password);
        }


        public async Task<bool> Connect(Uri uri, string user, string password)
        {
            _client.ServerAddress = uri;

            if(user != string.Empty && password != string.Empty)
            {
                _client.Security.UserIdentity = new OpcClientIdentity(user, password);
            }

            return await Task.Run(() =>
            {
                try
                {
                    _client.Connect();
                    return true;

                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<List<string>> GetServerFolder()
        {
            List<string> folder = new List<string>();


            return await Task.Run(() =>
            {
                var node = _client.BrowseNode(OpcObjectTypes.ObjectsFolder);
                folder = Browse(node, 0);

                return folder;
            });
        }

        private List<string> Browse(OpcNodeInfo node)
        {
            return Browse(node, 0);
        }

        private List<string> Browse(OpcNodeInfo node, int level)
        {
            //// In general attributes and children are retrieved from the server on demand. This
            //// is done to reduce the amount of traffic and to improve the preformance when
            //// searching/browsing for specific attributes or children. After attributes or
            //// children of a node were browsed they are stored internally so that subsequent
            //// attribute and children requests are processed without any interaction with the
            //// OPC UA server.

            // Browse the DisplayName attribute of the node. It is also possible to browse
            // multiple attributes at once (see the method Attributes(...)).

            List<string> toReturn = new List<string>();

            var displayName = node.Attribute(OpcAttribute.DisplayName);

            //if(node.NodeId == OpcNodeId.
                
                    
            string tmp = $"{new string(' ', level * 4)}{node.NodeId.ToString(OpcNodeIdFormat.Foundation)} ({displayName.Value})";
            toReturn.Add(tmp);

            // Browse the children of the node and continue browsing in preorder.
            foreach (var childNode in node.Children())
            {
                toReturn.AddRange(Browse(childNode, level + 1));
            }

            return toReturn;
        }

        public void Disconnect()
        {
            _client.Disconnect();

        }

        public void Stop()
        {
            Log?.Info($"Server {Name} URI: {ServiceURI.AbsoluteUri} stopped");

            //_client.Stop();
            //_client.Dispose();
        }

        public async Task<IService> Start()
        {
            Log?.Info($"{ServiceURI.AbsoluteUri} started");
            
            await Connect();

            return this;
        }

        public virtual void SetCallback()
        {

        }

        public virtual IService SetLogger(LoggerConfigurator loggerConfig)
        {
            Log = loggerConfig?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }

        public async virtual Task<ClientResult> Send(string key, object valueToSet)
        {
            
            ClientResult result = new ClientResult();
            //return result;

            if(ObjectsData.ClientDataConfig.OpcClientData.TryGetValue(key, out OpcObjectData variable))
            {
                return await Task.Run(() =>
                {
                    OpcStatus status = null;
                    try
                    {
                        //var valueToSend = Convert.ChangeType(valueToSet, variable.Type);

                        status = _client.WriteNode(ObjectsData.ClientDataConfig.OpcClientData[key].OpcString, valueToSet);
                        result.OpcResultDescription = status.Description;
                        result.Key = key;
                        result.NodeString = ObjectsData.ClientDataConfig.OpcClientData[key].OpcString;

                        if (status.Code == OpcStatusCode.Good)
                        {
                            result.OpcResult = true;
                            return result;
                        }

                        return result;
                    }
                    catch(Exception ex)
                    {
                        result.OpcResultDescription = ex.Message;
                        return result;
                    }

                });
            }
            else
            {
                result.OpcResultDescription = "No key in Service Configuration";
                return result;
            }
        }

        public async virtual Task<Dictionary<string, ClientResult>> Send(List<string> keys, List<object> values)
        {
            Dictionary<string, ClientResult> result = new Dictionary<string, ClientResult>();
            List<OpcWriteNode> commands = new List<OpcWriteNode>();

            //Questa lista mi serve per andare ad associare al risultato il node ID
            List<string> UsedKeys = new List<string>();
            List<object> UsedValues = new List<object>();

            for (int i = 0; i < keys.Count; i++)
            {
                if (ObjectsData.ClientDataConfig.OpcClientData.ContainsKey(keys[i]))
                {
                    UsedKeys.Add(keys[i]);
                    UsedValues.Add(values[i]);
                    commands.Add(new OpcWriteNode(ObjectsData.ClientDataConfig.OpcClientData[keys[i]].OpcString, values[i]));
                }
            }

            return await Task.Run(() =>
            {
                var opcReturnResult = _client.WriteNodes(commands);

                try
                {
                    for (int i = 0; i < opcReturnResult.Count(); i++)
                    {
                        if (opcReturnResult[i].Code == OpcStatusCode.Good)
                        {
                            result[UsedKeys[i]] = new ClientResult()
                            {
                                OpcResultDescription = opcReturnResult[i].Description,
                                Value = UsedValues[i],
                                Key = UsedKeys[i],
                                NodeString = ObjectsData.ClientDataConfig.OpcClientData[UsedKeys[i]].OpcString,
                                OpcResult = true
                            };
                        }
                        else
                        {
                            result[UsedKeys[i]] = new ClientResult()
                            {
                                OpcResultDescription = opcReturnResult[i].Description,
                                Key = UsedKeys[i],
                                NodeString = ObjectsData.ClientDataConfig.OpcClientData[UsedKeys[i]].OpcString,
                                OpcResult = false
                            };
                        }
                    }

                    return result;
                }
                catch
                {
                    return result;
                }
            });
        }



        public async virtual Task<Dictionary<string, ClientResult>> Read(List<string> keys)
        {
            Stopwatch s = Stopwatch.StartNew();

            Dictionary<string, ClientResult> result = new Dictionary<string, ClientResult>();
            List<OpcReadNode> commands = new List<OpcReadNode>();

            //Questa lista mi serve per andare ad associare al risultato il node ID
            List<string> UsedKeys = new List<string>();

            foreach (string keyValue in keys)
            {
                if (ObjectsData.ClientDataConfig.OpcClientData.ContainsKey(keyValue))
                {
                    UsedKeys.Add(keyValue);
                    commands.Add(new OpcReadNode(ObjectsData.ClientDataConfig.OpcClientData[keyValue].OpcString));
                }
            }


            return await Task.Run(() =>
            {
                var opcReturnResult = _client.ReadNodes(commands);

                try
                {
                    var returnList = opcReturnResult.ToList();

                    for (int i = 0; i < returnList.Count(); i++)
                    {
                        result[UsedKeys[i]] = new ClientResult()
                        {
                            OpcResultDescription = returnList[i].Status.Description,
                            Key = UsedKeys[i],
                            NodeString = ObjectsData.ClientDataConfig.OpcClientData[UsedKeys[i]].OpcString,
                            OpcResult = false,
                            Timestamp = returnList[i].ServerTimestamp,
                        };

                        if (returnList[i].Status.Code == OpcStatusCode.Good)
                        {
                            result[UsedKeys[i]].OpcResult = true;
                            result[UsedKeys[i]].Value = returnList[i].Value;
                        }
                    }

                    s.Stop();

                    return result;
                }
                catch
                {
                    return result;
                }
            });


        }



        public async virtual Task<ClientResult> Read(string key)
        {
            
            ClientResult result = new ClientResult();
            Stopwatch s = Stopwatch.StartNew();

            
            if (ObjectsData.ClientDataConfig.OpcClientData.ContainsKey(key))
            {

                return await Task.Run(() =>
                {
                    try
                    {
                        
                        OpcValue opcReturnResult = _client.ReadNode(ObjectsData.ClientDataConfig.OpcClientData[key].OpcString);

                        if (opcReturnResult.Status.Code == OpcStatusCode.Good)
                        {
                            result.OpcResultDescription = opcReturnResult.Status.Description;

                            //Type valueType = opcReturnResult.Value.GetType();
                            //Type singleValueType = opcReturnResult.Value.GetType();


                            //if (valueType.IsArray)
                            //{
                            //    var length = (opcReturnResult.Value as Array).Length;
                            //    var array = Activator.CreateInstance(valueType, length);

                            //    //foreach(var val in opcReturnResult.Value)
                            //    //{

                            //    //}

                            //    result.Value = Convert.ChangeType(opcReturnResult.Value as Array, ObjectsData.ClientDataConfig.OpcClientData[key].Type);

                            //}
                            //result.Value = Convert.ChangeType(opcReturnResult.Value, ObjectsData.ClientDataConfig.OpcClientData[key].Type);
                            result.Key = key;
                            result.NodeString = ObjectsData.ClientDataConfig.OpcClientData[key].OpcString;
                            result.Value = opcReturnResult.Value;
                            result.OpcResult = true;

                            s.Stop();

                            return result;
                        }


                        return result;
                    }
                    catch (Exception ex)
                    {
                        result.OpcResultDescription = ex.Message;
                        return result;
                    }

                });
            }
            else
            {
                result.OpcResultDescription = "No key in Service Configuration";
                return result;
            }
        }
    }
}
