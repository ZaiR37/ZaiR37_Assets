

namespace ZaiR37.Quest
{
    using System;
    
    [Serializable]
    public class QuestObjective
    {
        public QuestObjectiveType Type;
        public string Description;
        public string Target;
        public int Quantity;
    }
}