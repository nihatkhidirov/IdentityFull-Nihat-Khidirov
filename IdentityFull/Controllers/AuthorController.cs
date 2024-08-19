using FullIdentity.Data.Entities;
using FullIdentity.Dtos.UserDtos;
using FullIdentity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullIdentity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(SignUpDto signUpDto)
    {
        AppUser user = new() { FullName = signUpDto.Fullname, Email = signUpDto.Email, UserName = signUpDto.Username };
        IdentityResult result = await _userManager.CreateAsync(user, signUpDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        await _userManager.AddToRoleAsync(user, "member");
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null) return BadRequest();

        bool doesPasswordMatch = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!doesPasswordMatch) return BadRequest();

        if (!user.EmailConfirmed) return BadRequest();

        string token = _tokenService.GenerateJWTToken(user);
        return Ok(token);
    }

    [HttpPatch("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return NotFound();
        await _userManager.ConfirmEmailAsync(appUser, token);
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return NotFound();
        string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
        return Ok(token);
    }

    [HttpPut]
    public async Task<IActionResult> ResetPassword(string token, string email, string password, string confirmPassword)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return NotFound();
        if (password != confirmPassword) return BadRequest("Passwords do not match");
        var result = await _userManager.ResetPasswordAsync(appUser, token, password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        await _userManager.UpdateSecurityStampAsync(appUser);
        return NoContent();
    }
}
