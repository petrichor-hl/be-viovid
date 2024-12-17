using Application.DTOs;
using Application.DTOs.Channel;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;

    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPagingChannelRequest getPagingChannelRequest)
    {
        Console.WriteLine($"GetPagingChannelRequest: {getPagingChannelRequest}");
        return Ok(ApiResult<PaginationResponse<Channel>>.Success(
            await _channelService.GetAllAsync(getPagingChannelRequest)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChannelById(Guid id)
    {
        return Ok(ApiResult<Channel>.Success(await _channelService.GetByIdAsync(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateChannel(CreateChannelRequest createChannelRequest)
    {
        return Ok(ApiResult<Channel>.Success(await _channelService.CreateChannelAsync(createChannelRequest)));
    }
}