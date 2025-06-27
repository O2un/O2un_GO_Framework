using System;
using System.Collections.Generic;
using O2un.Core;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class PoolingManager
{
    private readonly IObjectResolver resolver;
    private Dictionary<Type, IObjectPool<IPoolingObject>> pool = new();

    public PoolingManager(IObjectResolver resolver)
    {
        this.resolver = resolver;
    }

    public T Get<T>(T origin) where T : IPoolingObject
    {
        var type = typeof(T);
        if (false == pool.TryGetValue(type, out var p))
        {
            p = new ObjectPool<IPoolingObject>(
                () => 
                {
                    var go = GameObject.Instantiate(origin);
                    resolver.Inject(go);

                    return go;
                },
                obj => ((IPoolingObject)obj).OnGet(),
                obj => ((IPoolingObject)obj).OnRelease(),
                obj => ((IPoolingObject)obj).OnDestroy(),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
            pool[type] = p;
        }

        return (T)p.Get();
    }

    public void Return<T>(T obj) where T : IPoolingObject
    {
        if(pool.TryGetValue(typeof(T), out var p))
        {
            p.Release(obj);
        }
    }
}
