namespace ZaiR37.Quest.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(QuestData))]
    public class QuestDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.89f, 0.353f, 0.353f) },
            };

            GUILayout.Label("Open the data on Quest Editor", textStyle);
        }
    }
}