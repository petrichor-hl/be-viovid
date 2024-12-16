using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface ITopicService
{
    Task<List<TopicResponse>> GetAllTopicFilmsAsync();
    
    Task<Topic> CreateTopicAsync(CreateTopicRequest createTopicRequest);

    Task<TopicResponse> AddFilmsToTopicAsync(Guid topicId, AddFilmsToTopicRequest addFilmsToTopicRequest);

    Task<TopicResponse>
        RemoveFilmsFromTopicAsync(Guid topicId, RemoveFilmsFromTopicRequest removeFilmsFromTopicRequest);

    Task<Topic> UpdateTopicAsync(Guid id, UpdateTopicRequest updateTopicRequest);

    Task<Guid> DeleteTopicAsync(Guid id);
}