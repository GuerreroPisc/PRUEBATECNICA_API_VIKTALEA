namespace ApiVIKTALEA.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Rol { get; set; } = string.Empty;
}
