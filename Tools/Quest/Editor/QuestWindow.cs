namespace ZaiR37.Quest.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class QuestWindow : Editor
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

        Vector2 mainScrollPos;

        bool showCreatorObjectives;
        bool showCreatorRewards;

        StringListSearchProvider searchProvider;
        Color lineColor = new Color(0.3f, 0.3f, 0.3f);
        QuestData creatorQuest;

        private void OnEnable()
        {
            searchProvider = CreateInstance<StringListSearchProvider>();
            InstantiateCreatorQuestData();
        }

        private void OnGUI()
        {
            EditorStructure(CreatorHeader, CreatorBody, CreatorFooter);
        }

        private void EditorStructure(Action headerFunction, Action bodyFunction, Action footerFunction)
        {
            EditorKit.HorizontalLayout(() =>
            {
                headerFunction();
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
                if (showCreatorObjectives) ObjectivePanel(creatorQuest);
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
                if (showCreatorRewards) RewardPanel(creatorQuest);
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

        private void CreatorFooter()
        {
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

                    quest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);

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

                    quest.ObjectiveList[i].Quantity = EditorGUILayout.IntField("Quantity", objective.Quantity);

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

                    quest.RewardList[i].ItemQuantity = EditorGUILayout.IntField("Quantity", reward.ItemQuantity);
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

                    quest.RewardList[i].ReputationProgress = EditorGUILayout.IntField("Progress", reward.ReputationProgress);
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

            Selection.activeObject = creatorQuest;

            Debug.Log("New Quest Data created and saved to: " + filePath);
        }

    }
}