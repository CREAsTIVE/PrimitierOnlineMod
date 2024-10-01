using LiteNetLib;
using System.Net;
using MessagePack;
using LiteNetLib.Utils;
using System.Net.Sockets;
using YuchiGames.POM.Shared;
using System.Text;
using YuchiGames.POM.Client.Assets;
using UnityEngine;
using Il2CppPinwheel.Jupiter;
using Il2Cpp;
using YuchiGames.POM.Shared.DataObjects;
using Il2CppMToon;
using Client;
using Il2CppInterop.Runtime;
using System.Collections.Generic;
using static Il2CppRootMotion.FinalIK.RagdollUtility;
using static Il2Cpp.SaveAndLoad;
using static MelonLoader.MelonLogger;

namespace YuchiGames.POM.Client.Managers
{
    public static class Network
    {
        public static int ID { get; private set; }
        public static int Ping { get; private set; }
        public static bool IsRunning { get; private set; }
        public static bool IsConnected { get; private set; }
        public static ServerInfoMessage ServerInfo { get; private set; }

        private static EventBasedNetListener s_listener;
        private static NetManager s_client;
        private static CancellationTokenSource s_cancelTokenSource;

        private static GameObject s_baseGroupedCube = null!;
        private static GameObject s_baseCube = null!;

        private const int s_serverId = -1;

        static int s_idCounter = 0;
        public static ObjectUID NextObjectUID() => new()
        {
            CreatorID = ID,
            LocalID = s_idCounter++,
        };

        static Network()
        {
            ID = -1;
            Ping = -1;
            IsConnected = false;
            ServerInfo = new ServerInfoMessage();

            s_listener = new EventBasedNetListener();
            s_client = new NetManager(s_listener)
            {
                AutoRecycle = true,
                ChannelsCount = 2
            };
            s_cancelTokenSource = new CancellationTokenSource();

            s_listener.PeerConnectedEvent += PeerConnectedEventHandler;
            s_listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
            s_listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            s_listener.NetworkErrorEvent += NetworkErrorEventHandler;
        }

        private static void PeerConnectedEventHandler(NetPeer peer)
        {
            Log.Debug($"Connected peer with ID{peer.RemoteId}");
            ID = peer.RemoteId;
            IServerDataMessage message = new RequestServerInfoMessage(
                Program.UserGUID);
            Send(message);
        }

        private static void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Debug($"Disconnected peer {disconnectInfo.Reason}");
            IsConnected = false;
            ID = -1;
            Ping = -1;
            s_cancelTokenSource.Cancel();
            if (GameObject.Find("/TitleSpace/TitleMenu/MainCanvas/StartButton") is not null)
                Assets.StartButton.IsInteractable = true;
            Log.Information($"Disconnected from Server");
        }

