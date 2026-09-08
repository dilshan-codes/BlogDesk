namespace BlogDesk.Api.DTOs;

public record RequestOtpRequest(string Email);
public record VerifyOtpRequest(string Email, string Code);
public record AdminSessionResponse(string Token, DateTime ExpiresAt);