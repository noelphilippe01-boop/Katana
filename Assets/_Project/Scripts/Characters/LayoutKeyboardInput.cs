using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    public static class LayoutKeyboardInput
    {
        public static bool IsLayoutKeyPressed(Keyboard keyboard, string layoutKeyName, Key usPhysicalFallback)
        {
            if (keyboard == null)
                return false;

            var keyControl = keyboard.FindKeyOnCurrentKeyboardLayout(layoutKeyName);
            if (keyControl == null)
                return keyboard[usPhysicalFallback].isPressed;

            return keyControl.isPressed;
        }

        public static bool WasLayoutKeyPressedThisFrame(Keyboard keyboard, string layoutKeyName, Key usPhysicalFallback)
        {
            if (keyboard == null)
                return Input.GetKeyDown(LegacyKeyCode(usPhysicalFallback));

            var keyControl = keyboard.FindKeyOnCurrentKeyboardLayout(layoutKeyName);
            if (keyControl == null)
                return keyboard[usPhysicalFallback].wasPressedThisFrame;

            return keyControl.wasPressedThisFrame;
        }

        static KeyCode LegacyKeyCode(Key key) => key switch
        {
            Key.Digit1 => KeyCode.Alpha1,
            Key.Digit2 => KeyCode.Alpha2,
            Key.Digit3 => KeyCode.Alpha3,
            Key.Digit4 => KeyCode.Alpha4,
            _ => KeyCode.None
        };
    }
}
