using System.Collections.Generic;
using GameCommonTypes;
using UnityEngine;

namespace O2un.Core
{
    public class ActorManager
    {
        static private ActorManager _instance;
        private ActorManager() { }
        public static ActorManager Instance => _instance ??= new();

        private Dictionary<ulong, Actor> ActorDictionary {get;} = new();

        public void AddActor(Actor req)
    {
        ActorDictionary.TryAdd(req.NetworkObjectId, req);
    }

    public void RemoveActor(Actor req)
    {
        ActorDictionary.Remove(req.NetworkObjectId);
    }

    public T GetActor<T>(ulong objectId) where T : Actor
    {
        if(ActorDictionary.TryGetValue(objectId, out var result))
        {
            return result as T;
        }

        return default;
    }

    public List<Actor> FindActors(Vector3 position, float radius, ActorType type)
    {
        List<Actor> list = new();
        foreach(var a in ActorDictionary.Values)
        {
            if(a.Is(type))
            {
                if(Vector3.Distance(a.Position, position) <= radius)
                {
                    list.Add(a);
                }
            }
        };
        
        return list;
    }
    }
}
