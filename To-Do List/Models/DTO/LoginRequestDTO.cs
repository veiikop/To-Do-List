using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email или имя пользователя обязательно")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}
