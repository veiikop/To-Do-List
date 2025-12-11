using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using To_Do_List.Models;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;

namespace To_Do_List.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtConfiguration _jwtSettings;
        private readonly IUserRepository _userRepository;

        public AuthService(IOptions<JwtConfiguration> jwtSettings, IUserRepository userRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
        }

        public AuthResponseDTO Login(LoginRequestDTO loginRequest)
        {
            try
            {
                var user = _userRepository.GetByEmailOrUsername(loginRequest.EmailOrUsername);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Пользователь не найден"
                    };
                }

                if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Неверный пароль"
                    };
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Сохраняем refresh token в базу
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpirateAtInDays);
                _userRepository.Update(user); // Нужно добавить метод Update в IUserRepository

                return new AuthResponseDTO
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken, // Добавляем refresh token
                    ValidTo = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirateAtInMinutes),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Ошибка при входе: {ex.Message}"
                };
            }
        }

        public AuthResponseDTO Register(RegisterRequestDTO registerRequest)
        {
            try
            {
                if (_userRepository.ExistsByEmail(registerRequest.Email))
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Пользователь с таким email уже существует"
                    };
                }

                if (_userRepository.ExistsByUsername(registerRequest.Username))
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Пользователь с таким именем уже существует"
                    };
                }

                var newUser = new User
                {
                    Username = registerRequest.Username,
                    Email = registerRequest.Email,
                    PasswordHash = HashPassword(registerRequest.Password),
                    Role = registerRequest.Role,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = _userRepository.Create(newUser);
                var token = GenerateJwtToken(createdUser);
                var refreshToken = GenerateRefreshToken();

                // Сохраняем refresh token
                createdUser.RefreshToken = refreshToken;
                createdUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); 
                _userRepository.Update(createdUser);

                return new AuthResponseDTO
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken, // Добавляем refresh token
                    ValidTo = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirateAtInMinutes),
                    User = new UserDTO
                    {
                        Id = createdUser.Id,
                        Username = createdUser.Username,
                        Email = createdUser.Email,
                        Role = createdUser.Role
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Ошибка при регистрации: {ex.Message}"
                };
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirateAtInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
        // Метод для генерации refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // Метод для обновления токенов
        public AuthResponseDTO RefreshToken(string token, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var user = _userRepository.GetById(userId);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Пользователь не найден"
                    };
                }

                if (user.RefreshToken != refreshToken)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Неверный refresh token"
                    };
                }

                if (user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        ErrorMessage = "Refresh token истек"
                    };
                }

                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                _userRepository.Update(user);

                return new AuthResponseDTO
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ValidTo = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirateAtInMinutes),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role
                    }
                };
            }
            catch (SecurityTokenException ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Неверный токен: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Ошибка при обновлении токена: {ex.Message}"
                };
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false // отключаем проверку срока действия
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException($"Token validation failed: {ex.Message}");
            }
        }
    }
}
