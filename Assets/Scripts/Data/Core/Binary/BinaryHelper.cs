using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameCommonTypes;
using Newtonsoft.Json;
using UnityEngine;

namespace O2un.Data.Binary
{
    public static class BinaryHelper
    {
        private static string password = "o2!2510UnEnt@";
        private static readonly string KEY = password.Substring(0, 128 / 8);

        public static BinaryWriter SaveToBinary(string path, bool isEncrypt = true)
        {
            var directory = Path.GetDirectoryName(path);
            if (false == Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (isEncrypt)
            {
                RijndaelManaged manager = new();
                manager.Mode = CipherMode.CBC;
                manager.Padding = PaddingMode.PKCS7;
                manager.KeySize = 128;

                var encryptor = manager.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

                FileStream fs = new(path, FileMode.Create);
                CryptoStream cs = new(fs, encryptor, CryptoStreamMode.Write);
                return new BinaryWriter(cs);
            }
            else
            {
                FileStream fs = new(path, FileMode.Create);
                return new BinaryWriter(fs);
            }
        }

        public static BinaryReader LoadFromBinary(string path, bool isEncrypt = true)
        {
            FileStream fs = new(path, FileMode.Open);
            if (null == fs)
            {
                Debug.LogError($"{path} 파일이 존재하지 않습니다.");

                return null;
            }

            if (isEncrypt)
            {
                RijndaelManaged manager = new();
                manager.Mode = CipherMode.CBC;
                manager.Padding = PaddingMode.PKCS7;
                manager.KeySize = 128;
                var decryptor = manager.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

                CryptoStream cs = new(fs, decryptor, CryptoStreamMode.Read);
                return new BinaryReader(cs);
            }
            else
            {
                return new BinaryReader(fs);
            }
        }

        public static BinaryReader LoadFromMemory(byte[] encrypt, bool isEncrypt = true)
        {
            var ms = new MemoryStream(encrypt);

            if (isEncrypt)
            {
                RijndaelManaged manager = new();
                manager.Mode = CipherMode.CBC;
                manager.Padding = PaddingMode.PKCS7;
                manager.KeySize = 128;
                var decryptor = manager.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

                CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
                return new BinaryReader(cs);
            }
            else
            {
                return new BinaryReader(ms);
            }
        }

        public static T Read<T>(this BinaryReader br)
        {
            var len = br.ReadInt32();
            var bytes = br.ReadBytes(len);

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static object Read(this BinaryReader br, Type type)
        {
            var len = br.ReadInt32();
            var bytes = br.ReadBytes(len);

            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type);
        }

        public static void Write<T>(this BinaryWriter bw, T obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
        
        public static void Write(this BinaryWriter bw, UniqueKey64 obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
    } 
}
