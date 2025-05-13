using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using O2un.Core.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace O2un.Resource
{
    public class AddressablesManager
    {
        private AddressablesManager() { }
    
        private static AddressablesManager _instance;
        public static AddressablesManager Instance => _instance ??= new();
    
        private Dictionary<string, Object> LoadedObjects { get; } = new();
        private static readonly Dictionary<string, AsyncOperationHandle> _loadingAssets = new();
    
        public async UniTask<T> Get<T>(string objectID) where T : Object
        {
            if (false == LoadedObjects.TryGetValue(objectID, out var result))
            {
                var loadData = await Addressables.LoadAssetAsync<Object>(objectID);
                LoadedObjects.TryAdd(objectID, loadData);
    
                return loadData as T;
            }
    
            return result as T;
        }
    
        public async UniTask<T> Get<T>(AssetReferenceT<T> reference) where T : Object
        {
            if (reference.Asset != null)
                return reference.Asset as T;
    
            var key = reference.RuntimeKey.ToString();
    
            // 진행 중인 작업이 있다면 기다림
            if (_loadingAssets.TryGetValue(key, out var existingHandle) && existingHandle.IsValid())
            {
                await existingHandle.Task;
                return existingHandle.Result as T;
            }
    
            // 새로운 로드 작업 추가
            var handle = reference.LoadAssetAsync<T>();
            _loadingAssets.AddOrSwap(key, handle);
    
            return await handle.ToUniTask(); // 작업 완료까지 기다림
        }
    }
    
    public static class AddressablesManagerHelper
        {
            public static async UniTask<T> Get<T>(this AssetReferenceT<T> reference) where T : Object
            {
                return await AddressablesManager.Instance.Get<T>(reference);
            }
    
            public static async UniTask<T> Get<T>(string address) where T : Object
            {
                return await AddressablesManager.Instance.Get<T>(address);
            }
            
            public async static UniTask<Sprite> GetSprite(string address, string name)
            {
                var loadData = await AddressablesManager.Instance.Get<SpriteAtlas>(address);
                if (null != loadData)
                {
                    return loadData.GetSprite(name);
                }
    
                return null;
            }
    
            public static void SetSprite(string address, string name, System.Action<Sprite> action)
            {
                GetSprite(address, name).ContinueWith(action).Forget();
            }
        }
}
