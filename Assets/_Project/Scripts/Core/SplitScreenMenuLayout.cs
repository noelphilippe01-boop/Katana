using UnityEngine;
using UnityEngine.UI;

namespace Katana.Core
{
    /// <summary>
    /// Jeu en pause avec assombrissement plein écran ; fenêtres menu alignées à droite.
    /// </summary>
    public static class SplitScreenMenuLayout
    {
        /// <summary>Largeur de la colonne menu en unités UI (réf. 1920×1080).</summary>
        public const float RightPaneReferenceWidth = 520f;

        public const float ReferenceScreenWidth = 1920f;
        public const float ReferenceScreenHeight = 1080f;

        /// <summary>Ratio hauteur/largeur des fenêtres inventaire (ancien format 720×870).</summary>
        public const float WindowReferenceAspect = 870f / 720f;

        public const float WindowVerticalMargin = 40f;

        /// <summary>Marge intérieure recommandée entre le bord du panneau et une fenêtre.</summary>
        public const float WindowPaneInset = 12f;

        public static float WindowReferenceWidth => RightPaneReferenceWidth - WindowPaneInset * 2f;

        public static float WindowReferenceHeight(float windowWidth) => windowWidth * WindowReferenceAspect;

        /// <summary>
        /// Hauteur maximale utilisable (réf. ou écran réel).
        /// </summary>
        public static float GetMaxWindowHeight(Canvas canvas)
        {
            var maxHeight = ReferenceScreenHeight - WindowVerticalMargin * 2f;
            if (canvas == null)
                return maxHeight;

            var scale = Mathf.Max(0.01f, canvas.scaleFactor);
            return Mathf.Min(maxHeight, canvas.pixelRect.height / scale - WindowVerticalMargin * 2f);
        }

        /// <summary>
        /// Hauteur de fenêtre : privilégie l'espace vertical disponible tout en respectant le ratio minimum.
        /// </summary>
        public static float ResolveWindowHeight(Canvas canvas, float windowWidth)
        {
            var proportional = WindowReferenceHeight(windowWidth);
            var maxHeight = GetMaxWindowHeight(canvas);
            return Mathf.Clamp(Mathf.Max(proportional, maxHeight * 0.92f), proportional, maxHeight);
        }

        public static float ResolveContentScale(float windowWidth, float windowHeight, float designHeight) =>
            designHeight <= 0f ? 1f : windowHeight / designHeight;

        const int GameplayDimCanvasOrder = 85;

        static int sessionCount;
        static GameObject gameplayDimRoot;

        public static bool IsActive => sessionCount > 0;

        public static void Enter()
        {
            sessionCount++;
            if (sessionCount != 1)
                return;

            ShowGameplayDim();
        }

        public static void Exit()
        {
            sessionCount = Mathf.Max(0, sessionCount - 1);
            if (sessionCount > 0)
                return;

            HideGameplayDim();
        }

        public static void ForceReset()
        {
            sessionCount = 0;
            HideGameplayDim();
            RestoreCamera();
        }

        static void RestoreCamera()
        {
            if (Camera.main != null)
                Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        }

        static void ShowGameplayDim()
        {
            EnsureGameplayDimOverlay();
            if (gameplayDimRoot != null)
                gameplayDimRoot.SetActive(true);
        }

        static void HideGameplayDim()
        {
            if (gameplayDimRoot != null)
                gameplayDimRoot.SetActive(false);
        }

        static void EnsureGameplayDimOverlay()
        {
            if (gameplayDimRoot != null)
                return;

            KatanaUiFactory.EnsureEventSystem();
            var canvas = KatanaUiFactory.CreateOverlayCanvas("GameplayPauseDimCanvas", GameplayDimCanvasOrder);
            gameplayDimRoot = canvas.gameObject;
            var dimPanel = KatanaUiFactory.CreateFullScreenPanel(
                canvas.transform,
                "GameplayDimPanel",
                KatanaUiTheme.GameplayPauseDim);
            dimPanel.GetComponent<Image>().raycastTarget = false;
        }
    }

    public enum ScreenHalf
    {
        Left,
        Right
    }
}
