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
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(ApiResult<List<TopicFilmResponse>>.Success(await _topicService.GetAllTopicFilmsAsync()));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTopic(CreateTopicRequest createTopicRequest)
    {
        return Ok(ApiResult<Topic>.Success(await _topicService.CreateTopicAsync(createTopicRequest)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTopic(Guid id,[FromBody] UpdateTopicRequest updateTopicRequest)
    {
        return Ok(ApiResult<Topic>.Success(await _topicService.UpdateTopicAsync(id, updateTopicRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTopic(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _topicService.DeleteTopicAsync(id)));
    }
}