        private static void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte[] buffer = new byte[reader.AvailableBytes];
            reader.GetBytes(buffer, buffer.Length);
            NetworkReceiveEventProcess(peer, buffer, channel, deliveryMethod);
        }

        private static void NetworkErrorEventHandler(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Error($"A network error has occurred {socketError}");
        }

        public static void Connect(string ipAddress, int port)
        {
            Log.Information($"Connecting to Server...");
            s_cancelTokenSource = new CancellationTokenSource();
            s_client.Start();
            AuthData authData = new AuthData(Program.Version);
            byte[] buffer = MessagePackSerializer.Serialize(authData);
            NetDataWriter data = new NetDataWriter();
            data.Put(buffer);
            s_client.Connect(ipAddress, port, data);
            Thread tcpThread = new Thread(() => ReceiveDataRequestsThread(s_cancelTokenSource.Token));
            tcpThread.Start();
        }

        public static void Disconnect()
        {
            Log.Information($"Disconnecting from Server...");
            s_cancelTokenSource.Cancel();
            s_client.Stop();
            Log.Information($"Disconnected from Server");
        }

        public static void OnUpdate()
        {
            s_client.PollEvents();
            if (s_client.FirstPeer != null)
                Ping = s_client.FirstPeer.Ping;
            if (IsConnected)
            {
                IGameDataMessage message = new PlayerPositionMessage(new PlayerPositionData(
                    Player.s_clientPlayerTransform[0].ToShared(),
                    Player.s_clientPlayerTransform[1].ToShared(),
                    Player.s_clientPlayerTransform[2].ToShared()));
                Send(message);
            }
        }

        private static void ReceiveDataRequestsThread(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, s_client.LocalPort);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                if (!listener.Pending())
                    continue;
                TcpClient tcpClient = listener.AcceptTcpClient();
                Task.Run(() => DataRequestHandler(tcpClient), token);
            }
            listener.Stop();
        }

        private static void DataRequestHandler(TcpClient client)
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] length = new byte[4];
                stream.Read(length, 0, length.Length);
                byte[] channel = new byte[1];
                stream.Read(channel, 0, channel.Length);
                byte[] data = new byte[BitConverter.ToInt32(length)];
                byte[] buffer = new byte[1024];
                int readDataLength = 0;
                int i;
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    Array.Copy(buffer, 0, data, readDataLength, i);
                    readDataLength += i;
                }

                if (readDataLength != data.Length)
                    throw new Exception("Data length mismatch.");

                NetworkReceiveEventProcess(s_client.FirstPeer, data, channel[0], DeliveryMethod.ReliableOrdered);
            }
        }

        private static void NetworkReceiveEventProcess(NetPeer peer, byte[] buffer, byte channel, DeliveryMethod deliveryMethod)
        {
            switch (channel)
            {
                case 0x00:
                    ProcessGameDataMessage(MessagePackSerializer.Deserialize<IGameDataMessage>(buffer));
                    break;
                case 0x01:
                    ProcessServerDataMessage(MessagePackSerializer.Deserialize<IServerDataMessage>(buffer));
                    break;
            }
        }

        public static Dictionary<ObjectUID, GroupSyncerComponent> SyncedObjects { get; set; } = new();

        // Override cube base cube init or something like that
        static void SyncAllGroups()
        {
            foreach (var cube in UnityEngine.Object.FindObjectsOfTypeAll(Il2CppType.Of<RigidbodyManager>()).Select(cb => cb.Cast<RigidbodyManager>().gameObject))
                if (cube.GetComponent<GroupSyncerComponent>() == null)
                    cube.AddComponent<GroupSyncerComponent>().Apply(g => SyncedObjects.Add(g.UID, g));
        }

        public static void ClaimHost(ObjectUID uid)
        {
            Log.Information($"Trying to claim a host of {SyncedObjects[uid].UID} from: {SyncedObjects[uid].HostID} to: {ID}");

            SyncedObjects[uid].HostID = ID;
            
            Send(new GroupSetHostMessage()
            {
                GroupID = uid,
                NewHostID = Network.ID
            });
        }

        private static void ProcessServerDataMessage(IServerDataMessage serverDataMessage)
        {
            switch (serverDataMessage)
            {
                case ServerInfoMessage message:
                    ID = message.UID;

                    var dayNightCycleButton = UnityUtils.FindGameObjectOfType<DayNightCycleButton>();
                    dayNightCycleButton.SwitchState(message.IsDayNightCycle);
                    ServerInfo = message;
                    World.LoadWorldData(message.WorldData);
                    IsConnected = true;
                    Log.Information($"Connected to Server with ID: {ID}");
                    Assets.StartButton.JoinGame();
                    foreach (NetPeer i in s_client.ConnectedPeerList)
                    {
                        if (i.Id == ID)
                            continue;
                        Player.SpawnPlayer(i.Id);
                    }
                    break;

                // If server request to generate a new chunk
                case RequestNewChunkDataMessage message:
                    CubeGenerator.GenerateNewChunk(message.ChunkPos.ToUnity());
                    Send(new RequestedChunkDataMessage() { Pos = message.ChunkPos });
                    
                    // SyncAllGroups();

                    // TODO: Remove that???
                    /*Send(new SavedChunkDataMessage() // Server will save generated chunk data
                    {
                        Pos = message.ChunkPos,
                        Chunk = DataConverter.ToChunk(SaveAndLoad.chunkDict[message.ChunkPos.ToUnity()])
                    });*/
                    break;

                // If server tells that this chunk already loaded by somebody
                case ChunkUnloadMessage message:
                    CubeGenerator.generatedChunks.Add(message.Pos.ToUnity());
                    break;

                // If server send saved chunk
                case SavedChunkDataMessage message:
                    SaveAndLoad.chunkDict[message.Pos.ToUnity()] = DataConverter.ToIl2CppChunk(message.Chunk);
                    CubeGenerator.GenerateSavedChunk(message.Pos.ToUnity());
                    // SyncAllGroups();
                    break;

                
            }
        }
        private static void ProcessGameDataMessage(IGameDataMessage gameDataMessage)
        {
            switch (gameDataMessage)
            {
                case JoinMessage message:
                    Log.Information($"Joined player with ID{message.JoinID}");
                    Player.SpawnPlayer(message.JoinID);
                    break;
                case LeaveMessage message:
                    Log.Information($"Player left with ID{message.LeaveID}");
                    Player.DespawnPlayer(message.LeaveID);
                    break;
                case PlayerPositionUpdateMessage message:
                    Player.s_activePlayers[message.PlayerID].SetPositionData(message.PlayerPos);
                    break;

                case GroupUpdateMessage message:
                    if (SyncedObjects.TryGetValue(message.GroupUID, out var groupSyncerComponent))
                    {
                        if (groupSyncerComponent.RBM == null)
                        {
                            Log.Warning("Rigidbody manager is null! Update skipped...");
                            break;
                        }
                        UpdateGroupValues(groupSyncerComponent.RBM, message.GroupData);
                        break;
                    }

                    // IF GROUP DIDN'T EXISTS:

                    //// Attempt 2:

                    /*CubeGenerator.GenerateGroup(new GroupData
                    {
                        pos = message.GroupData.Position.ToUnity(),
                        rot = message.GroupData.Rotation.ToUnity(),
                        cubes = message.GroupData.Cubes.Select(DataConverter.ToIl2CppCube).ToList().ToIl2cpp()
                    }, message.GroupData.Position.ToUnity(), message.GroupData.Rotation.ToUnity(), false, true);

                    // FIXME: THERE CHECK THAT CODE PLEASE
                    var rbm = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<RigidbodyManager>()).First(c => c.Cast<RigidbodyManager>().GetComponent<GroupSyncerComponent>() == null).Cast<RigidbodyManager>();
                    s_syncedObjects[message.GroupUID] = rbm.gameObject.AddComponent<GroupSyncerComponent>().Apply(c =>
                    {
                        c.HostID = -1;
                        c.UID = message.GroupUID;
                    });*/

                    //// Attempt 1:

                    /*var rbm = GameObject.Instantiate(s_baseGroupedCube).GetComponent<RigidbodyManager>();
                    for (var i = 0; i < message.GroupData.Cubes.Count; i++)
                        GameObject.Instantiate(s_baseCube).Apply(cube => cube.transform.SetParent(rbm.transform));*//*

                    groupSyncerComponent = rbm.gameObject.AddComponent<GroupSyncerComponent>();
                    s_syncedObjects[message.GroupUID] = groupSyncerComponent;*/

                    //// Attempt 3:

                    var newGroup = GameObject.Instantiate(s_baseGroupedCube);

                    groupSyncerComponent = newGroup.GetComponent<GroupSyncerComponent>() ?? newGroup.AddComponent<GroupSyncerComponent>();

                    groupSyncerComponent.UID = message.GroupUID;
                    groupSyncerComponent.HostID = -1; // From message?

                    foreach (var cube in message.GroupData.Cubes)
                    {
                        var go = CubeGenerator.GenerateCube(
                            cube.Position.ToUnity(),
                            cube.Scale.ToUnity(),
                            (Il2Cpp.Substance)cube.Substance,
                            (CubeAppearance.SectionState)cube.SectionState, 
                            new()
                            {
                                back = cube.UVOffset.Back.ToUnity(),
                                bottom = cube.UVOffset.Bottom.ToUnity(),
                                front = cube.UVOffset.Front.ToUnity(),
                                left = cube.UVOffset.Left.ToUnity(),
                                right = cube.UVOffset.Right.ToUnity(),
                                top = cube.UVOffset.Top.ToUnity()
                            }
                        );

                        go.transform.SetParent(groupSyncerComponent.transform, false);
                    }

                    SyncedObjects[message.GroupUID] = groupSyncerComponent;

                    break;

                case GroupDestroyedMessage message:
                    if (!SyncedObjects.ContainsKey(message.GroupID)) break;
                    GameObject.Destroy(SyncedObjects[message.GroupID].gameObject);
                    SyncedObjects.Remove(message.GroupID);
                    break;

                case GroupSetHostMessage message:
                    Log.Information($"Host claimed of {message.GroupID} from {SyncedObjects[message.GroupID].HostID} to {message.NewHostID}");
                    SyncedObjects[message.GroupID].HostID = message.NewHostID;
                    break;
            }
        }

        public static Group GetGroupValues(RigidbodyManager source) => new()
        {
            Position = source.transform.localPosition.ToShared(),
            Rotation = source.transform.localRotation.ToShared(),
            Velocity = source.rb.velocity.ToShared(),
            AngularVelocity = source.rb.angularVelocity.ToShared(),
            IsFixedToGroup = source.IsFixedToGround,

            Cubes = source.transform.Childrens().Select(child =>
            {
                var cubeBase = child.GetComponent<CubeBase>();
                var cubeConnector = child.GetComponent<CubeConnector>();
                var heat = child.GetComponent<Heat>();
                var fluid = child.GetComponent<FluidDynamics>();
                var cubeAppearance = child.GetComponent<CubeAppearance>();

                var cube = new Cube
                {
                    Position = child.transform.localPosition.ToShared(),
                    Rotation = child.transform.localRotation.ToShared(),
                    Scale = child.transform.localScale.ToShared(),

                    Life = cubeBase.life,
                    MaxLife = cubeBase.maxLife,

                    Anchor = (Anchor)cubeConnector.anchor,
                    Substance = (Shared.DataObjects.Substance)cubeBase.substance,
                    Name = (Shared.DataObjects.CubeName)cubeBase.cubeName,
                    Connections = cubeConnector.Connections.ToSystem()
                        .Select(connection => source.transform.Childrens()
                        .IndexOf(connection.gameObject))
                        .ToList(),

                    Temperature = heat.Temperature,
                    IsBurning = heat.isBurning,
                    BurnedRatio = heat.burnedRatio,
                    SectionState = (SectionState)cubeAppearance.sectionState,
                    UVOffset = new()
                    {
                        Back = cubeAppearance.uvOffset.back.ToShared(),
                        Bottom = cubeAppearance.uvOffset.bottom.ToShared(),
                        Front = cubeAppearance.uvOffset.front.ToShared(),
                        Left = cubeAppearance.uvOffset.left.ToShared(),
                        Right = cubeAppearance.uvOffset.right.ToShared(),
                        Top = cubeAppearance.uvOffset.top.ToShared()
                    }
                };
                return cube;
            }).ToList()
        };

        public static void UpdateGroupValues(RigidbodyManager group, Group source)
        {
            group.transform.localPosition = source.Position.ToUnity();
            group.transform.localRotation = source.Rotation.ToUnity();

            group.rb.velocity = source.Velocity.ToUnity();
            group.rb.angularVelocity = source.AngularVelocity.ToUnity();

            group.IsFixedToGround = source.IsFixedToGroup;
            group.rb.isKinematic = group.IsFixedToGround;

            for (int childIndex = 0; childIndex < group.transform.GetChildCount(); childIndex++)
            {
                try
                {
                    var child = group.transform.GetChild(childIndex);
                    var sourceCube = source.Cubes[childIndex];

                    var cubeBase = child.GetComponent<CubeBase>();
                    var cubeConnector = child.GetComponent<CubeConnector>();
                    var heat = child.GetComponent<Heat>();
                    var fluid = child.GetComponent<FluidDynamics>();
                    var cubeAppearance = child.GetComponent<CubeAppearance>();

                    child.transform.localPosition = sourceCube.Position.ToUnity();
                    child.transform.localRotation = sourceCube.Rotation.ToUnity();
                    child.transform.localScale = sourceCube.Scale.ToUnity();

                    cubeBase.life = sourceCube.Life;
                    cubeBase.maxLife = sourceCube.Life;

                    cubeConnector.anchor = (CubeConnector.Anchor)sourceCube.Anchor;
                    cubeBase.substance = (Il2Cpp.Substance)sourceCube.Substance;
                    cubeBase.cubeName = (Il2Cpp.CubeName)sourceCube.Name;

                    cubeBase.cubeConnector.Connections = new HashSet<CubeConnector>(
                        sourceCube.Connections
                        .Select(connectionId => group.transform.GetChild(connectionId).GetComponent<CubeConnector>())
                        .ToList()
                    ).ToIl2Cpp();

                    heat.Temperature = sourceCube.Temperature;
                    heat.isBurning = sourceCube.IsBurning;
                    heat.burnedRatio = sourceCube.BurnedRatio;

                    cubeAppearance.sectionState = (CubeAppearance.SectionState)sourceCube.SectionState;
                    cubeAppearance.uvOffset = new()
                    {
                        back = sourceCube.UVOffset.Back.ToUnity(),
                        bottom = sourceCube.UVOffset.Bottom.ToUnity(),
                        front = sourceCube.UVOffset.Front.ToUnity(),
                        left = sourceCube.UVOffset.Left.ToUnity(),
                        right = sourceCube.UVOffset.Right.ToUnity(),
                        top = sourceCube.UVOffset.Top.ToUnity()
                    };
                } catch (Exception ex)
                {
                    Log.Error($"Exception for group {group.GetComponent<GroupSyncerComponent>().UID}");
                    Log.Error(ex);
                }
            }
        }

        

        public static void Send(IDataMessage message)
        {
            byte[] data = IDataMessage.Serialize(message);

            s_client.FirstPeer.Send(data, message.Channel, message.Protocol switch
            {
                ProtocolType.Tcp => DeliveryMethod.ReliableOrdered,
                ProtocolType.Udp => DeliveryMethod.Sequenced,
                _ => throw new NotSupportedException()
            });
        }

        public static void Init()
        {
            s_baseGroupedCube = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<RigidbodyManager>())
                .Select(obj => obj.Cast<RigidbodyManager>().gameObject)
                .First(obj => obj.name == "GroupedCube");
            s_baseGroupedCube = GameObject.Instantiate(s_baseGroupedCube);
            s_baseCube = s_baseGroupedCube.transform.Childrens().First().gameObject;
            s_baseCube = GameObject.Instantiate(s_baseCube);

            GameObject.Destroy(s_baseGroupedCube.transform.Childrens().First().gameObject);
        }
    }
}