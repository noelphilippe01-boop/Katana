using System;
using UnityEngine;

namespace Katana.Core
{
    public static class GameQuit
    {
        public static Action EditorQuitOverride { get; set; }

        public static void Quit()
        {
            if (EditorQuitOverride != null)
            {
                EditorQuitOverride.Invoke();
                return;
            }

            Application.Quit();
        }
    }
}
