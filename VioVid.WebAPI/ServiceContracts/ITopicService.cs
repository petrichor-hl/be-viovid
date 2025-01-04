using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface ITopicService
{
    Task<List<BrowseTopicResponse>> GetBrowseTopicsAsync();
    
    Task<List<TopicResponse>> GetAllTopicsAsync();
    
    Task<BrowseTopicResponse> GetBrowseTopicAsync(Guid id);
    
    Task<List<TopicResponse>> ReorderTopicsAsync(ReorderTopicsRequest reorderTopicsRequest);
    
    Task<TopicResponse> CreateTopicAsync(CreateTopicRequest createTopicRequest);

    // Task<TopicResponse> AddFilmsToTopicAsync(Guid topicId, AddFilmsToTopicRequest addFilmsToTopicRequest);
    
    // Task<TopicResponse> RemoveFilmsFromTopicAsync(Guid topicId, RemoveFilmsFromTopicRequest removeFilmsFromTopicRequest);

    Task<bool> UpdateTopicAsync(Guid id, UpdateTopicRequest updateTopicRequest);
    
    Task<BrowseTopicResponse> UpdateListFilm(Guid topicId, UpdateListFilmRequest updateListFilmRequest);

    Task<Guid> DeleteTopicAsync(Guid id);
    
    Task<Guid> DeleteFilmInTopicAsync(Guid topicId, Guid filmId);
}