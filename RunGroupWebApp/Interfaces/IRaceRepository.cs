using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IRaceRepository
    {

        Task<IEnumerable<Races>> GetAll();
        Task<Races> GetByIdAsync(int id);
        Task<Club> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Races>> GetClubByCity(string city);
        bool Add(Races races);
        bool Update(Races races);
        bool Delete(Races races);
        bool Save();
    }
}
