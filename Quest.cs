using System;

namespace ZaiR37.Quest
{
    [Serializable]
    public class Quest
    {
        public QuestData data;
        public QuestProgress[] progressList;
        public bool isComplete;
    }
}