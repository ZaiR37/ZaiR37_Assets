
namespace ZaiR37.Quest
{
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [SerializeField] private List<Quest> questList;
        [SerializeField] private List<QuestData> questLibrary;
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
            string npcName = "Gabriel Maier";
            for (int i = 0; i < questLibrary.Count; i++)
            {
                AddCurrentQuest(questLibrary[i].Title);
            }

            CheckObjectiveTypeTalk(npcName);
            CheckObjectiveTypeTalk(npcName);
        }

        public void AddCurrentQuest(string questTitle)
        {
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

            Debug.LogWarning("Adding Quest Failed! Can't find quest title : " + questTitle);
        }

        public void RemoveCurrentQuest(string questTitle)
        {
            foreach (Quest quest in questList)
            {
                if (quest.data.Title != questTitle) continue;
                questList.Remove(quest);
                return;
            }

            Debug.LogWarning("Removing Quest Failed! Can't find quest title : " + questTitle);
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

                    if (objective.type != QuestObjectiveType.Talk) continue;
                    Debug.Log("Quest Target : " + objective.target);

                    if (objective.target != npcName) continue;

                    Debug.Log(quest.data.Title + " : is Complete!");
                    quest.progressList[i].isComplete = true;
                    return;
                }

            }

            Debug.Log("No quest with type talk for this npc.");
        }

    }
}