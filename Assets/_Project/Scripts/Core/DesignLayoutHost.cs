using UnityEngine;

namespace Katana.Core
{
    /// <summary>
    /// Met à l'échelle le layout design (coordonnées fixes) pour remplir la zone Inner.
    /// </summary>
    public class DesignLayoutHost : MonoBehaviour
    {
        float designWidth;
        float designHeight;
        Transform layoutRoot;

        public void Initialize(float width, float height, Transform layout)
        {
            designWidth = width;
            designHeight = height;
            layoutRoot = layout;
        }

        public void Refresh(float innerWidth, float innerHeight)
        {
            if (layoutRoot == null)
                return;

            var scale = Mathf.Min(
                innerWidth / Mathf.Max(1f, designWidth),
                innerHeight / Mathf.Max(1f, designHeight));

            layoutRoot.localScale = Vector3.one * scale;
        }

        public void RefreshFromWindow(RectTransform window, GameplayWindowProfile profile)
        {
            Refresh(profile.InnerSize(window.sizeDelta).x, profile.InnerSize(window.sizeDelta).y);
        }
    }
}
