using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace GestorePiste
{
    public class socket
    {
        private StreamSocket _socket = new StreamSocket();
        private StreamSocketListener _listener = new StreamSocketListener();
        private List<StreamSocket> _connections = new List<StreamSocket>();
        private bool _connecting = false;
        private string msg = "";
        public Dato datoricevuto
        { get { return _datoricevuto; }
            set
            {
                if (_datoricevuto != datoricevuto)
                {
                    _datoricevuto = datoricevuto;
                    OnValueChanged();
                }
            }
        }
        public Dato _datoricevuto { get; set; }
        public string ip { get; set; }

        public socket(string IP)
        {
            ip = IP;
        }
        async public void WaitForData(StreamSocket socket)
        {
            var dr = new DataReader(socket.InputStream);
            //dr.InputStreamOptions = InputStreamOptions.Partial;
            var stringHeader = await dr.LoadAsync(4);

            int strLength = dr.ReadInt32();

            uint numStrBytes = await dr.LoadAsync((uint)strLength);
            msg = dr.ReadString(numStrBytes);
            if (msg != null)
            {
                _datoricevuto = JsonConvert.DeserializeObject<Dato>(msg);
            }
            //LogMessage(string.Format("Received (from {0}): {1}", socket.Information.RemoteHostName.DisplayName, msg));
            datoricevuto = _datoricevuto;
            OnValueChanged();
            WaitForData(socket);
        }

        async public void Connect()
        {
            try
            {
                _connecting = true;
                Windows.Networking.HostName Connection = new Windows.Networking.HostName(ip);
                await _socket.ConnectAsync(Connection, "3011");
                _connecting = false;
                

               // LogMessage(string.Format("Connected to {0}", _socket.Information.RemoteHostName.DisplayName));

                WaitForData(_socket);
            }
            catch (Exception ex)
            {
                _connecting = false;
               // updateControls(_connecting);
               
            }
        }

       

        async public void Listen()
        {
            _listener.ConnectionReceived += listenerConnectionReceived;
            await _listener.BindServiceNameAsync("3011");

           
        }

        void listenerConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _connections.Add(args.Socket);

            //LogMessage(string.Format("Incoming connection from {0}", args.Socket.Information.RemoteHostName.DisplayName));

            WaitForData(args.Socket);
        }



        public void Send(int primaauto, int secondaauto)
        {
            Dato dat = new Dato(primaauto, secondaauto);
            string g = JsonConvert.SerializeObject(dat, Formatting.Indented);
            SendMessage(_socket, g);
        }

        async private void SendMessage(StreamSocket socket, string message)
        {
            var writer = new DataWriter(socket.OutputStream);
            var len = writer.MeasureString(message); // Gets the UTF-8 string length.
            writer.WriteInt32((int)len);
            writer.WriteString(message);
            var ret = await writer.StoreAsync();
            writer.DetachStream();

           
        }

        public event EventHandler ValueChanged;

        protected void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }



    }
    }

