using Application.DTOs;
using Application.DTOs.Person.Req;
using Application.DTOs.Person.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonController : Controller
{
    private readonly IPersonService _personService;
    
    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPagingPersonRequest getPagingPersonRequest)
    {
        return Ok(ApiResult<PaginationResponse<Person>>.Success(await _personService.GetAllAsync(getPagingPersonRequest)));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        return Ok(ApiResult<PersonResponse>.Success(await _personService.GetByIdAsync(id)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePerson(CreatePersonRequest createPersonRequest)
    {
        return Ok(ApiResult<Person>.Success(await _personService.CreatePersonAsync(createPersonRequest)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePerson(Guid id,[FromBody] UpdatePersonRequest updatePersonRequest)
    {
        return Ok(ApiResult<Person>.Success(await _personService.UpdatePersonAsync(id, updatePersonRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _personService.DeletePersonAsync(id)));
    }
}