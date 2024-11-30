using Application.DTOs.Person;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPersonService
{
    Task<PaginationResponse<Person>> GetAllAsync(GetPagingPersonRequest getPagingPersonRequest);
    
    Task<Person> GetByIdAsync(Guid id);
    
    Task<Person> CreatePersonAsync(CreatePersonRequest createPersonRequest);

    Task<Person> UpdatePersonAsync(Guid id, UpdatePersonRequest updatePersonRequest);
    
    Task<Guid> DeletePersonAsync(Guid id);
}