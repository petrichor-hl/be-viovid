using Application.DTOs;
using Application.DTOs.Post;
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

    // [HttpGet]
    // public async Task<IActionResult> GetAllAsync([FromQuery] GetPagingPostRequest getPagingPostRequest)
    // {
    //     return Ok(ApiResult<PaginationResponse<Post>>.Success(await _postService.GetAllAsync(getPagingPostRequest)));
    // }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        return Ok(ApiResult<Post>.Success(await _postService.GetByIdAsync(id)));
    }

    // [HttpPost]
    // public async Task<IActionResult> CreatePost(CreatePostRequest createPostRequest)
    // {
    //     return Ok(ApiResult<Post>.Success(await _postService.CreatePostAsync(createPostRequest)));
    // }
}