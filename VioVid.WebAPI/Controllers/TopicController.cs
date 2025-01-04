using Application.DTOs;
using Application.DTOs.Topic.Req;
using Application.DTOs.Topic.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;
    
    public TopicController(ITopicService topicService)
    {
        _topicService = topicService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTopics()
    {
        return Ok(ApiResult<List<TopicResponse>>.Success(await _topicService.GetAllTopicAsync()));
    }
    
    [HttpPost("re-order")]
    public async Task<IActionResult> ReorderTopics(ReorderTopicsRequest reorderTopicsRequest) {
        return Ok(ApiResult<List<TopicResponse>>.Success(await _topicService.ReorderTopicsAsync(reorderTopicsRequest)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTopic(CreateTopicRequest createTopicRequest)
    {
        return Ok(ApiResult<TopicResponse>.Success(await _topicService.CreateTopicAsync(createTopicRequest)));
    }
    
    // [HttpPost("{topicId:guid}/add-films")]
    // public async Task<IActionResult> AddFilmsToTopic(Guid topicId, [FromBody] AddFilmsToTopicRequest addFilmsToTopicRequest)
    // {
    //     return Ok(ApiResult<TopicResponse>.Success(await _topicService.AddFilmsToTopicAsync(topicId, addFilmsToTopicRequest)));
    // }
    
    // [HttpPost("{topicId:guid}/remove-films")]
    // public async Task<IActionResult> RemoveFilmsFromTopic(Guid topicId, [FromBody] RemoveFilmsFromTopicRequest removeFilmsFromTopicRequest)
    // {
    //     return Ok(ApiResult<TopicResponse>.Success(await _topicService.RemoveFilmsFromTopicAsync(topicId, removeFilmsFromTopicRequest)));
    // }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] UpdateTopicRequest updateTopicRequest)
    {
        return Ok(ApiResult<bool>.Success(await _topicService.UpdateTopicAsync(id, updateTopicRequest)));
    }
    
    [HttpPut("{id:guid}/update-list-film")]
    public async Task<IActionResult> UpdateListFilm(Guid id, [FromBody] UpdateListFilmRequest updateListFilmRequest)
    {
        return Ok(ApiResult<TopicResponse>.Success(await _topicService.UpdateListFilm(id, updateListFilmRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTopic(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _topicService.DeleteTopicAsync(id)));
    }
}