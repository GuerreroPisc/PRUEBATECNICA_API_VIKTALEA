using ApiVIKTALEA.DTOs;

namespace ApiVIKTALEA.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
