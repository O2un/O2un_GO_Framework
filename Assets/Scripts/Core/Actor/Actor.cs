using System;
using GameCommonTypes;
using O2un.Network;
using UnityEngine;

namespace O2un.Core
{
    public class Actor : SafeNetworkBehaviour
    {
        private ActorType _actorType;
        public bool Is(ActorType actorType) => _actorType.HasFlag(actorType);

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        private void TypeChecker(Type thisType)
        {
            if (false == Enum.TryParse(thisType.Name, out ActorType type))
            {
                Debug.LogError($"{name} 오브젝트는 Actor를 상속받았지만 존재하지않는 ActorType입니다 ActorType 추가 작업을 진행해주세요");
                return;
            }
            _actorType |= type;

            if (thisType == typeof(Actor))
            {
                return;
            }
            else
            {
                TypeChecker(thisType.BaseType);
            }
        }

        protected override void Init()
        {
            TypeChecker(GetType());
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            ActorManager.Instance.AddActor(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            ActorManager.Instance.RemoveActor(this);
        }

        public virtual void MoveTo(Vector3 position)
        {
            transform.position = position;
        }

        public virtual void RotateTo(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}
