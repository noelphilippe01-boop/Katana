using UnityEngine;

namespace Katana.Core
{
    /// <summary>
    /// Profil de fenêtre gameplay redimensionnable (inventaire, paramètres in-game, futures stats/compétences).
    /// </summary>
    public readonly struct GameplayWindowProfile
    {
        public string WindowId { get; }
        public float DesignWidth { get; }
        public float DesignHeight { get; }
        public float DefaultWidth { get; }
        public float DefaultHeight { get; }

        public GameplayWindowProfile(
            string windowId,
            float designWidth,
            float designHeight,
            float defaultWidth,
            float defaultHeight)
        {
            WindowId = windowId;
            DesignWidth = designWidth;
            DesignHeight = designHeight;
            DefaultWidth = defaultWidth;
            DefaultHeight = defaultHeight;
        }

        public float Aspect => DesignWidth / Mathf.Max(1f, DesignHeight);

        public const float InventoryDesignWidth = 354f;
        public const float InventoryDesignHeight = 706f;

        public static GameplayWindowProfile Inventory(Canvas canvas)
        {
            var width = InventoryDesignWidth;
            var proportional = width * (InventoryDesignHeight / InventoryDesignWidth);
            var height = Mathf.Min(proportional, SplitScreenMenuLayout.GetMaxWindowHeight(canvas));
            return new GameplayWindowProfile("inventory", width, InventoryDesignHeight, width, height);
        }

        public static GameplayWindowProfile Settings(Canvas canvas)
        {
            var width = SplitScreenMenuLayout.WindowReferenceWidth;
            var proportional = width * (480f / 600f);
            var height = Mathf.Min(SplitScreenMenuLayout.ResolveWindowHeight(canvas, width), proportional);
            return new GameplayWindowProfile("settings", width, 480f, width, height);
        }

        /// <summary>Fenêtre stats (à brancher plus tard) — même pipeline que l'inventaire.</summary>
        public static GameplayWindowProfile Stats(Canvas canvas)
        {
            var width = SplitScreenMenuLayout.WindowReferenceWidth;
            var height = SplitScreenMenuLayout.ResolveWindowHeight(canvas, width);
            return new GameplayWindowProfile("stats", width, 870f, width, height);
        }

        /// <summary>Fenêtre compétences (à brancher plus tard).</summary>
        public static GameplayWindowProfile Skills(Canvas canvas)
        {
            var width = SplitScreenMenuLayout.WindowReferenceWidth;
            var height = SplitScreenMenuLayout.ResolveWindowHeight(canvas, width);
            return new GameplayWindowProfile("skills", width, 720f, width, height);
        }

        public Vector2 ResolveWindowSize(float scale)
        {
            scale = Mathf.Clamp(scale, GameSettings.MinGameplayWindowScale, GameSettings.MaxGameplayWindowScale);
            return new Vector2(DefaultWidth * scale, DefaultHeight * scale);
        }

        public Vector2 InnerSize(Vector2 windowSize)
        {
            var inset = KatanaUiSprites.ContentInset(KatanaUiSprites.SpriteId.WindowFrame);
            return new Vector2(windowSize.x - inset * 2f, windowSize.y - inset * 2f);
        }
    }

    public static class GameplayWindowLayout
    {
        public static void ApplyRightAlignedWindow(RectTransform window, Vector2 size)
        {
            KatanaUiFactory.LayoutRightMenuWindow(window);
            window.sizeDelta = size;
        }
    }
}
