using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models.DTO
{
    public class UpdateTodoItemDTO
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public Priority? Priority { get; set; }

        public bool? IsCompleted { get; set; }
    }
}