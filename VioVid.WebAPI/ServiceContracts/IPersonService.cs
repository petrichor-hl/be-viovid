using Application.DTOs.Person.Req;
using Application.DTOs.Person.Res;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPersonService
{
    Task<PaginationResponse<Person>> GetAllAsync(GetPagingPersonRequest getPagingPersonRequest);
    
    Task<PersonResponse> GetByIdAsync(Guid id);
    
    Task<Person> CreatePersonAsync(CreatePersonRequest createPersonRequest);

    Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    
    Task<Guid> DeletePersonAsync(Guid id);
}