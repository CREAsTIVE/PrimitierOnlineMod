using LiteNetLib;
using MelonLoader;
using System.Net;
using System.Reflection;

namespace YuchiGames.POM.Client.Network
{
    public class Sender
    {
        private string _ip;
        private int _port;
        private IPEndPoint _localEndPoint;
        private EventBasedNetListener _listener;
        private NetManager _client;
        private Thread _pollEventsThread;

        public Sender(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _localEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _pollEventsThread = new Thread(PollEvents);
        }

        public void Start()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                throw new Exception("Version not found.");

            _client.Start();
            _pollEventsThread.Start();
            _client.Connect(_localEndPoint, version.ToString());
        }

        public void PollEvents()
        {
            Log.Debug("PollEvents occurred.");
            while (_client.IsRunning)
            {
                _client.PollEvents();
            }
        }

        public void Stop()
        {
            _client.Stop();
        }
    }
}