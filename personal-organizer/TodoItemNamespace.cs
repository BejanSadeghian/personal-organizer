namespace TodoItemNamespace
{
    public class TodoItem
    {
        public Guid UUID { get; set; }
        public string UUID_LastSix { get; set; }
        public string Description { get; set; }
        public float Importance { get; set; }
        public float Urgency { get; set; }
        public float Composite { get; set; }

        public TodoItem()
        {

        }

        public TodoItem(string description, float importance, float urgency)
        {
            UUID = Guid.NewGuid();
            UUID_LastSix = UUID.ToString().Substring(UUID.ToString().Length - 6);
            Description = description;
            Importance = importance;
            Urgency = urgency;
            Composite = (importance + urgency) / 2;
        }

        public override String ToString()
        {
            return $"{Description}\n\t" +
                $"id: {UUID_LastSix}\t" +
                $"importance: {Importance}\t" +
                $"urgency: {Urgency}\t" +
                $"composite score: {Composite}";
        }
    }
}

