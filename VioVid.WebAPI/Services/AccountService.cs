using System.Security.Claims;
using System.Web;
using Application.DTOs.Account;
using Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using VioVid.Core.Entities;
using VioVid.Core.Identity;
using VioVid.Core.ServiceContracts;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    // private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly IJwtService _jwtService;
    private readonly IEmailSender _emailSender;

    private readonly IConfiguration _configuration;

    public AccountService(
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager, 
        SignInManager<ApplicationUser> signInManager, 
        IJwtService jwtService, IEmailSender emailSender, 
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        // _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _emailSender = emailSender;
        _configuration = configuration;
    }
    
    public async Task<Guid> Register(RegisterRequest registerRequest)
    {
        // Email is registered
        if (_userManager.Users.Any(u => u.Email == registerRequest.Email))
        {
            throw new InvalidModelException("Email đã tồn tại");
        }
        
        ApplicationUser user = new()
        {
            Email = registerRequest.Email,
            UserName = registerRequest.Email,   // UserName is used to log in
            UserProfile = new UserProfile()
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.Name,
                Avatar = "https://kpaxjjmelbqpllxenpxz.supabase.co/storage/v1/object/public/avatar/default_avt.png",
            }
        };
        
        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        
        if (result.Succeeded)
        {
            var verifyEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyEmailTokenEncoded = HttpUtility.UrlEncode(verifyEmailToken);
            Console.WriteLine("verifyEmailToken = " + verifyEmailToken);

            var confirmLink = $"{_configuration["VioVidDomain"]}/#/confirm-email?email={registerRequest.Email}&verifyEmailToken={verifyEmailTokenEncoded}";
            // await _emailSender.SendMailAsync(registerDTO.Email, "[VioVid] Please confirm your email address", EmailTemplate.ConfirmEmail(registerDTO.Name, redirectTo: confirmLink));
            
            return user.Id;
        }
        else
        {
            var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); // error1 | error2
            throw new InvalidModelException(errorMessage);
        }
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, isPersistent: false, lockoutOnFailure: false);
        ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);
            
        if (result.Succeeded)
        {
            string accessToken = _jwtService.GenerateAccessToken(user);
            (string refreshToken, DateTime expirationDateTime) = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            
            user.RefreshTokenExpirationDateTime = expirationDateTime;
            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        else
        {
            if (result.IsNotAllowed)
            {
                throw new InvalidModelException("Đăng nhập thất bại. Email chưa được xác thực.");
            }
            throw new InvalidModelException("Đăng nhập thất bại. Vui lòng kiểm tra lại Email và Mật Khẩu.");
        }
    }

    public async Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(confirmEmailRequest.Email);

        Console.WriteLine("verifyEmailToken = " + confirmEmailRequest.VerifyEmailToken);
        var result = await _userManager.ConfirmEmailAsync(user, confirmEmailRequest.VerifyEmailToken);
        return result.Succeeded;
    }

    public async Task<RefreshTokenDto> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        ClaimsPrincipal? principal;
        try
        {
            principal = _jwtService.GetPrincipalFromJwtToken(refreshTokenDto.AccessToken);
        }
        catch (Exception ex)
        {
            throw new InvalidModelException(ex.Message);
        }

        string email = principal.FindFirstValue(ClaimTypes.Email);
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
        {
            throw new InvalidModelException("RefreshToken không hợp lệ hoặc đã quá hạn.");
        }

        string accessToken = _jwtService.GenerateAccessToken(user);
        (string refreshToken, DateTime expirationDateTime) = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDateTime = expirationDateTime;
        await _userManager.UpdateAsync(user);

        return new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<bool> Logout(ClaimsPrincipal userPrincipal)
    {
        string email = userPrincipal.FindFirstValue(ClaimTypes.Email);

        ApplicationUser user = await _userManager.FindByEmailAsync(email);
        user.RefreshToken = null;
        user.RefreshTokenExpirationDateTime = null;
        user.TokenVersion++;
        await _userManager.UpdateAsync(user);
        
        // Chưa rõ tác dụng của:
        await _signInManager.SignOutAsync();

        return true;
    }
}