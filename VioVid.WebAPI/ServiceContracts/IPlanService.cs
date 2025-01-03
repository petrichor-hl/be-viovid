using Application.DTOs.Plan;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPlanService
{
    Task<List<Plan>> GetAllAsync();
    
    Task<Plan> CreatePlanAsync(CreatePlanRequest createPlanRequest);
    
    Task<Plan> UpdatePlanAsync(Guid id, UpdatePlanRequest updateGenreRequest);
    
    Task<Guid> DeletePlanAsync(Guid id);
}
