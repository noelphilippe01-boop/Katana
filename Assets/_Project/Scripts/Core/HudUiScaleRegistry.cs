using System.Collections.Generic;
using UnityEngine.UI;

namespace Katana.Core
{
    public static class HudUiScaleRegistry
    {
        static readonly List<CanvasScaler> Scalers = new();

        public static void Register(CanvasScaler scaler)
        {
            if (scaler == null || Scalers.Contains(scaler))
                return;

            Scalers.Add(scaler);
            KatanaUiFactory.ApplyMenuScale(scaler, GameSettings.HudUiScale);
        }

        public static void Unregister(CanvasScaler scaler)
        {
            if (scaler != null)
                Scalers.Remove(scaler);
        }

        public static void ApplyAll()
        {
            var scale = GameSettings.HudUiScale;
            for (var i = Scalers.Count - 1; i >= 0; i--)
            {
                if (Scalers[i] == null)
                {
                    Scalers.RemoveAt(i);
                    continue;
                }

                KatanaUiFactory.ApplyMenuScale(Scalers[i], scale);
            }
        }
    }
}
