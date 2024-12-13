using meetings_app_server.Models.Domain;

namespace meetings_app_server.Repositories
{
    public interface IMeetingRepository
    {
    
        Task<List<Meeting>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000, string _embed = "");
       
       
    }
}

