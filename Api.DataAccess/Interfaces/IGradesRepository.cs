using Api.DataAccess.Models;

namespace Api.DataAccess.Interfaces;

/// <summary>
/// Interface for the grade repository
/// </summary>
public interface IGradesRepository
{
    /// <summary>
    /// Creates a new grade request asynchronous.
    /// </summary>
    /// <param name="grade">The grade model.</param>
    /// <returns>grade</returns>
    Task<Grade> CreateAsync(Grade grade);

    /// <summary>
    /// Get all grades asynchronous.
    /// </summary>
    /// <returns>List of all grades</returns>
    Task<IEnumerable<Grade>> GetAllAsync();

    /// <summary>
    /// Gets a grade by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>found grade</returns>
    Task<Grade?> GetByIdAsync(int id);

    /// <summary>
    /// Updates a grade by Model asynchronous.
    /// </summary>
    /// <param name="grade">The grade.</param>
    /// <returns>Save all changes in database-context and gives true back, if minimum one line was affected, otherwise false.</returns>
    Task<bool> UpdateAsync(Grade grade);

    /// <summary>
    /// Deletes a grade by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Save all changes in database-context and gives true back, if minimum one line was affected, otherwise false.</returns>
    Task<bool> DeleteAsync(int id);
}