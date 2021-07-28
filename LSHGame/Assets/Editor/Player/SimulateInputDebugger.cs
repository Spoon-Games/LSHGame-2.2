using LSHGame.Util;
using UnityEditor;

namespace LSHGame.Editor
{
    public class SimulateInputDebugger : EditorWindow
    {
        private bool IsJump = false;
        private bool IsA = false;
        private bool IsD = false;
        private bool IsW = false;
        private bool IsS = false;

        [MenuItem("Window/Util/Simulate Input Debugger")]
        public static void GetWindow()
        {
            GetWindow<SimulateInputDebugger>("Simulate Input Debugger");
        }

        private void OnGUI()
        {
            IsW = EditorGUILayout.Toggle("W", IsW);
            IsS = EditorGUILayout.Toggle("S", IsS);
            IsA = EditorGUILayout.Toggle("A", IsA);
            IsD = EditorGUILayout.Toggle("D", IsD);

            EditorGUILayout.Space();

            IsJump = EditorGUILayout.Toggle("Space", IsJump);

            GameInput.Debug_OverrideJump(IsJump);
            GameInput.Debug_OverrideWASD(IsW, IsA, IsS, IsD);
        }
    }
}
