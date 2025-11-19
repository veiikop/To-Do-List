using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;

namespace To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Получить профиль текущего пользователя
        /// </summary>
        [HttpGet("profile")]
        [Authorize] // Доступно всем аутентифицированным
        public ActionResult<UserDTO> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = _userRepository.GetById(userId);
            if (user == null) return NotFound();

            return Ok(new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            });
        }

        /// <summary>
        /// Получить список всех пользователей (только Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = _userRepository.GetAll().Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });

            return Ok(users);
        }
    }
}