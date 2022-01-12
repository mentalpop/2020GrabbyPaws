using UnityEngine;
using Invector.vCharacterController;

namespace PixelCrushers.InvectorSupport
{

    /// <summary>
    /// Utility class to pause and unpause player.
    /// </summary>
    public static class InvectorPlayerUtility
    {

        private static vThirdPersonInput s_input = null;
        private static vThirdPersonInput input
        {
            get
            {
                if (s_input == null) s_input = GameObject.FindObjectOfType<vThirdPersonInput>();
                return s_input;
            }
        }

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            s_input = null;
        }
#endif

        public static void PausePlayer()
        {
            if (input != null)
            {
                input.SetLockAllInput(true);
                input.SetLockCameraInput(true);
            }
            UpdateEventSystemInput();
            if (InputDeviceManager.deviceUsesCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public static void UnpausePlayer()
        {
            if (input != null)
            {
                input.SetLockAllInput(false);
                input.SetLockCameraInput(false);
            }
            if (InputDeviceManager.deviceUsesCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

#if USE_INVECTOR_INVENTORY

        public static void UpdateEventSystemInput()
        {
            var inventory = GameObject.FindObjectOfType<Invector.vItemManager.vInventory>();
            var inputModule = GameObject.FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule>();
            if (inventory != null && inputModule != null)
            {
                inputModule.horizontalAxis = inventory.horizontal.buttonName;
                inputModule.verticalAxis = inventory.vertical.buttonName;
                inputModule.submitButton = inventory.submit.buttonName;
                inputModule.cancelButton = inventory.cancel.buttonName;
            }
        }

#else

        public static void UpdateEventSystemInput() {}

#endif

    }
}
