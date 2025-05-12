using O2un.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using static O2un.Input.InputSystem_Actions;

namespace O2un.Input
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : SingletonObject<InputManager>, ISingletonObject, IPlayerActions, IUIActions
    {
        public bool IsDontDestroy => true;

        private EventSystem _eventSystem;

        public EventSystem EventSystem => _eventSystem ??= GetComponent<EventSystem>();

        public InputSystem_Actions Input { get; private set;}

        private void RegistEvent()
        {
            Input = new();

            Input.Player.Enable();
            Input.UI.Enable();

            Input.Player.SetCallbacks(this);
            Input.UI.SetCallbacks(this);
        }

        protected override void Awake()
        {
            base.Awake();
            RegistEvent();
        }

        #region PLAYERINPUTACTIONS
        public void OnAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Debug.Log("어택");
        }

        public void OnCrouch(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnNext(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnPrevious(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }
        #endregion

        #region UIINPUTACTIONS
        public void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnNavigate(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnPoint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}
