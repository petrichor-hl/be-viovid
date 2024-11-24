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
    
    public async Task<Guid> Register(RegisterDto registerDto)
    {
        // Email is registered
        if (_userManager.Users.Any(u => u.Email == registerDto.Email))
        {
            throw new InvalidModelException("Email đã tồn tại");
        }
        
        ApplicationUser user = new()
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,   // UserName is used to log in
            UserProfile = new UserProfile()
            {
                Id = Guid.NewGuid(),
                Name = registerDto.Name,
                Avatar = "https://kpaxjjmelbqpllxenpxz.supabase.co/storage/v1/object/public/avatar/default_avt.png",
            }
        };
        
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        
        if (result.Succeeded)
        {
            var verifyEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyEmailTokenEncoded = HttpUtility.UrlEncode(verifyEmailToken);
            Console.WriteLine("verifyEmailToken = " + verifyEmailToken);

            var confirmLink = $"{_configuration["VioVidDomain"]}/#/confirm-email?email={registerDto.Email}&verifyEmailToken={verifyEmailTokenEncoded}";
            // await _emailSender.SendMailAsync(registerDTO.Email, "[VioVid] Please confirm your email address", EmailTemplate.ConfirmEmail(registerDTO.Name, redirectTo: confirmLink));
            
            return user.Id;
        }
        else
        {
            var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); // error1 | error2
            throw new InvalidModelException(errorMessage);
        }
    }
}
