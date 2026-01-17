using Shared.DTOs;

namespace Api.Services.Interfaces;

public interface IGradeService
{
    Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId);
    Task<List<GradeResponse>> GetGradesAsync(string? status);
    Task<GradeResponse?> GetByIdAsync(int id);
    Task<bool> DecideAsync(int gradeId, string status, string? decisionNote);
    Task<bool> DeleteGradeAsync(int id);
}