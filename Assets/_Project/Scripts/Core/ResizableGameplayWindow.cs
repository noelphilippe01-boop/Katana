using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Katana.Core
{
    /// <summary>
    /// Redimensionnement manuel des fenêtres gameplay : proportions, alignement droite, centrage vertical.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ResizableGameplayWindow : MonoBehaviour
    {
        const float EdgeThickness = 12f;
        const float CornerSize = 20f;
        const float DragSensitivity = 0.0032f;

        enum ResizeHandle
        {
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        GameplayWindowProfile profile;
        RectTransform windowRect;
        DesignLayoutHost layoutHost;
        float currentScale = 1f;
        static int activeDragCount;

        public static ResizableGameplayWindow Attach(
            RectTransform window,
            GameplayWindowProfile windowProfile,
            bool resizable)
        {
            var component = window.gameObject.GetComponent<ResizableGameplayWindow>();
            if (component == null)
                component = window.gameObject.AddComponent<ResizableGameplayWindow>();

            component.Configure(windowProfile, resizable);
            return component;
        }

        void Configure(GameplayWindowProfile windowProfile, bool resizable)
        {
            profile = windowProfile;
            windowRect = (RectTransform)transform;
            layoutHost = GetComponentInChildren<DesignLayoutHost>(true);
            currentScale = GameSettings.GetGameplayWindowScale(profile.WindowId);
            ApplyScale(currentScale);

            if (resizable)
                EnsureResizeBorder();
        }

        public void ApplyScale(float scale)
        {
            currentScale = Mathf.Clamp(scale, GameSettings.MinGameplayWindowScale, GameSettings.MaxGameplayWindowScale);
            var size = profile.ResolveWindowSize(currentScale);
            GameplayWindowLayout.ApplyRightAlignedWindow(windowRect, size);
            layoutHost?.RefreshFromWindow(windowRect, profile);
        }

        void EnsureResizeBorder()
        {
            var legacyGrip = transform.Find("ResizeGrip");
            if (legacyGrip != null)
                Destroy(legacyGrip.gameObject);

            if (transform.Find("ResizeBorder") != null)
                return;

            var border = new GameObject("ResizeBorder");
            border.transform.SetParent(transform, false);
            border.transform.SetAsLastSibling();

            var borderRect = border.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;

            CreateHandle(border.transform, "EdgeLeft", ResizeHandle.Left,
                new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f),
                new Vector2(EdgeThickness, 0f), Vector2.zero);
            CreateHandle(border.transform, "EdgeRight", ResizeHandle.Right,
                new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f),
                new Vector2(EdgeThickness, 0f), Vector2.zero);
            CreateHandle(border.transform, "EdgeTop", ResizeHandle.Top,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, EdgeThickness), Vector2.zero);
            CreateHandle(border.transform, "EdgeBottom", ResizeHandle.Bottom,
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, EdgeThickness), Vector2.zero);
            CreateHandle(border.transform, "CornerTopLeft", ResizeHandle.TopLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(CornerSize, CornerSize), Vector2.zero);
            CreateHandle(border.transform, "CornerTopRight", ResizeHandle.TopRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(CornerSize, CornerSize), Vector2.zero);
            CreateHandle(border.transform, "CornerBottomLeft", ResizeHandle.BottomLeft,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(CornerSize, CornerSize), Vector2.zero);
            CreateHandle(border.transform, "CornerBottomRight", ResizeHandle.BottomRight,
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(CornerSize, CornerSize), Vector2.zero);
        }

        static void CreateHandle(
            Transform parent,
            string name,
            ResizeHandle handle,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 sizeDelta,
            Vector2 anchoredPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.sizeDelta = sizeDelta;
            rect.anchoredPosition = anchoredPosition;

            var image = go.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0f);
            image.raycastTarget = true;

            var zone = go.AddComponent<WindowResizeZone>();
            zone.Initialize(handle);
        }

        void AdjustScaleByDrag(ResizeHandle handle, Vector2 screenDelta)
        {
            ApplyScale(currentScale + ResolveDragDelta(handle, screenDelta) * DragSensitivity);
        }

        void CommitScale() =>
            GameSettings.SetGameplayWindowScale(profile.WindowId, currentScale);

        static float ResolveDragDelta(ResizeHandle handle, Vector2 screenDelta)
        {
            return handle switch
            {
                ResizeHandle.Left => -screenDelta.x,
                ResizeHandle.Right => screenDelta.x,
                ResizeHandle.Top => screenDelta.y,
                ResizeHandle.Bottom => -screenDelta.y,
                ResizeHandle.TopLeft => (-screenDelta.x + screenDelta.y) * 0.5f,
                ResizeHandle.TopRight => (screenDelta.x + screenDelta.y) * 0.5f,
                ResizeHandle.BottomLeft => (screenDelta.x + screenDelta.y) * 0.5f,
                ResizeHandle.BottomRight => (screenDelta.x - screenDelta.y) * 0.5f,
                _ => 0f
            };
        }

        static UiResizeCursorKind CursorForHandle(ResizeHandle handle) =>
            handle switch
            {
                ResizeHandle.Left or ResizeHandle.Right => UiResizeCursorKind.Horizontal,
                ResizeHandle.Top or ResizeHandle.Bottom => UiResizeCursorKind.Vertical,
                ResizeHandle.TopLeft or ResizeHandle.BottomRight => UiResizeCursorKind.DiagonalNwSe,
                _ => UiResizeCursorKind.DiagonalNeSw
            };

        sealed class WindowResizeZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
            IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            ResizableGameplayWindow owner;
            ResizeHandle handle;

            public void Initialize(ResizeHandle resizeHandle) => handle = resizeHandle;

            void Awake() => owner = GetComponentInParent<ResizableGameplayWindow>();

            public void OnPointerEnter(PointerEventData eventData) =>
                UiResizeCursors.Set(CursorForHandle(handle));

            public void OnPointerExit(PointerEventData eventData)
            {
                if (activeDragCount == 0)
                    UiResizeCursors.Reset();
            }

            public void OnBeginDrag(PointerEventData eventData) => activeDragCount++;

            public void OnDrag(PointerEventData eventData)
            {
                if (owner != null)
                    owner.AdjustScaleByDrag(handle, eventData.delta);
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                activeDragCount = Mathf.Max(0, activeDragCount - 1);
                owner?.CommitScale();
                if (activeDragCount == 0)
                    UiResizeCursors.Reset();
            }
        }
    }
}
