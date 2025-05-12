using UnityEngine;

namespace O2un.Network
{
    public enum PayloadType
    {
        Nak = 0, // 네트워크 실패 메세지
    }

    public class SymbolNo
    {
        public enum Symbol
        {
            UnknownError = 0,
            Success = 1,
        }

        public static readonly SymbolNo UnknownError = new(Symbol.UnknownError);
        public static readonly SymbolNo Success = new(Symbol.Success);

        private Symbol _symbol;
        public SymbolNo(Symbol v) { _symbol = v; }
    }

    public interface IPayload
    {
        public abstract PayloadType Type {get;}
    }

    public interface ICustomNetworkMessageDispatcher
    {
        public abstract SymbolNo ReceiveMessage(ulong clientId, byte[] payayload);
    }
    
    public abstract class ReqDispatcher<T> : ICustomNetworkMessageDispatcher where T : IPayload
    {
        public SymbolNo ReceiveMessage(ulong clientId, byte[] data)
        {
            if(false == ServerManager.Instance.IsServer)
            {
                return SymbolNo.UnknownError;
            }

            return ReceiveMessage(clientId, (T)CustomNetwokrMessageManagerHelper.ToObject(data));
        }
        protected abstract SymbolNo ReceiveMessage(ulong clientId, T payload);
    }

    public abstract class AckDispatcher<T> : ICustomNetworkMessageDispatcher where T : IPayload
    {
        public SymbolNo ReceiveMessage(ulong clientId, byte[] data)
        {
            if(false == ServerManager.Instance.IsClient)
            {
                return SymbolNo.UnknownError;
            }

            return ReceiveMessage(clientId, (T)CustomNetwokrMessageManagerHelper.ToObject(data));
        }
        protected abstract SymbolNo ReceiveMessage(ulong clientId, T payload);
    }

    [System.Serializable]
    public struct Nak : IPayload
    {

        public PayloadType Type => PayloadType.Nak;
        public SymbolNo _symbol;
    }

    public class NakDispatcher : ReqDispatcher<Nak>
    {
        protected override SymbolNo ReceiveMessage(ulong clientId, Nak payload)
        {

            return SymbolNo.Success;
        }
    }
}
