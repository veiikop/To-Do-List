namespace To_Do_List.Models.DTO
{
    public class TodoListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TodoItemDTO> Items { get; set; } = new();
    }
}
