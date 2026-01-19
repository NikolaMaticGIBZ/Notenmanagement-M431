using Shared.DTOs;

namespace Api.Services.Interfaces;

/// <summary>
/// Interface for grade service
/// </summary>
public interface IGradeService
{
    /// <summary>
    /// Creates a grade asynchronous.
    /// </summary>
    /// <param name="request">The request dto.</param>
    /// <param name="teacherId">The teacher identifier.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">
    /// Teacher not found.
    /// or
    /// Rektor not found.
    /// </exception>
    Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId);

    /// <summary>
    /// Gets all the grades asynchronous.
    /// </summary>
    /// <param name="status">The status. (approved / pending) </param>
    /// <returns>Returns the Grades in a List</returns>
    Task<List<GradeResponse>> GetGradesAsync(string? status);

    /// <summary>
    /// Gets all grades from specific teacher asynchronous.
    /// </summary>
    /// <param name="teacherId">The teacher identifier.</param>
    /// <returns>Returns grades in a list</returns>
    Task<List<GradeResponse>> GetMyGradesAsync(int teacherId);

    /// <summary>
    /// Deletes a specific grade from specific teacher asynchronous.
    /// </summary>
    /// <param name="id">The grade identifier.</param>
    /// <param name="teacherId">The teacher identifier.</param>
    /// <returns>Returns false if grade isnt found or is teacher isnt owner and not pending, else true if succesful</returns>
    Task<bool> DeleteMyGradeAsync(int id, int teacherId);

    /// <summary>
    /// Updates a grade request from a specific teacher asynchronous.
    /// </summary>
    /// <param name="gradeId">The grade identifier.</param>
    /// <param name="teacherId">The teacher identifier.</param>
    /// <param name="request">The request dto.</param>
    /// <returns>Returns false if grade isnt found, else if succesful true</returns>
    /// <exception cref="System.InvalidOperationException">Rektor not found.</exception>
    Task<bool> UpdateMyGradeAsync(int gradeId, int teacherId, UpdateGradeRequest request);

    /// <summary>
    /// Gets a grade by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns the grade</returns>
    Task<GradeResponse?> GetByIdAsync(int id);

    /// <summary>
    /// Decides a grade the asynchronous.
    /// </summary>
    /// <param name="gradeId">The grade identifier.</param>
    /// <param name="status">The status (approved / pending).</param>
    /// <param name="decisionNote">The decision note.</param>
    /// <returns>Returns false if grade isnt found, else a true if succesful</returns>
    /// <exception cref="System.ArgumentException">Status must be 'approved' or 'rejected'.</exception>
    Task<bool> DecideAsync(int gradeId, string status, string? decisionNote);

    /// <summary>
    /// Deletes a specific grade asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>if grade isnt found it returns a false, else if succesful a true</returns>
    Task<bool> DeleteGradeAsync(int id);
}