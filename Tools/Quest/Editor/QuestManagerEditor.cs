namespace ZaiR37.Quest.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;

    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : Editor
    {
        StringListSearchProvider searchProvider;

        QuestData questDataSearch;
        Quest quest;

        string questTitleSearch;

        private bool showQuestObjectives;
        private bool showQuestRewards;
        bool showQuestNotFound;
        private string[] questArray;

        private void OnEnable()
        {
            searchProvider = CreateInstance<StringListSearchProvider>();

            QuestManager questManager = (QuestManager)target;

            questManager.RefreshQuestLibrary();
            questArray = questManager.GetQuestArray();
        }

        public override void OnInspectorGUI()
        {
            QuestManager questManager = (QuestManager)target;

            if (GUILayout.Button("Refresh List"))
            {
                questManager.RefreshQuestLibrary();
                questArray = questManager.GetQuestArray();
                return;
            };

            EditorKit.HorizontalLayout(() =>
            {
                GUILayout.Label(new GUIContent("Search Quest"), GUILayout.Width(Screen.width / 2.8f));

                if (GUILayout.Button(questTitleSearch, EditorStyles.popup))
                {
                    searchProvider.Init(questArray, (x) => { questTitleSearch = (string)x; });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }

                if (GUILayout.Button("Find", GUILayout.Width(60)))
                {
                    if (questManager.IsGameStarted()) FindQuestTitle(questManager);
                    Debug.Log("Game hasn't been started!");
                };
            });

            EditorKit.HorizontalLayout(() =>
            {
                questDataSearch = (QuestData)EditorGUILayout.ObjectField(" ", questDataSearch, typeof(QuestData), false);
                if (GUILayout.Button("Find", GUILayout.Width(60)))
                {
                    if (questManager.IsGameStarted()) FindQuestData(questManager);
                    Debug.Log("Game hasn't been started!");
                };
            });

            GUILayout.Space(3);
            var textStyle = new GUIStyle(EditorStyles.boldLabel);
            textStyle.normal.textColor = new Color(0.89f, 0.353f, 0.353f);
            if (showQuestNotFound) EditorKit.CenterLabelField("Quest Not Found.", textStyle);
            if (quest != null) QuestInfo(quest.data);
            GUILayout.Space(3);
        }

        private void FindQuestData(QuestManager questManager)
        {
            if (questDataSearch == null)
            {
                showQuestNotFound = false;
                return;
            }

            quest = questManager.FindQuest(questDataSearch);

            if (quest == null)
            {
                showQuestNotFound = true;
                return;
            }

            QuestInfo(quest.data);
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

            quest = questManager.FindQuest(questTitleSearch);
            if (quest == null)
            {
                showQuestNotFound = true;
                return;
            }

            QuestInfo(quest.data);
            questDataSearch = quest.data;
            showQuestNotFound = false;
        }

        private void QuestInfo(QuestData quest)
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
    }
}