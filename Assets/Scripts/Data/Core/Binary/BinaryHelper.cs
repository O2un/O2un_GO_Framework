using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GameCommonTypes;
using UnityEngine;

namespace O2un.Data.Binary
{
    public static class BinaryHelper
    {
        //NOTE 비밀번호는 16자
        private static string password = "o2!2510UnEnt@sib";
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

        // NOTE UniqueKey64 나 특정 타입 Serialize시 문제 발생으로 Netonsoft.Json 대신 System.Text.Json 사용 정확한 이유 파악은 못함
        public static T Read<T>(this BinaryReader br)
        {
            var len = br.ReadInt32();
            var bytes = br.ReadBytes(len);

            return (T)JsonSerializer.Deserialize(bytes, typeof(T));
        }

        public static object Read(this BinaryReader br, Type type)
        {
            var len = br.ReadInt32();
            var bytes = br.ReadBytes(len);

            return JsonSerializer.Deserialize(bytes, type);
        }

        public static void Write<T>(this BinaryWriter bw, T obj)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
        
        public static void Write(this BinaryWriter bw, UniqueKey64 obj)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            bw.Write(bytes.Length);
            bw.Write(bytes);
        }
    } 
}
