using System;

namespace Katana.Core
{
    public static class GameplayOverlayState
    {
        public static bool IsInventoryOpen { get; private set; }

        public static event Action InventoryCloseRequested;

        public static void SetInventoryOpen(bool isOpen) => IsInventoryOpen = isOpen;

        public static void RequestCloseInventory() => InventoryCloseRequested?.Invoke();
    }
}
