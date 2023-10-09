namespace ZaiR37.Quest.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public static class EditorKit
    {
        public static void HorizontalLayout(Action function)
        {
            EditorGUILayout.BeginHorizontal();
            function();
            EditorGUILayout.EndHorizontal();
        }

        public static void VerticalLayout(Action function)
        {
            EditorGUILayout.BeginVertical();
            function();
            EditorGUILayout.EndVertical();
        }

        public static void HorizontalLine(Color lineColor)
        {
            GUIStyle newStyle = new GUIStyle(GUI.skin.box);
            newStyle.normal.background = MakeTex(2, 2, lineColor);

            GUILayout.Box("", newStyle, GUILayout.Height(2), GUILayout.ExpandWidth(true));
        }

        public static void Indent(int indentLevel, Action function)
        {
            EditorGUI.indentLevel += indentLevel;
            function();
            EditorGUI.indentLevel -= indentLevel;
        }


        private static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}