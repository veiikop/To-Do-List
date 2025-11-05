using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models.DTO
{
    public class CreateTodoListDTO
    {
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, ErrorMessage = "Название до 100 символов")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Описание до 500 символов")]
        public string? Description { get; set; }
    }
}
