using Api.DataAccess.Models;

namespace Api.DataAccess.Interfaces;

public interface IGradesRepository
{
    Task<Grades> CreateAsync(Grades grade);
    Task<IEnumerable<Grades>> GetAllAsync();
    Task<Grades?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(Grades grade);
    Task<bool> DeleteAsync(int id);
}
