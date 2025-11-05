using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models.DTO
{
    public class CreateTodoItemDTO
    {
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200, ErrorMessage = "Название до 200 символов")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Описание до 1000 символов")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;

        [Required(ErrorMessage = "Укажите список")]
        [Range(1, int.MaxValue, ErrorMessage = "ID списка должен быть больше 0")]
        public int TodoListId { get; set; }
    }
}
