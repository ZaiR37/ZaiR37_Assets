
namespace ZaiR37.Quest
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        string questDataDirectory = "Assets/Data/Quest";
        [SerializeField] private List<QuestData> questLibrary;
        private List<Quest> questList;
        string[] questArray;

        bool gameStarted = false;

        private void OnEnable()
        {
            RefreshQuestLibrary();
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one QuestManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            questList = new List<Quest>();
            RefreshQuestLibrary();
            gameStarted = true;

            foreach (QuestData questData in questLibrary)
            {
                AddCurrentQuest(questData);
            }
        }

        public void AddCurrentQuest(QuestData questData)
        {
            Quest quest = FindQuest(questData);
            if (quest != null)
            {
                Debug.Log("This Quest is already in the list!");
                return;
            }

            if (questData.ObjectiveList == null || questData.RewardList == null)
            {
                Debug.LogError($"Quest {questData.Title} have null objective / reward list");
                return;
            }

            int progressLength = questData.ObjectiveList.Count;
            Quest newQuest = new Quest
            {
                data = questData,
                isComplete = false,
                progressList = new QuestProgress[progressLength]
            };

            for (int i = 0; i < newQuest.progressList.Length; i++)
            {
                newQuest.progressList[i] = new QuestProgress();
            }

            questList.Add(newQuest);
        }

        public void AddCurrentQuest(string questTitle)
        {
            Quest quest = FindQuest(questTitle);
            if (quest != null)
            {
                Debug.Log("This Quest is already in the list!");
                return;
            }

            foreach (QuestData questData in questLibrary)
            {
                if (questData.Title != questTitle) continue;
                int progressLength = questData.ObjectiveList.Count;
                Quest newQuest = new Quest
                {
                    data = questData,
                    isComplete = false,
                    progressList = new QuestProgress[progressLength]
                };

                for (int i = 0; i < newQuest.progressList.Length; i++)
                {
                    newQuest.progressList[i] = new QuestProgress();
                }

                questList.Add(newQuest);
                return;
            }

            Debug.LogWarning("Can't find this quest inside library : " + questTitle);
        }

        public void RemoveCurrentQuest(Quest quest)
        {
            if (quest != null) questList.Remove(quest);
            else Debug.LogError("Removing Quest Failed! Can't find : " + quest.data.Title);
        }

        public void RemoveCurrentQuest(QuestData questData)
        {
            Quest quest = FindQuest(questData);
            RemoveCurrentQuest(quest);
        }

        public void RemoveCurrentQuest(string questTitle)
        {
            Quest quest = FindQuest(questTitle);
            RemoveCurrentQuest(quest);
        }

        public QuestObjective GetCurrentObjective(Quest quest)
        {
            if (quest == null)
            {
                Debug.Log("Can't find quest");
                return null;
            }

            int currentObjectiveIndex = -1;

            for (int i = 0; i < quest.progressList.Length; i++)
            {
                if (quest.progressList[i].isComplete) continue;
                currentObjectiveIndex = i;
                break;
            }

            if (currentObjectiveIndex < 0)
            {
                Debug.LogError("Can't find current objective!");
                return null;
            }

            return quest.data.ObjectiveList[currentObjectiveIndex];
        }

        public QuestObjective GetCurrentObjective(QuestData questData)
        {
            Quest quest = FindQuest(questData);
            return GetCurrentObjective(quest);
        }

        public QuestObjective GetCurrentObjective(string questTitle)
        {
            Quest quest = FindQuest(questTitle);
            return GetCurrentObjective(quest);
        }

        public void CompleteObjectiveQuest(Quest quest, int objectiveIndex)
        {
            int ObjectiveLength = quest.data.ObjectiveList.Count;

            if (objectiveIndex >= ObjectiveLength || objectiveIndex < 0)
            {
                Debug.LogWarning("Index is out of objective list length!");
                return;
            }

            int objectiveQuantity = quest.data.ObjectiveList[objectiveIndex].Quantity;
            quest.progressList[objectiveIndex].progressQuantity = objectiveQuantity;
            quest.progressList[objectiveIndex].isComplete = true;
        }

        public void CompleteObjectiveQuest(QuestData questData, int objectiveIndex)
        {
            Quest quest = FindQuest(questData);

            if (quest == null)
            {
                Debug.LogWarning($"'{questData.Title}' is not in the Quest List!");
                return;
            }

            CompleteObjectiveQuest(quest, objectiveIndex);
        }

        public void CompleteObjectiveQuest(string questTitle, int objectiveIndex)
        {
            Quest quest = FindQuest(questTitle);

            if (quest == null)
            {
                Debug.LogWarning($"'{questTitle}' is not in the Quest List!");
                return;
            }

            CompleteObjectiveQuest(quest, objectiveIndex);
        }

        public void CompleteTheQuest(Quest quest)
        {
            int ObjectiveLength = quest.data.ObjectiveList.Count;

            for (int i = 0; i < ObjectiveLength; i++)
            {
                int objectiveQuantity = quest.data.ObjectiveList[i].Quantity;
                quest.progressList[i].progressQuantity = objectiveQuantity;
                quest.progressList[i].isComplete = true;
            }

            quest.isComplete = true;
        }

        public void CompleteTheQuest(QuestData questData)
        {
            Quest quest = FindQuest(questData);

            if (quest == null)
            {
                Debug.LogWarning($"'{questData.Title}' is not in the Quest List!");
                return;
            }

            CompleteTheQuest(quest);
        }

        public void CompleteTheQuest(string questTitle)
        {
            Quest quest = FindQuest(questTitle);

            if (quest == null)
            {
                Debug.LogWarning($"'{questTitle}' is not in the Quest List!");
                return;
            }

            CompleteTheQuest(quest);
        }

        public void CheckObjectiveTypeTalk(string npcName)
        {
            foreach (Quest quest in questList)
            {
                if (quest.isComplete) continue;

                int objectivelength = quest.data.ObjectiveList.Count;
                for (int i = 0; i < objectivelength; i++)
                {
                    QuestObjective objective = quest.data.ObjectiveList[i];

                    if (quest.progressList[i].isComplete) continue;
                    if (objective.Type != QuestObjectiveType.Talk) continue;
                    Debug.Log("Quest Target : " + objective.Target);
                    if (objective.Target != npcName) continue;

                    Debug.Log(quest.data.Title + " : is Complete!");
                    quest.progressList[i].isComplete = true;
                    return;
                }
            }

            Debug.Log("No quest with type talk for this npc.");
        }

        public bool HasQuest(QuestData questData)
        {
            Quest checkQuest = FindQuest(questData);

            if (checkQuest != null) return true;
            else return false;
        }

        public bool HasQuest(string questTitle)
        {
            Quest checkQuest = FindQuest(questTitle);

            if (checkQuest != null) return true;
            else return false;
        }

        public Quest FindQuest(QuestData questData)
        {
            foreach (Quest quest in questList)
            {
                if (quest.data != questData) continue;
                return quest;
            }

            return null;
        }

        public Quest FindQuest(string questTitle)
        {
            foreach (Quest quest in questList)
            {
                if (quest.data.Title != questTitle) continue;
                return quest;
            }

            return null;
        }

        public void RefreshQuestLibrary()
        {
            List<QuestData> questLibrary = new List<QuestData>();

            QuestData[] questDataAssets = AssetDatabase.FindAssets("t:QuestData", new[] { questDataDirectory })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<QuestData>)
                .ToArray();

            questLibrary.AddRange(questDataAssets);
            SetQuestLibrary(questLibrary);

            List<string> questList = new List<string>();
            foreach (QuestData questData in questDataAssets)
            {
                string quesType = questData.Type.ToString();
                string questTitle = questData.Title;
                string quest = quesType + "/" + questTitle;
                questList.Add(quest);
            }
            questArray = questList.ToArray();

            Debug.Log("Quest List Refreshed!");
        }

        public string[] GetQuestArray() => questArray;
        public bool IsGameStarted() => gameStarted;

        public List<QuestData> GetQuestLibrary() => questLibrary;
        public void SetQuestLibrary(List<QuestData> newQuestLibrary) => questLibrary = newQuestLibrary;

        public List<Quest> GetQuestList() => questList;
        public void SetQuestList(List<Quest> newQuestList) => questList = newQuestList;
    }
}