namespace To_Do_List.Models.DTO
{
    public class TodoItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Priority Priority { get; set; }
        public int TodoListId { get; set; }
    }
}
