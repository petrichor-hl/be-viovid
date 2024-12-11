using Application.DTOs.Film.Res;
using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class TopicService : ITopicService
{
    private readonly ApplicationDbContext _dbContext;
    
    public TopicService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<TopicResponse>> GetAllTopicFilmsAsync()
    {
        var topics = await _dbContext.Topics
            .Include(topic => topic.TopicFilms)
            .ThenInclude(tp => tp.Film)
            .OrderBy(topic => topic.Order)
            .ToListAsync(); // Tải dữ liệu bất đồng bộ

        return topics.Select(topic => new TopicResponse
        {
            TopicId = topic.Id,
            Name = topic.Name,
            Order = topic.Order,
            Films = topic.TopicFilms.Select(tp => new SimpleFilmResponse
            {
                FilmId = tp.FilmId,
                Name = tp.Film.Name,
                PosterPath = tp.Film.PosterPath,
            }).ToList(),
        }).ToList();
    }

    public async Task<Topic> CreateTopicAsync(CreateTopicRequest createTopicRequest)
    {
        var maxTopicOrder = await _dbContext.Topics.AnyAsync()
            ? await _dbContext.Topics.MaxAsync(topic => topic.Order)
            : -1;
        var newTopic = new Topic
        {
            Order = maxTopicOrder + 1,
            Name = createTopicRequest.Name,
        };
        await _dbContext.Topics.AddAsync(newTopic);
        await _dbContext.SaveChangesAsync();
        return newTopic;
    }

    public async Task<TopicResponse> AddFilmsToTopicAsync(Guid topicId, AddFilmsToTopicRequest addFilmsToTopicRequest)
    {
        var topic = await _dbContext.Topics
            .Include(topic => topic.TopicFilms)
            .ThenInclude(topicFilm => topicFilm.Film)
            .FirstOrDefaultAsync(topic => topic.Id == topicId);
        if (topic == null)
        {
            throw new NotFoundException($"Không tìm thấy Topic có id {topicId}");
        }
        
        foreach (var filmId in addFilmsToTopicRequest.FilmIds)
        {
            var film = await _dbContext.Films.FindAsync(filmId);
            if (film == null)
            {
                throw new NotFoundException($"Không tìm thấy Film {filmId}");
            }
            
            if (topic.TopicFilms.Any(topicFilm => topicFilm.FilmId == filmId))
            {
                throw new DuplicateException($"Topic {topicId} đã chứa Film {filmId} rồi");
            }
            
            topic.TopicFilms.Add(new TopicFilm
            {
                Film = film
            });
        }

        _dbContext.Topics.Update(topic);
        await _dbContext.SaveChangesAsync();

        return new TopicResponse
        {
            TopicId = topic.Id,
            Name = topic.Name,
            Order = topic.Order,
            Films = topic.TopicFilms.Select(topicFilm => new SimpleFilmResponse
            {
                FilmId = topicFilm.FilmId,
                Name = topicFilm.Film.Name,
                PosterPath = topicFilm.Film.PosterPath,
            }).ToList()
        };
    }

    public async Task<TopicResponse> RemoveFilmsFromTopicAsync(Guid topicId, RemoveFilmsFromTopicRequest removeFilmsFromTopicRequest)
    {
        var topic = await _dbContext.Topics
            .Include(topic => topic.TopicFilms)
            .ThenInclude(topicFilm => topicFilm.Film)
            .FirstOrDefaultAsync(topic => topic.Id == topicId);
        if (topic == null)
        {
            throw new NotFoundException($"Không tìm thấy Topic có id {topicId}");
        }
        
        foreach (var filmId in removeFilmsFromTopicRequest.FilmIds)
        {
            var topicFilm =
                topic.TopicFilms.FirstOrDefault(topicFilm =>
                    topicFilm.TopicId == topicId && topicFilm.FilmId == filmId);
            
            if (topicFilm == null)
            {
                throw new NotFoundException($"Topic {topicId} không tồn tại Film {filmId}");
            } 
            
            topic.TopicFilms.Remove(topicFilm);
        }

        _dbContext.Topics.Update(topic);
        await _dbContext.SaveChangesAsync();

        return new TopicResponse
        {
            TopicId = topic.Id,
            Name = topic.Name,
            Order = topic.Order,
            Films = topic.TopicFilms.Select(topicFilm => new SimpleFilmResponse
            {
                FilmId = topicFilm.FilmId,
                Name = topicFilm.Film.Name,
                PosterPath = topicFilm.Film.PosterPath,
            }).ToList()
        };
    }
    
    public async Task<Topic> UpdateTopicAsync(Guid id, UpdateTopicRequest updateTopicRequest)
    {
        var topic = await _dbContext.Topics.FindAsync(id);
        if (topic == null)
        {
            throw new NotFoundException($"Không tìm thấy Topic có id {id}");
        }

        topic.Order = updateTopicRequest.Order ?? topic.Order;
        topic.Name = updateTopicRequest.Name;
        await _dbContext.SaveChangesAsync();

        return topic;
    }

    public async Task<Guid> DeleteTopicAsync(Guid id)
    {
        var topic = await _dbContext.Topics.FindAsync(id);
        if (topic == null)
        {
            throw new NotFoundException($"Không tìm thấy Topic có id {id}");
        }
        _dbContext.Topics.Remove(topic);
        await _dbContext.SaveChangesAsync();
        return id;
    }
}