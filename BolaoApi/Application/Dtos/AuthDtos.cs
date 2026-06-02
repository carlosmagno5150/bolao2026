namespace BolaoApi.Application.Dtos;

public record RegisterRequestDto(string Name, string Email, string Password);

public record AuthenticateRequestDto(string Email, string Password);

public record AuthenticateResponseDto(string Token);

public record RegisterResponseDto(string Message, Guid UserId);
