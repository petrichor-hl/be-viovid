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
    
    public async Task<List<TopicFilmResponse>> GetAllTopicFilmsAsync()
    {
        var topics = await _dbContext.Topics
            .Include(topic => topic.TopicFilms)
            .ThenInclude(tp => tp.Film)
            .OrderBy(topic => topic.Order)
            .ToListAsync(); // Tải dữ liệu bất đồng bộ

        return topics.Select(topic => new TopicFilmResponse
        {
            TopicId = topic.Id,
            Name = topic.Name,
            Films = topic.TopicFilms.Select(tp => new SimpleFilmResponse
            {
                Id = tp.FilmId,
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