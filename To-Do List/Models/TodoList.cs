namespace To_Do_List.Models
{
    public class TodoList
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
    }
}
