namespace To_Do_List
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirateAtInMinutes { get; set; }
    }
}
