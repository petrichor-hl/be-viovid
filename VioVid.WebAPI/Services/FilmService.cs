using Application.DTOs.Film.Req;
using Application.DTOs.Film.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.Core.Enum;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class FilmService : IFilmService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPushNotificationService _pushNotificationService;
    
    public FilmService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPushNotificationService pushNotificationService)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _pushNotificationService = pushNotificationService;
    }
    
    public async Task<PaginationResponse<SimpleFilmResponse>> GetAllAsync(GetPagingFilmRequest getPagingFilmRequest)
    {
        var pageIndex = getPagingFilmRequest.PageIndex;
        var pageSize = getPagingFilmRequest.PageSize;
        var searchText = getPagingFilmRequest.SearchText?.ToLower();
        
        var query = _dbContext.Films.AsQueryable();
        
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(p => p.Id.ToString().ToLower().Contains(searchText) || 
                                     p.Name.ToLower().Contains(searchText));
        }
        
        // Tính tổng số lượng record
        var totalRecords = await query.CountAsync();
        
        // Lấy ra trang trong request cần
        var films = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PaginationResponse<SimpleFilmResponse>
        {
            TotalCount = totalRecords,
            Items = films.Select(f => new SimpleFilmResponse
            {
                FilmId = f.Id,
                Name = f.Name,
                PosterPath = f.PosterPath,
            }),
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<FilmResponse> GetByIdAsync(Guid id)
{
    var film = await _dbContext.Films
        .Include(f => f.Seasons)
            .ThenInclude(season => season.Episodes)
        .Include(f => f.GenreFilms)
            .ThenInclude(genreFilm => genreFilm.Genre)
        .FirstOrDefaultAsync(f => f.Id == id);

    if (film == null)
    {
        throw new NotFoundException($"Không tìm thấy Film có id {id}");
    }

    var user = _httpContextAccessor.HttpContext?.User;
    var userIdClaim = user?.FindFirst("UserId");
    var applicationUserId = Guid.Parse(userIdClaim?.Value!);

    var seasonResponses = new List<SeasonResponse>();
    foreach (var season in film.Seasons.OrderBy(s => s.Order))
    {
        var episodeResponses = new List<EpisodeResponse>();
        foreach (var episode in season.Episodes.OrderBy(e => e.Order))
        {
            var trackingProgress = await _dbContext.TrackingProgresses
                .FirstOrDefaultAsync(tp => tp.ApplicationUserId == applicationUserId && tp.EpisodeId == episode.Id);

            episodeResponses.Add(new EpisodeResponse
            {
                Id = episode.Id,
                Order = episode.Order,
                Title = episode.Title,
                Summary = episode.Summary,
                Source = episode.Source,
                Duration = episode.Duration,
                StillPath = episode.StillPath,
                IsFree = episode.IsFree,
                Progress = trackingProgress?.Progress ?? 0,
            });
        }

        seasonResponses.Add(new SeasonResponse
        {
            Id = season.Id,
            Order = season.Order,
            Name = season.Name,
            Episodes = episodeResponses,
        });
    }

    var genreResponses = film.GenreFilms
        .Select(genreFilm => new SimpleGenreResponse
        {
            Id = genreFilm.Genre.Id,
            Name = genreFilm.Genre.Name,
        })
        .ToList();

    return new FilmResponse
    {
        FilmId = film.Id,
        Name = film.Name,
        Overview = film.Overview,
        PosterPath = film.PosterPath,
        BackdropPath = film.BackdropPath,
        ContentRating = film.ContentRating,
        ReleaseDate = film.ReleaseDate,
        Seasons = seasonResponses,
        Genres = genreResponses,
    };
}
    public async Task<List<ReviewResponse>> GetReviewsAsync(Guid id)
    {
        var filmExists = await _dbContext.Films.AnyAsync(f => f.Id == id);
        if (!filmExists)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {id}");
        }
        return await _dbContext.Reviews
            .Include(review => review.ApplicationUser)
            .ThenInclude(user => user.UserProfile)
            .Where(review => review.FilmId == id)
            .OrderByDescending(review => review.CreateAt)   // Sắp xếp theo bình luận mới nhất trước
            .Select(review => new ReviewResponse
            {
                Id = review.Id,
                UserName = review.ApplicationUser.UserProfile.Name,
                UserAvatar = review.ApplicationUser.UserProfile.Avatar,
                Start = review.Start,
                Content = review.Content,
                CreateAt = review.CreateAt,
            })
            .ToListAsync();
    }

    public async Task<ReviewResponse> PostReview(Guid filmId, PostReviewRequest postReviewRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdClaim = user!.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);
        
        var film = await _dbContext.Films
            .Include(f => f.Reviews)
            .FirstOrDefaultAsync(f => f.Id == filmId);
        
        if (film == null)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {filmId}");
        }
        
        var applicationUser = await _dbContext.Users
            .Include(u => u.UserProfile)
            .FirstAsync(u => u.Id == applicationUserId);
        
        var review = new Review
        {
            Start = postReviewRequest.Start,
            Content = postReviewRequest.Content,
            CreateAt = DateTime.UtcNow,
            FilmId = filmId,
            ApplicationUserId = applicationUserId,
            ApplicationUser = applicationUser,
        };
        
        await _dbContext.Reviews.AddAsync(review);
        await _dbContext.SaveChangesAsync();
        
        return new ReviewResponse
        {
            Id = review.Id,
            UserName = review.ApplicationUser.UserProfile.Name,
            UserAvatar = review.ApplicationUser.UserProfile.Avatar,
            Start = review.Start,
            Content = review.Content,
            CreateAt = review.CreateAt,
        };
    }

    public async Task<List<SimpleCastResponse>> GetCastsAsync(Guid id)
    {
        var filmExists = await _dbContext.Films.AnyAsync(f => f.Id == id);
        if (!filmExists)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {id}");
        }
        return await _dbContext.Casts
            .Where(s => s.FilmId == id)
            .Include(c => c.Person)
            .OrderByDescending(cast => cast.Person.Popularity)
            .Select(cast => new SimpleCastResponse
            {
                CastId = cast.Id,
                PersonId = cast.PersonId,
                Character = cast.Character,
                PersonName = cast.Person.Name,
                PersonProfilePath = cast.Person.ProfilePath,
            })
            .ToListAsync();
    }

    public async Task<List<SimpleCrewReponse>> GetCrewsAsync(Guid id)
    {
        var filmExists = await _dbContext.Films.AnyAsync(f => f.Id == id);
        if (!filmExists)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {id}");
        }
        return await _dbContext.Crews
            .Where(s => s.FilmId == id)
            .Include(c => c.Person)
            .OrderByDescending(crew => crew.Person.Popularity)
            .Select(crew => new SimpleCrewReponse
            {
                CrewId = crew.Id,
                PersonId = crew.PersonId,
                Role = crew.Role,
                PersonName = crew.Person.Name,
                PersonProfilePath = crew.Person.ProfilePath,
            })
            .ToListAsync();
    }

    public async Task<Film> CreateFilmAsync(CreateFilmRequest createFilmRequest)
    {
        var newFilm = new Film
        {
            Id = Guid.NewGuid(),
            Name = createFilmRequest.Name,
            Overview = createFilmRequest.Overview,
            PosterPath = createFilmRequest.PosterPath,
            BackdropPath = createFilmRequest.BackdropPath,
            ContentRating = createFilmRequest.ContentRating,
            ReleaseDate = createFilmRequest.ReleaseDate,
            Seasons = createFilmRequest.SeasonRequests.Select((seasonRequest, seasonIndex) => new Season
            {
                Order = seasonIndex,
                Name = seasonRequest.Name,
                Episodes = seasonRequest.EpisodeRequests.Select((episodeRequest, episodeIndex) => new Episode
                {
                    Order = episodeIndex,
                    Title = episodeRequest.Title,
                    Summary = episodeRequest.Summary,
                    Source = episodeRequest.Source,
                    Duration = episodeRequest.Duration,
                    StillPath = episodeRequest.StillPath,
                    IsFree = episodeRequest.IsFree,
                }).ToList(),
            }).ToList(),
            GenreFilms = createFilmRequest.GenreRequests.Select(genreRequest => new GenreFilm
            {
                GenreId = genreRequest.Id,
            }).ToList(),
            TopicFilms = createFilmRequest.TopicRequests.Select(topicRequest => new TopicFilm
            {
                TopicId = topicRequest.Id,
            }).ToList(),
            Casts = createFilmRequest.CastRequests.Select(castRequest => new Cast
            {
                PersonId = castRequest.PersonId,
                Character = castRequest.Character,
            }).ToList(),
            Crews = createFilmRequest.CrewRequests.Select(crewRequest => new Crew
            {
                PersonId = crewRequest.PersonId,
                Role = crewRequest.Role,
            }).ToList(),
            Reviews = new List<Review>(),
        };

        var newNoti = new UserNotification
        {
            Category = NotificationCategory.Film,
            CreatedDateTime = DateTime.UtcNow,
            ReadStatus = NotificationReadStatus.UnRead,
            Title = "Phim mới vừa được thêm!",
            Body = $"Phim {newFilm.Name} đã sẵn sàng để bạn thưởng thức.",
            Params = new Dictionary<string, object>
            {
                { "filmId", newFilm.Id },
                { "name", newFilm.Name },
                { "overview", newFilm.Overview },
                { "backdropPath", newFilm.BackdropPath },
                { "contentRating", newFilm.ContentRating },
            }
        };

        var dataPayload = new Dictionary<string, string>
        {
            { "type", "NewFilm" },
            { "filmId", newFilm.Id.ToString() },
        };
        
        // Lưu Film trước khi Push notification
        await _dbContext.Films.AddAsync(newFilm);
        await _dbContext.UserNotifications.AddAsync(newNoti);
        await _dbContext.SaveChangesAsync();
        
        // Push notification
        await _pushNotificationService.PushNotificationToTopicAsync(newNoti.Title, newNoti.Body, dataPayload, "NewFilm");

        return newFilm;
    }

    public async Task<Film> UpdateFilmAsync(Guid id, UpdateFilmRequest updateFilmRequest)
    {
        var film = await _dbContext.Films
            .Include(f => f.Seasons).ThenInclude(season => season.Episodes)
            .Include(f => f.GenreFilms)
            .Include(f => f.TopicFilms)
            .Include(f => f.Casts)
            .Include(f => f.Crews)
            .FirstOrDefaultAsync();
        
        if (film == null)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {id}");
        }
        
        film.Name = updateFilmRequest.Name;
        film.Overview = updateFilmRequest.Overview;
        film.PosterPath = updateFilmRequest.PosterPath;
        film.BackdropPath = updateFilmRequest.BackdropPath;
        film.ContentRating = updateFilmRequest.ContentRating;
        film.ReleaseDate = updateFilmRequest.ReleaseDate;

        #region Xử lý Season

        // Các Season cần xoá
        var seasonsToRemove = film.Seasons
            .Where(s => updateFilmRequest.SeasonRequests.All(sr => sr.Id != s.Id));
        
        // Lấy ra MaxSeasonOrder
        var maxSeasonOrder = (await _dbContext.Seasons
            .Where(s => s.FilmId == film.Id)
            .Select(s => s.Order)
            .ToListAsync()) // Chuyển sang danh sách trong bộ nhớ
            .DefaultIfEmpty(-1) // Áp dụng giá trị mặc định nếu rỗng
            .Max();
        // Các Season mới => Season có Id = null
        var newSeasons = updateFilmRequest.SeasonRequests
            .Where(sr => sr.Id == null)
            .Select((sr, srIndex) => new Season
            {
                Order = maxSeasonOrder + srIndex + 1,
                Name = sr.Name,
                Episodes = sr.EpisodeRequests.Select((er, erIndex) => new Episode
                {
                    Order = erIndex,
                    Title = er.Title,
                    Summary = er.Summary,
                    Source = er.Source,
                    Duration = er.Duration,
                    StillPath = er.StillPath,
                    IsFree = er.IsFree,
                }).ToList(),
                FilmId = film.Id,
            });
        
        // Các Season cần cập nhật
        foreach (var currentSeason in film.Seasons)
        {
            var updatedSeason = updateFilmRequest.SeasonRequests.FirstOrDefault(sr => sr.Id == currentSeason.Id);
            if (updatedSeason != null)
            {
                currentSeason.Order = updatedSeason.Order ?? currentSeason.Order;
                currentSeason.Name = updatedSeason.Name;
                
                #region Xử lý Episode
        
                // Các Episode cần xoá
                var episodesToRemove = currentSeason.Episodes
                    .Where(e => updatedSeason.EpisodeRequests.All(er => er.Id != e.Id));
                
                // Lấy ra MaxEpisodeOrder
                var maxEpisodeOrder = (await _dbContext.Episodes
                    .Where(e => e.SeasonId == currentSeason.Id)
                    .Select(e => e.Order)
                    .ToListAsync())
                    .DefaultIfEmpty(0)
                    .Max();
                // Các Episode mới => Episode có Id = null
                var newEpisodes = updatedSeason.EpisodeRequests
                    .Where(er => er.Id == null)
                    .Select((er, erIndex) => new Episode
                    {
                        Order = maxEpisodeOrder + erIndex + 1,
                        Title = er.Title,
                        Summary = er.Summary,
                        Source = er.Source,
                        Duration = er.Duration,
                        StillPath = er.StillPath,
                        IsFree = er.IsFree,
                        SeasonId = currentSeason.Id,
                    });
                
                // Các Episode cần cập nhật
                foreach (var currentEpisode in currentSeason.Episodes)
                {
                    var updatedEpisode = updatedSeason.EpisodeRequests.FirstOrDefault(er => er.Id == currentEpisode.Id);
                    if (updatedEpisode != null)
                    {
                        currentEpisode.Order = updatedEpisode.Order ?? currentEpisode.Order;
                        currentEpisode.Title = updatedEpisode.Title;
                        currentEpisode.Summary = updatedEpisode.Summary;
                        currentEpisode.Source = updatedEpisode.Source;
                        currentEpisode.Duration = updatedEpisode.Duration;
                        currentEpisode.StillPath = updatedEpisode.StillPath;
                        currentEpisode.IsFree = updatedEpisode.IsFree;
                    }
                }
                
                _dbContext.Episodes.RemoveRange(episodesToRemove);
                _dbContext.Episodes.AddRange(newEpisodes);

                #endregion
            }
        }
        
        _dbContext.Seasons.RemoveRange(seasonsToRemove);
        _dbContext.Seasons.AddRange(newSeasons);

        #endregion

        #region Xử lý GenreFilms

        // Xoá danh sách GenreFilms hiện tại của Film
        _dbContext.RemoveRange(film.GenreFilms);
        
        // Thêm danh sách GenreFilms từ updateFilmRequest vào Film
        film.GenreFilms = updateFilmRequest.GenreRequests.Select(gr => new GenreFilm
        {
            GenreId = gr.Id,
        }).ToList();
        
        #endregion
        
        #region Xử lý TopicFilms

        // Xoá danh sách GenreFilms hiện tại của Film
        _dbContext.RemoveRange(film.TopicFilms);
        
        // Thêm danh sách GenreFilms từ updateFilmRequest vào Film
        film.TopicFilms = updateFilmRequest.TopicRequests.Select(tr => new TopicFilm
        {
            TopicId = tr.Id,
        }).ToList();
        
        #endregion
        
        #region Xử lý Cast
        
        // Các Cast cần xoá
        var castsToRemove = film.Casts
            .Where(c => updateFilmRequest.CastRequests.All(cr => cr.PersonId != c.PersonId));
        
        // Các Cast mới
        var newCasts = updateFilmRequest.CastRequests
            .Where(cr => film.Casts.All(c => c.PersonId != cr.PersonId)).Select(cr => new Cast
            {
                FilmId = film.Id,
                PersonId = cr.PersonId,
                Character = cr.Character,
            });
        
        // Các Cast cần cập nhật
        foreach (var currentCast in film.Casts)
        {
            var updatedCast = updateFilmRequest.CastRequests.FirstOrDefault(cr => cr.PersonId == currentCast.PersonId);
            if (updatedCast != null && currentCast.Character != updatedCast.Character)
            {
                // Cập nhật Character
                currentCast.Character = updatedCast.Character;
            }
        }
        
        _dbContext.Casts.RemoveRange(castsToRemove);
        _dbContext.Casts.AddRange(newCasts);
        
        #endregion
        
        #region Xử lý Crew
        
        // Các Crew cần xoá
        var crewsToRemove = film.Crews
            .Where(c => updateFilmRequest.CrewRequests.All(cr => cr.PersonId != c.PersonId))
            .ToList();
        
        // Các Crew mới
        var newCrews = updateFilmRequest.CrewRequests
            .Where(cr => film.Crews.All(c => c.PersonId != cr.PersonId)).Select(cr => new Crew()
            {
                FilmId = film.Id,
                PersonId = cr.PersonId,
                Role = cr.Role,
            }).ToList();

        // Crew cần cập nhật
        foreach (var currentCrew in film.Crews)
        {
            var updatedCrew = updateFilmRequest.CrewRequests.FirstOrDefault(cr => cr.PersonId == currentCrew.PersonId);
            if (updatedCrew != null && currentCrew.Role != updatedCrew.Role)
            {
                // Cập nhật Role
                currentCrew.Role = updatedCrew.Role;
            }
        }
        
        _dbContext.Crews.RemoveRange(crewsToRemove);
        _dbContext.Crews.AddRange(newCrews);
        
        #endregion
        
        _dbContext.Films.Update(film);
        await _dbContext.SaveChangesAsync();
        return film;
    }

    public async Task<Guid> DeleteFilmAsync(Guid id)
    {
        var film = await _dbContext.Films.FindAsync(id);
        if (film == null)
        {
            throw new NotFoundException($"Không tìm thấy Film có id {id}");
        }
        
        _dbContext.Films.Remove(film);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}