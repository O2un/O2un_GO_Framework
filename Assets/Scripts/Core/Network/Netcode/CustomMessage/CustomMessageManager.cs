using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace O2un.Network
{
    public class CustomMessageManager
    {
        static private CustomMessageManager _instance;
        private CustomMessageManager() 
        {
            var count = Enum.GetValues(typeof(PayloadType)).Length;
            if(_messageDispachers.Count != count)
            {
                Debug.LogError("패킷 개수가 다릅니다");
            }
        }
        public static CustomMessageManager Instance => _instance ??= new();

        private Dictionary<PayloadType, ICustomNetworkMessageDispatcher> _messageDispachers {get;} = new()
        {
            // NOTE _DEV_CustomMessageManager 를 통해 자동 생성되는 코드입니다 수정하지 마세요
            //#STARTLIST
            {PayloadType.Nak, new NakDispatcher()},
  //#ENDLIST
            // NOTE _DEV_CustomMessageManager 를 통해 자동 생성되는 코드입니다 수정하지 마세요
        };

        public bool GetDispatcher(PayloadType type, out ICustomNetworkMessageDispatcher dispatcher)
        {
            return _messageDispachers.TryGetValue(type, out dispatcher);
        }
    }

    public static class CustomNetwokrMessageManagerHelper
    {
        #region HELPER
        public static byte[] ToBytes(this IPayload obj)
        {
            if(null == obj)
            {
                return null;
            }

            BinaryFormatter bf = new();
            MemoryStream ms = new();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        public static System.Object ToObject(byte[] bytes)
        {
            MemoryStream ms = new();
            BinaryFormatter bf = new();
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            
            return (System.Object)bf.Deserialize(ms);
        }
        #endregion
    }
}
