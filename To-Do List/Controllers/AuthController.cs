using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models.DTO;
using To_Do_List.Services;

namespace To_Do_List.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<AuthResponseDTO> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "некорректные данные"
                    });
                }

                var result = _authService.Register(registerRequest);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("пользователь {Username} успешно зарегистрирован", registerRequest.Username);
                return CreatedAtAction(nameof(Register), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ошибка при регистрации пользователя");
                return StatusCode(500, new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = "внутренняя ошибка сервера"
                });
            }
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponseDTO> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "некорректные данные"
                    });
                }

                var result = _authService.Login(loginRequest);

                if (!result.Success)
                {
                    _logger.LogWarning("неудачная попытка входа для {EmailOrUsername}", loginRequest.EmailOrUsername);
                    return Unauthorized(result);
                }

                _logger.LogInformation("пользователь {EmailOrUsername} успешно вошел в систему", loginRequest.EmailOrUsername);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ошибка при входе пользователя");
                return StatusCode(500, new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = "внутренняя ошибка сервера"
                });
            }
        }

        /// <summary>
        /// Обновление токена
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponseDTO> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var result = _authService.RefreshToken(request.Token, request.RefreshToken);

                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении токена");
                return StatusCode(500, new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = "Внутренняя ошибка сервера"
                });
            }
        }
    }
}