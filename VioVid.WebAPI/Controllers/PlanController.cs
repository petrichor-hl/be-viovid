using Application.DTOs;
using Application.DTOs.Genre;
using Application.DTOs.Plan;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlanController : ControllerBase
{
    private readonly IPlanService _planService;

    public PlanController(IPlanService planService)
    {
        _planService = planService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(ApiResult<List<Plan>>.Success(await _planService.GetAllAsync()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        return Ok(ApiResult<Plan>.Success(await _planService.GetByIdAsync(id)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGenre(CreatePlanRequest createPlanRequest)
    {
        return Ok(ApiResult<Plan>.Success(await _planService.CreatePlanAsync(createPlanRequest)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGenre(Guid id,[FromBody] UpdatePlanRequest updatePlanRequest)
    {
        return Ok(ApiResult<Plan>.Success(await _planService.UpdatePlanAsync(id, updatePlanRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _planService.DeletePlanAsync(id)));
    }
}