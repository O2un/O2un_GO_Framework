using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager
{
    private AddressablesManager() {}

    private static AddressablesManager _instance;
    public static AddressablesManager Instance => _instance ??= new();

    private static void ClearEditor()
    {
        
    }

    private Dictionary<string, Object> LoadedObjects {get;} = new();
    private static readonly Dictionary<string, AsyncOperationHandle> _loadingAssets = new();

    public async UniTask<T> Get<T>(string objectID) where T : Object
    {
        if(false == LoadedObjects.TryGetValue(objectID, out var result))
        {
            var loadData = await Addressables.LoadAssetAsync<Object>(objectID);
            LoadedObjects.TryAdd(objectID, loadData);

            return loadData as T;
        }

        return result as T;
    }
}
