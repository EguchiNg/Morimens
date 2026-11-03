using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Morimens.Api.DTOs.Requests;
using Morimens.Api.DTOs.Responses;
using Morimens.Api.Services;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Infrastructure.Identity;

namespace Morimens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser>  _users;
    private readonly IRefreshTokenRepository       _refreshTokens;
    private readonly IUserProfileRepository        _profiles;
    private readonly JwtService                    _jwt;

    public AuthController(
        UserManager<ApplicationUser>  users,
        IRefreshTokenRepository       refreshTokens,
        IUserProfileRepository        profiles,
        JwtService                    jwt)
    {
        _users         = users;
        _refreshTokens = refreshTokens;
        _profiles      = profiles;
        _jwt           = jwt;
    }

    // ── POST /api/auth/register ───────────────────────────────────────────────
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        var user = new ApplicationUser
        {
            UserName = req.Username,
            Email    = req.Email,
        };

        var result = await _users.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // Create profile
        var profile = new UserProfile
        {
            UserId      = user.Id,
            DisplayName = req.DisplayName ?? req.Username,
            Settings    = new UserSettings { UserId = user.Id },
        };
        await _profiles.CreateAsync(profile);

        return Ok(await BuildAuthResponse(user));
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        // Support email or username
        var user = req.Login.Contains('@')
            ? await _users.FindByEmailAsync(req.Login)
            : await _users.FindByNameAsync(req.Login);

        if (user is null || !await _users.CheckPasswordAsync(user, req.Password))
            return Unauthorized("Invalid credentials.");

        return Ok(await BuildAuthResponse(user));
    }

    // ── POST /api/auth/refresh ────────────────────────────────────────────────
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest req)
    {
        var rt = await _refreshTokens.GetActiveAsync(req.RefreshToken);
        if (rt is null) return Unauthorized("Invalid or expired refresh token.");

        var user = await _users.FindByIdAsync(rt.UserId);
        if (user is null) return Unauthorized();

        // Rotate: revoke old, issue new
        await _refreshTokens.RevokeAsync(rt.Token);
        return Ok(await BuildAuthResponse(user));
    }

    // ── POST /api/auth/logout ─────────────────────────────────────────────────
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
    {
        await _refreshTokens.RevokeAsync(req.RefreshToken);
        return NoContent();
    }

    // ── GET /api/auth/discord ─────────────────────────────────────────────────
    [HttpGet("discord")]
    public IActionResult Discord()
    {
        var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(DiscordCallback), "Auth", null, Request.Scheme)
        };
        return Challenge(props, "Discord");
    }

    // ── GET /api/auth/discord/callback ────────────────────────────────────────
    [HttpGet("discord/callback")]
    public async Task<IActionResult> DiscordCallback()
    {
        var result = await HttpContext.AuthenticateAsync("Discord");
        if (!result.Succeeded) return BadRequest("Discord authentication failed.");

        var discordId    = result.Principal!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var discordEmail = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var discordName  = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        if (discordId is null) return BadRequest("Could not read Discord user info.");

        // Find existing user linked to this Discord account
        var user = _users.Users.FirstOrDefault(u => u.DiscordId == discordId);

        if (user is null)
        {
            // Create a new account linked to Discord
            user = new ApplicationUser
            {
                UserName  = discordName ?? $"discord_{discordId}",
                Email     = discordEmail,
                DiscordId = discordId,
                EmailConfirmed = true,
            };
            var createResult = await _users.CreateAsync(user);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors.Select(e => e.Description));

            await _profiles.CreateAsync(new UserProfile
            {
                UserId      = user.Id,
                DisplayName = discordName ?? user.UserName ?? "Discord User",
                AvatarUrl   = null,
                Settings    = new UserSettings { UserId = user.Id },
            });
        }

        var auth = await BuildAuthResponse(user);

        // Redirect the browser to the React app with tokens in the fragment
        // (avoids tokens appearing in server logs)
        var frontendUrl = HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Frontend:Url"] ?? "http://localhost:5173";

        return Redirect($"{frontendUrl}/auth/callback#access_token={auth.AccessToken}&refresh_token={auth.RefreshToken}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<AuthResponse> BuildAuthResponse(ApplicationUser user)
    {
        var (accessToken, expiresIn) = _jwt.GenerateAccessToken(user);
        var refreshToken = await _refreshTokens.CreateAsync(user.Id, TimeSpan.FromDays(30));

        return new AuthResponse
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn    = expiresIn,
            UserId       = user.Id,
            Username     = user.UserName ?? "",
            Email        = user.Email    ?? "",
        };
    }
}
