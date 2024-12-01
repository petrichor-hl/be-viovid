using Application.DTOs.User.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Enum;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<UserProfileResponse> GetUserProfileAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        }
        
        var userIdClaim = user.FindFirst("user-id");
        if (userIdClaim == null)
        {
            throw new Exception("Không tìm thấy claim 'user-id' trong AccessToken.");
        }
        var applicationUserId = Guid.Parse(userIdClaim.Value);
        
        // Tìm người dùng từ cơ sở dữ liệu
        var applicationUser = await _dbContext.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserPlans) // Eager loading UserPlans
                .ThenInclude(userPlans => userPlans.Plan)
            .FirstOrDefaultAsync(u => u.Id == applicationUserId);
        
        if (applicationUser == null)
        {
            throw new NotFoundException("Người dùng không tồn tại.");
        }

        var response = new UserProfileResponse
        {
            ApplicationUserId = applicationUserId,
            Name = applicationUser.UserProfile.Name,
            Email = applicationUser.Email,
            Avatar = applicationUser.UserProfile.Avatar,
        };

        var latestUserPlan = applicationUser.UserPlans.OrderBy(userPlan => userPlan.StartDate).LastOrDefault();
        if (latestUserPlan == null || latestUserPlan.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            response.PlanName = "Normal";
            return response;
        }

        response.PlanName = latestUserPlan.Plan.Name;
        response.StartDate = latestUserPlan.StartDate;
        response.EndDate = latestUserPlan.EndDate;
        return response;
    }
}