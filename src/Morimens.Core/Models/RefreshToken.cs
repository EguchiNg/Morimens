namespace Morimens.Core.Models;

/// <summary>
/// Persisted refresh token. One active token per user session.
/// Invalidated on use (rotation) or on logout.
/// </summary>
public class RefreshToken
{
    public int      Id          { get; set; }
    public string   UserId      { get; set; } = "";
    public string   Token       { get; set; } = "";
    public DateTime ExpiresAt   { get; set; }
    public bool     IsRevoked   { get; set; } = false;
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
}
