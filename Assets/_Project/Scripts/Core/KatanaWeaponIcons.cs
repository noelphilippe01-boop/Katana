using System.Collections.Generic;
using UnityEngine;

namespace Katana.Core
{
    /// <summary>
    /// Icônes d'armes UI. Les PNG dans Resources/UI/Weapons doivent avoir un fond transparent
    /// (pas de rectangle sombre) pour s'afficher correctement dans les slots d'équipement.
    /// Post-traitement standard : Katana/Process UI Sprites ou Tools/ui-sprite-postprocess.
    /// </summary>
    public static class KatanaWeaponIcons
    {
        static readonly Dictionary<string, Sprite> Cache = new();

        public static void ClearCache() => Cache.Clear();

        public static Sprite Get(string weaponId)
        {
            if (string.IsNullOrEmpty(weaponId))
                return null;

            if (Cache.TryGetValue(weaponId, out var cached))
                return cached;

            var resourcePath = weaponId switch
            {
                "katana" => "UI/Weapons/katana_icon",
                _ => null
            };

            Sprite sprite = null;
            if (resourcePath != null)
            {
                sprite = Resources.Load<Sprite>(resourcePath);
                if (sprite == null)
                {
                    var texture = Resources.Load<Texture2D>(resourcePath);
                    if (texture != null)
                    {
                        sprite = Sprite.Create(
                            texture,
                            new Rect(0f, 0f, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f),
                            100f);
                    }
                }
            }

            Cache[weaponId] = sprite;
            return sprite;
        }
    }
}
