namespace To_Do_List.Models.DTO
{
    public class RefreshTokenRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
