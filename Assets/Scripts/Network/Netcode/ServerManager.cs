using System;
using Cysharp.Threading.Tasks;
using K4os.Compression.LZ4;
using O2un.Core;
using O2un.Core.Utils;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace O2un.Network
{
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(UnityTransport))]
    public class ServerManager : SingletonObject<ServerManager>, ISingletonObject
    {
        public bool IsDontDestroy => true;

        private NetworkManager _netManager;
        public NetworkManager NetManager => _netManager ??= GetComponent<NetworkManager>();

        private UnityTransport _transport;
        public UnityTransport Transport => _transport ??= GetComponent<UnityTransport>();

        public bool IsServer => NetManager.IsServer;
        public bool IsClient => NetManager.IsClient;
        public bool IsHost => NetManager.IsHost;
        public ulong ClientID => NetManager.LocalClientId;

        // NOTE 호스트로 처리할 때 ClientID로 판단할 수 없어서 서버가 주인인 캐릭터는 ulong.Max값을 ID로한다 따라서 ulong.Max값에게 CustomMessage를 보낼 경우 서버에게 보내는것이라고 생각하고 처리한다
        public static readonly ulong ServerSpawnID = ulong.MaxValue;
        [SerializeField] bool _packetCompress = false;

        protected override void Start()
        {
            base.Start();

            NetManager.SetSingleton();

            NetManager.OnClientConnectedCallback += OnClientConnected;
            NetManager.OnClientDisconnectCallback += OnClientDisconnected;
            NetManager.OnServerStarted += OnServerStarted;
            NetManager.OnServerStopped += OnServerStopped;
            NetManager.OnClientStarted += OnClientStarted;
            NetManager.OnClientStopped += OnClientStopped;
        }

        #region NETWORK EVENTS

        public event Action<ulong> OnServerConnectedClient = delegate { };
        public event Action<ulong> OnServerDisconnectedClient = delegate { };
        public event Action<ulong> OnClientConnectedToServer = delegate { };
        public event Action OnClientDisconnectedFromServer = delegate { };

        // 클라
        private void OnClientStarted()
        {
            LogHelper.Log(LogHelper.LogLevel.Info, $"ClientStarted", LogHelper.LogFilter.Client);
            // NOTE 호스트일때 메세지처리 중복 안되게
            if (false == IsHost)
            {
                NetManager.CustomMessagingManager.OnUnnamedMessage += DispatcherCustomMessage;
            }
        }

        private void OnClientStopped(bool obj)
        {
            LogHelper.Log(LogHelper.LogLevel.Info, $"ClientStopped", LogHelper.LogFilter.Client);
        }

        // 서버
        private void OnServerStarted()
        {
            LogHelper.Log(LogHelper.LogLevel.Info, $"ServerStarted", LogHelper.LogFilter.Server);

            NetManager.CustomMessagingManager.OnUnnamedMessage += DispatcherCustomMessage;
            // 서버는 targetFrameRate를 30fps
            Application.targetFrameRate = 30;
            
        }

        private void OnServerStopped(bool obj)
        {
            LogHelper.Log(LogHelper.LogLevel.Info, $"ServerStopped", LogHelper.LogFilter.Server);
        }

        // 접속
        private void OnClientConnected(ulong clientID)
        {
            if (IsClient)
            {
                LogHelper.Log(LogHelper.LogLevel.Info, $"클라이언트 LocalClientId : {clientID}가 서버에 접속함",
                    LogHelper.LogFilter.Client);
                
                // 클라이언트는 targetFrameRate를 60fps
                Application.targetFrameRate = 60;

                OnClientConnectedToServer?.Invoke(clientID);
            }
            else
            {
                LogHelper.Log(LogHelper.LogLevel.Info, $"클라이언트 ClientId : {clientID}가 서버에 접속함",
                    LogHelper.LogFilter.Server);
                
                OnServerConnectedClient?.Invoke(clientID);
            }
        }

        private void OnClientDisconnected(ulong clientID)
        {
            if (IsClient)
            {
                LogHelper.Log(LogHelper.LogLevel.Info, $"클라이언트 LocalClientId : {clientID} 접속 끊김",
                    LogHelper.LogFilter.Client);
                OnClientDisconnectedFromServer?.Invoke();
            }
            else
            {
                LogHelper.Log(LogHelper.LogLevel.Info, $"클라이언트 ClientId : {clientID} 접속 끊김",
                    LogHelper.LogFilter.Server);
                OnServerDisconnectedClient?.Invoke(clientID);
            }

        }
        #endregion

        #region CUSTOM MESSAGE
        private void DispatcherCustomMessage(ulong clientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out bool isCompress);
            reader.ReadValueSafe(out int originalSize);
            reader.ReadValueSafe(out PayloadType type);
            reader.ReadValueSafe(out byte[] receiveData);
            var data = isCompress ? Decompress(receiveData, originalSize) : receiveData;

            if (false == CustomMessageManager.Instance.GetDispatcher(type, out var dispatcher))
            {
                Debug.LogError($"{type} 타입 패킷의 Dispatcher가 등록되어 있지 않습니다.");
            }

            dispatcher.ReceiveMessage(clientId, data);
        }

        private byte[] Decompress(byte[] compressed, int originalSize)
        {
            byte[] decompressed = new byte[originalSize]; // 적절한 크기 예측 필요
            int decompressedSize =
                LZ4Codec.Decode(compressed, 0, compressed.Length, decompressed, 0, decompressed.Length);
            // Array.Resize(ref decompressed, decompressedSize);
            return decompressed;
        }

        private void WirtePayload(FastBufferWriter writer, IPayload payload, byte[] data, int originalSize)
        {
            writer.WriteValueSafe(_packetCompress);
            writer.WriteValueSafe(originalSize);
            writer.WriteValueSafe(payload.Type);
            writer.WriteValueSafe(data);
        }

        private byte[] CompressPacket(IPayload payload, out int originalSize, out int transferSize)
        {
            var origin = CustomNetwokrMessageManagerHelper.ToBytes(payload);
            originalSize = origin.Length;
            var data = _packetCompress ? Compress(origin) : origin;

            // NOTE 패킷의 기본 구조는 압축유무/ 압축전 payload 크기 / 패킷 타입 / payload 이다
            // 일단 유니티 자체 TLS를 사용하여 암호화를 진행할것이므로 보안관련 처리는 생각하지 않는다
            transferSize = sizeof(bool) + sizeof(int) + sizeof(PayloadType) + FastBufferWriter.GetWriteSize(data);

            return data;
        }

        private byte[] Compress(byte[] origin)
        {
            int max = LZ4Codec.MaximumOutputSize(origin.Length);
            byte[] compressed = new byte[max];
            int compressedSize = LZ4Codec.Encode(origin, 0, origin.Length, compressed, 0, compressed.Length);
            Array.Resize(ref compressed, compressedSize);

            return compressed;
        }

        public void SendTo(IPayload message, ulong clientId = NetworkManager.ServerClientId)
        {
            ulong sendTo = clientId;
            if (ServerSpawnID == clientId)
            {
                sendTo = NetworkManager.ServerClientId;
            }

            var bytes = CompressPacket(message, out var originSize, out var transferSize);
            using (var writer = new FastBufferWriter(transferSize, Allocator.Temp))
            {
                WirtePayload(writer, message, bytes, originSize);
                NetManager.CustomMessagingManager.SendUnnamedMessage(clientId, writer);
            }
        }

        public void SendToAll(IPayload message)
        {
            var bytes = CompressPacket(message, out var originSize, out var transferSize);
            using (var writer = new FastBufferWriter(transferSize, Allocator.Temp))
            {
                WirtePayload(writer, message, bytes, originSize);
                NetManager.CustomMessagingManager.SendUnnamedMessageToAll(writer);
            }
        }
        #endregion

        #region SPAWN

        public async UniTask<NetworkObject> Spawn(string prefabPath, Vector3 position = new Vector3(),
            ulong clientId = ulong.MaxValue, bool destroyScene = false)
        {
            var obj = await Addressables.InstantiateAsync(prefabPath);
            if (null == obj)
            {
                Debug.LogError($"비정상적인 프리팹 경로 {prefabPath}");

                return null;
            }

            var no = obj.GetComponent<NetworkObject>();
            if (null == no)
            {
                Debug.LogError($"{prefabPath}경로의 프리팹은 네트워크오브젝트가 아닌데 스폰하려 합니다.");

                return null;
            }
            
            no.SpawnWithOwnership(clientId, destroyScene);

            // TODO ActorProxy패턴(OverridePrefab)을 사용할경우 아래로 변경해야함
            return no;
            //return NetworkManager.SpawnManager.InstantiateAndSpawn(no, clientId, destroyScene, isPlayerObject, false, position);
        }

        public async UniTask<NetworkObject> Spawn(AssetReferenceGameObject refObj, Vector3 position = new Vector3(),
            ulong clientId = ulong.MaxValue, bool destroyScene = false)
        {
            var obj = await refObj.InstantiateAsync();
            if (null == obj)
            {
                Debug.LogError($"비정상적인 프리팹 경로 {refObj}");

                return null;
            }

            var no = obj.GetComponent<NetworkObject>();
            if (null == no)
            {
                Debug.LogError($"{refObj}경로의 프리팹은 네트워크오브젝트가 아닌데 스폰하려 합니다.");

                return null;
            }
            
            no.SpawnWithOwnership(clientId, destroyScene);

            // TODO ActorProxy패턴(OverridePrefab)을 사용할경우 아래로 변경해야함
            return no;
            //return NetworkManager.SpawnManager.InstantiateAndSpawn(no, clientId, destroyScene, isPlayerObject, false, position);
        }

        public T GetSpawnedObject<T>(ulong id) where T : NetworkBehaviour
        {
            if (false == NetManager.SpawnManager.SpawnedObjects.TryGetValue(id, out var value))
            {
                return null;
            }

            return value.GetComponent<T>();
        }
        #endregion


        #region MANAGE FUNCTION
        public void DisconnectClient()
        {
            if (IsClient)
            {
                NetManager.Shutdown();
                return;
            }
        }
        
        public void StartHost()
        {
            NetManager.ConnectionApprovalCallback = (req, response) =>
            {
                response.Approved = true;
                response.CreatePlayerObject = false;
            };
            
            NetManager.StartHost();
        }

        public void StartServer()
        {
            NetManager.ConnectionApprovalCallback = (req, response) =>
            {
                response.Approved = true;
                response.CreatePlayerObject = false;
            };

            NetManager.StartServer();
        }
        
        private static readonly string SERVERADRESS = "0.0.0.0";
        public bool StartServer(ushort port)
        {
            Transport.ConnectionData.Address = SERVERADRESS;
            Transport.ConnectionData.Port = port;

            NetManager.ConnectionApprovalCallback = (req, response) => {
                response.Approved = true;
                response.CreatePlayerObject = false;
            };
            
            return NetManager.StartServer();
        }
    
        public void StartClient()
        {
            NetManager.StartClient();
        }

        public void StartClient(string ip, ushort port)
        {
            Transport.ConnectionData.Address = ip;
            Transport.ConnectionData.Port = port;

            NetManager.StartClient();
        }
        #endregion
    }
}
