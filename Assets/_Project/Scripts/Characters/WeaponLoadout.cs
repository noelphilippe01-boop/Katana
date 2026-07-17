using Katana.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Katana.Characters
{
    public class WeaponLoadout : MonoBehaviour
    {
        public static readonly string[] WeaponSlotKeys = { "&", "é", "\"" };
        static readonly Key[] WeaponSlotFallbackKeys = { Key.Digit1, Key.Digit2, Key.Digit3 };

        [SerializeField] WeaponProfile[] weapons =
        {
            WeaponProfile.Katana,
            WeaponProfile.Arc,
            WeaponProfile.BatonMage
        };

        int currentIndex;

        void Awake() => EnsureDefaultLoadout();

        void EnsureDefaultLoadout()
        {
            if (IsExpectedLoadout(weapons))
                return;

            weapons = new[]
            {
                WeaponProfile.Katana,
                WeaponProfile.Arc,
                WeaponProfile.BatonMage
            };
            currentIndex = Mathf.Clamp(currentIndex, 0, weapons.Length - 1);
        }

        static bool IsExpectedLoadout(WeaponProfile[] list)
        {
            if (list == null || list.Length != 3)
                return false;

            return list[0].Id == "katana"
                && list[1].Id == "arc"
                && list[2].Id == "baton_mage";
        }

        public int WeaponCount => weapons?.Length ?? 0;
        public int CurrentIndex => currentIndex;
        public WeaponProfile Current => weapons[currentIndex];

        public event Action<int, WeaponProfile> WeaponChanged;

        void Update()
        {
            if (Time.timeScale <= 0f)
                return;

            HandleWeaponSwitchInput();
        }

        void HandleWeaponSwitchInput()
        {
            var keyboard = Keyboard.current;
            var count = Mathf.Min(weapons.Length, WeaponSlotKeys.Length);

            for (var i = 0; i < count; i++)
            {
                if (!LayoutKeyboardInput.WasLayoutKeyPressedThisFrame(keyboard, WeaponSlotKeys[i], WeaponSlotFallbackKeys[i]))
                    continue;

                SelectWeapon(i);
                return;
            }
        }

        public void SelectWeapon(int index)
        {
            if (weapons == null || weapons.Length == 0)
                return;

            index = Mathf.Clamp(index, 0, weapons.Length - 1);
            if (index == currentIndex)
                return;

            currentIndex = index;
            WeaponChanged?.Invoke(currentIndex, Current);
            GameEventBus.RaiseWeaponChanged(Current);
        }

        public string GetSlotKeyLabel(int index)
        {
            if (index < 0 || index >= WeaponSlotKeys.Length)
                return "?";

            return WeaponSlotKeys[index];
        }

        public WeaponProfile GetWeaponAt(int index)
        {
            if (weapons == null || index < 0 || index >= weapons.Length)
                return default;

            return weapons[index];
        }
    }
}
