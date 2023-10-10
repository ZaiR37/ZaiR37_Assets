namespace ZaiR37.Quest
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Quest", menuName = "Battle/Quest")]
    public class QuestData : ScriptableObject
    {
        private string title;                    // The Quest's Name.
        private QuestType type;                  // Quest Type - Main, Side, or Commission.
        private string from;                     // Quest Provider's Name.
        private string location;                 // Location of the Quest Objective.
        private string description;              // Quest Description.
        private bool orderly;                    // Completing objectives in a specific sequence.
        private List<QuestObjective> objectives; // List of objective to Complete the Quest.
        private List<QuestReward> rewards;       // Rewards you receive upon completing the quest.
        private Texture2D image;                 // Quest Image Displaying Objective Location.

        public string Title => title;
        public QuestType Type => type;
        public string From => from;
        public string Location => location;
        public string Description => description;
        public bool Orderly => orderly;
        public List<QuestObjective> ObjectiveList => objectives;
        public List<QuestReward> RewardList => rewards;
        public Texture2D Image => image;

        public void SetTitle(string newTitle) => title = newTitle;
        public void SetType(QuestType newType) => type = newType;
        public void SetFrom(string newNpc) => from = newNpc;
        public void SetLocation(string newLocation) => location = newLocation;
        public void SetDescription(string newDescription) => description = newDescription;
        public void SetOrderly(bool newOrderly) => orderly = newOrderly;
        public void SetObjectiveList(List<QuestObjective> newObjectiveList) => objectives = newObjectiveList;
        public void SetQuestRewardList(List<QuestReward> newRewardList) => rewards = newRewardList;
        public void SetImage(Texture2D newImage) => image = newImage;

    }
}