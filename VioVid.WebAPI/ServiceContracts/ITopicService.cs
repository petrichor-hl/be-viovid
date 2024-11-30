using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface ITopicService
{
    Task<List<TopicFilmResponse>> GetAllTopicFilmsAsync();
    
    Task<Topic> CreateTopicAsync(CreateTopicRequest createTopicRequest);

    Task<Topic> UpdateTopicAsync(Guid id, UpdateTopicRequest updateTopicRequest);

    Task<Guid> DeleteTopicAsync(Guid id);
}