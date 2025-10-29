namespace To_Do_List.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        public int TodoListId { get; set; }

        public TodoList TodoList { get; set; } = null!;
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
}

