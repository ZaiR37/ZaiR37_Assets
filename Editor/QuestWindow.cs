namespace ZaiR37.Quest.Editor
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class QuestWindow : EditorWindow
    {
        enum WindowType
        {
            Creator,
            Editor
        }

        public string[] locationList = new string[]
        {
            "Chapter1/Chillshore/Northhollow",
            "Chapter1/Chillshore/Lastcairn",
            "Chapter1/Chillshore/Clayrock",
            "Chapter1/Chillshore/Littlehollow",
            "Chapter1/Silkband/Sandshade",
            "Chapter1/Silkband/Oxpond",
            "Chapter1/Silkband/Mythvault",
            "Chapter1/Silkband/Steelwick",
            "Chapter2/Magedrift/Timbervault",
            "Chapter2/Magedrift/Stillwatch",
            "Chapter2/Magedrift/Lostkeep",
            "Chapter2/Magedrift/Dustgulf"
        };

        public string[] npcList = new string[]
        {
            "Chillshore/Russell Lopez",
            "Chillshore/Ralph Dorsey",
            "Chillshore/Henryet Thévenet",
            "Chillshore/Josset Desmarais",
            "Silkband/Gabriel Maier",
            "Silkband/Valentin Baumgartner",
            "Silkband/Bartolo Albani",
            "Silkband/Bonaguida Spizega",
            "Magedrift/Arnao De Paredes",
            "Magedrift/Adelhard Lasch",
            "Magedrift/Rocco Wilhelm",
            "Magedrift/Vopiscus Lupercus"
        };

        Vector2 mainScrollPos;

        bool showCreatorObjectives;
        bool showCreatorRewards;
        Color lineColor = new Color(0.3f, 0.3f, 0.3f);
        WindowType currentWindowType = WindowType.Creator;

        StringListSearchProvider searchProvider;

        string questTitle = "Quest Title";
        QuestType questType = QuestType.Main;
        string questFrom = "None";
        string questLocation = "None";
        string questDescription = "None";
        bool orderly = false;
        Texture2D questImage = null;

        private QuestData targetObject;

        [MenuItem("Window/ZaiR37 Editor/Quest")]
        public static void ShowWindow()
        {
            GetWindow<QuestWindow>("Quest");
        }

        private void OnEnable()
        {
            searchProvider = ScriptableObject.CreateInstance<StringListSearchProvider>();
        }

        private void OnGUI()
        {
            switch (currentWindowType)
            {
                case WindowType.Creator:
                    EditorStructure(CreatorHeader, CreatorBody, CreatorFooter);
                    break;

                case WindowType.Editor:
                    EditorStructure(EditorHeader, EditorBody, EditorFooter);
                    break;
            }
        }

        private void EditorStructure(Action headerFunction, Action bodyFunction, Action footerFunction)
        {
            EditorKit.HorizontalLayout(() =>
            {
                if (GUILayout.Button("<", GUILayout.Width(80)))
                {
                    if (currentWindowType == WindowType.Creator) currentWindowType = WindowType.Editor;
                    else currentWindowType--;
                }

                headerFunction();

                if (GUILayout.Button(">", GUILayout.Width(80)))
                {
                    if (currentWindowType == WindowType.Editor) currentWindowType = WindowType.Creator;
                    else currentWindowType++;
                }
            });

            EditorKit.HorizontalLine(lineColor);
            GUILayout.Space(5);

            mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

            bodyFunction();

            GUILayout.Space(5);
            EditorKit.HorizontalLine(lineColor);

            footerFunction();

            GUILayout.EndScrollView();
        }

        private void CreatorHeader()
        {
            GUILayout.Label("Quest Creator", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17, alignment = TextAnchor.MiddleCenter });
        }

        private void CreatorBody()
        {
            questTitle = EditorGUILayout.TextField(new GUIContent("Title", "The Quest's Name"), questTitle);

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Type", "Quest Type - Main, Side, or Commission"), GUILayout.Width(148));
                questType = (QuestType)EditorGUILayout.EnumPopup(questType);
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("From", "Quest Provider's Name"), GUILayout.Width(148));

                if (GUILayout.Button(questFrom, EditorStyles.popup))
                {
                    searchProvider.Init(npcList, (x) => { questFrom = (string)x; });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Location", "Location of the Quest Objective"), GUILayout.Width(148));

                if (GUILayout.Button(questLocation, EditorStyles.popup))
                {
                    searchProvider.Init(locationList, (x) => { questLocation = (string)x; });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Description", "Quest Description"), GUILayout.Width(149));
                questDescription = GUILayout.TextArea(questDescription);
            });

            orderly = EditorGUILayout.Toggle(new GUIContent("Orderly", "Completing objectives in a specific sequence"), orderly);

            EditorKit.HorizontalLayout(() =>
            {
                showCreatorObjectives = EditorGUILayout.Foldout(showCreatorObjectives,
                    new GUIContent("Objectives", "Objective List to Complete the Quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button("Add New Objective (+)"))
                {
                }
            });

            EditorKit.Indent(1, () =>
            {
                if (showCreatorObjectives) CreatorObjectivePanel();
            });

            GUILayout.Space(3);

            EditorKit.HorizontalLayout(() =>
            {
                showCreatorRewards = EditorGUILayout.Foldout(showCreatorRewards,
                    new GUIContent("Rewards", "Rewards you receive upon completing the quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button("Add New Reward (+)"))
                {
                }
            });

            if (showCreatorRewards) CreatorRewardPanel();

            GUILayout.Space(3);

            questImage = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Image", "Quest Image Displaying Objective Location"), questImage, typeof(Texture2D), false);

        }

        private void CreatorObjectivePanel()
        {

        }

        private void CreatorRewardPanel()
        {

        }

        private void CreatorFooter()
        {

        }

        private void EditorHeader()
        {
            GUILayout.Label("Quest Editor", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17, alignment = TextAnchor.MiddleCenter });
        }

        private void EditorBody()
        {
            targetObject = (QuestData)EditorGUILayout.ObjectField("Quest", targetObject, typeof(QuestData), false);
        }

        private void EditorFooter()
        {

        }
    }
}