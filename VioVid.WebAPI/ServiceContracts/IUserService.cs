using Application.DTOs.User.Res;

namespace VioVid.WebAPI.ServiceContracts;

public interface IUserService
{
    Task<UserProfileResponse> GetUserProfileAsync();
}