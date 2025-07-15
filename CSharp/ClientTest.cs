using Opc.UaFx;
using Org.BouncyCastle.Bcpg.OpenPgp;
using RSACommon;
using RSACommon.Configuration;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class ClientTest : Form
    {
        private OpcClientService _client;
        private Uri _clientUri;
        public List<string> LogMemoryString = new List<string>();
        public ClientTest(OpcClientService clientService)
        {
            InitializeComponent();
            _client = clientService;
        }

        public static int MAX_STRING_MEMORY = 500;

        private void ClientTest_Load(object sender, EventArgs e)
        {

            if (_client.Configuration is OpcClientConfiguration confi)
            {
                hostTxtbox.Text = confi.Host;
                portTxtbox.Text = confi.Port.ToString();
            }

            schemeTxtbox.Text = _client.Configuration.Scheme;

            textBoxLogTxtbox.Multiline = true;
            textBoxLogTxtbox.ReadOnly = true;
            textBoxLogTxtbox.BackColor = System.Drawing.Color.LightYellow;
            textBoxLogTxtbox.AllowDrop = false;
            textBoxLogTxtbox.WordWrap = false;
            textBoxLogTxtbox.BorderStyle = BorderStyle.Fixed3D;
            textBoxLogTxtbox.ScrollBars = ScrollBars.Vertical;
        }



        public void ThreadSafeResetTextbox()
        {
            if (textBoxLogTxtbox.InvokeRequired)
            {
                textBoxLogTxtbox.Invoke((MethodInvoker)delegate
                {
                    textBoxLogTxtbox.Text = "";
                });
            }
        }
        /// <summary>
        /// This will write text filling the textbox, no wrap, no horizontal scrollbar
        /// </summary>
        /// <param name="text"></param>
        public void ThreadSafeWriteMessage(string text)
        {
            try
            {
                ThreadSafeResetTextbox();

                string textLimited = $"{DateTime.Now.ToString("hh:mm:ss")} | {text}";

                // Set string format.
                StringFormat newStringFormat = new StringFormat();
                newStringFormat.FormatFlags = StringFormatFlags.NoWrap;
                int charactersFitted;
                int linesFilled;

                using (Graphics g = CreateGraphics())
                {
                    Size s = new Size(textBoxLogTxtbox.Size.Width - 60, textBoxLogTxtbox.Size.Height);
                    SizeF size = g.MeasureString(textLimited, textBoxLogTxtbox.Font, s, newStringFormat, out charactersFitted, out linesFilled);
                }

                if (textLimited.Length > charactersFitted)
                    textLimited = textLimited.Substring(0, charactersFitted) + "...";

                LogMemoryString.Insert(0, textLimited);

                if (LogMemoryString.Count > MAX_STRING_MEMORY)
                {
                    LogMemoryString.RemoveRange(10, LogMemoryString.Count - MAX_STRING_MEMORY);
                }

                string textToWriteAsync = "";
                foreach (var textToWrite in LogMemoryString)
                {
                    textToWriteAsync += textToWrite + Environment.NewLine;
                }

                if (textBoxLogTxtbox.InvokeRequired)
                {
                    textBoxLogTxtbox.Invoke((MethodInvoker)delegate
                    {
                        textBoxLogTxtbox.AppendText(textToWriteAsync);
                    });
                }

            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _clientUri = RSAInterface.Helper.Helper.BuildUri(schemeTxtbox.Text, hostTxtbox.Text, Int32.Parse(portTxtbox.Text));
            AddressLabel.Text = _clientUri.AbsoluteUri;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private async void Connect()
        {
            if (_clientUri == null)
            {
                try
                {

                    _clientUri = RSAInterface.Helper.Helper.BuildUri(schemeTxtbox.Text, hostTxtbox.Text, Int32.Parse(portTxtbox.Text));
                    AddressLabel.Text = _clientUri.AbsoluteUri;
                    await Task.Run(() => ThreadSafeWriteMessage($"Automatically generated URI: {_clientUri.AbsoluteUri}"));
                }
                catch (System.UriFormatException ex)
                {
                    await Task.Run(() => ThreadSafeWriteMessage($"Error generatig URI H:{hostTxtbox.Text}, S:{schemeTxtbox.Text}, P:{Int32.Parse(portTxtbox.Text)}"));
                    return;
                }
            }

            bool connectionResult = await _client.Connect(_clientUri.AbsoluteUri);

            if (connectionResult)
            {
                await Task.Run(() => ThreadSafeWriteMessage("Connected"));

            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage("Failed to connect"));
            }
        }

        private void textBoxLogTxtbox_TextChanged(object sender, EventArgs e)
        {
            textBoxLogTxtbox.SelectionStart = 0;
            textBoxLogTxtbox.ScrollToCaret();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (!_client.ClientIsConnected)
                Connect();

            List<string> ret = await _client.GetServerFolder();

            foreach (string item in ret)
            {
                await Task.Run(() => ThreadSafeWriteMessage(item));
            }

            await Task.Run(() => ThreadSafeWriteMessage("End the folder browsing"));
        }

        private async void jobAltoBtn_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2JogDown";

            var readResult = await _client.Send(keyToSend, jogAltoCheckbox.Checked);

            if (readResult.OpcResult)
            {
                await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void ReadJogAltoBtn_Click(object sender, EventArgs e)
        {
            string keyToSend = "pcM2JogDown";

            var readResult = await _client.Read(keyToSend);

            if (readResult.OpcResult)
            {
                await Task.Run(() => ThreadSafeWriteMessage($"{keyToSend}: {readResult.Value}"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            short[] arrayToSend = new short[5];

            arrayToSend[0] = -1;
            arrayToSend[1] = short.Parse(quota1.Text);
            arrayToSend[2] = short.Parse(quota2.Text);
            arrayToSend[3] = short.Parse(quota3.Text);
            arrayToSend[4] = short.Parse(quota4.Text);

            string keyToSend = "pc_quota_longitudinale";

            var readResult = await _client.Send(keyToSend, arrayToSend);

            if (readResult.OpcResult)
            {
                await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void readQuotaLong_Click(object sender, EventArgs e)
        {
            string keyToSend = "pc_quota_longitudinale";

            var readResult = await _client.Read(keyToSend);

            if (readResult.OpcResult)
            {
                var readValue = readResult.Value as short[];

                await Task.Run(() => ThreadSafeWriteMessage($"{keyToSend}: {readValue[0]} {readValue[1]} {readValue[2]} {readValue[3]} {readValue[4]}"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void pcPercVeloBtn_Click(object sender, EventArgs e)
        {
            string keyToSend = "pc_percentuale_velocità_in_manuale";

            var readResult = await _client.Send(keyToSend, short.Parse(velocitTxtbox.Text));

            if (readResult.OpcResult)
            {
                await Task.Run(() => ThreadSafeWriteMessage("Value set"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void button3_Click_1(object sender, EventArgs e)
        {
            string keyToSend = "pc_percentuale_velocità_in_manuale";

            var readResult = await _client.Read(keyToSend);

            if (readResult.OpcResult)
            {

                await Task.Run(() => ThreadSafeWriteMessage($"{keyToSend}: {readResult.Value}"));
            }
            else
            {
                await Task.Run(() => ThreadSafeWriteMessage($"Problem with set data {keyToSend}"));
            }
        }

        private async void readMultiploBtn_Click(object sender, EventArgs e)
        {

            List<string> keys = new List<string>()
            {
                "pc_percentuale_velocità_in_manuale",
                "pc_quota_finale_asse_in_manuale",
                "pcM2Status",
                "pc_quota_longitudinale"
            };

            var readResult = await _client.Read(keys);

            foreach (var result in readResult)
            {
                if (result.Value.OpcResult)
                {
                    Type singleValueType = result.Value.Value.GetType();

                    if (singleValueType.IsArray)
                    {
                        short[] arrayShort = (short[])result.Value.Value;

                        foreach (short value in arrayShort)
                        {
                            await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {value}"));
                        }
                    }
                    else
                        await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {result.Value.Value}"));
                }
                else
                {
                    await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {result.Value.OpcResultDescription}"));
                }
            }

        }
        
        private List<object> values = new List<object>()
        {
            1,
            2,
            3,
            new short[] { 0, 6, 7, 8, 9 }
        };


        private async void writeMultiploBtn_Click(object sender, EventArgs e)
        {
            List<string> keys = new List<string>()
            {
                "pc_percentuale_velocità_in_manuale",
                "pc_quota_finale_asse_in_manuale",
                "pcM2Status",
                "pc_quota_longitudinale"
            };

            await _client.Send(keys, values);

            /*
            foreach (var result in readResult)
            {
                if (result.Value.OpcResult)
                {

                Type singleValueType = result.Value.Value.GetType();

                if (singleValueType.IsArray)
                {
                    short[] arrayShort = (short[])result.Value.Value;

                    foreach (short value in arrayShort)
                    {
                    await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {value}"));
                    }
                }
                else
                    await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {result.Value.Value}"));

                }
                else
                {
                    await Task.Run(() => ThreadSafeWriteMessage($"{result.Key}: {result.Value.OpcResultDescription}"));
                }
            }
            */
        }

        private async void cleanLogBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                ThreadSafeResetTextbox();
                LogMemoryString.Clear();
            });
        }
    }
}
