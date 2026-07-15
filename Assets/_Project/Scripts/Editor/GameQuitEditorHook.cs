using Katana.Core;
using UnityEditor;

namespace Katana.Editor
{
    [InitializeOnLoad]
    public static class GameQuitEditorHook
    {
        static GameQuitEditorHook()
        {
            GameQuit.EditorQuitOverride = () => EditorApplication.isPlaying = false;
        }
    }
}
