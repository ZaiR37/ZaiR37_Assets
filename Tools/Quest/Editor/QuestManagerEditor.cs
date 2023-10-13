namespace ZaiR37.Quest.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;

    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : Editor
    {
        public string[] locationList = new string[]
{
            "Chillshore Town/Northhollow Gate",
            "Chillshore Town/Lastcairn Cave",
            "Chillshore Town/Clayrock Tavern",
            "Chillshore Town/Littlehollow Castle",
            "Silkband Town/Sandshade Gate",
            "Silkband Town/Oxpond Cave",
            "Silkband Town/Mythvault Tavern",
            "Silkband Town/Steelwick Castle",
            "Magedrift Town/Timbervault Gate",
            "Magedrift Town/Stillwatch Cave",
            "Magedrift Town/Lostkeep Tavern",
            "Magedrift Town/Dustgulf Castle"
};

        public string[] npcList = new string[]
        {
            "Chillshore Town/Russell Lopez",
            "Chillshore Town/Ralph Dorsey",
            "Chillshore Town/Henryet Thévenet",
            "Chillshore Town/Josset Desmarais",
            "Silkband Town/Gabriel Maier",
            "Silkband Town/Valentin Baumgartner",
            "Silkband Town/Bartolo Albani",
            "Silkband Town/Bonaguida Spizega",
            "Magedrift Town/Arnao De Paredes",
            "Magedrift Town/Adelhard Lasch",
            "Magedrift Town/Rocco Wilhelm",
            "Magedrift Town/Vopiscus Lupercus"
        };

        public string[] itemList = new string[]
        {
            "Gold",
            "Flower/Lily",
            "Flower/Sunflower",
            "Flower/Daisy",
            "Flower/Dandelion",
            "Flower/Marigold",
            "Mineral/Green Gatandrite",
            "Mineral/Danbanite",
            "Mineral/Blue-Green Uraium",
            "Mineral/Seliphorite",
            "Mineral/Stutonite",
        };

        public string[] reputationPlaceList = new string[]
        {
            "Chillshore Town",
            "Silkband Town",
            "Magedrift"
        };

        StringListSearchProvider searchProvider;
        string questDataDirectory = "Assets/Data/Quest";

        string questTitleSearch;
        QuestData questDataSearch;

        Quest quest;

        bool showQuestLibrary;
        bool showQuestNotFound;

        private bool showQuestObjectives;
        private bool showQuestRewards;

        private void OnEnable()
        {
            searchProvider = CreateInstance<StringListSearchProvider>();
        }

        public override void OnInspectorGUI()
        {
            QuestManager questManager = (QuestManager)target;

            EditorKit.HorizontalLayout(() =>
            {
                questTitleSearch = EditorGUILayout.TextField("Search Quest", questTitleSearch);
                if (GUILayout.Button("Find", GUILayout.Width(60)))
                {
                    FindQuestTitle(questManager);
                };
            });

            EditorKit.HorizontalLayout(() =>
            {
                questDataSearch = (QuestData)EditorGUILayout.ObjectField(" ", questDataSearch, typeof(QuestData), false);
                if (GUILayout.Button("Find", GUILayout.Width(60)))
                {
                    FindQuestData(questManager);
                };
            });

            GUILayout.Space(3);
            var textStyle = new GUIStyle(EditorStyles.boldLabel);
            textStyle.normal.textColor = new Color(0.89f, 0.353f, 0.353f);
            if (showQuestNotFound) EditorKit.CenterLabelField("Quest Not Found.", textStyle);
            if (quest != null) QuestInfo(questManager);
            GUILayout.Space(3);


            GUILayout.Space(3);
            EditorKit.HorizontalLine(new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(3);

            EditorGUILayout.TextField("Directory", questDataDirectory);

            if (GUILayout.Button(new GUIContent("Refresh Quest Library", "Get all quest from the folder to the library"), GUILayout.Height(30)))
            {
                RefreshQuestLibrary(questManager);
            }

            GUILayout.Space(3);
            EditorKit.HorizontalLine(new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(3);

            showQuestLibrary = EditorGUILayout.Foldout(showQuestLibrary, new GUIContent("Quest Library", "Library for all quest"), true);
            if (showQuestLibrary) QuestLibrary(questManager);
        }

        private void FindQuestData(QuestManager questManager)
        {
            if (questDataSearch == null)
            {
                showQuestNotFound = false;
                return;
            }

            quest = questManager.FindThisQuest(questDataSearch);

            if (quest == null)
            {
                showQuestNotFound = true;
                return;
            }

            QuestInfo(questManager);
            questTitleSearch = quest.data.Title;
            showQuestNotFound = false;
        }

        private void FindQuestTitle(QuestManager questManager)
        {
            if (questTitleSearch == "")
            {
                showQuestNotFound = false;
                return;
            }

            quest = questManager.FindThisQuest(questTitleSearch);
            if (quest == null)
            {
                showQuestNotFound = true;
                return;
            }

            QuestInfo(questManager);
            questDataSearch = quest.data;
            showQuestNotFound = false;
        }

        private void QuestInfo(QuestManager questManager)
        {
            CreatorBody(quest.data);
        }

        private void CreatorBody(QuestData quest)
        {
            if (quest == null) return;

            EditorKit.HorizontalLine(new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(3);

            EditorKit.CenterLabelField("Quest Info", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17 });

            GUILayout.Space(3);

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("Title", GUILayout.Width(148));
                GUILayout.TextField(quest.Title);
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("Type", GUILayout.Width(148));
                GUILayout.TextField(quest.Type.ToString());
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("From", GUILayout.Width(148));
                GUILayout.TextField(quest.From);
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("Location", GUILayout.Width(148));
                GUILayout.TextField(quest.Location);
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(148));
                GUILayout.TextArea(quest.Description);
            });

            EditorKit.HorizontalLayout(() =>
            {
                GUILayout.Label("Orderly", GUILayout.Width(148));
                GUILayout.Label(quest.Orderly.ToString());
            });


            showQuestObjectives = EditorGUILayout.Foldout(showQuestObjectives,
                new GUIContent("Objectives", "Objective List to Complete the Quest"),
                true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

            EditorKit.Indent(1, () =>
            {
                if (showQuestObjectives) ObjectivePanel(quest);
            });

            GUILayout.Space(3);

            showQuestRewards = EditorGUILayout.Foldout(showQuestRewards,
                new GUIContent("Rewards", "Rewards you receive upon completing the quest"),
                true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

            EditorKit.Indent(1, () =>
            {
                if (showQuestRewards) RewardPanel(quest);
            });

            GUILayout.Space(3);

            EditorGUILayout.ObjectField
            (
                new GUIContent("Image", "Quest Image Displaying Objective Location"),
                quest.Image,
                typeof(Texture2D),
                false
            );

        }

        private void ObjectivePanel(QuestData quest)
        {
            int listLength = quest.ObjectiveList.Count;

            EditorKit.VerticalLayoutBox(Color.black, () =>
            {
                for (int i = 0; i < listLength; i++)
                {
                    EditorKit.VerticalLayoutBox(Color.red, () =>
                    {
                        EditorKit.HorizontalLayout(() =>
                        {
                            int currentLoop = i + 1;
                            string numbering = (currentLoop < 10) ? $" {currentLoop}" : $"{currentLoop}";
                            EditorGUILayout.LabelField($"{numbering}. Type ", GUILayout.MaxWidth(149));
                            GUILayout.TextField(quest.ObjectiveList[i].Type.ToString());
                        });

                        EditorKit.Indent(1, () =>
                        {
                            ObjectiveType(i, quest);
                        });
                    });
                }
            });
        }

        private void ObjectiveType(int i, QuestData quest)
        {
            QuestObjective objective = quest.ObjectiveList[i];
            QuestProgress questProgress = this.quest.progressList[i];

            switch (objective.Type)
            {
                case QuestObjectiveType.Talk:
                case QuestObjectiveType.Guard:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("NPC Name", GUILayout.Width(148));
                        GUILayout.TextField(quest.ObjectiveList[i].Target);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Completed", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.isComplete.ToString());
                    });

                    break;

                case QuestObjectiveType.Visit:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Location", GUILayout.Width(148));
                        GUILayout.TextField(quest.ObjectiveList[i].Target);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Completed", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.isComplete.ToString());
                    });
                    break;

                case QuestObjectiveType.Defeat:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("NPC Name", GUILayout.Width(148));
                        GUILayout.TextField(quest.ObjectiveList[i].Target);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Progress", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.progressQuantity.ToString() + "/" + objective.Quantity.ToString());
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Completed", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.isComplete.ToString());
                    });

                    break;
                case QuestObjectiveType.Collect:
                case QuestObjectiveType.Craft:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Item", GUILayout.Width(148));
                        GUILayout.TextField(quest.ObjectiveList[i].Target);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Progress", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.progressQuantity.ToString() + "/" + objective.Quantity.ToString());
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Completed", GUILayout.Width(148));
                        GUILayout.TextField(questProgress.isComplete.ToString());
                    });
                    break;
            }
        }

        private void RewardPanel(QuestData quest)
        {
            int listLength = quest.RewardList.Count;

            EditorKit.VerticalLayoutBox(Color.black, () =>
            {
                for (int i = 0; i < listLength; i++)
                {
                    EditorKit.VerticalLayoutBox(Color.red, () =>
                    {
                        EditorKit.HorizontalLayout(() =>
                        {
                            int currentLoop = i + 1;
                            string numbering = (currentLoop < 10) ? $" {currentLoop}" : $"{currentLoop}";
                            EditorGUILayout.LabelField($"{numbering}. Type ", GUILayout.MaxWidth(119));

                            GUILayout.TextField(quest.RewardList[i].Type.ToString());
                        });

                        EditorKit.Indent(1, () =>
                        {
                            RewardType(i, quest);
                        });
                    });
                }
            });
        }

        private void RewardType(int i, QuestData quest)
        {
            QuestReward reward = quest.RewardList[i]; ;

            switch (reward.Type)
            {
                case QuestRewardType.Item:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Item", GUILayout.Width(148));
                        GUILayout.TextField(quest.RewardList[i].ItemName);
                    });
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Quantity", GUILayout.Width(148));
                        GUILayout.TextField(quest.RewardList[i].ItemQuantity.ToString());
                    });
                    break;

                case QuestRewardType.Reputation:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Location", GUILayout.Width(148));
                        GUILayout.TextField(quest.RewardList[i].ReputationPlace.ToString());
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField("Location", GUILayout.Width(148));
                        GUILayout.TextField(quest.RewardList[i].ReputationProgress.ToString());
                    });
                    break;
            }
        }

        private void QuestLibrary(QuestManager questManager)
        {
            List<QuestData> questLibrary = questManager.GetQuestLibrary();
            EditorKit.VerticalLayoutBox(Color.black, () =>
            {
                for (int i = 0; i < questLibrary.Count; i++)
                {
                    QuestData questData = questLibrary[i];

                    EditorKit.VerticalLayoutBox(Color.red, () =>
                    {
                        GUILayout.Label(i + 1 + ". " + questData.Title);
                    });
                };
            });


        }

        private void RefreshQuestLibrary(QuestManager questManager)
        {
            List<QuestData> questLibrary = new List<QuestData>();

            QuestData[] questDataAssets = AssetDatabase.FindAssets("t:QuestData", new[] { questDataDirectory })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<QuestData>)
                .ToArray();

            questLibrary.AddRange(questDataAssets);
            questManager.SetQuestLibrary(questLibrary);
            Debug.Log("Quest List Refreshed!");
        }
    }
}