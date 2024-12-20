using Application.DTOs;
using Application.DTOs.Channel;
using Application.DTOs.Channel.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
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
        return Ok(ApiResult<PaginationResponse<ChannelResponse>>.Success(
            await _channelService.GetAllAsync(getPagingChannelRequest)));
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetAllByUserAsync([FromQuery] GetPagingChannelRequest getPagingChannelRequest)
    {
        Console.WriteLine($"GetPagingChannelRequest: {getPagingChannelRequest}");
        return Ok(ApiResult<PaginationResponse<ChannelResponse>>.Success(
            await _channelService.GetAllByUserAsync(getPagingChannelRequest)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChannelById(Guid id)
    {
        return Ok(ApiResult<ChannelResponse>.Success(await _channelService.GetByIdAsync(id)));
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeChannel(SubscribeChannelRequest subscribeChannelRequest)
    {
        var success = await _channelService.SubscribeAsync(subscribeChannelRequest);
        return success
            ? Ok(ApiResult<bool>.Success(true))
            : BadRequest(ApiResult<bool>.Failure(new List<ApiResultError>
            {
                new(ApiResultErrorCodes.InternalServerError, "Unsubscribe failed")
            }));
    }

    [HttpPost("unsubscribe")]
    public async Task<IActionResult> UnsubscribeChannel(SubscribeChannelRequest subscribeChannelRequest)
    {
        var success = await _channelService.UnsubscribeAsync(subscribeChannelRequest);
        return success
            ? Ok(ApiResult<bool>.Success(true))
            : BadRequest(ApiResult<bool>.Failure(new List<ApiResultError>
            {
                new(ApiResultErrorCodes.InternalServerError, "Unsubscribe failed")
            }));
    }


    [HttpPost]
    public async Task<IActionResult> CreateChannel(CreateChannelRequest createChannelRequest)
    {
        return Ok(ApiResult<ChannelResponse>.Success(await _channelService.CreateChannelAsync(createChannelRequest)));
    }
}