namespace ZaiR37.Quest.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        Vector2 mainScrollPos;

        bool showCreatorObjectives;
        bool showCreatorRewards;

        bool showEditorObjectives;
        bool showEditorRewards;

        Color lineColor = new Color(0.3f, 0.3f, 0.3f);
        WindowType currentWindowType = WindowType.Creator;

        StringListSearchProvider searchProvider;

        QuestData creatorQuest;
        QuestData sourceQuest;

        [MenuItem("Window/ZaiR37 Editor/Quest")]
        public static void ShowWindow()
        {
            GetWindow<QuestWindow>("Quest");
        }

        private void OnEnable()
        {
            searchProvider = CreateInstance<StringListSearchProvider>();

            InstantiateCreatorQuestData();
            sourceQuest = null;
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
            GUILayout.Space(5);

            footerFunction();

            GUILayout.EndScrollView();
        }

        private void CreatorHeader()
        {
            EditorKit.CenterLabelField("Quest Creator", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17 });
        }

        private void CreatorBody()
        {
            creatorQuest.SetTitle(EditorGUILayout.TextField(new GUIContent("Title", "The Quest's Name"), creatorQuest.Title));

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

            creatorQuest.SetOrderly(EditorGUILayout.Toggle(new GUIContent("Orderly", "Completing objectives in a specific sequence"), creatorQuest.Orderly));

            EditorKit.HorizontalLayout(() =>
            {
                showCreatorObjectives = EditorGUILayout.Foldout(showCreatorObjectives,
                    new GUIContent("Objectives", "Objective List to Complete the Quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button($"Add New Objective ({creatorQuest.ObjectiveList.Count})"))
                {
                    QuestObjective newObjective = new QuestObjective();
                    creatorQuest.ObjectiveList.Add(newObjective);

                    showCreatorObjectives = true;
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

                if (GUILayout.Button($"Add New Reward ({creatorQuest.RewardList.Count})"))
                {
                    QuestReward newReward = new QuestReward();
                    creatorQuest.RewardList.Add(newReward);

                    showCreatorRewards = true;
                }
            });

            EditorKit.Indent(1, () =>
            {
                if (showCreatorRewards) CreatorRewardPanel();
            });

            GUILayout.Space(3);

            creatorQuest.SetImage((Texture2D)EditorGUILayout.ObjectField
            (
                new GUIContent("Image", "Quest Image Displaying Objective Location"),
                creatorQuest.Image,
                typeof(Texture2D),
                false
            ));

        }

        private void CreatorObjectivePanel()
        {
            int listLength = creatorQuest.ObjectiveList.Count;
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

                            QuestObjectiveType currentObjectiveType = creatorQuest.ObjectiveList[i].Type;
                            creatorQuest.ObjectiveList[i].Type = (QuestObjectiveType)EditorGUILayout.EnumPopup(currentObjectiveType);

                            if (currentObjectiveType != creatorQuest.ObjectiveList[i].Type)
                            {
                                creatorQuest.ObjectiveList[i].Target = "";
                                creatorQuest.ObjectiveList[i].Quantity = 0;
                            }

                            if (GUILayout.Button("-", GUILayout.Width(50)))
                            {
                                indicesToRemove.Add(i);
                            }
                        });

                        EditorKit.Indent(1, () =>
                        {
                            CreatorObjectiveType(i);
                        });
                    });
                }

                foreach (int index in indicesToRemove)
                {
                    creatorQuest.ObjectiveList.RemoveAt(index);
                }

            });
        }

        private void CreatorObjectiveType(int i)
        {
            QuestObjective objective = creatorQuest.ObjectiveList[i];

            switch (objective.Type)
            {
                case QuestObjectiveType.Talk:
                case QuestObjectiveType.Guard:
                    EditorKit.HorizontalLayout(() =>
                        {
                            EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                            if (GUILayout.Button(objective.Target, EditorStyles.popup))
                            {
                                searchProvider.Init(npcList, (x) => { creatorQuest.ObjectiveList[i].Target = (string)x; });
                                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                            }
                        });
                    break;

                case QuestObjectiveType.Visit:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Location", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(locationList, (x) => { creatorQuest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });
                    break;

                case QuestObjectiveType.Kill:
                    EditorKit.HorizontalLayout(() =>
                        {
                            EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                            if (GUILayout.Button(objective.Target, EditorStyles.popup))
                            {
                                searchProvider.Init(npcList, (x) => { creatorQuest.ObjectiveList[i].Target = (string)x; });
                                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                            }
                        });

                    creatorQuest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);
                    break;
                case QuestObjectiveType.Collect:
                case QuestObjectiveType.Craft:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Item", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(itemList, (x) => { creatorQuest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    creatorQuest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);
                    break;
            }
        }

        private void CreatorRewardPanel()
        {
            int listLength = creatorQuest.RewardList.Count;
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

                            QuestRewardType currentRewardType = creatorQuest.RewardList[i].Type;
                            creatorQuest.RewardList[i].Type = (QuestRewardType)EditorGUILayout.EnumPopup(currentRewardType);

                            if (currentRewardType != creatorQuest.RewardList[i].Type)
                            {
                                creatorQuest.RewardList[i].ItemName = "";
                                creatorQuest.RewardList[i].ItemQuantity = 0;
                            }

                            if (GUILayout.Button("-", GUILayout.Width(50)))
                            {
                                indicesToRemove.Add(i);
                            }
                        });


                        EditorKit.Indent(1, () =>
                        {
                            CreatorRewardType(i);
                        });
                    });
                }

                foreach (int index in indicesToRemove)
                {
                    creatorQuest.RewardList.RemoveAt(index);
                }

            });
        }

        private void CreatorRewardType(int i)
        {
            QuestReward reward = creatorQuest.RewardList[i]; ;

            switch (reward.Type)
            {
                case QuestRewardType.Item:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Item", "Reward Item"), GUILayout.Width(148));

                        if (GUILayout.Button(reward.ItemName, EditorStyles.popup))
                        {
                            searchProvider.Init(itemList, (x) => { creatorQuest.RewardList[i].ItemName = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    creatorQuest.RewardList[i].ItemQuantity = EditorGUILayout.IntField("Quantity", reward.ItemQuantity);
                    break;

                case QuestRewardType.Reputation:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Location", "Place where reputation increase."), GUILayout.Width(148));

                        if (GUILayout.Button(reward.ReputationPlace, EditorStyles.popup))
                        {
                            searchProvider.Init(reputationPlaceList, (x) => { creatorQuest.RewardList[i].ReputationPlace = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    creatorQuest.RewardList[i].ReputationProgress = EditorGUILayout.IntField("Progress", reward.ReputationProgress);
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

        private void CreatorFooter()
        {
            EditorKit.HorizontalCenterButton(100, 30, "Create", "Create new Quest Data.", () =>
            {
                CreateQuestData();
            });
        }

        private void EditorHeader()
        {
            EditorKit.CenterLabelField("Quest Editor", new GUIStyle(EditorStyles.boldLabel) { fontSize = 17 });
        }

        private void EditorBody()
        {
            sourceQuest = (QuestData)EditorGUILayout.ObjectField("Source", sourceQuest, typeof(QuestData), false);
            GUILayout.Space(5);

            if (sourceQuest == null) return;

            EditorKit.HorizontalLine(lineColor);

            GUILayout.Space(5);

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Title", "The Quest's Name (CAN'T EDIT)"), GUILayout.Width(148));
                GUILayout.TextField(sourceQuest.Title);
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Type", "Quest Type - Main, Side, or Commission"), GUILayout.Width(148));
                sourceQuest.SetType((QuestType)EditorGUILayout.EnumPopup(sourceQuest.Type));
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("From", "Quest Provider's Name"), GUILayout.Width(148));

                if (GUILayout.Button(sourceQuest.From, EditorStyles.popup))
                {
                    searchProvider.Init(npcList, (x) => { sourceQuest.SetFrom((string)x); });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Location", "Location of the Quest Objective"), GUILayout.Width(148));

                if (GUILayout.Button(sourceQuest.Location, EditorStyles.popup))
                {
                    searchProvider.Init(locationList, (x) => { sourceQuest.SetLocation((string)x); });
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                }
            });

            EditorKit.HorizontalLayout(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Description", "Quest Description"), GUILayout.Width(149));
                sourceQuest.SetDescription(GUILayout.TextArea(sourceQuest.Description));
            });

            sourceQuest.SetOrderly(EditorGUILayout.Toggle(new GUIContent("Orderly", "Completing objectives in a specific sequence"), sourceQuest.Orderly));

            EditorKit.HorizontalLayout(() =>
            {
                showEditorObjectives = EditorGUILayout.Foldout(showEditorObjectives,
                    new GUIContent("Objectives", "Objective List to Complete the Quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button($"Add New Objective ({sourceQuest.ObjectiveList.Count})"))
                {
                    QuestObjective newObjective = new QuestObjective();
                    sourceQuest.ObjectiveList.Add(newObjective);

                    showEditorObjectives = true;
                }
            });

            EditorKit.Indent(1, () =>
            {
                if (showEditorObjectives) EditorObjectivePanel();
            });

            GUILayout.Space(3);

            EditorKit.HorizontalLayout(() =>
            {
                showEditorRewards = EditorGUILayout.Foldout(showEditorRewards,
                    new GUIContent("Rewards", "Rewards you receive upon completing the quest"),
                    true, new GUIStyle(EditorStyles.foldout) { fixedWidth = 100 });

                GUILayout.Space(100);

                if (GUILayout.Button("Add New Reward (+)"))
                {

                    showEditorRewards = true;
                }
            });

            EditorKit.Indent(1, () =>
            {
                if (showEditorRewards) EditorRewardPanel();
            });

            GUILayout.Space(3);

            sourceQuest.SetImage((Texture2D)EditorGUILayout.ObjectField
            (
                new GUIContent("Image", "Quest Image Displaying Objective Location"),
                sourceQuest.Image,
                typeof(Texture2D),
                false
            ));
        }

        private void EditorObjectivePanel()
        {
            int listLength = sourceQuest.ObjectiveList.Count;
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

                            QuestObjectiveType currentObjectiveType = sourceQuest.ObjectiveList[i].Type;
                            sourceQuest.ObjectiveList[i].Type = (QuestObjectiveType)EditorGUILayout.EnumPopup(currentObjectiveType);

                            if (currentObjectiveType != sourceQuest.ObjectiveList[i].Type)
                            {
                                sourceQuest.ObjectiveList[i].Target = "";
                                sourceQuest.ObjectiveList[i].Quantity = 0;
                            }

                            if (GUILayout.Button("-", GUILayout.Width(50)))
                            {
                                indicesToRemove.Add(i);
                            }
                        });

                        EditorKit.Indent(1, () =>
                        {
                            EditorObjectiveType(i);
                        });
                    });
                }

                foreach (int index in indicesToRemove)
                {
                    sourceQuest.ObjectiveList.RemoveAt(index);
                }
            });
        }

        private void EditorObjectiveType(int i)
        {
            QuestObjective objective = sourceQuest.ObjectiveList[i];

            switch (objective.Type)
            {
                case QuestObjectiveType.Talk:
                case QuestObjectiveType.Guard:
                    EditorKit.HorizontalLayout(() =>
                        {
                            EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                            if (GUILayout.Button(objective.Target, EditorStyles.popup))
                            {
                                searchProvider.Init(npcList, (x) => { sourceQuest.ObjectiveList[i].Target = (string)x; });
                                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                            }
                        });
                    break;

                case QuestObjectiveType.Visit:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Location", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(locationList, (x) => { sourceQuest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });
                    break;

                case QuestObjectiveType.Kill:
                    EditorKit.HorizontalLayout(() =>
                        {
                            EditorGUILayout.LabelField(new GUIContent("NPC Name", "Target Objective"), GUILayout.Width(148));

                            if (GUILayout.Button(objective.Target, EditorStyles.popup))
                            {
                                searchProvider.Init(npcList, (x) => { sourceQuest.ObjectiveList[i].Target = (string)x; });
                                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                            }
                        });

                    sourceQuest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);
                    break;
                case QuestObjectiveType.Collect:
                case QuestObjectiveType.Craft:
                    EditorKit.HorizontalLayout(() =>
                    {
                        EditorGUILayout.LabelField(new GUIContent("Item", "Target Objective"), GUILayout.Width(148));

                        if (GUILayout.Button(objective.Target, EditorStyles.popup))
                        {
                            searchProvider.Init(itemList, (x) => { sourceQuest.ObjectiveList[i].Target = (string)x; });
                            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
                        }
                    });

                    sourceQuest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);
                    break;
            }

        }

        private void EditorRewardPanel()
        {
            int listLength = sourceQuest.RewardList.Count;
            if (EmptyListPanel(listLength)) return;
        }

        private void EditorFooter()
        {
            if (sourceQuest == null) return;
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

            Selection.activeObject = creatorQuest;

            Debug.Log("New Quest Data created and saved to: " + filePath);
        }

    }
}