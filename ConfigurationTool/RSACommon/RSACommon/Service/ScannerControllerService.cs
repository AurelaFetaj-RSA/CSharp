using log4net;
using Org.BouncyCastle.Bcpg;
using RSACommon.Configuration;
using RSAInterface.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RSAInterface;
using RSAInterface.Helper;

namespace RSACommon.Service
{
    public class ScannerAppControllerConfigurationService : IServiceConfigurationRemote
    {
        public int Port { get; set; } = 10001;
        public string Host { get; set; } = "localhost";
        public string ServiceName { get; set; } = "Scanner App Controller";
        public string Scheme { get; set; } = "tcp";
        public bool Active { get; set; } = true;
    }

    public class ScannerControllerService : IService
    {
        public string Name { get; set; } = "Scanner Controller";
        public ILog Log { get; private set; }
        public Uri ServiceURI { get; set; } = new UriBuilder().Uri;
        public bool IsActive { get; private set; }
        TcpClient _socket { get; set; } = new TcpClient();
        public ScannerAppControllerConfigurationService  ScannerConfig { get; private set; }
        public IServiceConfiguration Configuration { get; private set; }
        public virtual IService SetLogger(LoggerConfigurator loggerConfig)
        {
            Log = loggerConfig?.GetLogger(this);

            Log?.Info($"Create the {Name} at {ServiceURI.AbsoluteUri}");

            return this;
        }

        public ScannerControllerService(IServiceConfiguration configuration)
        {
            Configuration = configuration;

            if (configuration is ScannerAppControllerConfigurationService s)
            {
                ScannerConfig = s;
                ServiceURI = Helper.BuildUri(ScannerConfig);
            }
        }

        public async void SendInfo(string message)
        {
            if (!_socket.Connected)
            {
                Connect();
            }

            try
            {
                if(!_socket.Connected)
                {
                    Stop();
                    throw new Exception($"Not connected");
                }

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                // Get a client stream for reading and writing.
                NetworkStream stream = _socket.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch(Exception ex)
            {
                throw new Exception($"Connection error + {ex.Message}");
            } 


        }

        public async Task<IService> Start()
        {
            Connect();

            return this;

        }

        public async void Connect()
        {
            try
            {
                await _socket.ConnectAsync(ServiceURI.Host, ServiceURI.Port);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error on connecting {ex.Message}");
            }

        }


        public void Stop()
        {
            _socket.Close();
        }
    }
}
