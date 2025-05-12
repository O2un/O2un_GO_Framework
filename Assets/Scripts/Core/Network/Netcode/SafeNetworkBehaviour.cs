using System.Collections.Generic;
using System.Threading;
using O2un.Core.Utils;
using Unity.Netcode;
using UnityEngine;

namespace O2un.Network
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class SafeNetworkBehaviour : NetworkBehaviour
    {
        protected virtual void Awake()
        {
            InstatiateWithInit____NEVERCALL();
        }

        protected virtual void Start()
        {

        }

        /// <summary>
        /// Network Object를 Despawn하기 전에
        /// 자동적으로 Unitask Playerloop를 Cancel하는 함수입니다.
        /// </summary>
        protected void DespawnNetworkObject()
        {
            DisableAllToken();
            CommonUtils.DelayFrameAction(() => NetworkObject.Despawn());
        }


        // SafeInstatiate함수 외 다른데서 호출하지 마세요
        #region Instantiate With Initialzie
        bool _isInit = false;
        public void InstatiateWithInit____NEVERCALL()
        {
            if (false == _isInit)
            {
                Init();

                _isInit = true;
            }
        }

        protected abstract void Init();
        #endregion

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

        public void SafeTokenCancel(CancellationToken ct)
        {
            if (false == _instanceToken.TryGetValue(ct, out var cts))
            {
                return;
            }

            _instanceToken.Remove(ct);
            cts.Cancel();
            cts.Dispose();
        }

        protected void DisableAllToken()
        {
            if (null != _disableCancellationToken)
            {
                _disableCancellationToken.Cancel();
                _disableCancellationToken.Dispose();
                _disableCancellationToken = null;
            }

            foreach (var token in _instanceToken)
            {
                token.Value.Cancel();
                token.Value.Dispose();
            }
            _instanceToken.Clear();
        }
        #endregion
    }
}
