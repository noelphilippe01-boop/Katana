using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Core
{
    public static class InventoryInput
    {
        static InputAction inventoryAction;
        static bool initialized;

        public static void EnsureInitialized()
        {
            if (initialized)
                return;

            inventoryAction = new InputAction("InventoryMenu");
            inventoryAction.AddBinding("<Keyboard>/i");
            inventoryAction.AddBinding("<Keyboard>/b");
            inventoryAction.Enable();
            initialized = true;
        }

        public static bool WasPressedThisFrame()
        {
            EnsureInitialized();

            if (inventoryAction.WasPressedThisFrame())
                return true;

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.iKey.wasPressedThisFrame)
                return true;

            return Input.GetKeyDown(KeyCode.I);
        }
    }
}
