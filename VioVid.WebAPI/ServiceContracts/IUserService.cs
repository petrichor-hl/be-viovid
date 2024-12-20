using Application.DTOs.Film.Res;
using Application.DTOs.User.Req;
using Application.DTOs.User.Res;

namespace VioVid.WebAPI.ServiceContracts;

public interface IUserService
{
    Task<UserProfileResponse> GetUserProfileAsync();

    Task<List<SimpleFilmResponse>> GetMyListAsync();
    
    Task<SimpleFilmResponse> AddFilmToMyListAsync(AddFilmToMyListRequest addFilmToMyListRequest);
    
    Task<Guid> RemoveFilmFromMyListByFilmIdAsync(Guid filmId);
    
    Task<List<TrackingProgressResponse>> GetTrackingProgressAsync();
    
    Task<bool> UpdateTrackingProgressAsync(UpdateTrackingProgressRequest updateTrackingProgressRequest);
    
    // Just ignore this method
    Task<bool> AddUserPayment(AddUserPaymentRequest addUserPaymentRequest);
    
    Task<List<UserPlanResponse>> GetUserPayments();
}