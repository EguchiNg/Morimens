namespace Morimens.Api.DTOs.Responses;

public class AuthResponse
{
    public string AccessToken  { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public int    ExpiresIn    { get; set; }  // seconds until access token expires
    public string UserId       { get; set; } = "";
    public string Username     { get; set; } = "";
    public string Email        { get; set; } = "";
}

public class UserProfileResponse
{
    public string  UserId      { get; set; } = "";
    public string  Username    { get; set; } = "";
    public string  Email       { get; set; } = "";
    public string  DisplayName { get; set; } = "";
    public string? AvatarUrl   { get; set; }
    public bool    HasDiscord  { get; set; }
    public UserSettingsResponse Settings { get; set; } = new();
}

public class UserSettingsResponse
{
    public string  Theme                 { get; set; } = "dark";
    public int?    DefaultAwakenerId     { get; set; }
    public decimal DefaultCritMultiplier { get; set; } = 1.5m;
    public bool    DefaultIsCritical     { get; set; }
}

public class SavedSimulationResponse
{
    public int    Id        { get; set; }
    public string Name      { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public string UpdatedAt { get; set; } = "";

    /// <summary>Raw deserialized JSON objects — client handles typing.</summary>
    public object? Inputs   { get; set; }
    public object? Result   { get; set; }
}
