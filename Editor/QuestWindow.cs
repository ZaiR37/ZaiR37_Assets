namespace ZaiR37.Quest.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class QuestWindow : EditorWindow
    {
        enum WindowType
        {
            Creator,
            Editor
        }

        Vector2 mainScrollPos;

        private WindowType currentWindowType = WindowType.Creator;

        string questTitle;

        [MenuItem("Window/ZaiR37 Editor/Quest")]
        public static void ShowWindow()
        {
            GetWindow<QuestWindow>("Quest");
        }

        private void OnGUI()
        {
            EditorKit.HorizontalLayout(EditorHeader);
            EditorKit.HorizontalLine(new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(5);

            mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

            switch (currentWindowType)
            {
                case WindowType.Creator:
                    GUILayout.Label("Quest Creator");
                    break;

                case WindowType.Editor:
                    GUILayout.Label("Quest Editor");
                    break;
            }

            GUILayout.EndScrollView();
        }

        private void EditorHeader()
        {
            string titleWindow = "";

            if (GUILayout.Button("<", GUILayout.Width(80)))
            {
                if (currentWindowType == WindowType.Creator) currentWindowType = WindowType.Editor;
                else currentWindowType--;
            }

            switch (currentWindowType)
            {
                case WindowType.Creator:
                    titleWindow = "Quest Creator";
                    break;

                case WindowType.Editor:
                    titleWindow = "Quest Editor";
                    break;
            }

            GUILayout.Label(titleWindow, new GUIStyle(EditorStyles.boldLabel) { fontSize = 17, alignment = TextAnchor.MiddleCenter });

            if (GUILayout.Button(">", GUILayout.Width(80)))
            {
                if (currentWindowType == WindowType.Editor) currentWindowType = WindowType.Creator;
                else currentWindowType++;
            }
        }
        
    }
}