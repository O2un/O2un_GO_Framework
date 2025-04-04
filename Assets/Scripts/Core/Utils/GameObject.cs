
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace O2un.Core.Utils
{
    public static class GameObjectUtils
    {
        private static void CheckNullArgument(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentException(message);
            }
        }

        public static T InstantiateWithInit<T>(T origin) where T : SafeMonoBehaviour
        {
            CheckNullArgument(origin, "The Object you want to instantiate is null.");
            T newObj = GameObject.Instantiate(origin, origin.transform.parent);
            newObj.InstatiateWithInit____NEVERCALL();

            return newObj;
        }

        public static AsyncInstantiateOperation<T> InstantiateWithInitAsync<T>(T origin, int count = 1, Action<T> func = null)
            where T : SafeMonoBehaviour
        {
            CheckNullArgument(origin, "The Object you want to instantiate is null.");

            AsyncInstantiateOperation<T> op = GameObject.InstantiateAsync(origin, count, origin.transform.parent);
            AsyncInit(op, func).Forget();

            return op;
        }

        private static async UniTaskVoid AsyncInit<T>(AsyncInstantiateOperation<T> op, Action<T> func = null) where T : SafeMonoBehaviour
        {
            await UniTask.WaitUntil(() => op.isDone);
            var list = op.Result;
            int len = list.Length;
            for (int ii = 0; ii < len; ii++)
            {
                list[ii].InstatiateWithInit____NEVERCALL();
                func?.Invoke(list[ii]);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go.TryGetComponent(out T component))
            {
                return component;
            }

            return go.AddComponent<T>();
        }
    }
}