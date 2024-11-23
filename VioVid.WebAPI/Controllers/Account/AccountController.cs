using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Identity;
using VioVid.Core.ServiceContracts;

namespace VioVid.WebAPI.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        
        private readonly IConfiguration _configuration;
        
        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IJwtService jwtService, 
            IEmailSender emailSender, 
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _configuration = configuration;
        }
    }
}

