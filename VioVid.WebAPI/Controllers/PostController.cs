using Application.DTOs;
using Application.DTOs.Post;
using Application.DTOs.Post.Res;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter getPagingPostRequest)
    {
        return Ok(ApiResult<PaginationResponse<Post>>.Success(await _postService.GetAllAsync(getPagingPostRequest)));
    }


    [HttpGet("Channel")]
    public async Task<IActionResult> GetAllByChannelAsync([FromQuery] GetPagingPostRequest getPagingPostRequest)
    {
        return Ok(ApiResult<PaginationResponse<Post>>.Success(
            await _postService.GetAllByChannelAsync(getPagingPostRequest)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        return Ok(ApiResult<PostResponse>.Success(await _postService.GetByIdAsync(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
    {
        return Ok(ApiResult<Post>.Success(await _postService.CreatePostAsync(createPostRequest)));
    }

    [HttpPost("{id:guid}/Like")]
    public async Task<IActionResult> LikePost(Guid id)
    {
        await _postService.LikePostAsync(id);
        return Ok(ApiResult<string>.Success("Post liked successfully."));
    }

    [HttpPost("{id:guid}/Unlike")]
    public async Task<IActionResult> UnlikePost(Guid id)
    {
        await _postService.UnlikePostAsync(id);
        return Ok(ApiResult<string>.Success("Post unliked successfully."));
    }
}