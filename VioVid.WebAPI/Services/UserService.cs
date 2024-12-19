using Application.DTOs.Account;
using Application.DTOs.Film.Res;
using Application.DTOs.User.Req;
using Application.DTOs.User.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
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
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);
        
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
            FcmToken = applicationUser.FcmToken,
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

    public async Task<List<SimpleFilmResponse>> GetMyListAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        return await _dbContext.MyFilms
            .Where(myFilm => myFilm.ApplicationUserId == applicationUserId)
            .Include(myFilm => myFilm.Film)
            .Select(myFilm => new SimpleFilmResponse()
            {
                FilmId = myFilm.FilmId,
                Name = myFilm.Film.Name,
                PosterPath = myFilm.Film.PosterPath,
            }).ToListAsync();
    }

    public async Task<SimpleFilmResponse> AddFilmToMyListAsync(AddFilmToMyListRequest addFilmToMyListRequest)
    {
        var film = await _dbContext.Films.FindAsync(addFilmToMyListRequest.FilmId);
        if (film == null)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {addFilmToMyListRequest.FilmId}");
        }
        
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        if (await _dbContext.MyFilms.AnyAsync(mf =>
                mf.FilmId == addFilmToMyListRequest.FilmId && mf.ApplicationUserId == applicationUserId))
        {
            throw new DuplicateException($"Film {addFilmToMyListRequest.FilmId} đã tồn tại trong Danh sách của User {applicationUserId} rồi.");
        }

        var newMyFilm = new MyFilm
        {
            FilmId = addFilmToMyListRequest.FilmId,
            ApplicationUserId = applicationUserId,
        };
        await _dbContext.MyFilms.AddAsync(newMyFilm);
        await _dbContext.SaveChangesAsync();
        
        return new SimpleFilmResponse()
        {
            FilmId = addFilmToMyListRequest.FilmId,
            Name = film.Name,
            PosterPath = film.PosterPath,
        };
    }

    public async Task<Guid> RemoveFilmFromMyListByFilmIdAsync(Guid filmId)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        var myFilm = await _dbContext.MyFilms
            .FirstOrDefaultAsync(mf => mf.FilmId == filmId && mf.ApplicationUserId == applicationUserId);

        if (myFilm == null)
        {
            throw new NotFoundException($"Film {filmId} không tồn tại trong Danh sách của User {applicationUserId}.");
        }
        
        _dbContext.MyFilms.Remove(myFilm);
        await _dbContext.SaveChangesAsync();
        
        return filmId;
    }

    public async Task<List<TrackingProgressResponse>> GetTrackingProgressAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        return await _dbContext.TrackingProgresses
            .Where(trackingProgress => trackingProgress.ApplicationUserId == applicationUserId)
            .Select(tp => new TrackingProgressResponse
            {
                EpisodeId = tp.EpisodeId,
                Progress = tp.Progress,
            }).ToListAsync();
    }

    public async Task<bool> UpdateTrackingProgressAsync(UpdateTrackingProgressRequest updateTrackingProgressRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        var trackingProgress = await _dbContext.TrackingProgresses
            .FirstOrDefaultAsync(tl => tl.EpisodeId == updateTrackingProgressRequest.EpisodeId && tl.ApplicationUserId == applicationUserId);

        if (trackingProgress == null)
        {
            if (await _dbContext.Episodes.AnyAsync(episode => episode.Id == updateTrackingProgressRequest.EpisodeId))
            {
                await _dbContext.TrackingProgresses.AddAsync(new TrackingProgress
                {
                    ApplicationUserId = applicationUserId,
                    EpisodeId = updateTrackingProgressRequest.EpisodeId,
                    Progress = updateTrackingProgressRequest.Progress,
                });
            }
            else
            {
                throw new InvalidModelException($"Không tìm thấy Episode {updateTrackingProgressRequest.EpisodeId}");
            }
        }
        else
        {
            trackingProgress.Progress = updateTrackingProgressRequest.Progress;
        }
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddUserPayment(AddUserPaymentRequest addUserPaymentRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);
        
        var applicationUser = await _dbContext.Users
            .Include(u => u.UserPlans) // Eager loading UserPlans
            .Include(u => u.Payments)
            .FirstOrDefaultAsync(u => u.Id == applicationUserId);
        
        var plan = await _dbContext.Plans.FindAsync(addUserPaymentRequest.PlanId);

        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan {addUserPaymentRequest.PlanId}");
        }
        
        applicationUser!.UserPlans.Add(new UserPlan
        {
            ApplicationUserId = applicationUserId,
            PlanId = plan.Id,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(plan.Duration)),
        });
        
        applicationUser.Payments.Add(new Payment
        {
            ApplicationUserId = applicationUserId,
            PlanId = plan.Id,
            CreatedAt = DateTime.UtcNow,
            IsDone = true
        });
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}