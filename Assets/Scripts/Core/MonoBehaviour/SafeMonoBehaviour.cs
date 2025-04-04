using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace O2un.Core
{
    public abstract class SafeMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            InstatiateWithInit____NEVERCALL();
        }
    
        protected virtual void Start()
        {
            
        }
        
        protected virtual void OnDisable()
        {
            DisableAllToken();
        }
    
        // SafeInstatiate함수 외 다른데서 호출하지 마세요
        #region Instantiate With Initialzie
        bool _isInit = false;
        public void InstatiateWithInit____NEVERCALL()
        {
            if(false == _isInit)
            {
                Init();
    
                _isInit = true;
            }
        }
    
        protected abstract void Init();
        #endregion
    
        protected void DelayCall(float second, Action action)
        {
            UniTask.WaitForSeconds(second, false, PlayerLoopTiming.Update, DisableCancellationToken).ContinueWith(action).Forget();
        }
    
        #region CancellationToken
        private CancellationTokenSource _disableCancellationToken;
        public CancellationToken DisableCancellationToken
        {
            get
            {
                if (this == null)
                {
                    throw new MissingReferenceException("DisableCancellationToken token should be called atleast once before destroying the monobehaviour object");
                }
    
                if (_disableCancellationToken == null)
                {
                    _disableCancellationToken = new CancellationTokenSource();
                }
    
                return _disableCancellationToken.Token;
            }
        }
    
        private Dictionary<CancellationToken, CancellationTokenSource> _instanceToken = new();
        public CancellationToken NewInstanceToken()
        {
            if (this == null)
            {
                throw new MissingReferenceException("DisableCancellationToken token should be called atleast once before destroying the monobehaviour object");
            }
    
            var newToken = new CancellationTokenSource();
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(newToken.Token, DisableCancellationToken);
            _instanceToken.Add(combinedToken.Token, newToken);
    
            return combinedToken.Token;
        }
    
        protected void SafeTokenCancel(CancellationToken ct)
        {
            if(false == _instanceToken.TryGetValue(ct, out var cts))
            {
                return;
            }
    
            _instanceToken.Remove(ct);
            cts.Cancel();
            cts.Dispose();
        }
    
        private void DisableAllToken()
        {
            if(null != _disableCancellationToken)
            {
                _disableCancellationToken.Cancel();
                _disableCancellationToken.Dispose();
                _disableCancellationToken = null;
            }
    
            foreach(var token in _instanceToken)
            {
                token.Value.Dispose();
            }
            _instanceToken.Clear();
        }
        #endregion
    }
}
