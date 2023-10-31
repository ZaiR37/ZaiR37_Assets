namespace ZaiR37.Quest.Editor
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using System.Collections.Generic;
    using System.IO;

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
        Color lineColor = new Color(0.3f, 0.3f, 0.3f);
        QuestData creatorQuest;
        Quest quest;

        string questTitleSearch;

        private bool showQuestInfoObjectives;
        private bool showQuestInfoRewards;
        bool showQuestInfoNotFound;
        private string[] questArray;

        bool showQuestCreatorObjectives;
        bool showQuestCreatorRewards;

        private void OnEnable()
        {
            searchProvider = CreateInstance<StringListSearchProvider>();
            InstantiateCreatorQuestData();

            QuestManager questManager = (QuestManager)target;
            questManager.RefreshQuestLibrary();
            questArray = questManager.GetQuestArray();
        }

        public override void OnInspectorGUI()
        {
            QuestManager questManager = (QuestManager)target;

            if (questManager.IsGameStarted()) ShowQuestInfo();
            else ShowQuestCreator();

            EditorKit.HorizontalLine(lineColor);
            GUILayout.Space(5);

            if (GUILayout.Button("Refresh List"))
            {
                questManager.RefreshQuestLibrary();
                questArray = questManager.GetQuestArray();
                return;
            };
        }

        #region Quest Creator
        private void ShowQuestCreator()
        {
            EditorKit.CenterLabelField("Quest Creator", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17 });

            EditorKit.HorizontalLine(lineColor);
            GUILayout.Space(5);

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Title", "The Quest's Name"), GUILayout.Width(148));
                creatorQuest.SetTitle(EditorGUILayout.TextField(creatorQuest.Title));
            });


            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Type", "Quest Type - Main, Side, or Commission"), GUILayout.Width(148));
                creatorQuest.SetType((QuestType)EditorGUILayout.EnumPopup(creatorQuest.Type));
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("From", "Quest Provider's Name"), GUILayout.Width(148));

                if (GUILayout.Button(creatorQuest.From, EditorStyles.popup))
                {
                    searchProvider.Init(npcList, (x) => { creatorQuest.SetFrom((string)x); });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Location", "Location of the Quest Objective"), GUILayout.Width(148));

                if (GUILayout.Button(creatorQuest.Location, EditorStyles.popup))
                {
                    searchProvider.Init(locationList, (x) => { creatorQuest.SetLocation((string)x); });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Description", "Quest Description"), GUILayout.Width(149));
                creatorQuest.SetDescription(GUILayout.TextArea(creatorQuest.Description));
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Orderly", "Completing objectives in a specific sequence"), GUILayout.Width(149));
                creatorQuest.SetOrderly(EditorGUILayout.Toggle(creatorQuest.Orderly));
            });


            EditorKit.HorizontalLayout(() =>
            {
                showQuestCreatorObjectives = EditorGUILayout.Foldout(showQuestCreatorObjectives,
                    new GUIContent("Objectives", "Objective List to Complete the Quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button($"Add New Objective ({creatorQuest.ObjectiveList.Count})"))
                {
                    QuestObjective newObjective = new QuestObjective();
                    creatorQuest.ObjectiveList.Add(newObjective);

                    showQuestCreatorObjectives = true;
                }

            });

            EditorKit.Indent(1, () =>
            {
                if (showQuestCreatorObjectives) ObjectivePanel(creatorQuest);
            });

            GUILayout.Space(3);

            EditorKit.HorizontalLayout(() =>
            {
                showQuestCreatorRewards = EditorGUILayout.Foldout(showQuestCreatorRewards,
                    new GUIContent("Rewards", "Rewards you receive upon completing the quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button($"Add New Reward ({creatorQuest.RewardList.Count})"))
                {
                    QuestReward newReward = new QuestReward();
                    creatorQuest.RewardList.Add(newReward);

                    showQuestCreatorRewards = true;
                }
            });

            EditorKit.Indent(1, () =>
            {
                if (showQuestCreatorRewards) RewardPanel(creatorQuest);
            });

            GUILayout.Space(3);

            creatorQuest.SetImage((Texture2D)EditorGUILayout.ObjectField
            (
                new GUIContent("Image", "Quest Image Displaying Objective Location"),
                creatorQuest.Image,
                typeof(Texture2D),
                false
            ));

            EditorKit.HorizontalCenterButton(100, 30, "Create", "Create new Quest Data.", () =>
            {
                CreateQuestData();
            });
        }

        private void ObjectivePanel(QuestData quest)
        {
            int listLength = quest.ObjectiveList.Count;
            if (EmptyListPanel(listLength)) return;

            EditorKit.VerticalLayoutBox(Color.black, () =>
            {
                List<int> indicesToRemove = new List<int>();

                for (int i = 0; i < listLength; i++)
                {
                    EditorKit.VerticalLayoutBox(Color.red, () =>
                    {
                        EditorKit.HorizontalLayout(() =>
                        {
                            int currentLoop = i + 1;
                            string numbering = (currentLoop < 10) ? $" {currentLoop}" : $"{currentLoop}";
                            EditorGUILayout.LabelField($"{numbering}. Type ", GUILayout.MaxWidth(119));

                            QuestObjectiveType currentObjectiveType = quest.ObjectiveList[i].Type;
                            quest.ObjectiveList[i].Type = (QuestObjectiveType)EditorGUILayout.EnumPopup(currentObjectiveType);

                            if (currentObjectiveType != quest.ObjectiveList[i].Type)
                            {
                                quest.ObjectiveList[i].Target = "";
                                quest.ObjectiveList[i].Quantity = 0;
                            }

                            if (GUILayout.Button("-", GUILayout.Width(50)))
                            {
                                indicesToRemove.Add(i);
                            }
                        });

                        EditorKit.Indent(1, () =>
                        {
                            ObjectiveType(i, quest);
                        });
                    });
                }

                foreach (int index in indicesToRemove)
                {
                    quest.ObjectiveList.RemoveAt(index);
                }

            });
        }

        private void ObjectiveType(int i, QuestData quest)
        {
            QuestObjective objective = quest.ObjectiveList[i];

            switch (objective.Type)
            {
                case QuestObjectiveType.Talk:
                case QuestObjectiveType.Guard:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(npcList, (x) => { quest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Description", "Objective Description"), GUILayout.Width(149));
                        quest.ObjectiveList[i].Description = GUILayout.TextArea(quest.ObjectiveList[i].Description);
                    });
                    break;

                case QuestObjectiveType.Visit:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Location", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(locationList, (x) => { quest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Description", "Objective Description"), GUILayout.Width(149));
                        quest.ObjectiveList[i].Description = GUILayout.TextArea(quest.ObjectiveList[i].Description);
                    });

                    break;

                case QuestObjectiveType.Defeat:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(npcList, (x) => { quest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Quantity"), GUILayout.Width(118));
                        quest.ObjectiveList[i].Quantity = EditorGUILayout.IntField(objective.Quantity);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Description", "Objective Description"), GUILayout.Width(149));
                        quest.ObjectiveList[i].Description = GUILayout.TextArea(quest.ObjectiveList[i].Description);
                    });

                    break;
                case QuestObjectiveType.Collect:
                case QuestObjectiveType.Craft:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Item", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(itemList, (x) => { quest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Quantity"), GUILayout.Width(118));
                        quest.ObjectiveList[i].Quantity = EditorGUILayout.IntField(objective.Quantity);
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Description", "Objective Description"), GUILayout.Width(149));
                        quest.ObjectiveList[i].Description = GUILayout.TextArea(quest.ObjectiveList[i].Description);
                    });

                    break;
            }
        }

        private void RewardPanel(QuestData quest)
        {
            int listLength = quest.RewardList.Count;
            if (EmptyListPanel(listLength)) return;

            EditorKit.VerticalLayoutBox(Color.black, () =>
            {
                List<int> indicesToRemove = new List<int>();

                for (int i = 0; i < listLength; i++)
                {
                    EditorKit.VerticalLayoutBox(Color.red, () =>
                    {
                        EditorKit.HorizontalLayout(() =>
                        {
                            int currentLoop = i + 1;
                            string numbering = (currentLoop < 10) ? $" {currentLoop}" : $"{currentLoop}";
                            EditorGUILayout.LabelField($"{numbering}. Type ", GUILayout.MaxWidth(119));

                            QuestRewardType currentRewardType = quest.RewardList[i].Type;
                            quest.RewardList[i].Type = (QuestRewardType)EditorGUILayout.EnumPopup(currentRewardType);

                            if (currentRewardType != quest.RewardList[i].Type)
                            {
                                quest.RewardList[i].ItemName = "";
                                quest.RewardList[i].ItemQuantity = 0;
                            }

                            if (GUILayout.Button("-", GUILayout.Width(50)))
                            {
                                indicesToRemove.Add(i);
                            }
                        });


                        EditorKit.Indent(1, () =>
                        {
                            RewardType(i, quest);
                        });
                    });
                }

                foreach (int index in indicesToRemove)
                {
                    quest.RewardList.RemoveAt(index);
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
                        EditorGUILayout.LabelField(new GUIContent("Item", "Reward Item"), GUILayout.Width(148));

                        if (GUILayout.Button(reward.ItemName, EditorStyles.popup))
                        {
                            searchProvider.Init(itemList, (x) => { quest.RewardList[i].ItemName = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Quantity"), GUILayout.Width(118));
                        quest.RewardList[i].ItemQuantity = EditorGUILayout.IntField(reward.ItemQuantity);
                    });

                    break;

                case QuestRewardType.Reputation:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Location", "Place where reputation increase."), GUILayout.Width(148));

                        if (GUILayout.Button(reward.ReputationPlace, EditorStyles.popup))
                        {
                            searchProvider.Init(reputationPlaceList, (x) => { quest.RewardList[i].ReputationPlace = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Progress"), GUILayout.Width(118));
                        quest.RewardList[i].ReputationProgress = EditorGUILayout.IntField(reward.ReputationProgress);
                    });
                    break;
            }
        }

        private bool EmptyListPanel(int listLength)
        {
            if (listLength <= 0)
            {
                EditorKit.VerticalLayoutBox(Color.black, () =>
                {
                    GUIStyle gUIStyle = new GUIStyle();
                    gUIStyle.normal.textColor = Color.white;

                    EditorKit.Indent(1, () =>
                    {
                        EditorGUILayout.TextField("Empty", gUIStyle);
                    });
                });

                return true;
            }
            return false;
        }

        private void InstantiateCreatorQuestData()
        {
            creatorQuest = CreateInstance<QuestData>();

            creatorQuest.SetTitle("Quest Title");
            creatorQuest.SetType(QuestType.Main);
            creatorQuest.SetFrom("None");
            creatorQuest.SetLocation("None");
            creatorQuest.SetDescription("None");
            creatorQuest.SetOrderly(false);
            creatorQuest.SetObjectiveList(new List<QuestObjective>());
            creatorQuest.SetQuestRewardList(new List<QuestReward>());
            creatorQuest.SetImage(null);
        }

        public void CreateQuestData()
        {
            string directory = "Assets/Data/Quest";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = $"{creatorQuest.Title}.asset";
            string filePath = Path.Combine(directory, filename);

            if (File.Exists(filePath))
            {
                Debug.LogError($"Asset with name '{filename}' already exists.");
                return;
            }


            AssetDatabase.CreateAsset(creatorQuest, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(creatorQuest.ObjectiveList);

            InstantiateCreatorQuestData();

            Debug.Log("New Quest Data created and saved to: " + filePath);
        }
        #endregion

        #region Quest Info
        private void ShowQuestInfo()
        {
            QuestManager questManager = (QuestManager)target;

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
                    if (questManager.IsGameStarted())
                    {
                        FindQuest_Title(questManager);
                        return;
                    }

                    Debug.Log("Game hasn't been started!");
                };
            });

            GUILayout.Space(3);

            var textStyle = new GUIStyle(EditorStyles.boldLabel);
            textStyle.normal.textColor = new Color(0.89f, 0.353f, 0.353f);
            if (showQuestInfoNotFound) EditorKit.CenterLabelField("Quest Not Found.", textStyle);
            if (quest != null) QuestInfo(quest.data);

            GUILayout.Space(3);
        }

        private void FindQuest_Title(QuestManager questManager)
        {
            if (questTitleSearch == "")
            {
                showQuestInfoNotFound = false;
                return;
            }

            quest = questManager.FindQuest(questTitleSearch);
            if (quest == null)
            {
                showQuestInfoNotFound = true;
                return;
            }

            QuestInfo(quest.data);
            showQuestInfoNotFound = false;
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

            showQuestInfoObjectives = EditorGUILayout.Foldout(showQuestInfoObjectives,
                new GUIContent("Objectives", "Objective List to Complete the Quest"),
                true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

            EditorKit.Indent(1, () =>
            {
                if (showQuestInfoObjectives) ObjectivePanelInfo(quest);
            });

            GUILayout.Space(3);

            showQuestInfoRewards = EditorGUILayout.Foldout(showQuestInfoRewards,
                new GUIContent("Rewards", "Rewards you receive upon completing the quest"),
                true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

            EditorKit.Indent(1, () =>
            {
                if (showQuestInfoRewards) RewardPanelInfo(quest);
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

        private void ObjectivePanelInfo(QuestData quest)
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
                            ObjectiveTypeInfo(i, quest);
                        });
                    });
                }
            });
        }

        private void ObjectiveTypeInfo(int i, QuestData quest)
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

        private void RewardPanelInfo(QuestData quest)
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
                            RewardTypeInfo(i, quest);
                        });
                    });
                }
            });
        }

        private void RewardTypeInfo(int i, QuestData quest)
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
        #endregion
    }
#endif
}