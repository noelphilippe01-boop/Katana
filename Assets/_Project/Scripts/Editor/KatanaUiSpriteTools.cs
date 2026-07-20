using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Katana.Editor
{
    /// <summary>
    /// Post-traitement des PNG UI : transparence + crop, même esthétique que katana_icon.
    /// Pour un traitement batch hors Unity : Tools/ui-sprite-postprocess (npm run process).
    /// </summary>
    public static class KatanaUiSpriteTools
    {
        const int BackgroundThreshold = 42;
        const int IconPadding = 8;
        const int FramePadding = 4;

        static readonly (string assetPath, bool iconMode, float borderRatio)[] Targets =
        {
            ("Assets/_Project/Resources/UI/Weapons/katana_icon.png", true, 0f),
            ("Assets/_Project/Resources/UI/Sprites/window_frame.png", false, 0.10f),
            ("Assets/_Project/Resources/UI/Sprites/window_frame_overlay.png", false, 0.12f),
            ("Assets/_Project/Resources/UI/Sprites/grid_frame.png", false, 0.14f),
            ("Assets/_Project/Resources/UI/Sprites/health_bar_bg.png", false, 0.12f),
        };

        [MenuItem("Katana/Process UI Sprites (transparency + crop)")]
        public static void ProcessAllUiSprites()
        {
            var processed = 0;
            foreach (var target in Targets)
            {
                if (ProcessAsset(target.assetPath, target.iconMode, target.borderRatio))
                    processed++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"Katana: {processed} sprite(s) UI post-traité(s).");
        }

        static bool ProcessAsset(string assetPath, bool iconMode, float borderRatio)
        {
            var fullPath = Path.GetFullPath(assetPath);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"Katana: asset introuvable {assetPath}");
                return false;
            }

            var bytes = File.ReadAllBytes(fullPath);
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!source.LoadImage(bytes))
            {
                Object.DestroyImmediate(source);
                Debug.LogWarning($"Katana: lecture impossible {assetPath}");
                return false;
            }

            var width = source.width;
            var height = source.height;
            var pixels = source.GetPixels32();
            Object.DestroyImmediate(source);

            if (iconMode)
                RemoveIconBackground(pixels, width, height);
            else
                FloodBackgroundFromEdges(pixels, width, height);

            if (!TryGetCropBounds(pixels, width, height, iconMode ? IconPadding : FramePadding, out var crop))
            {
                Debug.LogWarning($"Katana: aucun pixel visible après traitement {assetPath}");
                return false;
            }

            var cropped = ExtractCrop(pixels, width, crop);
            var output = new Texture2D(crop.width, crop.height, TextureFormat.RGBA32, false);
            output.SetPixels32(cropped);
            output.Apply();

            File.WriteAllBytes(fullPath, output.EncodeToPNG());
            Object.DestroyImmediate(output);

            if (borderRatio > 0f)
                UpdateSpriteBorder(assetPath, crop.width, crop.height, borderRatio);

            Debug.Log($"Katana: {assetPath} -> {crop.width}x{crop.height}");
            return true;
        }

        static bool IsBackground(Color32 pixel)
        {
            var lum = 0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b;
            return pixel.r <= BackgroundThreshold
                && pixel.g <= BackgroundThreshold
                && pixel.b <= BackgroundThreshold
                && lum <= BackgroundThreshold + 8f;
        }

        static void RemoveIconBackground(Color32[] pixels, int width, int height)
        {
            for (var i = 0; i < pixels.Length; i++)
            {
                if (IsBackground(pixels[i]))
                    pixels[i].a = 0;
            }
        }

        static void FloodBackgroundFromEdges(Color32[] pixels, int width, int height)
        {
            var visited = new bool[pixels.Length];
            var queue = new Queue<int>();

            void TryAdd(int x, int y)
            {
                if (x < 0 || y < 0 || x >= width || y >= height)
                    return;

                var index = y * width + x;
                if (visited[index] || !IsBackground(pixels[index]))
                    return;

                visited[index] = true;
                queue.Enqueue(index);
            }

            for (var x = 0; x < width; x++)
            {
                TryAdd(x, 0);
                TryAdd(x, height - 1);
            }

            for (var y = 0; y < height; y++)
            {
                TryAdd(0, y);
                TryAdd(width - 1, y);
            }

            while (queue.Count > 0)
            {
                var index = queue.Dequeue();
                pixels[index].a = 0;
                var x = index % width;
                var y = index / width;
                TryAdd(x + 1, y);
                TryAdd(x - 1, y);
                TryAdd(x, y + 1);
                TryAdd(x, y - 1);
            }
        }

        static bool TryGetCropBounds(
            Color32[] pixels,
            int width,
            int height,
            int pad,
            out RectInt crop)
        {
            var minX = width;
            var minY = height;
            var maxX = 0;
            var maxY = 0;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (pixels[y * width + x].a <= 0)
                        continue;

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }

            if (maxX < minX || maxY < minY)
            {
                crop = default;
                return false;
            }

            minX = Mathf.Max(0, minX - pad);
            minY = Mathf.Max(0, minY - pad);
            maxX = Mathf.Min(width - 1, maxX + pad);
            maxY = Mathf.Min(height - 1, maxY + pad);

            crop = new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
            return true;
        }

        static Color32[] ExtractCrop(Color32[] pixels, int width, RectInt crop)
        {
            var result = new Color32[crop.width * crop.height];
            for (var y = 0; y < crop.height; y++)
            {
                for (var x = 0; x < crop.width; x++)
                {
                    result[y * crop.width + x] = pixels[(crop.y + y) * width + (crop.x + x)];
                }
            }

            return result;
        }

        static void UpdateSpriteBorder(string assetPath, int width, int height, float ratio)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                return;

            var border = Mathf.RoundToInt(Mathf.Min(width, height) * ratio);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.spriteBorder = new Vector4(border, border, border, border);
            importer.SaveAndReimport();
        }
    }
}
