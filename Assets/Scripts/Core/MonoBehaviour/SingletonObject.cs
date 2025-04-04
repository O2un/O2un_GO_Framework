using UnityEngine;

namespace O2un.Core 
{
    public interface ISingletonObject
    {
        public bool IsDontDestroy {get;}
        public void Replace();
    }
    
    public abstract class SingletonObject<T> : SafeMonoBehaviour where T : SafeMonoBehaviour, ISingletonObject
    {
        static T _instance;
        static System.Object _lock = new();
    
        public static T Instance
        {
            get
            {
                lock(_lock)
                {
                    if(null == _instance)
                    {
                        _instance = (T)FindFirstObjectByType(typeof(T));
                        if(null == _instance)
                        {
                            _instance = new GameObject($"{typeof(T)}(Singleton)").AddComponent<T>();
                            
                        }
    
                        if (_instance.IsDontDestroy)
                        {
                            DontDestroyOnLoad(_instance);
                        }
                    }
                }
    
                return _instance;
            }
        }
    
        protected override void Init() {}
    
        /// <summary>
        /// 미리 세팅된 데이터가 필요하여
        /// Addressable등 시스템적으로 Prefab에서 SingletonObject가 생성될때 기존에 잘못 생성되어 있던 싱글톤을 삭제하고 재생성할때 사용함
        /// </summary>
        public void Replace()
        {
            if (null != _instance)
            {
                Destroy(_instance);
            }
            
            _instance = GetComponent<T>();
            if (_instance.IsDontDestroy)
            {
                DontDestroyOnLoad(_instance);
            }
        }
    }
}