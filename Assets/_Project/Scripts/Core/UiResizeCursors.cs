using UnityEngine;

namespace Katana.Core
{
    public enum UiResizeCursorKind
    {
        Horizontal,
        Vertical,
        DiagonalNwSe,
        DiagonalNeSw
    }

    /// <summary>
    /// Curseurs de redimensionnement générés à la volée (pas d'asset requis).
    /// </summary>
    public static class UiResizeCursors
    {
        static Texture2D horizontal;
        static Texture2D vertical;
        static Texture2D diagonalNwSe;
        static Texture2D diagonalNeSw;
        static UiResizeCursorKind? activeKind;

        public static void Set(UiResizeCursorKind kind)
        {
            activeKind = kind;
            var texture = kind switch
            {
                UiResizeCursorKind.Horizontal => horizontal ??= CreateHorizontal(),
                UiResizeCursorKind.Vertical => vertical ??= CreateVertical(),
                UiResizeCursorKind.DiagonalNwSe => diagonalNwSe ??= CreateDiagonalNwSe(),
                UiResizeCursorKind.DiagonalNeSw => diagonalNeSw ??= CreateDiagonalNeSw(),
                _ => null
            };

            if (texture != null)
                Cursor.SetCursor(texture, new Vector2(8f, 8f), CursorMode.Auto);
        }

        public static void Reset()
        {
            activeKind = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        static Texture2D CreateHorizontal()
        {
            var tex = CreateBlank(16, 16);
            DrawLine(tex, 2, 8, 13, 8);
            DrawLine(tex, 2, 8, 4, 6);
            DrawLine(tex, 2, 8, 4, 10);
            DrawLine(tex, 13, 8, 11, 6);
            DrawLine(tex, 13, 8, 11, 10);
            tex.Apply();
            return tex;
        }

        static Texture2D CreateVertical()
        {
            var tex = CreateBlank(16, 16);
            DrawLine(tex, 8, 2, 8, 13);
            DrawLine(tex, 8, 2, 6, 4);
            DrawLine(tex, 8, 2, 10, 4);
            DrawLine(tex, 8, 13, 6, 11);
            DrawLine(tex, 8, 13, 10, 11);
            tex.Apply();
            return tex;
        }

        static Texture2D CreateDiagonalNwSe()
        {
            var tex = CreateBlank(16, 16);
            DrawLine(tex, 3, 3, 12, 12);
            DrawLine(tex, 3, 3, 5, 3);
            DrawLine(tex, 3, 3, 3, 5);
            DrawLine(tex, 12, 12, 10, 12);
            DrawLine(tex, 12, 12, 12, 10);
            tex.Apply();
            return tex;
        }

        static Texture2D CreateDiagonalNeSw()
        {
            var tex = CreateBlank(16, 16);
            DrawLine(tex, 12, 3, 3, 12);
            DrawLine(tex, 12, 3, 10, 3);
            DrawLine(tex, 12, 3, 12, 5);
            DrawLine(tex, 3, 12, 5, 12);
            DrawLine(tex, 3, 12, 3, 10);
            tex.Apply();
            return tex;
        }

        static Texture2D CreateBlank(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var clear = new Color32(0, 0, 0, 0);
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = clear;
            tex.SetPixels32(pixels);
            return tex;
        }

        static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1)
        {
            var dx = Mathf.Abs(x1 - x0);
            var dy = Mathf.Abs(y1 - y0);
            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;
            var err = dx - dy;
            var color = new Color32(255, 255, 255, 255);

            while (true)
            {
                tex.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1)
                    break;

                var e2 = err * 2;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}
