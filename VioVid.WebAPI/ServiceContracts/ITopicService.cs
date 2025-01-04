using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface ITopicService
{
    Task<List<TopicResponse>> GetAllTopicAsync();
    
    Task<List<TopicResponse>> ReorderTopicsAsync(ReorderTopicsRequest reorderTopicsRequest);
    
    Task<TopicResponse> CreateTopicAsync(CreateTopicRequest createTopicRequest);

    // Task<TopicResponse> AddFilmsToTopicAsync(Guid topicId, AddFilmsToTopicRequest addFilmsToTopicRequest);
    
    // Task<TopicResponse> RemoveFilmsFromTopicAsync(Guid topicId, RemoveFilmsFromTopicRequest removeFilmsFromTopicRequest);

    Task<bool> UpdateTopicAsync(Guid id, UpdateTopicRequest updateTopicRequest);
    
    Task<TopicResponse> UpdateListFilm(Guid topicId, UpdateListFilmRequest updateListFilmRequest);

    Task<Guid> DeleteTopicAsync(Guid id);
}