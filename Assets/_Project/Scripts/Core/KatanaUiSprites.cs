using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Katana.Core
{
    /// <summary>
    /// Sprites UI chargés depuis Resources/UI/Sprites. Fallback procédural si absent.
    /// </summary>
    public static class KatanaUiSprites
    {
        public enum SpriteId
        {
            WindowFrame,
            HudFrame,
            GridFrame,
            HealthBarBackground
        }

        static readonly Dictionary<SpriteId, Sprite> Cache = new();

        public static bool UseArtworkSprites { get; set; } = true;

        public static void ClearCache() => Cache.Clear();

        public static Sprite Get(SpriteId id)
        {
            if (!UseArtworkSprites)
                return null;

            if (Cache.TryGetValue(id, out var cached))
                return cached;

            var path = id switch
            {
                SpriteId.WindowFrame => "UI/Sprites/window_frame",
                SpriteId.HudFrame => "UI/Sprites/window_frame",
                SpriteId.GridFrame => "UI/Sprites/grid_frame",
                SpriteId.HealthBarBackground => "UI/Sprites/health_bar_bg",
                _ => null
            };

            Cache[id] = string.IsNullOrEmpty(path) ? null : LoadSprite(path);
            return Cache[id];
        }

        static Sprite LoadSprite(string resourcePath)
        {
            var sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
                return sprite;

            var texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
                return null;

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        public static float ContentInset(SpriteId id)
        {
            if (Get(id) == null)
                return 2f;

            return id switch
            {
                SpriteId.HealthBarBackground => 10f,
                SpriteId.GridFrame => 30f,
                _ => 34f
            };
        }

        public static bool TryApplySliced(Image image, SpriteId id, Color fallbackColor)
        {
            if (image == null)
                return false;

            var sprite = Get(id);
            if (sprite == null)
            {
                image.sprite = null;
                image.type = Image.Type.Simple;
                image.color = fallbackColor;
                return false;
            }

            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.preserveAspect = false;
            return true;
        }
    }
}
