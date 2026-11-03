using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Morimens.Api.DTOs.Requests;
using Morimens.Api.DTOs.Responses;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;
using Morimens.Infrastructure.Identity;

namespace Morimens.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IUserProfileRepository      _profiles;
    private readonly UserManager<ApplicationUser> _users;

    public ProfileController(IUserProfileRepository profiles, UserManager<ApplicationUser> users)
    {
        _profiles = profiles;
        _users    = users;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException("No user ID in token.");

    // ── GET /api/profile ──────────────────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<UserProfileResponse>> Get()
    {
        var user    = await _users.FindByIdAsync(UserId);
        if (user is null) return NotFound();

        var profile = await _profiles.GetByUserIdAsync(UserId);
        if (profile is null) return NotFound();

        return Ok(MapProfile(user, profile));
    }

    // ── PATCH /api/profile ────────────────────────────────────────────────────
    [HttpPatch]
    public async Task<ActionResult<UserProfileResponse>> Update([FromBody] UpdateProfileRequest req)
    {
        var user    = await _users.FindByIdAsync(UserId);
        var profile = await _profiles.GetByUserIdAsync(UserId);
        if (user is null || profile is null) return NotFound();

        if (req.DisplayName is not null) profile.DisplayName = req.DisplayName;
        if (req.AvatarUrl   is not null) profile.AvatarUrl   = req.AvatarUrl;

        await _profiles.UpdateAsync(profile);
        return Ok(MapProfile(user, profile));
    }

    // ── GET /api/profile/settings ─────────────────────────────────────────────
    [HttpGet("settings")]
    public async Task<ActionResult<UserSettingsResponse>> GetSettings()
    {
        var settings = await _profiles.GetSettingsAsync(UserId);
        return Ok(MapSettings(settings));
    }

    // ── PUT /api/profile/settings ─────────────────────────────────────────────
    [HttpPut("settings")]
    public async Task<ActionResult<UserSettingsResponse>> UpdateSettings([FromBody] UpdateSettingsRequest req)
    {
        var current = await _profiles.GetSettingsAsync(UserId) ?? new UserSettings { UserId = UserId };

        if (req.Theme                 is not null) current.Theme                 = req.Theme;
        if (req.DefaultAwakenerId     is not null) current.DefaultAwakenerId     = req.DefaultAwakenerId;
        if (req.DefaultCritMultiplier is not null) current.DefaultCritMultiplier = req.DefaultCritMultiplier.Value;
        if (req.DefaultIsCritical     is not null) current.DefaultIsCritical     = req.DefaultIsCritical.Value;

        var saved = await _profiles.UpsertSettingsAsync(current);
        return Ok(MapSettings(saved));
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static UserProfileResponse MapProfile(ApplicationUser user, UserProfile profile) => new()
    {
        UserId      = user.Id,
        Username    = user.UserName ?? "",
        Email       = user.Email    ?? "",
        DisplayName = profile.DisplayName,
        AvatarUrl   = profile.AvatarUrl,
        HasDiscord  = user.DiscordId is not null,
        Settings    = MapSettings(profile.Settings),
    };

    private static UserSettingsResponse MapSettings(UserSettings? s) => s is null ? new() : new()
    {
        Theme                 = s.Theme,
        DefaultAwakenerId     = s.DefaultAwakenerId,
        DefaultCritMultiplier = s.DefaultCritMultiplier,
        DefaultIsCritical     = s.DefaultIsCritical,
    };
}
