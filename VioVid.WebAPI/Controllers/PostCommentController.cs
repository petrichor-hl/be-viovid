using Application.DTOs;
using Application.DTOs.PostComment;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostCommentController : ControllerBase
{
    private readonly IPostCommentService _postCommentService;

    public PostCommentController(IPostCommentService postCommentService)
    {
        _postCommentService = postCommentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter filter)
    {
        return Ok(ApiResult<PaginationResponse<PostComment>>.Success(
            await _postCommentService.GetAllAsync(filter)));
    }


    [HttpGet("Post")]
    public async Task<IActionResult> GetAllByPostAsync(
        [FromQuery] GetPagingPostCommentRequest getPagingPostCommentRequest)
    {
        return Ok(ApiResult<PaginationResponse<PostComment>>.Success(
            await _postCommentService.GetAllByPostAsync(getPagingPostCommentRequest)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPostCommentById(Guid id)
    {
        return Ok(ApiResult<PostComment>.Success(await _postCommentService.GetByIdAsync(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostComment(CreatePostCommentRequest createPostCommentRequest)
    {
        return Ok(ApiResult<PostComment>.Success(
            await _postCommentService.CreatePostCommentAsync(createPostCommentRequest)));
    }
}