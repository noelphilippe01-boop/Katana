using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Core
{
    public static class EscapeInput
    {
        static InputAction pauseAction;
        static bool initialized;

        public static void EnsureInitialized()
        {
            if (initialized)
                return;

            pauseAction = new InputAction("PauseMenu");
            pauseAction.AddBinding("<Keyboard>/escape");
            pauseAction.AddBinding("<Keyboard>/cancel");
            pauseAction.Enable();
            initialized = true;
        }

        public static bool WasPressedThisFrame()
        {
            EnsureInitialized();

            if (pauseAction.WasPressedThisFrame())
                return true;

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                return true;

            return Input.GetKeyDown(KeyCode.Escape);
        }

        public static InputAction PauseAction
        {
            get
            {
                EnsureInitialized();
                return pauseAction;
            }
        }
    }
}
