
namespace ZaiR37.Quest
{
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [SerializeField] private List<QuestData> questLibrary;
        private List<Quest> questList;

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

            foreach (var questData in questLibrary)
            {
                AddToCurrentQuest(questData);
            }
        }

        public void AddToCurrentQuest(QuestData questData)
        {
            Quest quest = FindThisQuest(questData);
            if (quest != null)
            {
                Debug.Log("This Quest is already in the list!");
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

        public void AddToCurrentQuest(string questTitle)
        {
            Quest quest = FindThisQuest(questTitle);
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

        public void RemoveCurrentQuest(string questTitle)
        {
            Quest quest = FindThisQuest(questTitle);

            if (quest != null) questList.Remove(quest);
            else Debug.LogWarning("Removing Quest Failed! Can't find : " + questTitle);
        }

        public void RemoveCurrentQuest(QuestData questData)
        {
            Quest quest = FindThisQuest(questData);

            if (quest != null) questList.Remove(quest);
            else Debug.LogWarning("Removing Quest Failed! Can't find : " + questData.Title);

        }

        public void CompleteObjectiveQuest()
        {
            foreach (Quest quest in questList)
            {

            }
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

        public bool HasThisQuest(QuestData questData)
        {
            Quest checkQuest = FindThisQuest(questData);

            if (checkQuest != null) return true;
            else return false;
        }

        public bool HasThisQuest(string questTitle)
        {
            Quest checkQuest = FindThisQuest(questTitle);

            if (checkQuest != null) return true;
            else return false;
        }

        public Quest FindThisQuest(string questTitle)
        {
            foreach (Quest quest in questList)
            {
                if (quest.data.Title != questTitle) continue;
                return quest;
            }

            return null;
        }

        public Quest FindThisQuest(QuestData questData)
        {
            foreach (Quest quest in questList)
            {
                if (quest.data != questData) continue;
                return quest;
            }

            return null;
        }
    
        
        public List<QuestData> GetQuestLibrary() => questLibrary;
        public void SetQuestLibrary(List<QuestData> newQuestLibrary) => questLibrary = newQuestLibrary;
        
        public List<Quest> GetQuestList() => questList;
        public void SetQuestList(List<Quest> newQuestList) => questList = newQuestList;
    }
}