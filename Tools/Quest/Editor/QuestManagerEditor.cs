namespace ZaiR37.Quest.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(3);

            EditorKit.HorizontalLine(new Color(0.3f, 0.3f, 0.3f));

            GUILayout.Space(3);

            EditorKit.HorizontalCenterButton(150, 30, "Refresh Quest List", () =>
            {
                Debug.Log("Quest List Refreshed!");
            });

            GUILayout.Space(3);
        }
    }
}